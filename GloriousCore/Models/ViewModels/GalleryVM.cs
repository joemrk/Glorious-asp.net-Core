using GloriousCore.Models.Data.Entities;

namespace GloriousCore.Models.ViewModels
{
    public class GalleryVM
    {
        public GalleryVM()
        {

        }
        public GalleryVM(GalleryDBO row)
        {
            Id = row.Id;
            ProductId = row.ProductId;
            Img = row.Img;
            ImgType = row.ImgType;
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public byte[] Img { get; set; }
        public string ImgType { get; set; }
    }
}
