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
    public class Dm_CapXuLyController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Dm_CapXuLy.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Cap_ID,TenCap,ThuTu,NguoiXuLy,TrangThai")] Dm_CapXuLy dm_CapXuLy)
        {
            dm_CapXuLy.TrangThai = TrangThai.HoatDong;
            if (ModelState.IsValid)
            {
                db.Dm_CapXuLy.Add(dm_CapXuLy);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dm_CapXuLy);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dm_CapXuLy dm_CapXuLy = await db.Dm_CapXuLy.FindAsync(id);
            if (dm_CapXuLy == null)
            {
                return HttpNotFound();
            }
            return View(dm_CapXuLy);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Cap_ID,TenCap,ThuTu,NguoiXuLy,TrangThai")] Dm_CapXuLy dm_CapXuLy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dm_CapXuLy).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dm_CapXuLy);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dm_CapXuLy dm_CapXuLy = await db.Dm_CapXuLy.FindAsync(id);
            dm_CapXuLy.TrangThai = TrangThai.KhongHoatDong;
            db.Entry(dm_CapXuLy).State = EntityState.Modified;
            await db.SaveChangesAsync();
            if (dm_CapXuLy == null)
            {
                return HttpNotFound();
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
