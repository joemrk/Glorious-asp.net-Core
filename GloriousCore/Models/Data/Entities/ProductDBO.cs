using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GloriousCore.Models.Data.Entities
{
    [Table("tblProducts")]
    public class ProductDBO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string MaterialName { get; set; }
        public int MaterialId { get; set; }
        public byte[] Img { get; set; }
        public string ProductCode { get; set; }
        public string ImgType { get; set; }
        public decimal Discount { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryDBO Category { get; set; }
        [ForeignKey("MaterialId")]
        public virtual MaterialDBO Material { get; set; }
        [ForeignKey("SectionId")]
        public virtual SectionDBO Section { get; set; }
    }

}
