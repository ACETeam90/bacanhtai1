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
    public class TD_CauHinhController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: TD_CauHinh
        public ActionResult Index()
        {
            return View(db.TD_CauHinh.ToList());
        }

        private void GetViewBag()
        {
            ViewBag.ListUser = db.AspNetUsers.Select(o => new { o.Id, o.UserName });
        }

        // GET: TD_CauHinh/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_CauHinh tD_CauHinh = db.TD_CauHinh.Find(id);
            if (tD_CauHinh == null)
            {
                return HttpNotFound();
            }
            return View(tD_CauHinh);
        }

        // GET: TD_CauHinh/Create
        public ActionResult Create()
        {
            GetViewBag();
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CauHinh_ID,TenCauHinh,MaCauHinh,NoiDung,User_ID,IsGlobal")] TD_CauHinh tD_CauHinh)
        {
            if (ModelState.IsValid)
            {
                db.TD_CauHinh.Add(tD_CauHinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tD_CauHinh);
        }

        // GET: TD_CauHinh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GetViewBag();
            TD_CauHinh tD_CauHinh = db.TD_CauHinh.Find(id);
            if (tD_CauHinh == null)
            {
                return HttpNotFound();
            }
            return View(tD_CauHinh);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CauHinh_ID,TenCauHinh,MaCauHinh,NoiDung,User_ID,IsGlobal")] TD_CauHinh tD_CauHinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tD_CauHinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tD_CauHinh);
        }

        // GET: TD_CauHinh/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_CauHinh tD_CauHinh = db.TD_CauHinh.Find(id);
            if (tD_CauHinh == null)
            {
                return HttpNotFound();
            }
            db.TD_CauHinh.Remove(tD_CauHinh);
            db.SaveChanges();
            return Json(new { ok = "ok"});
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
