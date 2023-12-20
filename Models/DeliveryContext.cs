using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace delivery.Models;

public class Product
{
    public int Id { get; set; }

    public bool IsActive { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public string ImagePath { get; set; }

    public string Description { get; set; }

    public int CategoryId { get; set; }

    public ProductCategory Category { get; set; }

    public List<Modificator> Modificators { get; set; }

    public List<OrderItem> OrderItems { get; set; }
}

public class ProductCategory
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string ImagePath { get; set; }

    public List<Product> Products { get; set; }
}

public class Modificator
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string ImagePath { get; set; }

    public decimal Price { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; }

    public List<OrderModificator> OrderModificators { get; set; }
}

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public List<Order> Orders { get; set; } 
}

public class PaymentMethod
{
    public int Id { get; set; }

    public string Name { get; set; }
}

public class DeliveryMethod
{
    public int Id { get; set; }

    public string Name { get; set; }
}

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; }

    public int PaymentMethodId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public int DeliveryMethodId { get; set; }

    public DeliveryMethod DeliveryMethod { get; set; }

    public DateTime DateTimeCreation { get; set; }

    public DateTime DeliveryDate { get; set; }

    public string Comment { get; set; }

    public OrderDelivery OrderDelivery { get; set; }

    public List<OrderItem> OrderItems { get; set; }

    public int StatusId { get; set; }

    public OrderStatus Status { get; set; }
}

public class OrderDelivery
{
    public int Id { get; set; }

    public string City { get; set; }

    public string Street { get; set; }

    public string House { get; set; }

    public string? Entrance { get; set; }

    public string? Apartment { get; set; }

    public int OrderInfoId { get; set; }

    public Order OrderInfo { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; }

    public List<OrderModificator> Modificators { get; set; }
}

public class OrderModificator
{
    public int Id { get; set; }

    public int ModificatorId { get; set; }

    public Modificator Modificator { get; set; }

    public int OrderItemId { get; set; }

    public OrderItem OrderItem { get; set; }
}

public class OrderStatus
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<Order> Orders { get; set; } 
}

public class DeliveryContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }

    public DbSet<Modificator> Modificators { get; set; }

    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    public DbSet<DeliveryMethod> DeliveryMethods { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<OrderDelivery> OrderDeliveries { get; set; }

    public DbSet<OrderModificator> OrderModificators { get; set; }

    public DeliveryContext(DbContextOptions<DeliveryContext> options) : base(options)
    {

    }   
     
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Product>().Property(p => p.CategoryId).IsRequired(false);

        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.IsActive).IsRequired();
        builder.Property(w => w.Name).HasColumnType("varchar(100)").IsRequired();
        builder.Property(w => w.Price).HasColumnName("numeric(6,2)").IsRequired();
        builder.Property(w => w.ImagePath).IsRequired();
        builder.Property(w => w.Description).HasColumnType("text");
        
        builder.HasOne(w => w.Category)
            .WithMany(w => w.Products)
            .HasForeignKey(w => w.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.CustomerId)
            .IsRequired();

        builder.Property(w => w.PaymentMethodId)
            .IsRequired();

        builder.Property(w => w.DeliveryMethodId)
            .IsRequired();

        builder.Property(w => w.DateTimeCreation)
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(w => w.DeliveryDate)
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(w => w.Comment)
            .HasColumnType("varchar(400)");

        builder.Property(w => w.StatusId)
            .IsRequired();

        builder.HasOne(w => w.OrderDelivery)
            .WithOne(w => w.OrderInfo)
            .HasForeignKey<OrderDelivery>(w => w.OrderInfoId);
    }
}