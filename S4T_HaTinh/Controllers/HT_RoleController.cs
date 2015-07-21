using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class HT_RoleController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(MvcApplication.ListRole);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,TenHienThi,TrangThai,NhomDoiTuong_ID")] AspNetRoles aspNetRole)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                aspNetRole.Id = Guid.NewGuid().ToString();
                aspNetRole.TrangThai = TrangThai.HoatDong;
                db.AspNetRoles.Add(aspNetRole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetRole);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRoles aspNetRole = db.AspNetRoles.Find(id);
            if (aspNetRole == null)
            {
                return HttpNotFound();
            }
            return View(aspNetRole);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TenHienThi,TrangThai,NhomDoiTuong_ID")] AspNetRoles aspNetRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetRole).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetRole);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRoles aspNetRole = db.AspNetRoles.Find(id);
            if (aspNetRole == null)
            {
                return HttpNotFound();
            }
            aspNetRole.TrangThai = TrangThai.KhongHoatDong;
            db.Entry(aspNetRole).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { msg = "ok" });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Active(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRoles aspNetRole = db.AspNetRoles.Find(id);
            if (aspNetRole == null)
            {
                return HttpNotFound();
            }
            aspNetRole.TrangThai = TrangThai.HoatDong;
            db.Entry(aspNetRole).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { msg = "ok" });
        }

        /// <summary>
        /// Update lại các biến static Đơn vị
        /// </summary>
        private void ReloadListRole()
        {
            MvcApplication.ListRole = db.AspNetRoles.ToList();
        }

        protected override void Dispose(bool disposing)
        {
            ReloadListRole();
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
