using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Encodings.Web;
using delivery.Models;
using delivery.DataTransferObjects;
using Microsoft.EntityFrameworkCore;



namespace delivery.Controllers;

[ApiController]
[Route("product_category")]
public class ProductCategoryController : ControllerBase
{
    private readonly JsonSerializerOptions serializerOptions;
    private DeliveryContext _context;
    
    public ProductCategoryController(DeliveryContext context)
    {
        _context = context;
        
        serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ProductCategoryDto categoryDto)
    {   
        ProductCategory newCategory = new ProductCategory
        {
            Name = categoryDto.Name,
            ImagePath = categoryDto.ImagePath
        };

        await _context.AddAsync(newCategory);
        await _context.SaveChangesAsync();

        return Created(Url.RouteUrl("GetProductCategoryById", new {id = newCategory.Id}),
                        new { result = "success" });
    }
    
    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {   
        List<ProductCategoryDto> categories = await _context.ProductCategories.AsNoTracking()
            .Join(_context.Products,
                  category => category.Id,
                  product => product.CategoryId,
                  (category, product) => new ProductCategoryDto
                  {
                    Name = category.Name,
                    ImagePath = Url.RouteUrl("GetImage", new { folder = "products", file = category.ImagePath}),
                    Url = Url.RouteUrl("GetProductCategoryById", new { id = category.Id })
                  }).ToListAsync();

        return new JsonResult(categories, serializerOptions);
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {   
        ProductCategory category = await _context.ProductCategories.FirstOrDefaultAsync(w => w.Id == id);

        if (category == null)
        {
            return new JsonResult(new { result = "NotFound" });
        }
        // Url.Action("Detail","Opinion",new RouteValueDictionary(new{cid=newop.CompanyID,oid=newop.ID}),HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority)

        ProductCategoryDto productCategory = new ProductCategoryDto
        {
            Name = category.Name,
            ImagePath = category.ImagePath
        };

        return new JsonResult(productCategory, serializerOptions);
    }

    [HttpPut("update")]
    public IActionResult Update([FromBody] UpdateProductCategoryDto categoryDto)
    {   
        ProductCategory category = _context.ProductCategories.FirstOrDefault(w => w.Id == categoryDto.Id);

        if (category == null)
        {
            return new JsonResult(new { result = "NotFound" });
        }
        category.Name = categoryDto.Name;
        category.ImagePath = categoryDto.ImagePath;
        
        _context.SaveChanges();

        return new JsonResult(new { result = "Success" });
    }

    [HttpDelete("remove/{id}")]
    public IActionResult Remove([FromRoute] int id)
    {       
        ProductCategory category = _context.ProductCategories.FirstOrDefault(w => w.Id == id);

        if (category == null)
        {
            return new JsonResult(new { result = "NotFound" });
        }

        _context.ProductCategories.Remove(category);
        _context.SaveChanges();
        
        return new JsonResult(new { result = "Success" });
    }
}