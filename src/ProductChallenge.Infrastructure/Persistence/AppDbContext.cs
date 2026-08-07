using Microsoft.EntityFrameworkCore;
using ProductChallenge.Domain;

namespace ProductChallenge.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(product => product.Id);
            entity.Property(product => product.Id).ValueGeneratedOnAdd();

            entity.Property(product => product.Name)
                  .IsRequired()
                  .HasMaxLength(Product.NameMaxLength);

            entity.Property(product => product.Description)
                  .HasMaxLength(Product.DescriptionMaxLength);

            // O SQLite não possui tipo decimal nativo; declarar a precisão evita
            // que valores monetários sejam persistidos como ponto flutuante.
            entity.Property(product => product.Price)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            entity.Property(product => product.Category)
                  .IsRequired()
                  .HasMaxLength(40)
                  .HasConversion<string>();

            entity.Property(product => product.StockQuantity)
                  .IsRequired();

            entity.Property(product => product.SearchText)
                  .IsRequired()
                  .HasMaxLength(Product.SearchTextMaxLength);

            entity.HasIndex(product => product.Name);
            entity.HasIndex(product => product.SearchText);
        });
    }
}
