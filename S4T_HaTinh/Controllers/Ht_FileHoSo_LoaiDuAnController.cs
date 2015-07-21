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
    public class Ht_FileHoSo_LoaiDuAnController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: Ht_FileHoSo_LoaiDuAn
        public ActionResult Index()
        {
            return View(db.Ht_FileHoSo_LoaiDuAn.ToList());
        }

        // GET: Ht_FileHoSo_LoaiDuAn/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_FileHoSo_LoaiDuAn ht_FileHoSo_LoaiDuAn = db.Ht_FileHoSo_LoaiDuAn.Find(id);
            if (ht_FileHoSo_LoaiDuAn == null)
            {
                return HttpNotFound();
            }
            return View(ht_FileHoSo_LoaiDuAn);
        }

        // GET: Ht_FileHoSo_LoaiDuAn/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LoaiFile_ID,LoaiDuAn_ID,TenFile,GhiChu,TrangThai")] Ht_FileHoSo_LoaiDuAn ht_FileHoSo_LoaiDuAn)
        {
            if (ModelState.IsValid)
            {
                db.Ht_FileHoSo_LoaiDuAn.Add(ht_FileHoSo_LoaiDuAn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ht_FileHoSo_LoaiDuAn);
        }

        // GET: Ht_FileHoSo_LoaiDuAn/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_FileHoSo_LoaiDuAn ht_FileHoSo_LoaiDuAn = db.Ht_FileHoSo_LoaiDuAn.Find(id);
            if (ht_FileHoSo_LoaiDuAn == null)
            {
                return HttpNotFound();
            }
            return View(ht_FileHoSo_LoaiDuAn);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoaiFile_ID,LoaiDuAn_ID,TenFile,GhiChu,TrangThai")] Ht_FileHoSo_LoaiDuAn ht_FileHoSo_LoaiDuAn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ht_FileHoSo_LoaiDuAn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ht_FileHoSo_LoaiDuAn);
        }

        // GET: Ht_FileHoSo_LoaiDuAn/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_FileHoSo_LoaiDuAn ht_FileHoSo_LoaiDuAn = db.Ht_FileHoSo_LoaiDuAn.Find(id);
            if (ht_FileHoSo_LoaiDuAn == null)
            {
                return HttpNotFound();
            }
            ht_FileHoSo_LoaiDuAn.TrangThai = TrangThai.KhongHoatDong;
            db.Entry(ht_FileHoSo_LoaiDuAn).State = EntityState.Modified;
            db.SaveChanges();
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
