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
    public class CongNghiepCNTTsController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: CongNghiepCNTTs
        public async Task<ActionResult> Index()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));
            GetViewBag();

            return View(await db.CongNghiepCNTT.ToListAsync());
        }

        private void GetViewBag()
        {
            if (MvcApplication.ListDonVi.Count() > 0)
                ViewBag.ListDonVi = MvcApplication.ListDonVi;
        }

        // GET: CongNghiepCNTTs/Create
        public ActionResult Create()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            GetViewBag();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CongNghiepCNTT_ID,DonVi_ID,FileUpload,DoanhThu,SoMayTinh,SoPhanMemUngDung,SoWebsite,SoMayTinhKetNoiMang,TuNgay,DenNgay,NgayTao,NgayCapNhat,Success,TruongNhapLai")] CongNghiepCNTT congNghiepCNTT)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                db.CongNghiepCNTT.Add(congNghiepCNTT);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            GetViewBag();
            return View(congNghiepCNTT);
        }

        // GET: CongNghiepCNTTs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            GetViewBag();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CongNghiepCNTT congNghiepCNTT = await db.CongNghiepCNTT.FindAsync(id);
            if (congNghiepCNTT == null)
            {
                return HttpNotFound();
            }
            return View(congNghiepCNTT);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CongNghiepCNTT_ID,DonVi_ID,FileUpload,DoanhThu,SoMayTinh,SoPhanMemUngDung,SoWebsite,SoMayTinhKetNoiMang,TuNgay,DenNgay,NgayTao,NgayCapNhat,Success,TruongNhapLai")] CongNghiepCNTT congNghiepCNTT)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                db.Entry(congNghiepCNTT).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            GetViewBag();
            return View(congNghiepCNTT);
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
