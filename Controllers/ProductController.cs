using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Encodings.Web;
using delivery.Models;
using delivery.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using delivery.Helpers;



namespace delivery.Controllers;

[ApiController]
[Route("product")]
public class ProductController : ControllerBase
{
    private ILogger<ProductController> _logger;

    private readonly JsonSerializerOptions serializerOptions;

    private readonly IWebHostEnvironment _environment;

    private DeliveryContext _context;
    
    public ProductController(DeliveryContext context, ILogger<ProductController> logger,
                             IWebHostEnvironment environment)
    {
        _environment = environment;
        _logger = logger;
        _context = context;
        
        serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        ProductDtoDetailed productDto = await _context.Products.Join(_context.ProductCategories,
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

        if (productDto == null)
        {
            return NotFound(new { result = "NotFound"} );
        }

        return new JsonResult(productDto, serializerOptions);
    }

    [HttpGet("get")]
    public async Task<IActionResult> FilterProducts([FromQuery] ProductFilterDto productDto)
    {
        IQueryable<Product> filterQuery = _context.Products;

        if (productDto.Name != null)
        {
            filterQuery = filterQuery.Where(w => w.Name.Contains(productDto.Name));
        }

        if (productDto.MinPrice != null)
        {
            filterQuery = filterQuery.Where(w => w.Price >= productDto.MinPrice);
        }

        if (productDto.MaxPrice != null)
        {
            filterQuery = filterQuery.Where(w => w.Price <= productDto.MaxPrice);
        }

        if (productDto.CategoryId != null)
        {
            filterQuery = filterQuery.Where(w => w.CategoryId == productDto.CategoryId);
        }

        if (productDto.SortByPrice)
        {
            filterQuery = filterQuery.OrderBy(w => w.Price);
        }

        if (productDto.SortByPriceDesc)
        {
            filterQuery = filterQuery.OrderByDescending(w => w.Price);
        }

        int pageSize = 1;
        
        ProductList productList = new ProductList();

        productList.Products = await filterQuery
                    .Select(product => new ProductDto {
                        Name = product.Name,
                        ImagePath = product.ImagePath,
                        Price = product.Price,
                        Url = Url.Link("GetById", new { controller = "product", action = "get", id = product.Id })
                    }).Skip((productDto.Page - 1) * pageSize).Take(pageSize).ToListAsync();
        productList.TotalCount = await filterQuery.CountAsync();
        productList.PageSize = pageSize;

        return new JsonResult(productList, serializerOptions);
    }

    // [Authorize(Roles = "Admin")]
    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {    
        Product product = new Product { Id = id };

        _context.Products.Attach(product);
        _context.Products.Remove(product);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            return new JsonResult(new { 
                result = "error", 
                description = "Ошибка при удалении товара или товара не существует" });   
        }

        return new JsonResult(new { result = "success" });
    }

    // [Authorize(Roles = "Admin")]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateProductDto productDto)
    {           
        Product product = await _context.Products.FirstOrDefaultAsync(w => w.Id == id);

        if (product == null)
        {
            return NotFound(new { result = "error", description = "Товар не найден" });
        }

        ProductCategory category = await _context.ProductCategories.FirstOrDefaultAsync(w => w.Id == productDto.CategoryId);

        if (category == null)
        {
            return NotFound(new { result = "error", description = "Категория не найдена" });
        }
        
        product.Name = productDto.Name;
        product.IsActive = productDto.IsActive;
        product.Price = productDto.Price;
        product.Description = productDto.Description;
        product.Category = category;

        string oldImage = product.ImagePath;
        string fileName = "";

        if (product.ImagePath.Substring(0, 35) != productDto.Image.FileName)
        {  
            Guid guid = Guid.NewGuid();
            fileName = $"{guid}{productDto.Image.FileName}";
            product.ImagePath = Path.Combine(FileHelper.ImagesFolder, FileHelper.ProductFolder, fileName);
        }

        _context.Products.Update(product);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            return new JsonResult(new { result = "error", description = "Не удалось обновить товар" });
        }
        finally
        {
            if (oldImage != product.ImagePath.Substring(0, 35))
            {
                FileHelper helper = new FileHelper();

                await helper.DeleteFileAsync(
                    Path.Combine(_environment.WebRootPath, FileHelper.ImagesFolder, FileHelper.ProductFolder)
                );
                
                await helper.CreateFileAsync(
                    productDto.Image,
                    Path.Combine(_environment.WebRootPath, FileHelper.ImagesFolder, FileHelper.ProductFolder),
                    fileName
                );
            }
        }

        return new JsonResult(new { result = "success", description = "Товар успешно обновлён" });
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<IActionResult> CreateAsync([FromForm] CreateProductDto productDto)
    {   
        ProductCategory category = await _context.ProductCategories
            .FirstOrDefaultAsync(w => w.Id == productDto.CategoryId);

        if (category == null)
        {
            return NotFound(new { result = "error", description = "Категория не найдена" });
        }

        Guid guid = Guid.NewGuid();
        string fileName = $"{guid}{productDto.Image.FileName}";
        string filePath = Path.Combine(
            FileHelper.ImagesFolder,
            FileHelper.ProductFolder,
            fileName
        );

        Product newProduct = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            ImagePath = filePath,
            Description = productDto.Description,
            Category = category
        };

        await _context.AddAsync(newProduct);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            return Created(Url.RouteUrl("GetProductById", new {id = newProduct.Id}), new { result = "success" });
        }
        finally
        {
            FileHelper helper = new FileHelper();

            await helper.CreateFileAsync(
                productDto.Image,
                Path.Combine(_environment.WebRootPath, FileHelper.ImagesFolder, FileHelper.ProductFolder),
                fileName
            );
        }

        return Created(Url.RouteUrl("GetProductById", new {id = newProduct.Id}), new { result = "success" });
    }
}
