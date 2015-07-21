using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using S4T_HaTinh.Common;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Controllers
{
    public class Dm_LoaiDuAnController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Dm_LoaiDuAn.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LoaiDuAn_ID,TenLoaiDuAn,SoNgayGiaiQuyet,TrangThai")] Dm_LoaiDuAn dm_LoaiDuAn)
        {
            dm_LoaiDuAn.TrangThai = TrangThai.HoatDong;
            if (ModelState.IsValid)
            {
                db.Dm_LoaiDuAn.Add(dm_LoaiDuAn);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dm_LoaiDuAn);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dm_LoaiDuAn dm_LoaiDuAn = await db.Dm_LoaiDuAn.FindAsync(id);
            if (dm_LoaiDuAn == null)
            {
                return HttpNotFound();
            }
            return View(dm_LoaiDuAn);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LoaiDuAn_ID,TenLoaiDuAn,SoNgayGiaiQuyet,TrangThai")] Dm_LoaiDuAn dm_LoaiDuAn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dm_LoaiDuAn).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dm_LoaiDuAn);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dm_LoaiDuAn dm_LoaiDuAn = await db.Dm_LoaiDuAn.FindAsync(id);
            if (dm_LoaiDuAn == null)
            {
                return HttpNotFound();
            }
            dm_LoaiDuAn.TrangThai = TrangThai.KhongHoatDong;
            db.Entry(dm_LoaiDuAn).State = EntityState.Modified;
            await db.SaveChangesAsync();
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
