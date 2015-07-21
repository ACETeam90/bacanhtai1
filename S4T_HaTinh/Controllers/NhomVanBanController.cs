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
    public class NhomVanBanController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        private void GetViewBag(ViewReport typeView)
        {
            // List Hình thức quản lý
            var items = new List<SelectListItem>();
            var listLoaiVB = db.LoaiVanBan.ToList();
            if (typeView == ViewReport.Index)
                listLoaiVB = listLoaiVB.Where(o => o.TrangThai == TrangThai.HoatDong).ToList();

            if (!listLoaiVB.Any())
                items.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
            else
            {
                var listItem = listLoaiVB.Select(o => new SelectListItem()
                {
                    Text = o.TenLoaiVanBan,
                    Value = o.LoaiVanBan_ID.ToString()
                });
                items.AddRange(listItem);
            }
            ViewBag.LoaiVB = items;
        }

        // GET: NhomVanBan
        public ActionResult Index()
        {
            GetViewBag(ViewReport.Index);
            return View(db.NhomVanBan.ToList());
        }

        public ActionResult _CreateOrUpdate(NhomVanBan obj)
        {
            return View(obj);
        }

        // GET: NhomVanBan/Create
        public ActionResult Create()
        {
            GetViewBag(ViewReport.Create);
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhomVanBan nhomVanBan)
        {
            if (ModelState.IsValid)
            {
                db.NhomVanBan.Add(nhomVanBan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetViewBag(ViewReport.Create);
            return View(nhomVanBan);
        }

        // GET: NhomVanBan/Edit/5
        public ActionResult Edit(int? id)
        {
            GetViewBag(ViewReport.Create);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhomVanBan nhomVanBan = db.NhomVanBan.Find(id);
            if (nhomVanBan == null)
            {
                return HttpNotFound();
            }
            return View(nhomVanBan);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NhomVanBan nhomVanBan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhomVanBan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetViewBag(ViewReport.Create);
            return View(nhomVanBan);
        }

        // GET: NhomVanBan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhomVanBan nhomVanBan = db.NhomVanBan.Find(id);
            if (nhomVanBan == null)
            {
                return HttpNotFound();
            }
            return View(nhomVanBan);
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
