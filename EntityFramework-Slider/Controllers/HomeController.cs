using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISliderService _sliderService;
        private readonly IExpertService _expertService;
        private readonly IFooterService _footerService;
        private readonly IBlogService _blogService;



        public HomeController(AppDbContext context, 
                              IBasketService basketService,
                              IProductService productService ,
                              ICategoryService categoryService,
                              ISliderService sliderService,
                              IExpertService expertService,
                              IFooterService footerService)
        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
            _categoryService = categoryService;
            _sliderService = sliderService;
            _expertService = expertService;
            _footerService = footerService;
     
        }




        public async Task<IActionResult> Index()
        {


            IEnumerable<Slider> sliders = await _sliderService.GetAll();


            SliderInfo sliderInfo = await _sliderService.GetInfo();

            IEnumerable<Category> categories = await _categoryService.GetAll();


            IEnumerable<Product> products = await _productService.GetAll();   

            About abouts = await _context.Abouts.Include(m => m.Adventages).FirstOrDefaultAsync();

            IEnumerable<Experts> experts = await _expertService.GetAll();

            IEnumerable<ExpertsHeader> expertsHeader = await _expertService.GetHeaders();

            Subscribe subscribe = await _context.Subscribes.FirstOrDefaultAsync();

            OurBlog ourBlog = await _context.OurBlogs.FirstOrDefaultAsync();

            IEnumerable<Say> says = await _context.Says.Where(m => m.SoftDelete == false).ToListAsync();

            IEnumerable<Instagram> instagrams = await _context.Instagrams.Where(m => m.SoftDelete == false).ToListAsync();

            IEnumerable<Social> socials = await _footerService.GetSocials();





            HomeVM model = new()
            {
                Categories = categories,
                Products = products,
                Abouts = abouts,
                Subscribe = subscribe,
                Says = says,
                OurBlog = ourBlog,
                Instagrams = instagrams,
            };

            return View(model);
        }





        [HttpPost] 
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest();

            Product dbproduct = await _productService.GetById((int)id);     

            if (dbproduct == null) return NotFound();

            List<BasketVM> basket = _basketService.GetBasketDatas();

            BasketVM existProduct = basket?.FirstOrDefault(m => m.Id == dbproduct.Id);


            _basketService.AddProductToBasket(existProduct, dbproduct, basket); 
            int basketCount = basket.Sum(m => m.Count);  
            return Ok(basketCount);
        }


    }


 
}