using GloriousCore.Models.Data.Entities;

namespace GloriousCore.Models
{
    public class CartLine
    {
        public ProductDBO Product { get; set; }
        public int Quantity { get; set; }
    }
}
