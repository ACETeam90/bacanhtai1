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
    public class TD_FileHoSoDinhKemController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: TD_FileHoSoDinhKem
        public async Task<ActionResult> Index()
        {
            return View(await db.TD_FileHoSoDinhKem.ToListAsync());
        }

        // GET: TD_FileHoSoDinhKem/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_FileHoSoDinhKem tD_FileHoSoDinhKem = await db.TD_FileHoSoDinhKem.FindAsync(id);
            if (tD_FileHoSoDinhKem == null)
            {
                return HttpNotFound();
            }
            return View(tD_FileHoSoDinhKem);
        }

        // GET: TD_FileHoSoDinhKem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TD_FileHoSoDinhKem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FileHoSoDinhKem_ID,SoBienNhan,TenFile,TenHienThi,DuongDan,GhiChu,TrangThai,LoaiFile_ID")] TD_FileHoSoDinhKem tD_FileHoSoDinhKem)
        {
            if (ModelState.IsValid)
            {
                db.TD_FileHoSoDinhKem.Add(tD_FileHoSoDinhKem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tD_FileHoSoDinhKem);
        }

        // GET: TD_FileHoSoDinhKem/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_FileHoSoDinhKem tD_FileHoSoDinhKem = await db.TD_FileHoSoDinhKem.FindAsync(id);
            if (tD_FileHoSoDinhKem == null)
            {
                return HttpNotFound();
            }
            return View(tD_FileHoSoDinhKem);
        }

        // POST: TD_FileHoSoDinhKem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FileHoSoDinhKem_ID,SoBienNhan,TenFile,TenHienThi,DuongDan,GhiChu,TrangThai,LoaiFile_ID")] TD_FileHoSoDinhKem tD_FileHoSoDinhKem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tD_FileHoSoDinhKem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tD_FileHoSoDinhKem);
        }

        // GET: TD_FileHoSoDinhKem/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_FileHoSoDinhKem tD_FileHoSoDinhKem = await db.TD_FileHoSoDinhKem.FindAsync(id);
            if (tD_FileHoSoDinhKem == null)
            {
                return HttpNotFound();
            }
            return View(tD_FileHoSoDinhKem);
        }

        // POST: TD_FileHoSoDinhKem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TD_FileHoSoDinhKem tD_FileHoSoDinhKem = await db.TD_FileHoSoDinhKem.FindAsync(id);
            db.TD_FileHoSoDinhKem.Remove(tD_FileHoSoDinhKem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
