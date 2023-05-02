using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetById(int id);
        Task <IEnumerable<Product>> GetAll();

        Task <Product> GetFullDataById(int id);

        Task<int> GetCountAsync(); 
        Task<List<Product>> GetPaginatedDatas(int page, int take);

    }
}
