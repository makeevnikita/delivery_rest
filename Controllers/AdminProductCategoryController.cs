using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Encodings.Web;
using delivery.Models;
using delivery.DataTransferObjects;
using Microsoft.EntityFrameworkCore;



namespace delivery.Controllers;

[ApiController]
[Route("admin/product_category")]
public class AdminProductCategoryController : ControllerBase
{
    private readonly JsonSerializerOptions serializerOptions;

    private DeliveryContext _context;

    private readonly IWebHostEnvironment _environment;
    
    public AdminProductCategoryController(DeliveryContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        
        serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    [HttpPost("create")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<IActionResult> Create([FromForm] string name, [FromForm] IFormFile image)
    {   
        Guid guid = Guid.NewGuid();

        string folder = "products";
        string folderPath = Path.Combine("images", folder, $"{guid}_{image.FileName}");
        string filePath = Path.Combine(_environment.WebRootPath, folderPath);

        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        ProductCategory newCategory = new ProductCategory
        {
            Name = name,
            ImagePath = filePath
        };

        await _context.AddAsync(newCategory);
        await _context.SaveChangesAsync();

        return Created(Url.RouteUrl("AdminGetProductCategoryById", new {id = newCategory.Id}),
                        new { result = "success", descripntion = "Категория создана успешно" });
    }
    
    [HttpGet("get")]
    public async Task<IActionResult> Get(int page)
    {   
        int pageSize = 20;
        CategoriesList categoriesList = new CategoriesList();
        
        List<ProductCategoryDto> categories = await _context.ProductCategories.AsNoTracking()
            .Select(category => new ProductCategoryDto
                {
                    Name = category.Name,
                    ImagePath = category.ImagePath,
                    Url = Url.RouteUrl("AdminGetProductCategoryById", new { id = category.Id })
                }).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        categoriesList.Categories = categories;
        categoriesList.TotalCount = categoriesList.Categories.Count;
        categoriesList.PageSize = pageSize;

        return new JsonResult(categories, serializerOptions);
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {   
        ProductCategoryDetailed productCategory = await _context.ProductCategories
            .Select(category => new ProductCategoryDetailed 
                {
                    Id = category.Id,
                    Name = category.Name,
                    ImagePath = category.ImagePath
                }).FirstOrDefaultAsync(w => w.Id == id);

        if (productCategory == null)
        {
            return new JsonResult(new { result = "NotFound", description = "Категория не найдена" });
        }

        return new JsonResult(productCategory, serializerOptions);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateProductCategoryDto categoryDto)
    {   
        ProductCategory category = await _context.ProductCategories.FirstOrDefaultAsync(w => w.Id == categoryDto.Id);
        
        if (category == null)
        {
            return new JsonResult(new { result = "NotFound", description = "Категория не найдена" });
        }

        category.Name = categoryDto.Name;
        category.ImagePath = categoryDto.ImagePath;

        await _context.Products.Where(w => categoryDto.DeletedProductsId.Contains(w.Id))
            .ExecuteUpdateAsync(s=>s.SetProperty(u=>u.CategoryId, u=> null));

        await _context.Products.Where(w => categoryDto.AddedProductsId.Contains(w.Id))
            .ExecuteUpdateAsync(s=>s.SetProperty(u=>u.CategoryId, u=> category.Id));

        return new JsonResult(new { result = "Success", description = "Категория успешно изменена" });
    }

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {       
        ProductCategory category = await _context.ProductCategories.FirstOrDefaultAsync(w => w.Id == id);

        if (category == null)
        {
            return new JsonResult(new { result = "NotFound", description = "Категория не найдена" });
        }

        _context.ProductCategories.Remove(category);
        await _context.SaveChangesAsync();
        
        return new JsonResult(new { result = "Success", description = "Категория удалена успешно" });
    }
}