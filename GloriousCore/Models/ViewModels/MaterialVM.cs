using GloriousCore.Models.Data.Entities;
using System.Collections.Generic;

namespace GloriousCore.Models.ViewModels
{
    public class MaterialVM
    {
        public ICollection<ProductVM> Products { get; set; }
        public MaterialVM()
        {
            Products = new List<ProductVM>();
        }

        public MaterialVM(MaterialDBO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}
