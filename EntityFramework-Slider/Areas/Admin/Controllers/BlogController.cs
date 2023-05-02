using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBlogService _blogService;
        private readonly IWebHostEnvironment _env;   
        public BlogController(AppDbContext context, 
                              IWebHostEnvironment env, 
                              IBlogService blogService)
        {
            _context = context;
            _env = env;
            _blogService= blogService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blog = await _blogService.GetAll();
            return View(blog);
        }



        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Blog dbBlog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            if (dbBlog is null) return NotFound();
            return View(dbBlog);
        }


        [HttpGet]
        public IActionResult Create()     
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            try
            {
                if (!ModelState.IsValid)  
                {
                    return View();
                }

                if (!blog.Photo.CheckFileType("image/")) 
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }
               
                if (!blog.Photo.CheckFileSize(200)) 
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }
           
                string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName;  

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);  

                using (FileStream stream = new FileStream(path, FileMode.Create)) 
                {
                    await blog.Photo.CopyToAsync(stream); 
                }

                blog.Image = fileName;

                await _context.Blogs.AddAsync(blog); 

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Blog dbBlog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

                if (dbBlog is null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", dbBlog.Image); 

                FileHelper.DeleteFile(path); 

                _context.Blogs.Remove(dbBlog);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();
            Blog dbBlog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (dbBlog is null) return NotFound();

            BlogUpdateVM model = new()
            {
                Image= dbBlog.Image,
                Header=dbBlog.Header,
                Description=dbBlog.Description,
                Date = dbBlog.Date,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, BlogUpdateVM blog)
        {
            try
            {
                if (id is null) return BadRequest();
                Blog dbBlog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
                if (dbBlog == null) return NotFound();

                BlogUpdateVM model = new()
                {
                    Image = dbBlog.Image,
                    Header = dbBlog.Header,
                    Description = dbBlog.Description,
                    Date = dbBlog.Date,
                };

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (blog.Photo != null)
                {

                    if (!blog.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }
                    if (!blog.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();
                    }

                    string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbBlog.Image);
                    FileHelper.DeleteFile(oldPath);

                   
                    string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName;

                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName); 

                    using (FileStream stream = new FileStream(newPath, FileMode.Create))     
                    {
                        await blog.Photo.CopyToAsync(stream);
                    }
                    dbBlog.Image = fileName;
                }
                else
                {
                    Blog newBlog = new()  
                    {
                        Image = dbBlog.Image  
                    };
                }
               
                dbBlog.Header = blog.Header;
                dbBlog.Description = blog.Description;
                dbBlog.Date = blog.Date;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }
    }
}
