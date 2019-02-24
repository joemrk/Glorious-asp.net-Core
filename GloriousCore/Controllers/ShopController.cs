using GloriousCore.Helpers;
using GloriousCore.Models;
using GloriousCore.Models.Data;
using GloriousCore.Models.Data.Entities;
using GloriousCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;
using System;
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

        [HttpGet("gallery/{id}")]
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

        [HttpGet("product/{pc}")]
        public IActionResult GetProduct(string pc)
        {
            CartLong();

            int id = Convert.ToInt32(pc.Substring(4));

            ProductVM product;
            using (Db db = new Db())
            {
                ProductDBO dto = db.Products.Find(id);
                product = new ProductVM(dto);
            }

            List<GalleryVM> gallery;
            List<int> ids = new List<int>();
            using (Db db = new Db())
            {
                gallery = db.ProductGallery
                            .ToArray()
                            .Where(x => x.ProductId == id)
                            .Select(x => new GalleryVM(x))
                            .ToList();
            }

            foreach (var item in gallery)
            {
                ids.Add(item.Id);
            }
            ViewBag.Ids = ids;

            if (Request.Headers["Referer"].ToString() == null)
                ViewBag.Ref = "glorious.com.ua";
            else
                ViewBag.Ref = Request.Headers["Referer"].ToString();

            return View(product);
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
            CartLong();
            return View();
        }
        [HttpGet("agreement")]
        public IActionResult Agreement()
        {
            CartLong();
            return View();
        }
        [HttpGet("sketch")]
        public IActionResult Sketch()
        {
            CartLong();
            return View();
        }
        [HttpGet("payments")]
        public IActionResult payments()
        {
            CartLong();
            return View();
        }
        [HttpGet("about")]
        public IActionResult About()
        {
            using (Db db = new Db())
            {
                ViewBag.ProductCounter = db.Products.Count();
                ViewBag.OrderCounter = 23;
            }

            CartLong();
            return View();
        }
        [HttpGet("book")]
        public IActionResult Book()
        {
            CartLong();
            return View();
        }
        [HttpGet("warranty")]
        public IActionResult Warranty()
        {
            CartLong();
            return View();
        }
        [HttpGet("terms")]
        public IActionResult Terms()
        {
            CartLong();
            return View();
        }
    }
}