using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class Ht_Role_PhanHe_ChucNangController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? status, string ddlVaiTro)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var listRole = db.AspNetRoles.Where(o => o.TrangThai == TrangThai.HoatDong).OrderBy(o => o.TenHienThi);
            ViewBag.ListRole = listRole;
            ViewBag.ListChucNang = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong).OrderBy(o => o.TenChucNang);
                
            // Khi mở View thì return danh sách rỗng để người dùng tìm kiếm trc
            if (status == null)
                ddlVaiTro = listRole.First().Id;
            var list = db.sp_GetChucNangByRole(ddlVaiTro);

            return View(list);
        }

        private void GetViewBag()
        {
            var listRole = db.AspNetRoles.OrderBy(o => o.TenHienThi);
            ViewBag.ListRole = listRole;
            var role_ID = listRole.First().Id;
            var listChucNangExists = db.Ht_Role_PhanHe_ChucNang.Where(o => o.RoleId == role_ID).Select(o => o.PhanHeChucNang_ID);
            var listChucNangNotExits = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && !listChucNangExists.Contains(o.PhanHeChucNang_ID)).OrderBy(o => o.TenChucNang);
            ViewBag.ListChucNang = listChucNangNotExits;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            GetViewBag();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "PhanHeChucNang_ID,RoleId,IsView,IsEdit")] Ht_Role_PhanHe_ChucNang ht_Role_PhanHe_ChucNang)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            if (ModelState.IsValid)
            {
                db.Ht_Role_PhanHe_ChucNang.Add(ht_Role_PhanHe_ChucNang);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(ht_Role_PhanHe_ChucNang);
        }

        // GET: Ht_Role_PhanHe_ChucNang/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? chucNang_ID, string role_ID)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            if (chucNang_ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var obj = db.Ht_Role_PhanHe_ChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == chucNang_ID && o.RoleId == role_ID);
            obj.TenChucNang = db.Ht_PhanHeChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == obj.PhanHeChucNang_ID).TenChucNang;
            obj.TenVaiTro = db.AspNetRoles.FirstOrDefault(o => o.Id == obj.RoleId).TenHienThi;
            if (obj == null)
            {
                return HttpNotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "PhanHeChucNang_ID,RoleId,IsView,IsEdit")] Ht_Role_PhanHe_ChucNang ht_Role_PhanHe_ChucNang)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            if (ModelState.IsValid)
            {
                db.Entry(ht_Role_PhanHe_ChucNang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ht_Role_PhanHe_ChucNang);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? chucNang_ID, string role_ID)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            if (chucNang_ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ht_Role_PhanHe_ChucNang = db.Ht_Role_PhanHe_ChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == chucNang_ID && o.RoleId == role_ID);
            if (ht_Role_PhanHe_ChucNang == null)
            {
                return HttpNotFound();
            }
            db.Ht_Role_PhanHe_ChucNang.Remove(ht_Role_PhanHe_ChucNang);
            db.SaveChanges();
            return Json(new { msg = "ok" });
        }

        #region Event
        /// <summary>
        /// Lấy danh sách Trực thuộc - View Create, Edit (hoặc ds đơn vị - View Index) theo Nhóm đơn vị
        /// </summary>
        /// <param name="nhomDonVi_ID">ID của nhóm đơn vị</param>
        /// <param name="view">Tên view cần hiển thị</param>
        public ActionResult ChangeListChucNang(string roleId)
        {
            try
            {
                var str = new StringBuilder();
                var listChucNangExists = db.Ht_Role_PhanHe_ChucNang.Where(o => o.RoleId == roleId).Select(o => o.PhanHeChucNang_ID);
                var listChucNangNotExits = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && !listChucNangExists.Contains(o.PhanHeChucNang_ID)).OrderBy(o => o.TenChucNang);

                if (listChucNangNotExits.Any())
                {
                    foreach (var item in listChucNangNotExits)
                    {
                        str.AppendFormat("<option value='{0}'>{1}</option>", item.PhanHeChucNang_ID, item.TenChucNang);
                    }
                }

                return Json(new { danhSach = str.ToString() });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }
        #endregion

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
