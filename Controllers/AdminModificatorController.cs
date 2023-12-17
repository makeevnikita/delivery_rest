using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Encodings.Web;
using delivery.DataTransferObjects;
using delivery.Models;
using Microsoft.EntityFrameworkCore;



namespace delivery.Controllers;

[ApiController]
[Route("admin/modificator")]
public class AdminModificatorController : ControllerBase
{
    private readonly JsonSerializerOptions serializerOptions;

    private DeliveryContext _context;

    private readonly IWebHostEnvironment _environment;
    
    public AdminModificatorController(DeliveryContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        
        serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    [HttpGet("create")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<IActionResult> Create([FromForm] CreateModificatorDto modificatorDto)
    {   
        Product product = await _context.Products.FirstOrDefaultAsync(w => w.Id == modificatorDto.ProductId);

        if (product == null)
        {
            return NotFound(new { result = "NotFound", description = "Товар не найден"});
        }

        Guid guid = Guid.NewGuid();

        string folder = "products";
        string folderPath = Path.Combine("images", folder, $"{guid}_{modificatorDto.Image.FileName}");
        string filePath = Path.Combine(_environment.WebRootPath, folderPath);

        Modificator modificator = new Modificator
        {
            Name = modificatorDto.Name,
            ImagePath = filePath,
            Product = product
        };

        _context.Modificators.AddAsync(modificator);
        await _context.SaveChangesAsync();

        return Created(Url.RouteUrl("AdminGetModificatorById", new {id = modificator.Id}), new { result = "success" });
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        Modificator modificator = await _context.Modificators.FirstOrDefaultAsync(w => w.Id == id);

        if (modificator == null)
        {
            return NotFound(new { result = "NotFound", description = "Модификатор не найден"});
        }

        return new JsonResult(modificator);
    }

    [HttpGet("remove/{id}")]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {
        Modificator modificator = new Modificator { Id = id };

        _context.Modificators.Attach(modificator);
        _context.Modificators.Remove(modificator);

        try
        {
            await _context.SaveChangesAsync();    
        }
        catch
        {
            return new JsonResult(new { 
                result = "error", 
                description = "Ошибка при удалении товара или модификатора не существует" });   
        }

        return new JsonResult(new { result = "success" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] UpdateModificatorDto modificatorDto)
    {
        Modificator modificator = await _context.Modificators.FirstOrDefaultAsync(w => w.Id == modificatorDto.Id);

        if (modificator == null)
        {
            return NotFound(new { result = "NotFound", description = "Модификатор не найден"});
        }

        if (modificator.Name != modificatorDto.Name)
        {
            modificator.Name = modificatorDto.Name;
        }

        if (modificator.ImagePath != modificatorDto.Image.FileName)
        {
            Guid guid = Guid.NewGuid();

            string folder = "products";
            string folderPath = Path.Combine("images", folder, $"{guid}_{modificatorDto.Image.FileName}");
            string filePath = Path.Combine(_environment.WebRootPath, folderPath);

            modificator.ImagePath = filePath;
        }

        if (modificator.ProductId != modificatorDto.ProductId)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(w => w.Id == modificatorDto.Id);

            if (product != null)
            {
                modificator.Product = product;
            }
            else
            {
                return NotFound(new { result = "NotFound", description = "Товар не найден"});
            }
        }
        
        _context.Modificators.Update(modificator);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            return NotFound(new { result = "Error", description = "Не удалось изменить модификатор"});
        }

        return new JsonResult(new { result = "success", description = "Модификатор успешно обновлён" });
    }
}
