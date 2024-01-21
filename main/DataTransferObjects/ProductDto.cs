using System.ComponentModel.DataAnnotations;

namespace delivery.DataTransferObjects;

public class ProductList
{
    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public List<ProductDto> Products { get; set; }
}

public class ProductDto
{
    public string Name { get; set; }//

    public string ImagePath { get; set; }//

    public decimal Price { get; set; }//

    public int Id { get; set; }//
}

public class UpdateProductDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "IsActive cannot be null")]
    public bool IsActive { get; set; }

    [Required(ErrorMessage = "Name cannot be null")]
    public string Name { get; set; }//
    
    [Range(0, Double.MaxValue, ErrorMessage = "Price must be greater than or equal to zero")]
    public decimal Price { get; set; }//

    public IFormFile Image { get; set; }

    public string Description { get; set; }//

    [Range(1, Double.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
    public int CategoryId { get; set;}
}

public class ProductDtoDetailed
{
    public int Id { get; set; }

    public string Name { get; set; }//

    public string ImagePath { get; set; }//

    public decimal Price { get; set; }//

    public string Description { get; set; }//

    public string CategoryName { get; set; }

    public bool IsActive { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; }//

    public decimal Price { get; set; }//

    public string Description { get; set; }//

    public int CategoryId { get; set; }

    public bool IsActive { get; set; }

    public IFormFile Image { get; set; }
}

public class ProductFilterDto
{
    public int Page { get; set; } = 1;

    public string? Name { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? CategoryId { get; set; }

    public bool SortByPrice { get; set; }

    public bool SortByPriceDesc { get; set; }
}