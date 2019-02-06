using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GloriousCore.Models.Data.Entities
{
    [Table("tblGallery")]
    public class GalleryDBO
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public byte[] Img { get; set; }
        public string ImgType { get; set; }
    }
}
