using GloriousCore.Models.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace GloriousCore.Models.Data
{
    public class Db : DbContext
    {
        public Db(DbContextOptions<Db> options) : base(options) { }

        public DbSet<CategoryDBO> Categories { get; set; }
        public DbSet<MaterialDBO> Materials { get; set; }
        public DbSet<ProductDBO> Products { get; set; }
        public DbSet<GalleryDBO> ProductGallery { get; set; }
        public DbSet<LoginDBO> Users { get; set; }
        public DbSet<OrderDBO> Orders { get; set; }
        public DbSet<SectionDBO> Sections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        }
    }

    public class ContextFactory : IDesignTimeDbContextFactory<Db>
    {
        public Db CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Db>();
            optionsBuilder.UseNpgsql("Data Source=db.db");

            return new Db(optionsBuilder.Options);
        }
    }
}
