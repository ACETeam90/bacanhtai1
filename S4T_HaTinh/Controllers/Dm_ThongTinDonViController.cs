using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using S4T_HaTinh.Common;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Controllers
{
    public class Dm_ThongTinDonViController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: Dm_ThongTinDonVi/Edit/5
        public ActionResult Edit()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            
            var dm_ThongTinDonVi = db.Dm_ThongTinDonVi.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID);

            // Nếu không có thì tạo mới 1 bản ghi
            if (dm_ThongTinDonVi == null)
            {
                var objDonVi = db.Dm_DonVi.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID);
                if (objDonVi == null)
                    return Content("Không tìm thấy đơn vị");
                else
                {
                    dm_ThongTinDonVi = new Dm_ThongTinDonVi
                    {
                        DonVi_ID = objDonVi.DonVi_ID,
                        TenDonVi = objDonVi.TenDonVi
                    };
                }
            }
            return View(dm_ThongTinDonVi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Dm_ThongTinDonVi dm_ThongTinDonVi)
        {
            ModelState.Remove("ThongTin_ID");
            if (ModelState.IsValid)
            {
                // Nếu không có thì tạo mới 1 bản ghi
                if (dm_ThongTinDonVi.ThongTin_ID == null)
                {
                    db.Dm_ThongTinDonVi.Add(dm_ThongTinDonVi);
                }
                else
                {
                    db.Entry(dm_ThongTinDonVi).State = EntityState.Modified;
                }
                
                await db.SaveChangesAsync();
                return RedirectToAction("Index","Home");
            }
            return View(dm_ThongTinDonVi);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
