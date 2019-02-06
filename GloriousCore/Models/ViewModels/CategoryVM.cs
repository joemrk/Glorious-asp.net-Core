using GloriousCore.Models.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GloriousCore.Models.ViewModels
{
    public class CategoryVM
    {
        public ICollection<ProductVM> Products { get; set; }
        public CategoryVM()
        {
            Products = new List<ProductVM>();

        }
        public CategoryVM(CategoryDBO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
            SectionId = row.SectionId;
            SectionName = row.SectionName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }

        public IEnumerable<SelectListItem> Sections { get; set; }
    }
}
