 using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<SliderInfo> SliderInfos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Adventage> Adventages { get; set; }   
        public DbSet<ExpertsHeader> ExpertsHeaders { get; set; }
        public DbSet<Experts> Experts { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<OurBlog> OurBlogs { get; set; }
        public DbSet<Say> Says { get; set; }
        public DbSet<Instagram> Instagrams { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Social> Socials { get; set; }




 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasQueryFilter(m => !m.SoftDelete);

            modelBuilder.Entity<Blog>().HasQueryFilter(m => !m.SoftDelete);

            modelBuilder.Entity<OurBlog>().HasQueryFilter(m => !m.SoftDelete);

            modelBuilder.Entity<Category>().HasQueryFilter(m => !m.SoftDelete);


            modelBuilder.Entity<SliderInfo>().HasQueryFilter(m => !m.SoftDelete);


            modelBuilder.Entity<Experts>().HasQueryFilter(m => !m.SoftDelete);

            modelBuilder.Entity<ExpertsHeader>().HasQueryFilter(m => !m.SoftDelete);

            modelBuilder.Entity<Social>().HasQueryFilter(m => !m.SoftDelete);







            modelBuilder.Entity<Setting>() 
                .HasData(
                new Setting   
                {
                    Id = 1,
                    Key = "HeaderLogo",
                    Value = "logo.png"
                },
                new Setting
                {
                      Id = 2,
                      Key = "Phone",
                      Value = "0124586589855"
                },
                new Setting
                {
                      Id = 3,
                      Key = "Email",
                      Value = "P135@code.edu.az",
                }

              ); 
        }



    }
}
