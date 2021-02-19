using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using ReadingIsGood.ProductService.Entities;
using ReadingIsGood.Shared;

namespace ReadingIsGood.ProductService.Context
{
    public class ProductServiceDbContext : DbContext
    {
        private const string SchemaName = Constants.ProductServiceSchemaName;

        public ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    ApplicationConfiguration.Instance.GetValue<string>("ProductService:DatabaseConnectionString"),
                    builder => builder.EnableRetryOnFailure(3));
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
                Database.EnsureCreated();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SchemaName);

            modelBuilder.Entity<Product>().ToTable("Product").HasKey(t => t.Id);
            modelBuilder.Entity<Product>().OwnsOne(t => t.AuditInformation);
            modelBuilder.Entity<Product>().Property(t => t.Stock).IsConcurrencyToken();
            modelBuilder.Entity<Product>().Property(t => t.Name).HasMaxLength(100).HasColumnType("VARCHAR(100)");
        }
    }

    /// <summary>
    ///     DesingTimeContextFactory
    /// </summary>
    public class DesingTimeContextFactory : IDesignTimeDbContextFactory<ProductServiceDbContext>
    {
        public ProductServiceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductServiceDbContext>();
            optionsBuilder.UseSqlServer(
                ApplicationConfiguration.Instance.GetValue<string>("ProductService:DatabaseConnectionString"),
                builder => builder.EnableRetryOnFailure(3));
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

            return new ProductServiceDbContext(optionsBuilder.Options);
        }
    }
}