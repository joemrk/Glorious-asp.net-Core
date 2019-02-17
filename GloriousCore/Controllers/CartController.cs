using GloriousCore.Helpers;
using GloriousCore.Models;
using GloriousCore.Models.Data;
using GloriousCore.Models.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace GloriousCore.Controllers
{
    public class CartController : Controller
    {
        [Route("cart")]
        [HttpGet]
        public IActionResult Cart()
        {
            var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");

            ViewBag.Cart = cart;
            try
            {
                ViewBag.Total = cart.Sum(p => p.Product.Price * p.Quantity);
            }
            catch
            {
                ViewBag.Total = 0;
            }

            return View();
        }
        [Route("add")]
        [HttpPost]
        public IActionResult Add(int id, int a, int amount = 1)
        {
            if (amount > 0 && id > 0 && (a == 1 || a == 2))
            {
                using (Db db = new Db())
                {
                    if (SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart") == null)
                    {
                        var cart = new List<CartLine>();
                        cart.Add(new CartLine()
                        {
                            Product = db.Products.Find(id),
                            Quantity = amount
                        });
                        SessionHelper.Set(HttpContext.Session, "cart", cart);
                    }
                    else
                    {
                        var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");
                        int index = Exists(cart, id);
                        if (index == -1)
                        {
                            cart.Add(new CartLine()
                            {
                                Product = db.Products.Find(id),
                                Quantity = amount
                            });
                        }
                        else
                        {
                            cart[index].Quantity += amount;
                        }
                        SessionHelper.Set(HttpContext.Session, "cart", cart);
                    }
                }

                if (a == 1)
                    return Redirect("/cart");
                else
                    return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public int Exists(List<CartLine> cart, int id)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Edit(int id, int amount)
        {
            if (id > 0 && amount > 0)
            {
                var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");
                int index = Exists(cart, id);
                cart[index].Quantity = amount;
                SessionHelper.Set(HttpContext.Session, "cart", cart);
            }
        }
        [Route("del")]
        public IActionResult Del(int id)
        {
            if (id>0)
            {
                var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");
                for (int i = 0; i < cart.Count; i++)
                {
                    if (cart[i].Product.Id == id)
                    {
                        cart.Remove(cart[i]);
                        break;
                    }
                }
                SessionHelper.Set(HttpContext.Session, "cart", cart);
                return Redirect("/cart");
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public IActionResult Buy(string name, string num, string email, string city, string post, string adres, string note)
        {
            #region  shitCode, FIXED /////////////////////////////////////////
            int conrtrol = 0;

            List<string> model = new List<string>();

            model.Add(name);
            model.Add(num);
            model.Add(email);
            model.Add(city);
            model.Add(post);
            model.Add(adres);
            model.Add(note);

            foreach (string item in model)
            {
                bool result = !(item.Contains("{") ||
                                item.Contains("}") ||
                                item.Contains("<") ||
                                item.Contains(">") ||
                                item.Contains("&") ||
                                item.Contains("#") ||
                                item.Contains("$"));
                if (!result)
                {
                    conrtrol += 1;
                }
            }
            #endregion
            if (conrtrol == 0)
            {
                MailAddress from = new MailAddress("glorius.order@gmail.com", "Glorius new order");
                //MailAddress to = new MailAddress("porka23@i.ua");
                MailAddress to = new MailAddress("o9ino4ka@gmail.com");
                var pageHtml = HttpContext.Request;

                using (MailMessage message = new MailMessage(from, to))
                using (SmtpClient smtp = new SmtpClient())
                {
                    message.Subject = "Новый заказ";
                    message.IsBodyHtml = true;
                    //message.Body = str + "\n" + "\n" + name + "\n" + "\n" + "380" + num + "\n" + "\n" + email + "\n" + "\n" + city + "\n" + "\n" + post + "\n" + "\n" + adres + "\n" + "\n" + note;

                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(from.Address, "gloriusorder7878");

                    smtp.Send(message);
                }

                HttpContext.Session.Clear();

                return RedirectToAction("buyPost", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Введен запрещенный символ");
                return Redirect("/cart");
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

        [HttpGet("getpreview")]
        public FileContentResult GetPreview(int id)
        {
            using (Db db = new Db())
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
        }
    }
}