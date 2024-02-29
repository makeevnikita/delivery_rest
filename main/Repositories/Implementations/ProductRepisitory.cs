using delivery.Models;
using Microsoft.EntityFrameworkCore;
using delivery.DataTransferObjects;



namespace delivery.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DeliveryContext _deliveryContext;

    public ProductRepository(DeliveryContext deliveryContext)
    {
        _deliveryContext = deliveryContext;
    }

    public async Task<ProductDtoDetailed> GetById(int id)
    {
        ProductDtoDetailed product = await _deliveryContext.Products.Join(_deliveryContext.ProductCategories,
                                    product => product.CategoryId,
                                    category => category.Id,
                                    (product, category) => new ProductDtoDetailed
                                        {
                                            Id = product.Id,
                                            Name = product.Name,
                                            Price = product.Price,
                                            ImagePath = product.ImagePath,
                                            Description = product.Description,
                                            IsActive = product.IsActive,
                                            CategoryName = category.Name,
                                        }).FirstOrDefaultAsync(w => w.Id == id);
        
        return product;
    }

    public async Task<ProductList> GetPaginatedAndFiltered(
        int page = 1, string name = null, decimal? minPrice = null, decimal? maxPrice = null,
        int? categoryId = null, bool sortByPrice = false, bool sortByPriceDesc = false
    )
    {
        IQueryable<Product> filterQuery = _deliveryContext.Products;

        if (name != null)
        {
            filterQuery = filterQuery.Where(w => w.Name.Contains(name));
        }

        if (minPrice != null)
        {
            filterQuery = filterQuery.Where(w => w.Price >= minPrice);
        }

        if (maxPrice != null)
        {
            filterQuery = filterQuery.Where(w => w.Price <= maxPrice);
        }

        if (categoryId != null)
        {
            filterQuery = filterQuery.Where(w => w.CategoryId == categoryId);
        }

        if (sortByPrice)
        {
            filterQuery = filterQuery.OrderBy(w => w.Price);
        }

        if (sortByPriceDesc)
        {
            filterQuery = filterQuery.OrderByDescending(w => w.Price);
        }

        int pageSize = 10;

        ProductList productList = new ProductList();

        try
        {
            productList.Products = await filterQuery
                .Select(product => new ProductDto {
                    Name = product.Name,
                    ImagePath = product.ImagePath,
                    Price = product.Price,
                    Id = product.Id
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            productList.TotalCount = await filterQuery.CountAsync();
            productList.PageSize = pageSize;

            return productList;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Remove(Product product)
    {
        _deliveryContext.Products.Attach(product);
        _deliveryContext.Products.Remove(product);

        try
        {
            await _deliveryContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex; 
        }
    }

    public async Task Update(
        int productId,
        bool isActive,
        string name,
        decimal price,
        string imagePath,
        string description,
        int categoryId
    )
    {
        Product product = new Product
        { 
            Id = productId,
            Name = name,
            CategoryId = categoryId,
            IsActive = isActive,
            Price = price,
            Description = description,
            ImagePath = imagePath
        };

        _deliveryContext.Products.Attach(product);
        _deliveryContext.Products.Update(product);

        try
        {
            _deliveryContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<int> Create(
        bool IsActive,
        string name,
        decimal price,
        string filePath,
        string description,
        int categoryId
    )
    {

        Product newProduct = new Product
        {
            Name = name,
            Price = price,
            ImagePath = filePath,
            Description = description,
            CategoryId = categoryId
        };

        await _deliveryContext.AddAsync(newProduct);

        try
        {
            await _deliveryContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return newProduct.Id;
    }

    public async Task<string> GetProductImage(int id)
    {
        try
        {
            string imagePath = await _deliveryContext.Products
                .Where(w => w.Id == id)
                .Select(w => w.ImagePath)
                .FirstAsync();

            return imagePath;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
