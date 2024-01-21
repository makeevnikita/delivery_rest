using System.ComponentModel.DataAnnotations;

namespace delivery.DataTransferObjects;

public class ProductCategoryDto
{
    public string Name { get; set; }

    public string ImagePath { get; set; }

    public string Url { get; set; }
}

public class UpdateProductCategoryDto
{
    [Range(1, Double.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int? Id { get; set; }

    public string Name { get; set; }

    public string ImagePath { get; set; }

    public int[]? AddedProductsId { get; set; }

    public int[]? DeletedProductsId { get; set; }
}

public class ProductCategoryDetailed
{
    public int? Id { get; set; }

    public string Name { get; set; }

    public string ImagePath { get; set; }
}

public class CategoryFilter
{
    public int Page { get; set; } = 1;

    public string Name { get; set; }

    public bool SortByCapacity { get; set; }

    public bool SortByCapacityDesc { get; set; }
}

public class CategoriesList
{
    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public List<ProductCategoryDto> Categories { get; set; }
}