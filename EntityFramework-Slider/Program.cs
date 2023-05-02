using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(); 

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(option =>
{
    option.Password.RequiredLength = 8;
    option.Password.RequireDigit = true;
    option.Password.RequireLowercase = true;  
    option.Password.RequireUppercase = true;
    option.Password.RequireNonAlphanumeric = true; 

    option.User.RequireUniqueEmail = true; 

    option.Lockout.MaxFailedAccessAttempts = 3;  
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);  
    option.Lockout.AllowedForNewUsers = true; 

});




builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();   

builder.Services.AddScoped<ILayoutService, LayoutService>();

builder.Services.AddScoped<IBasketService, BasketService>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<ISliderService, SliderService>();

builder.Services.AddScoped<IExpertService, ExpertService>();

builder.Services.AddScoped<IFooterService, FooterService>();









var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession(); 
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
