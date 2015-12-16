using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bitministry_Task.Models;
using System.Web.Security;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace Bitministry_Task.Controllers
{
    [AllowAnonymous]
    public class LoginAccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login l, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
            {
                string encryptpass = FormsAuthentication.HashPasswordForStoringInConfigFile(l.Password, "SHA1");
                using (BankAccountDb b = new BankAccountDb())
                {
                    var u = b.Generaltable.Where(a => a.Username.Equals(l.Username) && a.password.Equals(encryptpass)).FirstOrDefault();
                    if (u != null)
                    {
                        //FormsAuthentication.SetAuthCookie(u.Username, l.Rememberme);
                        FormsAuthentication.SetAuthCookie(u.Username, false);
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("MyProfile", "Home");
                        }
                    }
                }
            }
            ViewBag.forpas = "Wrong Password";
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Views");
        }

        public ActionResult Views()
        {
            return View();
        }
    }
}