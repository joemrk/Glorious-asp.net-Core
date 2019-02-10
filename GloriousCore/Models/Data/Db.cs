using GloriousCore.Models.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloriousCore.Models.Data
{
    public class Db: DbContext
    {
        //public Db(DbContextOptions<Db> options) : base(options) { }

        public DbSet<CategoryDBO> Categories { get; set; }
        public DbSet<MaterialDBO> Materials { get; set; }
        public DbSet<ProductDBO> Products { get; set; }
        public DbSet<GalleryDBO> ProductGallery { get; set; }
        public DbSet<LoginDBO> Users { get; set; }
        public DbSet<OrderDBO> Orders { get; set; }
        public DbSet<SectionDBO> Sections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joe\Desktop\GloriousCore\GloriousCore\AppData\Db.mdf;Integrated Security=True;Connect Timeout=30");
        }
    }
}
