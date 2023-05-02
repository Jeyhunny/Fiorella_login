using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertHeaderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IExpertService _expertService;

        public ExpertHeaderController(AppDbContext context, IExpertService expertService)
        {
            _context = context;
            _expertService = expertService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _expertService.GetHeaders());
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertsHeader expertsHeader)     
        {
            try
            {
                if (!ModelState.IsValid)    
                {
                    return View();
                }

                await _context.ExpertsHeaders.AddAsync(expertsHeader); 
                await _context.SaveChangesAsync();    
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return RedirectToAction("Error", new { msj = ex.Message });
            }

        }


        public IActionResult Error(string msj)
        {
            ViewBag.error = msj;   
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)   
        {
            if (id is null) return BadRequest();

            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);  

            if (expertsHeader is null) return NotFound();

            _context.ExpertsHeaders.Remove(expertsHeader);    

            await _context.SaveChangesAsync();   

            return RedirectToAction(nameof(Index));

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int? id)  
        {
            if (id is null) return BadRequest();

            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);  

            if (expertsHeader is null) return NotFound();

            expertsHeader.SoftDelete = true;   

            await _context.SaveChangesAsync();   

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)  
        {
            if (id is null) return BadRequest();

            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);   

            if (expertsHeader is null) return NotFound();

            return View(expertsHeader);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ExpertsHeader expertsHeader)   
        {
            if (id is null) return BadRequest();

            ExpertsHeader dbexpertsHeader = await _context.ExpertsHeaders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);  

            if (dbexpertsHeader is null) return NotFound();

            if (dbexpertsHeader.Title.Trim().ToLower() == dbexpertsHeader.Description.Trim().ToLower())  
            {
                return RedirectToAction(nameof(Index));
            }

            _context.ExpertsHeaders.Update(expertsHeader); 

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);   

            if (expertsHeader is null) return NotFound();

            return View(expertsHeader);
        }




    }
}
