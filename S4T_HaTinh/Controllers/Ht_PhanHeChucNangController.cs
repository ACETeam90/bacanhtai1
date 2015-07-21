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
    public class Ht_PhanHeChucNangController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: Ht_PhanHeChucNang
        public async Task<ActionResult> Index(int? ddlPhanHe)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            var listPhanHe = db.Ht_PhanHe.Where(o => o.TrangThai == TrangThai.HoatDong).OrderBy(o => o.TenPhanHe);
            ViewBag.ListPhanHe = listPhanHe;

            if (ddlPhanHe != null)
            {
                return View(await db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && o.PhanHe_ID == ddlPhanHe).OrderBy(o => o.ThuTu).ToListAsync());
            }
            else
            {
                if (listPhanHe.Any())
                    return View(await db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && o.PhanHe_ID == listPhanHe.FirstOrDefault().PhanHe_ID).OrderBy(o => o.ThuTu).ToListAsync());
                else
                    return View();
            }
        }

        // GET: Ht_PhanHeChucNang/Create
        public ActionResult Create()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (db.Ht_PhanHe.Count() > 0)
                ViewBag.ListPhanHe = db.Ht_PhanHe.Where(o => o.TrangThai == TrangThai.HoatDong);
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Ht_PhanHeChucNang ht_PhanHeChucNang)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                db.Ht_PhanHeChucNang.Add(ht_PhanHeChucNang);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ht_PhanHeChucNang);
        }

        // GET: Ht_PhanHeChucNang/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (db.Ht_PhanHe.Count() > 0)
                ViewBag.ListPhanHe = db.Ht_PhanHe.Where(o => o.TrangThai == TrangThai.HoatDong);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_PhanHeChucNang ht_PhanHeChucNang = await db.Ht_PhanHeChucNang.FindAsync(id);
            if (ht_PhanHeChucNang == null)
            {
                return HttpNotFound();
            }
            return View(ht_PhanHeChucNang);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Ht_PhanHeChucNang ht_PhanHeChucNang)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                db.Entry(ht_PhanHeChucNang).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ht_PhanHeChucNang);
        }

        // GET: Ht_PhanHeChucNang/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            Ht_PhanHeChucNang ht_PhanHeChucNang = await db.Ht_PhanHeChucNang.FindAsync(id);
            if (ht_PhanHeChucNang == null)
            {
                //return HttpNotFound();
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                ht_PhanHeChucNang.TrangThai = TrangThai.KhongHoatDong;
                db.Entry(ht_PhanHeChucNang).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return Json(new { msg = "ok" });
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
