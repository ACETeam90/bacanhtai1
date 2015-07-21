using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Controllers
{
    public class LoaiVanBanController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: LoaiVanBan
        public ActionResult Index()
        {
            return View(db.LoaiVanBan.ToList());
        }

        public ActionResult _CreateOrUpdate(LoaiVanBan obj)
        {
            return View(obj);
        }

        // GET: LoaiVanBan/Create
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoaiVanBan loaiVanBan)
        {
            if (ModelState.IsValid)
            {
                db.LoaiVanBan.Add(loaiVanBan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(loaiVanBan);
        }

        // GET: LoaiVanBan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiVanBan loaiVanBan = db.LoaiVanBan.Find(id);
            if (loaiVanBan == null)
            {
                return HttpNotFound();
            }
            return View(loaiVanBan);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiVanBan loaiVanBan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loaiVanBan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loaiVanBan);
        }

        // GET: LoaiVanBan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiVanBan loaiVanBan = db.LoaiVanBan.Find(id);
            if (loaiVanBan == null)
            {
                return HttpNotFound();
            }
            return View(loaiVanBan);
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
