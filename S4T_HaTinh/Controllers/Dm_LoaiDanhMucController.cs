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
    public class Dm_LoaiDanhMucController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: Dm_LoaiDanhMuc
        public async Task<ActionResult> Index(string strName, int? trangThai)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            var list = await db.Dm_LoaiDanhMuc.ToListAsync();
            if (!String.IsNullOrEmpty(strName))
            {
                list = list.Where(o => o.TenLoai.ToLower().Contains(strName.Trim().ToLower()) == true).ToList();
            }
            if (trangThai != null)
                list = list.Where(o => o.TrangThai == trangThai).ToList();

            return View(list);
        }

        // GET: Dm_LoaiDanhMuc/Create
        public ActionResult Create()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LoaiDanhMuc_ID,TenLoai,TrangThai")] Dm_LoaiDanhMuc dm_LoaiDanhMuc)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            dm_LoaiDanhMuc.TrangThai = TrangThai.HoatDong;
            if (ModelState.IsValid)
            {
                db.Dm_LoaiDanhMuc.Add(dm_LoaiDanhMuc);
                await db.SaveChangesAsync();
                UpdateLoaiDanhMuc(db.Dm_LoaiDanhMuc);
                return RedirectToAction("Index");
            }

            return View(dm_LoaiDanhMuc);
        }

        // GET: Dm_LoaiDanhMuc/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dm_LoaiDanhMuc dm_LoaiDanhMuc = await db.Dm_LoaiDanhMuc.FindAsync(id);
            if (dm_LoaiDanhMuc == null)
            {
                return HttpNotFound();
            }
            return View(dm_LoaiDanhMuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LoaiDanhMuc_ID,TenLoai,TrangThai")] Dm_LoaiDanhMuc dm_LoaiDanhMuc)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                db.Entry(dm_LoaiDanhMuc).State = EntityState.Modified;
                await db.SaveChangesAsync();
                UpdateLoaiDanhMuc(db.Dm_LoaiDanhMuc);
                return RedirectToAction("Index");
            }
            return View(dm_LoaiDanhMuc);
        }

        // GET: Dm_LoaiDanhMuc/Delete/5
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
            Dm_LoaiDanhMuc dm_LoaiDanhMuc = await db.Dm_LoaiDanhMuc.FindAsync(id);

            if (dm_LoaiDanhMuc == null)
            {
                //return HttpNotFound();
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                dm_LoaiDanhMuc.TrangThai = TrangThai.KhongHoatDong;
                db.Entry(dm_LoaiDanhMuc).State = EntityState.Modified;
                await db.SaveChangesAsync();
                UpdateLoaiDanhMuc(db.Dm_LoaiDanhMuc);
            }
            return Json(new { msg = "ok" });
        }

        public async Task<ActionResult> Active(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            Dm_LoaiDanhMuc dm_LoaiDanhMuc = await db.Dm_LoaiDanhMuc.FindAsync(id);

            if (dm_LoaiDanhMuc == null)
            {
                //return HttpNotFound();
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                dm_LoaiDanhMuc.TrangThai = TrangThai.HoatDong;
                db.Entry(dm_LoaiDanhMuc).State = EntityState.Modified;
                await db.SaveChangesAsync();
                UpdateLoaiDanhMuc(db.Dm_LoaiDanhMuc);
            }
            return Json(new { msg = "ok" });
        }

        /// <summary>
        /// Update lại biến Static LoaiDanhMuc
        /// </summary>
        private void UpdateLoaiDanhMuc(DbSet<Dm_LoaiDanhMuc> db)
        {
            MvcApplication.ListLoaiDanhMuc = db.ToList();
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
