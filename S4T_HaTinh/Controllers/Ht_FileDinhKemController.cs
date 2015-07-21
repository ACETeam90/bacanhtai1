using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class Ht_FileDinhKemController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: Ht_FileDinhKem
        public ActionResult Index()
        {
            return View(db.Ht_FileDinhKem.ToList());
        }

        // GET: Ht_FileDinhKem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_FileDinhKem ht_FileDinhKem = db.Ht_FileDinhKem.Find(id);
            if (ht_FileDinhKem == null)
            {
                return HttpNotFound();
            }
            return View(ht_FileDinhKem);
        }

        public ActionResult FileDetails(int? report, int phanhe, int loavanban)
        {
            if (report == null)
            {
                return HttpNotFound();
            }
            var listFile = db.Ht_FileDinhKem.Where(o => o.PhanHeChucNang_ID == phanhe
                                                         && o.BaoCao_ID == report
                                                         && o.LoaiVanBan_ID == loavanban).ToList();
            return View(listFile);
        }

        // GET: Ht_FileDinhKem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ht_FileDinhKem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ht_FileDinhKem ht_FileDinhKem)
        {
            if (ModelState.IsValid)
            {
                db.Ht_FileDinhKem.Add(ht_FileDinhKem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ht_FileDinhKem);
        }

        // GET: Ht_FileDinhKem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_FileDinhKem ht_FileDinhKem = db.Ht_FileDinhKem.Find(id);
            if (ht_FileDinhKem == null)
            {
                return HttpNotFound();
            }
            return View(ht_FileDinhKem);
        }

        // POST: Ht_FileDinhKem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Ht_FileDinhKem ht_FileDinhKem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ht_FileDinhKem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ht_FileDinhKem);
        }

        // GET: Ht_FileDinhKem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_FileDinhKem ht_FileDinhKem = db.Ht_FileDinhKem.Find(id);
            if (ht_FileDinhKem == null)
            {
                return HttpNotFound();
            }
            return View(ht_FileDinhKem);
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="tenFile">Tên file đã được đổi tên để lưu trên server</param>
        /// <param name="chucNang_ID">mã phân hệ chức năng</param>
        public ActionResult Download(string tenFile, int? chucNang_ID)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (tenFile == null || chucNang_ID == null) return JavaScript("Không tìm thấy file trên server!");
            var objChucNang = db.Ht_PhanHeChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == chucNang_ID);
            if(TempData[objChucNang.ControllerName + "_" + S4T_HaTinhBase.GetUserSession().Id] == null)
                return JavaScript("Không tìm thấy file trên server!");
            var filePath = "";
            var fileName = "";
            var list = (List<Ht_FileDinhKem>)TempData[objChucNang.ControllerName + "_" + S4T_HaTinhBase.GetUserSession().Id];
            TempData[objChucNang.ControllerName + "_" + S4T_HaTinhBase.GetUserSession().Id] = list;

            var objFile = list.FirstOrDefault(o => o.PhanHeChucNang_ID == chucNang_ID && o.TenFile == tenFile);
            if (objFile != null)
            {
                filePath = objFile.DuongDan;
                fileName = objFile.TenHienThi;

                var contentType = S4T_HaTinhBase.GetContentType(filePath);
                if (System.IO.File.Exists(filePath))
                    return File(new FileStream(filePath, FileMode.Open), contentType, fileName);
            }

            return JavaScript("Không tìm thấy file trên server!");
        }

        // POST: Ht_FileDinhKem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ht_FileDinhKem ht_FileDinhKem = db.Ht_FileDinhKem.Find(id);
            db.Ht_FileDinhKem.Remove(ht_FileDinhKem);
            db.SaveChanges();
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
