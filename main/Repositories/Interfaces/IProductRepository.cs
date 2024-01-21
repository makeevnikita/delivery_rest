using delivery.DataTransferObjects;
using delivery.Models;



namespace delivery.Repositories;

public interface IProductRepository
{
    public Task<ProductDtoDetailed> GetById(int id);

    public Task<ProductList> GetPaginatedAndFiltered(
        int page, string name = null, decimal? minPrice = null, decimal? maxPrice = null,
        int? categoryId = null, bool sortByPrice = false, bool sortByPriceDesc = false
    );

    public Task Remove(Product product);

    public Task Update(
        int productId,
        bool IsActive,
        string name,
        decimal price,
        string filePath,
        string Description,
        int categoryId
    );

    public Task<int> Create(bool IsActive, string name, decimal price, string filePath,
        string Description, int categoryId);

    public Task<string> GetProductImage(int id);
}