﻿using GloriousCore.Models.Data;
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
        private readonly Db db;

        public ShopController(Db db)
        {
            this.db = db;
        }

        //not used
        public IActionResult Index()
        {
            return View();
        }

        //[Route("/products")]
        [HttpGet("products")]
        public IActionResult Products(int? page, int? catId, int? matId)
        {
            var pageNumber = page ?? 1;

            ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            ViewBag.Materials = new SelectList(db.Materials.ToList(), "Id", "Name");

            ViewBag.SelectedCat = catId.ToString();
            ViewBag.SelectedMat = matId.ToString();


            PagedList<ProductVM> Pp = new PagedList<ProductVM>(db.Products
                .Where(x => (catId == null || catId == 0 || x.CategoryId == catId) && (matId == null || matId == 0 || x.MaterialId == matId))
                .Select(x => new ProductVM(x))
                , pageNumber, 3);

            return View(Pp);
        }

        public FileContentResult GetPreview(int id)
        {
            ProductDBO img = db.Products.Find(id);

            if (img != null)
            {
                return File(img.Img, img.ImgType);
            }
            else
            {
                return null;
            }
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            ViewData["Title"] = "Контакты";

            return View();
        }
    }
}