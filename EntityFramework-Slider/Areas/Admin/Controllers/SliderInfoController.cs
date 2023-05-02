using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderInfoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISliderService _sliderService;
        private readonly IWebHostEnvironment _env;
        public SliderInfoController(AppDbContext context,
                                    ISliderService sliderService, 
                                    IWebHostEnvironment env)
        {
            _context = context;
            _sliderService = sliderService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<SliderInfo> sliderInfos = await _context.SliderInfos.ToListAsync();
            return View(sliderInfos);
        }



        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            SliderInfo dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);
            if (dbSliderInfo is null) return NotFound();

            return View(dbSliderInfo);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderInfo sliderInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!sliderInfo.Photo.CheckFileType("image/") )
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }

                if (!sliderInfo.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }

                string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.Photo.FileName;

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using(FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await sliderInfo.Photo.CopyToAsync(stream);
                }

                sliderInfo.SignatureImage = fileName;

                await _context.SliderInfos.AddAsync(sliderInfo);

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
                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (id == null) return BadRequest();
                SliderInfo dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);
                if (dbSliderInfo == null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSliderInfo.SignatureImage);

                FileHelper.DeleteFile(path);

                _context.SliderInfos.Remove(dbSliderInfo);

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
            SliderInfo dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(s => s.Id == id);
            if (dbSliderInfo is null) return NotFound();

            SliderInfoUpdateVM model = new()
            {
                SignatureImage = dbSliderInfo.SignatureImage,
                Title = dbSliderInfo.Title,
                Description = dbSliderInfo.Description,
            };
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, SliderInfoUpdateVM sliderInfo)
        {
            try
            {
                if (id is null) return BadRequest();
                SliderInfo dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);
                if (dbSliderInfo == null) return NotFound();

                SliderInfoUpdateVM model = new()
                {
                    SignatureImage = dbSliderInfo.SignatureImage,
                    Title = dbSliderInfo.Title,
                    Description = dbSliderInfo.Description,
                };

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (sliderInfo.Photo != null)
                {

                    if (!sliderInfo.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }
                    if (!sliderInfo.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();
                    }
                    
                    string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSliderInfo.SignatureImage);

                    FileHelper.DeleteFile(oldPath);

                
                    string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.Photo.FileName;

                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName); 

                    using (FileStream stream = new FileStream(newPath, FileMode.Create))     
                    {
                        await sliderInfo.Photo.CopyToAsync(stream);
                    }
                    dbSliderInfo.SignatureImage = fileName;
                }
                else
                {
                    SliderInfo newSlider = new()  
                    {
                        SignatureImage = dbSliderInfo.SignatureImage
                    };
                }
         
                dbSliderInfo.Title = sliderInfo.Title;
                dbSliderInfo.Description = sliderInfo.Description;

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
