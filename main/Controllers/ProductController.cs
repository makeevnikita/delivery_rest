using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Encodings.Web;
using delivery.Models;
using delivery.DataTransferObjects;
using delivery.Helpers;
using delivery.Repositories;



namespace delivery.Controllers;

[ApiController]
[Route("product")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    private readonly JsonSerializerOptions serializerOptions;

    private readonly IWebHostEnvironment _environment;

    private readonly IProductRepository _productRepository;
    
    public ProductController(
        IProductRepository productRepository,
        ILogger<ProductController> logger,
        IWebHostEnvironment environment)
    {
        _environment = environment;
        _logger = logger;
        _productRepository = productRepository;
        
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
        ProductDtoDetailed product = await _productRepository.GetById(id);

        if (product == null)
        {
            string message = $"Product with id = {id} not found";
            _logger.LogError(message);
            return NotFound(new { result = "NotFound", description = message } );
        }

        return new JsonResult(product, serializerOptions);
    }

    [HttpGet("get")]
    public async Task<IActionResult> FilterProducts([FromQuery] ProductFilterDto productDto)
    {        
        try
        {
            ProductList productList = await _productRepository.GetPaginatedAndFiltered(
                page: productDto.Page,
                name: productDto.Name,
                minPrice: productDto.MinPrice,
                maxPrice: productDto.MaxPrice,
                categoryId: productDto.CategoryId, 
                sortByPrice: productDto.SortByPrice,
                sortByPriceDesc: productDto.SortByPriceDesc
            );

            return new JsonResult(productList, serializerOptions);
        }
        catch (Exception ex)
        {
            string message = "an error occurred while searching for products";
            _logger.LogError(message + "\n" + ex.Message + "\n" + ex.StackTrace);

            return StatusCode(500);
        }
    }

    // [Authorize(Roles = "Admin")]
    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {    
        try
        {
            Product product = new Product { Id = id };

            await _productRepository.Remove(product);

            return new JsonResult(new { result = "success" });
        }
        catch
        {
            string message = $"An error occurred during the deletion of the product id = {id} or the product does not exist";
            _logger.LogError(message);

            return new JsonResult(new { 
                    result = "error", 
                    description = message 
                }
            );     
        }
    }

    // [Authorize(Roles = "Admin")]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update([FromForm] UpdateProductDto productDto)
    {   
        string oldImage = await _productRepository.GetProductImage(productDto.Id);

        string fileName = oldImage;

        if (oldImage.Substring(0, 35) != productDto.Image.FileName)
        {  
            Guid guid = Guid.NewGuid();
            fileName = $"{guid}{productDto.Image.FileName}";
        }

        try
        {
            await _productRepository.Update(
                productDto.Id,
                productDto.IsActive,
                productDto.Name,
                productDto.Price,
                Path.Combine(FileHelper.ImagesFolder, FileHelper.ProductFolder, fileName),
                productDto.Description,
                productDto.CategoryId
            );

            return new JsonResult(new { result = "success", description = "Товар успешно обновлён" });
        }
        catch
        {
            string message = "failed to update the product";
            _logger.LogError(message);

            return new JsonResult(new {
                    result = "error",
                    description = message 
                }
            );
        }
        finally
        {
            if (oldImage != fileName)
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
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    [AbpRequestSizeLimitAttribute(50 * 1024 * 1024)]
    public async Task<IActionResult> CreateAsync([FromForm] CreateProductDto productDto)
    {   
        Console.WriteLine(HttpContext.Request.ContentLength);
        Console.WriteLine(HttpContext.Request.ContentType);
        Console.WriteLine(HttpContext.Request.Form);
        foreach (var data in HttpContext.Request.Form)
        {
            Console.WriteLine(data);
        }
        foreach (var data in HttpContext.Request.Headers)
        {
            Console.WriteLine($"{data.Key} {data.Value}");
        }

        Guid guid = Guid.NewGuid();
        string fileName = $"{guid}{productDto.Image.FileName}";
        string filePath = Path.Combine(
            FileHelper.ImagesFolder,
            FileHelper.ProductFolder,
            fileName
        );

        try
        {
            int newProductId = await _productRepository.Create(
                productDto.IsActive,
                productDto.Name,
                productDto.Price,
                filePath,
                productDto.Description,
                productDto.CategoryId
            );

            return Created(Url.RouteUrl("GetProductById", new {id = newProductId}), new { result = "success" });
        }
        catch (Exception ex)
        {
            string message = "failed to create the product";
            _logger.LogError(message + '\n' + ex.Message + '\n' + ex.StackTrace);
            
            return new JsonResult(new {
                    result = "error",
                    description = message + '\n' + ex.Message + '\n' + ex.StackTrace
                }
            );
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
    }
}
