using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GloriousCore.Models.Data;
using GloriousCore.Models.Data.Entities;
using GloriousCore.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;

namespace GloriousCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        //private readonly Db db;

        //public DashBoardController(Db db)
        //{
        //    this.db = db;
        //}

        /// <summary>
        /// PRODUCTS
        /// </summary>
        /// <returns></returns>
        //[HttpGet("addproduct")]
        public IActionResult AddProduct(int? secId)
        {
            ProductVM model = new ProductVM();
            List<CategoryVM> categoriesList;

            using (Db db = new Db())
            {
                categoriesList = db.Categories.ToArray()
               .Where(x => (secId == null || secId == 0 || x.SectionId == secId))
               .Select(x => new CategoryVM(x))
               .ToList();

                model.Categories = new SelectList(categoriesList, "Id", "Name");
                model.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");
                model.Sections = new SelectList(db.Sections.ToList(), "Id", "Name");

                ViewBag.SelectedSec = secId.ToString();
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductVM model, IFormFile file = null)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    model.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");
                }

                return View(model);

            }

            if (file != null && file.Length > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        model.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");
                    }

                    ModelState.AddModelError("", "Формат файла неподдерживается. Доступные форматы: jpg,jpeg,pjpeg,gif,x-png,png");
                    return View(model);
                }
            }
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    model.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");

                    ModelState.AddModelError("", "Имя уже существует!");
                    return View(model);
                }

                int id;

                ProductDBO dto = new ProductDBO
                {
                    Name = model.Name,
                    Slug = model.Name.Replace(" ", "-").ToLower(),
                    Description = model.Description,
                    Price = model.Price,
                    Discount = model.Discount,
                    ProductCode = "1",
                    CategoryId = model.CategoryId,
                    MaterialId = model.MaterialId,
                    SectionId = model.SectionId,
                    SectionName = model.SectionName
                };

                if (file != null && file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        dto.Img = stream.ToArray();
                        dto.ImgType = file.ContentType;
                    }
                }

                CategoryDBO catDTo = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTo.Name;

                MaterialDBO matDTO = db.Materials.FirstOrDefault(x => x.Id == model.MaterialId);
                dto.MaterialName = matDTO.Name;

                SectionDBO secDTO = db.Sections.FirstOrDefault(x => x.Id == model.SectionId);
                dto.SectionName = secDTO.Name;

                db.Products.Add(dto);
                db.SaveChanges();

                id = db.Products.Max(i => i.Id);
                string prodCode = model.SectionId.ToString("00") + model.CategoryId.ToString("00") + id.ToString("0000");
                ProductDBO dto_ = db.Products.Find(id);

                dto_.ProductCode = prodCode;
                db.SaveChanges();
            }


            TempData["SM"] = "Продукт добавлен!";

            return RedirectToAction("AddProduct");
        }

        public IActionResult Products(int? page, int? catId, int? matId)
        {
            PagedList<ProductVM> Pp;
            var pageNumber = page ?? 1;
            using (Db db = new Db())
            {
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                ViewBag.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");

                ViewBag.SelectedCat = catId.ToString();
                ViewBag.SelectedMat = matId.ToString();

                Pp = new PagedList<ProductVM>(db.Products
                   .Where(x => (catId == null || catId == 0 || x.CategoryId == catId) && (matId == null || matId == 0 || x.MaterialId == matId))
                   .Select(x => new ProductVM(x))
                   , pageNumber, 3);
            }

            return View(Pp);
        }
        [HttpGet]
        public IActionResult EditProduct(int id, int? secId)
        {
            List<CategoryVM> categoriesList;

            ProductVM model;

            using (Db db = new Db())
            {
                ProductDBO dto = db.Products.Find(id);

                if (dto == null)
                {
                    return Content("Продукт не найден!");
                }
                categoriesList = db.Categories.ToArray()
                   .Where(x => (secId == null || secId == 0 || x.SectionId == secId))
                   .Select(x => new CategoryVM(x))
                   .ToList();

                model = new ProductVM(dto);

                model.Categories = new SelectList(categoriesList, "Id", "Name");
                model.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");
                model.Sections = new SelectList(db.Sections.ToList(), "Id", "Name");

                ViewBag.SelectedSec = dto.SectionId.ToString();

                //GALLRY///////////////////////////////////////
                List<GalleryVM> gallery;
                List<int> ids = new List<int>();

                gallery = db.ProductGallery
                    .ToArray()
                    .Where(x => x.ProductId == id)
                    .Select(x => new GalleryVM(x))
                    .ToList();

                foreach (var item in gallery)
                {
                    ids.Add(item.Id);
                }
                ViewBag.Ids = ids;
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductVM model, IFormFile file = null)
        {
            using (Db db = new Db())
            {
                int id = model.Id;

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                model.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");

                if (file != null && file.Length > 0)
                {
                    string ext = file.ContentType.ToLower();

                    if (ext != "image/jpg" &&
                        ext != "image/jpeg" &&
                        ext != "image/pjpeg" &&
                        ext != "image/gif" &&
                        ext != "image/x-png" &&
                        ext != "image/png")
                    {

                        ModelState.AddModelError("", "Формат файла неподдерживается. Доступные форматы: jpg,jpeg,pjpeg,gif,x-png,png");
                        return View(model);
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Имя уже существует");
                }

                ProductDBO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.Discount = model.Discount;
                dto.CategoryId = model.CategoryId;
                dto.MaterialId = model.MaterialId;
                dto.SectionId = model.SectionId;
                dto.SectionName = model.SectionName;
                if (file != null && file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        dto.Img = stream.ToArray();
                        dto.ImgType = file.ContentType;
                    }
                }
                CategoryDBO catDTo = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTo.Name;

                MaterialDBO matDTO = db.Materials.FirstOrDefault(x => x.Id == model.MaterialId);
                dto.MaterialName = matDTO.Name;

                SectionDBO secDTO = db.Sections.FirstOrDefault(x => x.Id == model.SectionId);
                dto.SectionName = secDTO.Name;

                db.SaveChanges();
            }


            TempData["SM"] = "Продукт изменен!";

            return RedirectToAction("Products");
        }

        public IActionResult DeleteProduct(int id)
        {
            using (Db db = new Db())
            {
                ProductDBO dto = db.Products.Find(id);

                db.Products.Remove(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "Продукт удален!";

            return RedirectToAction("Products");
        }
        /// <summary>
        /// SECTION
        /// </summary>
        /// <returns></returns>
        public IActionResult Sections()
        {
            List<SectionVM> SectionVMList;
            using (Db db = new Db())
            {
                SectionVMList = db.Sections
                .ToArray()
                .OrderBy(x => x.Sorting)
                .Select(x => new SectionVM(x))
                .ToList();

                ViewBag.Sections = new SelectList(db.Sections.ToList(), "Id", "Name");
            }

            return View(SectionVMList);
        }
        [HttpPost]
        public string AddNewSection(string secName)
        {
            string id;
            using (Db db = new Db())
            {
                if (db.Sections.Any(x => x.Name == secName))
                {
                    return "titletaken";
                }
                SectionDBO dto = new SectionDBO();

                dto.Name = secName;
                dto.Slug = secName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.Sections.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();

                return id;
            }
        }
        public IActionResult DeleteSection(int id)
        {
            using (Db db = new Db())
            {
                SectionDBO dto = db.Sections.Find(id);

                db.Sections.Remove(dto);
                db.SaveChanges();
            }

            return RedirectToAction("Sections");
        }
        [HttpPost]
        public string RenameSections(string newSecName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Sections.Any(x => x.Name == newSecName))
                    return "Имя существует";
                var dto = db.Categories.Find(id);
                dto.Name = newSecName;
                dto.Slug = newSecName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }

            return "okay";
        }

        /// <summary>
        /// CATEGORIES
        /// </summary>
        /// <returns></returns>
        public IActionResult Categories()
        {
            List<CategoryVM> CategoryVMList;
            using (Db db = new Db())
            {
                CategoryVMList = db.Categories
                .ToArray()
                .OrderBy(x => x.Sorting)
                .Select(x => new CategoryVM(x))
                .ToList();
            }

            return View(CategoryVMList);
        }

        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }
                CategoryDBO dto = new CategoryDBO
                {
                    Name = catName,
                    Slug = catName.Replace(" ", "-").ToLower(),
                    Sorting = 100
                };

                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();
            }

            return id;
        }

        public IActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDBO dto = db.Categories.Find(id);

                db.Categories.Remove(dto);
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }

        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "Имя существует";
                var dto = db.Categories.Find(id);
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }

            return "okay";
        }

        /// <summary>
        /// MATERIALS
        /// </summary>
        /// <returns></returns>
        public IActionResult Materials()
        {
            List<MaterialVM> MaterialVMList;
            using (Db db = new Db())
            {
                MaterialVMList = db.Materials
               .ToArray()
               .OrderBy(x => x.Sorting)
               .Select(x => new MaterialVM(x))
               .ToList();
            }

            return View(MaterialVMList);
        }
        [HttpPost]
        public string AddNewMaterial(string matName)
        {
            string id;
            using (Db db = new Db())
            {
                if (db.Materials.Any(x => x.Name == matName))
                {
                    return "Имя существует";
                }
                MaterialDBO dto = new MaterialDBO
                {
                    Name = matName,
                    Slug = matName.Replace(" ", "-").ToLower(),
                    Sorting = 100
                };

                db.Materials.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();
            }

            return id;
        }

        public IActionResult DeleteMaterial(int id)
        {
            using (Db db = new Db())
            {
                MaterialDBO dto = db.Materials.Find(id);

                db.Materials.Remove(dto);
                db.SaveChanges();
            }

            return RedirectToAction("Materials");
        }

        public string RenameMaterial(string newMatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Materials.Any(c => c.Name == newMatName))
                    return "Имя существует";

                MaterialDBO dto = db.Materials.Find(id);

                dto.Name = newMatName;
                dto.Slug = newMatName.Replace(" ", "-").ToLower();

                db.SaveChanges();
            }

            return "ok";
        }

        /// <summary>
        /// OTHER METHODS
        /// </summary>
        /// <param name="id"></param>

        [HttpPost]
        public async void SaveGallaryImages(int id, IFormFile file) //не работает при создании товара т.к. у товора до создания нету ИД
        {
            using (Db db = new Db())
            {
                if (file != null && file.Length > 0)
                {
                    GalleryDBO dto = new GalleryDBO();

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        dto.ProductId = id;
                        dto.Img = stream.ToArray();
                        dto.ImgType = file.ContentType;
                    }

                    db.ProductGallery.Add(dto);
                    db.SaveChanges();

                }
            }
        }
        //for preview image
        public FileContentResult GetPreview(int id)
        {
            using (Db db = new Db())
            {
                ProductDBO img = db.Products.Find(id);

                if (img != null)
                    return File(img.Img, img.ImgType);
                else
                    return null;
            }
        }

        //for gallery image
        public FileContentResult GetGallery(int id)
        {
            using (Db db = new Db())
            {
                GalleryDBO img = db.ProductGallery.Find(id);

                if (img != null)
                    return File(img.Img, img.ImgType);
                else
                    return null;
            }
        }

        [HttpPost]
        public void DeleteImg(int id)
        {
            using (Db db = new Db())
            {
                GalleryDBO dto = db.ProductGallery.Find(id);
                db.ProductGallery.Remove(dto);
                db.SaveChanges();
            }
        }
    }
}