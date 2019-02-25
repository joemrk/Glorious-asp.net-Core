using GloriousCore.Models.Data;
using GloriousCore.Models.Data.Entities;
using GloriousCore.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GloriousCore.Areas.Admin.Controllers
{

    public class AutorisationController : Controller
    {
        public readonly Db db;
        public AutorisationController(Db context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {

                LoginDBO loginModel = await db.Users.FirstOrDefaultAsync(u => u.Log == model.Log && u.Pass == model.Pass);

                if (loginModel != null)
                {
                    await Authenticate(model.Log); // аутентификация
                    return Redirect("/admin/DashBoard/Products");
                }
                else
                {
                    TempData["SM"] = "Некорректные логин и(или) пароль";
                    return View(model);
                }

            }
            else return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Exit()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Autorisation");
        }
    }
}