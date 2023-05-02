using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IExpertService _expertService;
        private readonly IWebHostEnvironment _env;
        public ExpertController(AppDbContext context,
                                IExpertService expertService, 
                                IWebHostEnvironment env)
        {
            _context = context;
            _expertService = expertService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Experts> experts = await _context.Experts.ToListAsync();
            return View(experts);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Experts dbExpert = await _context.Experts.FirstOrDefaultAsync(m=>m.Id == id);
            if (dbExpert == null) return NotFound();
            return View(dbExpert);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Experts expert)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!expert.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }

                if (!expert.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }

                string fileName = Guid.NewGuid().ToString() + "_" + expert.Photo.FileName;

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await expert.Photo.CopyToAsync(stream);
                }

                expert.Image = fileName;

                await _context.Experts.AddAsync(expert);

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

                Experts dbExpert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);

                if (dbExpert is null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", dbExpert.Image);

                FileHelper.DeleteFile(path);

                _context.Experts.Remove(dbExpert);  

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
            Experts dbExpert = await _context.Experts.FirstOrDefaultAsync(e => e.Id == id);
            if (dbExpert is null) return NotFound();

            ExpertUpdateVM model = new()
            {
                Image = dbExpert.Image,
                Name = dbExpert.Name,
                Position = dbExpert.Position
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ExpertUpdateVM expert)
        {
            try
            {
                if (id is null) return BadRequest();
                Experts dbExpert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);
                if (dbExpert == null) return NotFound();

                ExpertUpdateVM model = new()
                {
                    Image = dbExpert.Image,
                    Name = dbExpert.Name,
                    Position = dbExpert.Position
                };

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (expert.Photo != null)
                {
                    if (!expert.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }

                    if (!expert.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();
                    }

                    string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbExpert.Image);

                    FileHelper.DeleteFile(oldPath);


                    string fileName = Guid.NewGuid().ToString() + "_" + expert.Photo.FileName;

                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    await FileHelper.SaveFileAsync(newPath, expert.Photo);

                    dbExpert.Image = fileName;
                }
                else
                {
                    Experts newExpert = new()
                    {
                        Image = expert.Image
                    };
                }

                dbExpert.Name= expert.Name;
                dbExpert.Position = expert.Position;

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
