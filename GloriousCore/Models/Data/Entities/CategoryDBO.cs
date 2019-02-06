using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GloriousCore.Models.Data.Entities
{
    [Table("tblCategories")]
    public class CategoryDBO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }

        [ForeignKey("SectionId")]
        public virtual SectionDBO Section { get; set; }
    }
}
