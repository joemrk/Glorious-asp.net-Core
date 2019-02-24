using GloriousCore.Helpers;
using GloriousCore.Models;
using GloriousCore.Models.Data;
using GloriousCore.Models.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GloriousCore.Controllers
{
    public class CartController : Controller
    {
        [Route("cart")]
        [HttpGet]
        public IActionResult Cart()
        {
            CartLong();
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

            return View(cart);
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
            if (id > 0)
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
        [Route("buy")]
        public IActionResult Buy(OrderDBO model)
        {

            //    //content =
            //    //content.Replace("\\", string.Empty).
            //    //Replace("|", string.Empty).
            //    //Replace("(", string.Empty).
            //    //Replace(")", string.Empty).
            //    //Replace("[", string.Empty).
            //    //Replace("]", string.Empty).
            //    //Replace("*", string.Empty).
            //    //Replace("?", string.Empty).
            //    //Replace("}", string.Empty).
            //    //Replace("{", string.Empty).
            //    //Replace("^", string.Empty).
            //    //Replace("+", string.Empty);

            if (ModelState.IsValid)
            {
                MailAddress from = new MailAddress("glorius.order@gmail.com", "Glorius new order");
                MailAddress to = new MailAddress("o9ino4ka@gmail.com");

                using (MailMessage message = new MailMessage(from, to))
                using (SmtpClient smtp = new SmtpClient())
                {
                    message.Subject = "Новый заказ";
                    message.IsBodyHtml = true;

                    message.Body = HtmlCart(model);
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(from.Address, "gloriusorder7878");

                    smtp.Send(message);
                }

                HttpContext.Session.Clear();

                return Redirect("/buy_");
            }
            else
            {
                ModelState.AddModelError("", "Введен запрещенный символ");
                return Redirect("/cart");
            }
        }
        [HttpGet("buy_")]
        public IActionResult Buy_()
        {
            return View();
        }

        public string HtmlCart(OrderDBO model)
        {
            var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");
            
            string cartLine = "";
            int total = 0;

            foreach (var item in cart)
            {
                int sum = 0;
                if (item.Product.Discount != 0) {
                    sum = (item.Quantity * (int)item.Product.Discount);
                    total = total + (item.Quantity * (int)item.Product.Discount);
                }
                else {
                    sum = (item.Quantity * (int)item.Product.Price);
                    total = total + (item.Quantity * (int)item.Product.Price);
                }

                cartLine += String.Format(@"
                        <tr style=""text-align: center; border-bottom: 1px solid #000000"">
						<td>{0}</td>
						<td><a href=""glorious.com.ua/product/{0}"" style=""text-decoration: none;"">{1}</a></td>
						<td>{2}</td>
						<td>{3}</td>
					    </tr>", item.Product.ProductCode, item.Product.Name, item.Quantity.ToString(), sum.ToString("0.00"));
            }

            string result = String.Format(@"<table bgcolor=""#F7F7F7"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""margin:0; padding:0 "" width=""100%"">
		<tr style=""display: grid;"">
			<td height=""100%"" style=""margin: 10px"">
				<table style=""border-collapse: collapse; font-family: sans-serif; padding: 20px; box-shadow: 5px 5px 5px grey; background-color: #fff; margin: 0 auto;"" cellpadding=""10px"">
					<tr style=""background-color: #000000; color: #ffffff;"">
						<th>Code</th>
						<th>Name</th>
						<th>Quantity</th>
						<th>Sum</th>
					</tr>
					{0}
					<tr style=""border-bottom: 2px solid #000000""><td colspan=""4"" style=""font-weight: bold; text-align: right;"">Total: {1}</td></tr>
				</table>
			</td>
			<td height=""100%"" style=""margin: 10px"">
				<table style=""border-collapse: collapse; font-family: sans-serif; padding: 20px; box-shadow: 5px 5px 5px grey; background-color: #fff; margin: 0 auto;"" cellpadding=""10px"">
					<tr>
						<td>ФИО: </td>
						<td>{2}</td>
					</tr>
					<tr>
						<td>Телефон: </td>
						<td>{3}</td>
					</tr>
					<tr>
						<td>Email:</td>
						<td>{4}</td>
					</tr>
					<tr>
						<td>Город: </td>
						<td>{5}</td>
					</tr>
					<tr>
						<td>Служба доставки: </td>
						<td>{6}</td>
					</tr>
					<tr>
						<td>Адрес/отделение/почтовый индекс: </td>
						<td>{7}</td>
					</tr>
					<tr>
						<td>Дополнительная информация: </td>
						<td style=""max-width: 400px;"">{8}</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>", cartLine, total.ToString("0.00"), model.Name, model.Number, model.Mail, model.City, model.Addres, model.Post, model.Note);

            return result;
        }


        public void CartLong()
        {
            var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");
            int index = 0;
            if (cart == null)
                index = 0;
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