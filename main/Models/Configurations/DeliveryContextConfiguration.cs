using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace delivery.Models.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.IsActive).IsRequired();
        builder.Property(w => w.Name).HasColumnType("varchar(100)").IsRequired();
        builder.Property(w => w.Price).HasColumnType("numeric(6,2)").IsRequired();
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

        builder.HasMany(w => w.OrderItems)
            .WithOne(w => w.Order)
            .HasForeignKey(w => w.OrderId);
    }
}

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.ImagePath)
            .IsRequired();

        builder.Property(w => w.Name)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.HasMany(w => w.Products)
            .WithOne(w => w.Category)
            .HasForeignKey(w => w.CategoryId);
    }
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(w => w.Phone)
            .HasColumnType("varchar(11)");

        builder.HasMany(w => w.Orders)
            .WithOne(w => w.Customer)
            .HasForeignKey(w => w.CustomerId);
    }
}

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.HasMany(w => w.Orders)
            .WithOne(w => w.PaymentMethod)
            .HasForeignKey(w => w.PaymentMethodId);
    }
}

public class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
{
    public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.HasMany(w => w.Orders)
            .WithOne(w => w.DeliveryMethod)
            .HasForeignKey(w => w.DeliveryMethodId);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.OrderId)
            .IsRequired();
        
        builder.Property(w => w.ProductId)
            .IsRequired();

        builder.HasOne(w => w.Product)
            .WithMany(w => w.OrderItems);
        
        builder.HasOne(w => w.Order)
            .WithMany(w => w.OrderItems);
    
        builder.HasMany(w => w.Modificators)
            .WithOne(w => w.OrderItem)
            .HasForeignKey(w => w.OrderItemId);
    }
}

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.HasMany(w => w.Orders)
            .WithOne(w => w.Status)
            .HasForeignKey(w => w.StatusId);
    }
}

public class OrderModificatorConfiguration : IEntityTypeConfiguration<OrderModificator>
{
    public void Configure(EntityTypeBuilder<OrderModificator> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.ModificatorId)
            .IsRequired();
        
        builder.Property(w => w.OrderItemId)
            .IsRequired();

        builder.HasOne(w => w.Modificator)
            .WithMany(w => w.OrderModificators)
            .HasForeignKey(w => w.ModificatorId);

        builder.HasOne(w => w.OrderItem)
            .WithMany(w => w.Modificators)
            .HasForeignKey(w => w.OrderItemId);  
    }
}