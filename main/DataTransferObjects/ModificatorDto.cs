namespace delivery.DataTransferObjects;

public class CreateModificatorDto
{
    public string Name { get; set; }

    public IFormFile Image { get; set; }

    public int ProductId { get; set; }
}

public class UpdateModificatorDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IFormFile Image { get; set; }

    public int ProductId { get; set; }
}