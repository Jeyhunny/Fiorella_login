using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
   
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;
        public ProductController(IProductService productService,
                                 ICategoryService categoryService,
                                 IWebHostEnvironment env, AppDbContext context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1, int take = 4) 
        {
            List<Product> products = await _productService.GetPaginatedDatas(page, take);  

            List<ProductListVM> mappedDatas = GetMappedDatas(products);           

            int pageCount = await GetPageCountAsync(take); 

            ViewBag.take = take;

            Paginate<ProductListVM> paginaDatas = new(mappedDatas, page, pageCount);
            return View(paginaDatas);

        }



        private List<ProductListVM> GetMappedDatas(List<Product> products)
        {
            List<ProductListVM> mappedDatas = new(); 
            foreach (var product in products)
            {
                ProductListVM productVM = new()   
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Count = product.Count,
                    CategoryName = product.Category.Name,
                    MainImage = product.Images.Where(m => m.IsMain).FirstOrDefault()?.Image

                };
                mappedDatas.Add(productVM);
            }
            return mappedDatas;
        } 

        private async Task<int> GetPageCountAsync(int take)  
        {
            var productCount = await _productService.GetCountAsync();  

            return (int)Math.Ceiling((decimal)productCount / take);  
        }                                                               


   
    
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            
            ViewBag.categories = await GetCategoriesAsync();
            return View();
        }



       
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            try
            {
                ViewBag.categories = await GetCategoriesAsync(); 

                if (!ModelState.IsValid) 
                {
                    return View(model);
                }
                foreach (var photo in model.Photos)     
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }
                  
                }

                List<ProductImage> productImages = new();  
                foreach (var photo in model.Photos) 
                {

                    string fileName = Guid.NewGuid().ToString() + " " + photo.FileName;
                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);
                    await FileHelper.SaveFileAsync(newPath, photo); 

                    ProductImage newProductImage = new()
                    {
                        Image = fileName 
                    };
                    productImages.Add(newProductImage); 

                }

                productImages.FirstOrDefault().IsMain = true; 
                decimal convertedPrice = decimal.Parse(model.Price); 
                Product product = new()
                {
                    Name = model.Name, 
                    Price = convertedPrice, 
                    Count = model.Count, 
                    Description = model.Description, 
                    CategoryId = model.CategoryId,  
                    Images = productImages   
                };

                await _context.ProductImages.AddRangeAsync(productImages);
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                throw;
            }

        }
        private async Task<SelectList> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _categoryService.GetAll(); 
            return new SelectList(categories, "Id", "Name");
        }




        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _productService.GetFullDataById((int)id);
            if (product == null) return NotFound();
            ViewBag.description = Regex.Replace(product.Description, "<.*?>", String.Empty);  
           
            return View(product);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]   
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            Product product = await _productService.GetFullDataById((int)id);

            foreach (var item in product.Images)  
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", item.Image);   

                FileHelper.DeleteFile(path);
            }
            _context.Products.Remove(product); 

            await _context.SaveChangesAsync(); 

            return RedirectToAction(nameof(Index));

        }



        [HttpGet]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Product dbProduct = await _productService.GetFullDataById((int)id);
            if (dbProduct == null) return NotFound();
            ViewBag.categories = await GetCategoriesAsync();

            ProductUpdateVM model = new()
            {
                Images = dbProduct.Images,
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Price = dbProduct.Price,
                Count = dbProduct.Count,
                CategoryId = dbProduct.CategoryId,

            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProductUpdateVM model)
        {
            try
            {
                if (id == null) return BadRequest();
                Product dbProduct = await _productService.GetFullDataById((int)id);
                if (dbProduct == null) return NotFound();
                ViewBag.categories = await GetCategoriesAsync();


                ProductUpdateVM newProduct = new()
                {
                    Images = dbProduct.Images,
                    Name = dbProduct.Name,
                    Description = dbProduct.Description,
                    Price = dbProduct.Price,
                    Count = dbProduct.Count,
                    CategoryId = dbProduct.CategoryId,
                };

                if (!ModelState.IsValid)
                {
                    return View(newProduct);
                }

                if (model.Photos != null)
                {
                    foreach (var photo in model.Photos)
                    {
                        if (!photo.CheckFileType("image/"))
                        {
                            ModelState.AddModelError("Photo", "File type must be image");
                            return View(newProduct);
                        }
                        if (!photo.CheckFileSize(200))
                        {
                            ModelState.AddModelError("Photo", "Image size must be max 200kb");
                            return View(newProduct);
                        }
                    }

                    foreach (var item in dbProduct.Images) 
                    {
                        string path = FileHelper.GetFilePath(_env.WebRootPath, "img", item.Image);  

                        FileHelper.DeleteFile(path);
                    }


                    List<ProductImage> productImages = new();  
                    foreach (var photo in model.Photos)  
                    {

                        string fileName = Guid.NewGuid().ToString() + " " + photo.FileName;
                        string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);
                        await FileHelper.SaveFileAsync(newPath, photo); 

                        ProductImage newProductImage = new()
                        {
                            Image = fileName  
                        };
                        productImages.Add(newProductImage);
                    }
                    productImages.FirstOrDefault().IsMain = true;
                    _context.ProductImages.AddRange(productImages);
                    dbProduct.Images = productImages;

                }
                else
                {
                    Product prod = new()
                    {
                        Images = dbProduct.Images
                    };
                }

                dbProduct.Name = model.Name;
                dbProduct.Price = model.Price;
                dbProduct.Count = model.Count;
                dbProduct.Description = model.Description;
                dbProduct.CategoryId = model.CategoryId;  
                   
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
             
            }
            catch (Exception)
            {

                throw;
            }


        }






        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Product dbProduct = await _productService.GetFullDataById((int) id);
            if (dbProduct is null) return NotFound();
            ViewBag.description = Regex.Replace(dbProduct.Description, "<.*?>", String.Empty);
            return View(dbProduct);
        }

    }

}
