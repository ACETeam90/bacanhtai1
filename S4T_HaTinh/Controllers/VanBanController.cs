using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class VanBanController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        private void GetViewBag(ViewReport typeView, int? loaiVanBan)
        {
            // List Hình thức quản lý
            var items = new List<SelectListItem>();
            var listLoaiVB = db.LoaiVanBan.ToList();
            if (typeView != ViewReport.Index)
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

            items = new List<SelectListItem>();
            List<NhomVanBan> listNhomVB = new List<NhomVanBan>();
            if (listLoaiVB.Any())
            {
                if (loaiVanBan == null)
                {
                    int firstLoaiVB = listLoaiVB.FirstOrDefault().LoaiVanBan_ID;
                    listNhomVB = db.NhomVanBan.Where(o => o.LoaiVanBan_ID == firstLoaiVB).ToList();
                }else
                {
                    listNhomVB = db.NhomVanBan.Where(o => o.LoaiVanBan_ID == loaiVanBan).ToList();
                }
                if (typeView != ViewReport.Index)
                    listNhomVB = listNhomVB.Where(o => o.TrangThai == TrangThai.HoatDong).ToList();
                var listItem = listNhomVB.Select(o => new SelectListItem()
                {
                    Text = o.TenNhomVanBan,
                    Value = o.NhomVanBan_ID.ToString()
                });
                items.AddRange(listItem);
            }
            else
            {
                items.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
            }
            ViewBag.NhomVB = items;
        }

        // GET: VanBans
        public ActionResult Index(int? loaiVanBan, int? nhomVanBan)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            if (loaiVanBan == null)
                loaiVanBan = db.LoaiVanBan.FirstOrDefault().LoaiVanBan_ID;

            var list = db.VanBan.Where(o => o.LoaiVanBan_ID == loaiVanBan);

            if (nhomVanBan != null)
                list = list.Where(o => o.NhomVanBan_ID == nhomVanBan);

            GetViewBag(ViewReport.Index, null);
            return View(list);
        }
        

        // GET: VanBans/Create
        public ActionResult Create()
        {
            GetViewBag(ViewReport.Create, null);
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VanBan vanBan)
        {
            if (ModelState.IsValid)
            {
                db.VanBan.Add(vanBan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetViewBag(ViewReport.Create, vanBan.LoaiVanBan_ID);
            return View(vanBan);
        }

        // GET: VanBans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VanBan vanBan = db.VanBan.Find(id);
            if (vanBan == null)
            {
                return HttpNotFound();
            }
            GetViewBag(ViewReport.Create, vanBan.LoaiVanBan_ID);
            return View(vanBan);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VanBan vanBan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vanBan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetViewBag(ViewReport.Create, vanBan.LoaiVanBan_ID);
            return View(vanBan);
        }

        // GET: VanBans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VanBan vanBan = db.VanBan.Find(id);
            if (vanBan == null)
            {
                return HttpNotFound();
            }
            return View(vanBan);
        }

        #region Event
        public ActionResult LoaiVanBan_OnChange(int loaiVanBan, string view)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                if(view == "Index")
                {
                    str.AppendFormat("<option value=''>Tất cả</option>");
                }
                var list = db.NhomVanBan.Where(o => o.LoaiVanBan_ID == loaiVanBan);
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        str.AppendFormat("<option value='{0}'>{1}</option>", item.NhomVanBan_ID, item.TenNhomVanBan);
                    }
                }
                return Json(new { danhSach = str.ToString() });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }
        #endregion Event

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
