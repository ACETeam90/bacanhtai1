using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class Ht_PhanHeController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: Ht_PhanHe
        public async Task<ActionResult> Index()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            return View(await db.Ht_PhanHe.OrderBy(o => o.TrangThai).ThenBy(o => o.ThuTu).ToListAsync());
        }

        // GET: Ht_PhanHe/Create
        public ActionResult Create()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PhanHe_ID,TenPhanHe,TrangThai,ControllerName,ActionName,ThuTu")] Ht_PhanHe ht_PhanHe)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                db.Ht_PhanHe.Add(ht_PhanHe);
                await db.SaveChangesAsync();
                ReloadPhanHe(db.Ht_PhanHe);
                return RedirectToAction("Index");                
            }

            return View(ht_PhanHe);
        }

        // GET: Ht_PhanHe/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_PhanHe ht_PhanHe = await db.Ht_PhanHe.FindAsync(id);
            if (ht_PhanHe == null)
            {
                return HttpNotFound();
            }
            return View(ht_PhanHe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PhanHe_ID,TenPhanHe,TrangThai,ControllerName,ActionName,ThuTu")] Ht_PhanHe ht_PhanHe)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                db.Entry(ht_PhanHe).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ReloadPhanHe(db.Ht_PhanHe);
                return RedirectToAction("Index");
            }
            return View(ht_PhanHe);
        }

        // GET: Ht_PhanHe/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            Ht_PhanHe ht_PhanHe = await db.Ht_PhanHe.FindAsync(id);
            if (ht_PhanHe == null)
            {
                //return HttpNotFound();
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                ht_PhanHe.TrangThai = TrangThai.KhongHoatDong;
                db.Entry(ht_PhanHe).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ReloadPhanHe(db.Ht_PhanHe);
                return Json(new { msg = "ok" });
            }
        }

        /// <summary>
        /// Update lại biến Static ListPhanHe
        /// </summary>
        private void ReloadPhanHe(DbSet<Ht_PhanHe> db)
        {
            MvcApplication.ListPhanHe = db.OrderBy(o => o.TrangThai).ThenBy(o => o.ThuTu).ToList().ToList();
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
