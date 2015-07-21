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
    public class Dm_DanhMucChungController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: Dm_DanhMucChung
        public ActionResult Index(string strName, int? loaiDanhMuc, int? trangThai)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            var listLoaiDanhMuc = MvcApplication.ListLoaiDanhMuc.Where(o => o.TrangThai == TrangThai.HoatDong).OrderBy(o => o.TenLoai);
            ViewBag.SelectListLoaiDanhMuc = new SelectList(listLoaiDanhMuc, "LoaiDanhMuc_ID", "TenLoai");

            var list = (IEnumerable<Dm_DanhMucChung>)db.Dm_DanhMucChung;
            
            // Load lần đầu
            if (loaiDanhMuc == null && listLoaiDanhMuc.Any())
                return View(list.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == listLoaiDanhMuc.First().LoaiDanhMuc_ID));
            else
                list = list.Where(o => o.LoaiDanhMuc_ID == loaiDanhMuc);

            if (trangThai != null)
                list = list.Where(o => o.TrangThai == trangThai);

            if (!String.IsNullOrEmpty(strName))
            {
                list = list.Where(o => o.TenDanhMuc.ToLower().Contains(strName.Trim().ToLower()) == true).ToList();
            }

            return View(list);
        }

        private void GetViewBag()
        {
            var slLoaiDanhMuc = new SelectList(MvcApplication.ListLoaiDanhMuc.Where(o => o.TrangThai == TrangThai.HoatDong).OrderBy(o => o.TenLoai), "LoaiDanhMuc_ID", "TenLoai");
            ViewBag.SelectListLoaiDanhMuc = slLoaiDanhMuc;
        }

        // GET: Dm_DanhMucChung/Create
        public ActionResult Create()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            GetViewBag();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DanhMuc_ID,TenDanhMuc,LoaiDanhMuc_ID,TrangThai")] Dm_DanhMucChung dm_DanhMucChung)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            dm_DanhMucChung.TrangThai = TrangThai.HoatDong;
            if (ModelState.IsValid)
            {
                db.Dm_DanhMucChung.Add(dm_DanhMucChung);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            GetViewBag();
            return View(dm_DanhMucChung);
        }

        // GET: Dm_DanhMucChung/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            GetViewBag();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dm_DanhMucChung dm_DanhMucChung = await db.Dm_DanhMucChung.FindAsync(id);
            if (dm_DanhMucChung == null)
            {
                return HttpNotFound();
            }
            return View(dm_DanhMucChung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> Edit([Bind(Include = "DanhMuc_ID,TenDanhMuc,LoaiDanhMuc_ID,TrangThai")] Dm_DanhMucChung dm_DanhMucChung)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                db.Entry(dm_DanhMucChung).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            GetViewBag();
            return View(dm_DanhMucChung);
        }

        // GET: Dm_DanhMucChung/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            Dm_DanhMucChung dm_DanhMucChung = await db.Dm_DanhMucChung.FindAsync(id);

            if (dm_DanhMucChung == null)
            {
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                dm_DanhMucChung.TrangThai = TrangThai.KhongHoatDong;
                db.Entry(dm_DanhMucChung).State = EntityState.Modified;
                await db.SaveChangesAsync();
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
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            Dm_DanhMucChung dm_DanhMucChung = await db.Dm_DanhMucChung.FindAsync(id);

            if (dm_DanhMucChung == null)
            {
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                dm_DanhMucChung.TrangThai = TrangThai.HoatDong;
                db.Entry(dm_DanhMucChung).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return Json(new { msg = "ok" });
        }

        /// <summary>
        /// Update lại biến Static 
        /// </summary>
        private void ReloadDanhMucChung(DbSet<Dm_LoaiDanhMuc> db)
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
