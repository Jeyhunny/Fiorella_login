using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EntityFramework_Slider.Controllers
{

    public class CardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;


        public CardController(AppDbContext context, IBasketService basketService, IProductService productService)
        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
        }


        public async Task<IActionResult> Index()
        {

            List<BasketVM> basketProducts = _basketService.GetBasketDatas();     

           


            List<BasketDetailVM> basketDetails = new();

            foreach (var product in basketProducts)  
            {
                Product dbProduct = await _productService.GetFullDataById(product.Id);

                basketDetails.Add(new BasketDetailVM
                {
                    Id = dbProduct.Id,
                    Name = dbProduct.Name,
                    CategoryName = dbProduct.Category.Name,
                    Description = dbProduct.Description,
                    Price = dbProduct.Price,
                    Image = dbProduct.Images.Where(m => m.IsMain).FirstOrDefault().Image,
                    Count = product.Count,
                    Total = dbProduct.Price * product.Count

                });
            }


            return View(basketDetails);
        }

        public IActionResult DeleteProductFromBasket(int? id)
        {
            if (id == null) return BadRequest();

            _basketService.DeleteProductFromBasket((int)id);
            var datas = _basketService.GetBasketDatas();
            int count = datas.Sum(m => m.Count);
            return Ok(count);
        }


        public IActionResult DecreasetProductCount(int? id)
        {

            if (id == null) return BadRequest();

            List<BasketVM> basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);

            BasketVM decreaseProduct = basketProducts.FirstOrDefault(m => m.Id == id);

            if (decreaseProduct.Count > 1)    
            {
                var productCount = --decreaseProduct.Count;   

                Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketProducts));

                return Ok(productCount); 
            }



            return Ok();
        }




        public IActionResult IncreaseProductCount(int? id)
        {
            List<BasketVM> basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);    

            BasketVM? increaseProduct = basketProducts.Find(m => m.Id == id);   


            var productCount =  ++increaseProduct.Count;   

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketProducts));   

           
            return Ok(productCount);   
        }
    }
}
