using GloriousCore.Models.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloriousCore.Models.Data
{
    public class Db: DbContext
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {

        }

        public DbSet<CategoryDBO> Categories { get; set; }
        public DbSet<MaterialDBO> Materials { get; set; }
        public DbSet<ProductDBO> Products { get; set; }
        public DbSet<GalleryDBO> ProductGallery { get; set; }
        public DbSet<LoginDBO> Logins { get; set; }
        public DbSet<OrderDBO> Orders { get; set; }
        public DbSet<SectionDBO> Sections { get; set; }
    }
}
