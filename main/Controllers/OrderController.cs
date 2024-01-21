using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Encodings.Web;
using delivery.DataTransferObjects;
using delivery.Models;



namespace delivery.Controllers;

[ApiController]
[Route("admin/order")]
[Route("manager/order")]
public class OrderController : ControllerBase
{
    private readonly JsonSerializerOptions serializerOptions;

    private DeliveryContext _context;

    private readonly IWebHostEnvironment _environment;
    
    public OrderController(DeliveryContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        
        serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public IActionResult Create([FromBody] OrderRequestDto orderDto)
    {   
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        Order order = new Order
        {
            CustomerId = orderDto.CustomerId,
            PaymentMethodId = orderDto.PaymentMethodId,
            DeliveryMethodId = orderDto.DeliveryMethodId,
            StatusId = orderDto.StatusId,
            DeliveryDate = dateTime.AddSeconds(orderDto.DeliveryTimeStamp),
            DateTimeCreation = DateTime.Now,
            Comment = orderDto.Comment
        };
        _context.Orders.Add(order);

        using (var transaction = _context.Database.BeginTransaction())
        {
            _context.SaveChanges();

            try
            {
                if (orderDto.Delivery != null)
                {
                    OrderDelivery delivery = new OrderDelivery
                    {
                        City = orderDto.Delivery.City,
                        Street = orderDto.Delivery.Street,
                        House = orderDto.Delivery.House,
                        Entrance = orderDto.Delivery.Entrance,
                        Apartment = orderDto.Delivery.Apartment,
                        OrderInfo = order
                    };
                    _context.OrderDeliveries.Add(delivery);

                    _context.SaveChanges();
                }

                foreach (var item in orderDto.Items)
                {
                    OrderItem newItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId
                    };
                    _context.OrderItems.Add(newItem);

                    _context.SaveChanges();

                    foreach (var modId in item.Modificators)
                    {
                        OrderModificator modificator = new OrderModificator
                        {
                            ModificatorId = modId,
                            OrderItemId = newItem.Id
                        };
                        _context.OrderModificators.Add(modificator);

                        _context.SaveChanges();
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
        }

        return Created(Url.RouteUrl("GetOrderById", new {id = order.Id}),
                        new { result = "success" });
    }

    [HttpGet("get/{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var order = _context.Orders.Where(w => w.Id == id)
            .Select(order => new Orderdto
                {
                    CustomerId = order.CustomerId,
                    PaymentMethodId = order.PaymentMethodId,
                    DeliveryMethodId = order.DeliveryMethodId,
                    CreationTimeStamp = ((DateTimeOffset)order.DateTimeCreation).ToUnixTimeSeconds().ToString(),
                    DeliveryTimeStamp = ((DateTimeOffset)order.DeliveryDate).ToUnixTimeSeconds().ToString(),
                    Items = order.OrderItems.Select(item => new OrderItemDto
                        {
                            ProductId = item.ProductId,
                            Name = item.Product.Name,
                            Modificators = item.Modificators == null ? null : 
                                item.Modificators.Select(mod => new Modificatordto
                                {
                                    Id = mod.ModificatorId,
                                    Name = mod.Modificator.Name
                                }).ToList()
                        }).ToList(),
                    Delivery = order.OrderDelivery == null ? null :
                    new DeliveryDto
                    {
                        City = order.OrderDelivery.City,
                        Street = order.OrderDelivery.Street,
                        House = order.OrderDelivery.House,
                        Entrance = order.OrderDelivery.Entrance,
                        Apartment = order.OrderDelivery.Apartment
                    } 
                });

        return new JsonResult(order, serializerOptions);
    }
}
