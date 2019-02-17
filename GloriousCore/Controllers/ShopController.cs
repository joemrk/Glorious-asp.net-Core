using GloriousCore.Helpers;
using GloriousCore.Models;
using GloriousCore.Models.Data;
using GloriousCore.Models.Data.Entities;
using GloriousCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;
using System.Collections.Generic;
using System.Linq;

namespace GloriousCore.Controllers
{
    public class ShopController : Controller
    {
        //not used
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Products(int? page, int? catId, int? matId)
        {
            CartLong();
            var pageNumber = page ?? 1;
            PagedList<ProductVM> Pp;
            using (Db db = new Db())
            {
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                ViewBag.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");

                ViewBag.SelectedCat = catId.ToString();
                ViewBag.SelectedMat = matId.ToString();

                Pp = new PagedList<ProductVM>(db.Products
                     .Where(x => (catId == null || catId == 0 || x.CategoryId == catId) && (matId == null || matId == 0 || x.MaterialId == matId))
                     .Select(x => new ProductVM(x))
                     , pageNumber, 20);
            }           
            return View(Pp);
        }

        public FileContentResult GetPreview(int id)
        {
            using (Db db = new Db())
            {
                ProductDBO img = db.Products.Find(id);

                if (img != null) return File(img.Img, img.ImgType);
                else return null;
            }          
        }

        public void CartLong()
        {
            var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");
            int index = 0;
            if (cart == null)
            {
                index = 0;
            }
            else
            {
                foreach (var item in cart)
                {
                    index += item.Quantity;
                }
                ViewBag.CartLong = index;
            }            
        }

        [Route("search")]
        public IActionResult Search(string str, int? page)
        {
            CartLong();
            PagedList<ProductVM> Pp;
            var pageNumber = page ?? 1;

            var strCheck = !(str.Contains('{') ||
                           str.Contains('}') ||
                           str.Contains('<') ||
                           str.Contains('>') ||
                           str.Contains('&') ||
                           str.Contains('#') ||
                           str.Contains('$'));

            if (strCheck)
            {
                using (Db db = new Db())
                {
                    ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ViewBag.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");

                    Pp = new PagedList<ProductVM>(db.Products
                         .Where(x => x.Name.Contains(str) || x.ProductCode.Contains(str))
                         .Select(x => new ProductVM(x))
                         , pageNumber, 20);
                }
                ViewBag.search = str;
                return View(Pp);
            }
            else return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            ViewData["Title"] = "Контакты";

            return View();
        }
    }
}