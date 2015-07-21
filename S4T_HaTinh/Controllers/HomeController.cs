using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class HomeController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //return View();
            var user_ID = User.Identity.GetUserId();
            var user = System.Web.HttpContext.Current.Session["User"] as ApplicationUser;
            if (user != null)
            {
                var roleAdmin = db.AspNetRoles.FirstOrDefault(o => o.Name.Equals("Admin"));
                var roleAdmin_ID = roleAdmin.Id;
                var isAdmin = db.AspNetUserRoles.Where(o => o.UserId == user.Id && o.RoleId == roleAdmin_ID);

                var dbAsp = new ApplicationDbContext();
                var dbRole = dbAsp.Roles;
                foreach (var role in dbRole)
                {
                    //role.
                }
                if (isAdmin.Any())
                {
                    ViewBag.Message = "Is Admin";
                }
                
                var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID);
                if (objLichNhap != null)
                {
                    ViewBag.Message = "check session";
                }
                else
                {

                }
                if (user.DonVi_ID != 3)
                {
                    ViewBag.Message = "Your application description page.";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Home/About" });
            }            
        }

        public ActionResult MenuUngDungCNTT()
        {
            return View();
        }
    }
}