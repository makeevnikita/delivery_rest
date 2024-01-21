namespace delivery.DataTransferObjects;

public class Orderdto
{
    public int CustomerId { get; set; }

    public int PaymentMethodId { get; set; }

    public int DeliveryMethodId { get; set; }

    public List<OrderItemDto> Items { get; set; }

    public string CreationTimeStamp { get; set; }

    public string DeliveryTimeStamp { get; set; }

    public DeliveryDto? Delivery { get; set; }
}

public class DeliveryDto
{
    public string City { get; set; }

    public string Street { get; set; }

    public string House { get; set; }

    public string? Entrance { get; set; }

    public string? Apartment { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }

    public string Name { get; set; }

    public List<Modificatordto>? Modificators { get; set; }
}

public class Modificatordto
{
    public int Id { get; set; }

    public string Name { get; set; }
}

public class OrderRequestDto
{
    public int CustomerId { get; set; }

    public int PaymentMethodId { get; set; }

    public int DeliveryMethodId { get; set; }

    public int StatusId { get; set; }

    public List<OrderItemRequestDto> Items { get; set; }

    public double DeliveryTimeStamp { get; set; }

    public DeliveryDto? Delivery { get; set; }

    public string Comment { get; set; }
}

public class OrderItemRequestDto
{
    public int ProductId { get; set; }

    public int[]? Modificators { get; set; }
}

public class ModificatorRequestDto
{
    public int Id { get; set; }
}

public class DeliveryRequestDto
{
    public string City { get; set; }

    public string Street { get; set; }

    public string House { get; set; }

    public string? Entrance { get; set; }

    public string? Apartment { get; set; }
}