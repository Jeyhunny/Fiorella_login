
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EntityFramework_Slider.ViewComponents
{
    public class BlogViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;
        public BlogViewComponent(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
 

            return await Task.FromResult(View(new BlogVM { Blogs = await _blogService.GetAll(), OurBlogHeader = await _blogService.GetOurBlogHeader() }));
        }


    }
}
