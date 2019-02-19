using GloriousCore.Helpers;
using GloriousCore.Models;
using GloriousCore.Models.Data;
using GloriousCore.Models.Data.Entities;
using GloriousCore.Tools;
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
        private readonly ViewRender renderView;

        public CartController(ViewRender renderView)
        {
            this.renderView = renderView;
        }

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
        //public IActionResult Buy(string name, string num, string email, string city, string post, string adres, string note)
        public IActionResult Buy(OrderDBO model)
        {
            #region  shitCode, FIXED /////////////////////////////////////////
            int conrtrol = 0;

            //List<string> model = new List<string>();
            //model.Add(name);
            //model.Add(num);
            //model.Add(email);
            //model.Add(city);
            //model.Add(post);
            //model.Add(adres);
            //model.Add(note);

            //foreach (string item in model)
            //{
            //    bool result = !(item.Contains("{") ||
            //                    item.Contains("}") ||
            //                    item.Contains("<") ||
            //                    item.Contains(">") ||
            //                    item.Contains("&") ||
            //                    item.Contains("#") ||
            //                    item.Contains("$"));
            //    if (!result)
            //    {
            //        conrtrol += 1;
            //    }

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

            //}
            #endregion
            if (conrtrol == 0)
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

                return RedirectToAction("products", "Shop");
            }
            else
            {
                ModelState.AddModelError("", "Введен запрещенный символ");
                return Redirect("/cart");
            }
        }
        [Route("htmlStr")]
        public async void GetHtmlString()
        {
            var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");

            //RenderView rv = HttpContext.RequestServices.GetService<RenderView>();
            string result = await renderView.RenderAsync("Cart", cart);


            string ad = "";



        }

        public string HtmlCart(OrderDBO model)
        {
            var cart = SessionHelper.Get<List<CartLine>>(HttpContext.Session, "cart");
            
            string cartLine = "";
            string total = "0";

            foreach (var item in cart)
            {
                string productName = item.Product.Name.ToString();
                string code = item.Product.ProductCode.ToString();
                string quantity = item.Quantity.ToString();
                string sum = "";
                if (item.Product.Discount != 0) {
                    sum = (item.Quantity * item.Product.Discount).ToString();
                    total = (Convert.ToInt32(total) + (item.Quantity * item.Product.Discount)).ToString();
                }
                else {
                    sum = (item.Quantity * item.Product.Price).ToString();
                    total = (Convert.ToInt32(total) + (item.Quantity * item.Product.Price)).ToString("0.00");
                }

                cartLine += @"
                        <tr style=""text-align: center; border-bottom: 1px solid #000000"">
						<td>{code}</td>
						<td><a href=""glorious.com.ua/product/{code}"" style=""text-decoration: none;"">{productName}</a></td>
						<td>{quanity}</td>
						<td>{sum}</td>
					    </tr>";
            }

            string name = model.Name.ToString();
            string phone = model.Number.ToString();
            string email = model.Mail.ToString();
            string city = model.City.ToString();
            string addres = model.Addres.ToString();
            string service = model.Post.ToString();
            string note = model.Note.ToString();

            string result = @"<table bgcolor=""#F7F7F7"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""margin:0; padding:0 "" width=""100%"">
		<tr style=""display: grid;"">
			<td height=""100%"" style=""margin: 10px"">
				<table style=""border-collapse: collapse; font-family: sans-serif; padding: 20px; box-shadow: 5px 5px 5px grey; background-color: #fff; margin: 0 auto;"" cellpadding=""10px"">
					<tr style=""background-color: #000000; color: #ffffff;"">
						<th>Code</th>
						<th>Name</th>
						<th>Quantity</th>
						<th>Sum</th>
					</tr>
					{cartLine}
					<tr style=""border-bottom: 2px solid #000000""><td colspan=""4"" style=""font-weight: bold; text-align: right;"">Total: {total}</td></tr>
				</table>
			</td>
			<td height=""100%"" style=""margin: 10px"">
				<table style=""border-collapse: collapse; font-family: sans-serif; padding: 20px; box-shadow: 5px 5px 5px grey; background-color: #fff; margin: 0 auto;"" cellpadding=""10px"">
					<tr>
						<td>ФИО: </td>
						<td>{name}</td>
					</tr>
					<tr>
						<td>Телефон: </td>
						<td>{phome}</td>
					</tr>
					<tr>
						<td>Email:</td>
						<td>{email}</td>
					</tr>
					<tr>
						<td>Город: </td>
						<td>{city}</td>
					</tr>
					<tr>
						<td>Служба доставки: </td>
						<td>{service}</td>
					</tr>
					<tr>
						<td>Адрес/отделение/почтовый индекс: </td>
						<td>{addres}</td>
					</tr>
					<tr>
						<td>Дополнительная информация: </td>
						<td style=""max-width: 400px;"">{note}</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>";

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