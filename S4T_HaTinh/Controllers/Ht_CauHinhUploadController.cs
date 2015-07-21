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

namespace S4T_HaTinh.Controllers
{
    public class Ht_CauHinhUploadController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();        

        // GET: Ht_CauHinhUpload
        public async Task<ActionResult> Index()
        {
            if (db.Dm_DonVi.Count() > 0)
                ViewBag.ListDonVi = db.Dm_DonVi;
            return View(await db.Ht_CauHinhUpload.ToListAsync());
        }

        // GET: Ht_CauHinhUpload/Create
        public ActionResult Create()
        {
            if (db.Dm_DonVi.Count() > 0)
                ViewBag.ListDonVi = db.Dm_DonVi;
            return View();
        }

        // POST: Ht_CauHinhUpload/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CauHinh_ID,PhanHe_ID,DuongDan,DonVi_ID,ThoiGian")] Ht_CauHinhUpload ht_CauHinhUpload)
        {
            if (ModelState.IsValid)
            {
                db.Ht_CauHinhUpload.Add(ht_CauHinhUpload);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ht_CauHinhUpload);
        }

        // GET: Ht_CauHinhUpload/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (db.Dm_DonVi.Count() > 0)
                ViewBag.ListDonVi = db.Dm_DonVi;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_CauHinhUpload ht_CauHinhUpload = await db.Ht_CauHinhUpload.FindAsync(id);
            if (ht_CauHinhUpload == null)
            {
                return HttpNotFound();
            }
            return View(ht_CauHinhUpload);
        }

        // POST: Ht_CauHinhUpload/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CauHinh_ID,PhanHe_ID,DuongDan,DonVi_ID,ThoiGian")] Ht_CauHinhUpload ht_CauHinhUpload)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ht_CauHinhUpload).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ht_CauHinhUpload);
        }

        // GET: Ht_CauHinhUpload/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            Ht_CauHinhUpload ht_CauHinhUpload = await db.Ht_CauHinhUpload.FindAsync(id);
            if (ht_CauHinhUpload == null)
            {
                //return HttpNotFound();
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                db.Ht_CauHinhUpload.Remove(ht_CauHinhUpload);
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
