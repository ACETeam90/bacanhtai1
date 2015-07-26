using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity.Validation;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class Dm_DonViController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();
        
        public ActionResult Index(int? status, string strName, string ddlNhomDonVi, string ddlDonViCap1, int? ddlTrangThai)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            // Fix trường hợp Nhóm đơn vị chọn tất cả khi chọn 1 Nhóm đơn vị sẽ select all theo nhóm đơn vị đó. Hiện tại chỉ lấy các đơn vị cấp 1
            if (!string.IsNullOrEmpty(ddlNhomDonVi))
                ddlNhomDonVi = ddlNhomDonVi.Trim();
            if (!string.IsNullOrEmpty(ddlNhomDonVi) && string.IsNullOrEmpty(ddlDonViCap1) && ddlNhomDonVi != DonVi.NhomDonViCapHuyen.ToString() && ddlNhomDonVi != DonVi.NhomDonViCapXa.ToString())
                ddlDonViCap1 = "-1";

            var list = (IEnumerable<Dm_DonVi>) MvcApplication.ListDonVi;

            if (ddlTrangThai == null || ddlTrangThai == TrangThai.HoatDong)
                list = list.Where(o => o.TrangThai == TrangThai.HoatDong);
            else
                list = list.Where(o => o.TrangThai == ddlTrangThai);

            int nhomDonVi_ID = 0, donViCap1_ID;
            if (!String.IsNullOrEmpty(ddlNhomDonVi) && int.TryParse(ddlNhomDonVi, out nhomDonVi_ID))
            {
                GetViewBag(nhomDonVi_ID, ViewReport.Index);
                list = list.Where(o => o.NhomDonVi_ID == nhomDonVi_ID);
            }
            else
                GetViewBag(0, ViewReport.Index);

            if (!String.IsNullOrEmpty(ddlDonViCap1) && int.TryParse(ddlDonViCap1, out donViCap1_ID))
            {
                if (nhomDonVi_ID == DonVi.NhomDonViCapXa && donViCap1_ID == -1)
                {

                }
                else
                {
                    list = list.Where(o => o.DonViCap1_ID == donViCap1_ID);
                }
            }
                

            if (!String.IsNullOrEmpty(strName))
            {
                list = list.Where(o => o.TenDonVi.ToLower().Contains(strName.Trim().ToLower()));
            }

            return View(list);
        }

        /// <summary>
        /// Get all ViewBag 
        /// </summary>
        private void GetViewBag(int? ddlNhomDonVi, ViewReport viewType)
        {
            // Lấy list đơn vị cấp trên 
            var listDonViCap1 = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong); 
            var items = new List<SelectListItem>();

            if (viewType == ViewReport.Index)
            {
                items.Add(new SelectListItem() { Text = "", Value = "", Selected = true });

                if (ddlNhomDonVi == DonVi.NhomDonViCapXa || ddlNhomDonVi == DonVi.NhomDonViCapHuyen)
                {
                    if (ddlNhomDonVi == DonVi.NhomDonViCapXa)
                        listDonViCap1 = listDonViCap1.Where(o => o.NhomDonVi_ID == DonVi.NhomDonViCapHuyen);
                    else if (ddlNhomDonVi == DonVi.NhomDonViCapHuyen)
                        listDonViCap1 = listDonViCap1.Where(o => o.NhomDonVi_ID == DonVi.NhomDonViCapTinh);

                    var slListDVCap1 = listDonViCap1.OrderBy(o => o.TenDonVi).Select(o => new SelectListItem()
                    {
                        Text = o.TenDonVi,
                        Value = o.DonVi_ID.ToString()
                    });

                    items.AddRange(slListDVCap1);
                }
            }
            else
            {
                if(ddlNhomDonVi != DonVi.NhomDonViCapHuyen && ddlNhomDonVi != DonVi.NhomDonViCapXa)
                    items.Add(new SelectListItem() { Text = "", Value = "-1", Selected = true });
                else
                {
                    if (ddlNhomDonVi == DonVi.NhomDonViCapXa)
                        listDonViCap1 = listDonViCap1.Where(o => o.NhomDonVi_ID == DonVi.NhomDonViCapHuyen);
                    else if (ddlNhomDonVi == DonVi.NhomDonViCapHuyen)
                        listDonViCap1 = listDonViCap1.Where(o => o.NhomDonVi_ID == DonVi.NhomDonViCapTinh);

                    var slListDVCap1 = listDonViCap1.OrderBy(o => o.TenDonVi).Select(o => new SelectListItem()
                    {
                        Text = o.TenDonVi,
                        Value = o.DonVi_ID.ToString()
                    });

                    items.AddRange(slListDVCap1);
                }
            }

            ViewBag.ListDonViCap1 = listDonViCap1;
            ViewBag.SelectListDonViCap1 = items;
        }

        // GET: Dm_DonVi/Create
        public ActionResult Create()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            var listNhomDonVi = MvcApplication.ListNhomDonVi().Where(o => o.TrangThai == TrangThai.HoatDong).OrderBy(o => o.TenDanhMuc);
            if (listNhomDonVi.Any()) GetViewBag(listNhomDonVi.First().DanhMuc_ID, ViewReport.Create);
            else
            {
                GetViewBag(0, ViewReport.Create);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,ChuyenVien")]
        public async Task<ActionResult> Create(Dm_DonVi dm_DonVi)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            dm_DonVi.TrangThai = TrangThai.HoatDong;
            if (ModelState.IsValid)
            {
                dm_DonVi.NgayTao = DateTime.Now;
                db.Dm_DonVi.Add(dm_DonVi);
                await db.SaveChangesAsync();
  
                ReloadListDonVi();
                return RedirectToAction("Index");
            }

            GetViewBag(MvcApplication.ListNhomDonVi().Where(o => o.DanhMuc_ID != S4T_HaTinh.Common.DonVi.NhomDonViCapXa).First().DanhMuc_ID, ViewReport.Create);
            return View(dm_DonVi);
        }

        // GET: Dm_DonVi/Edit/5
        public async Task<ActionResult> Edit(int? id, int? ddlNhomDonVi)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GetViewBag(ddlNhomDonVi, ViewReport.Edit);
            Dm_DonVi dm_DonVi = await db.Dm_DonVi.FindAsync(id);
            if (dm_DonVi == null)
            {
                return HttpNotFound();
            }

            if(dm_DonVi.NhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                return View("EditHuyen",dm_DonVi);
            else
                return View(dm_DonVi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Dm_DonVi dm_DonVi)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            var objDonVi = db.Dm_DonVi.Find(dm_DonVi.DonVi_ID);
            if (objDonVi == null) return Content("Không tìm thấy mã đơn vị");
            else
            {
                objDonVi.TenDonVi = dm_DonVi.TenDonVi;
                objDonVi.NhomDonVi_ID = dm_DonVi.NhomDonVi_ID;
                objDonVi.DonViCap1_ID = dm_DonVi.DonViCap1_ID;
                objDonVi.TrangThai = dm_DonVi.TrangThai;
            }
            try
            {
                if (ModelState.IsValid)
                {
                    dm_DonVi.NgayCapNhat = DateTime.Now;
                    db.Entry(objDonVi).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    ReloadListDonVi();

                    return RedirectToAction("Index");
                }
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("{0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                ModelState.AddModelError("", sb.ToString());
            }

            GetViewBag(dm_DonVi.NhomDonVi_ID, ViewReport.Edit);

            if (dm_DonVi.NhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                return View("EditHuyen", objDonVi);
            else
                return View(objDonVi);
        }

        #region Update thêm thông tin đơn vị - Dùng cho cán bộ CNTT của từng đơn vị vào cập nhật

        /// <summary>
        /// Dùng cho đơn vị Tỉnh
        /// </summary>
        public async Task<ActionResult> Update()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            Dm_DonVi dm_DonVi = await db.Dm_DonVi.FindAsync(user.DonVi_ID);
            if (dm_DonVi == null)
            {
                return HttpNotFound();
            }
            if (dm_DonVi.NhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                return View("UpdateHuyen", dm_DonVi);
            
            return View(dm_DonVi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Dm_DonVi dm_DonVi)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (dm_DonVi.DonVi_ID == null)
            {
                ModelState.AddModelError("", "Không tìm thấy mã đơn vị");
                return View(dm_DonVi);
            }
            
            using (var context = new S4T_HaTinhEntities())
            {
                var objDonViOld = context.Dm_DonVi.FirstOrDefault(o => o.DonVi_ID == dm_DonVi.DonVi_ID);
                dm_DonVi.NhomDonVi_ID = objDonViOld.NhomDonVi_ID;
                dm_DonVi.DonViCap1_ID = objDonViOld.DonViCap1_ID;
                dm_DonVi.TrangThai = objDonViOld.TrangThai;
            }

            if (ModelState.IsValid)
            {
                dm_DonVi.NgayCapNhat = DateTime.Now;
                db.Entry(dm_DonVi).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index","Home");
            }
            
            return View(dm_DonVi);
        }
        #endregion Update thêm thông tin đơn vị - Dùng cho cán bộ CNTT của từng đơn vị vào cập nhật

        public async Task<ActionResult> Delete(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            Dm_DonVi dm_DonVi = await db.Dm_DonVi.FindAsync(id);
            if (dm_DonVi == null)
            {
                //return HttpNotFound();
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                try
                {
                    dm_DonVi.TrangThai = TrangThai.KhongHoatDong;
                    db.Entry(dm_DonVi).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    ReloadListDonVi();
                    return Json(new { msg = "ok" });
                }
                catch (DbEntityValidationException ex)
                {
                    var sb = new StringBuilder();

                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("{0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }

                    return Json(new { msg = sb.ToString() });
                }
            }
        }

        public async Task<ActionResult> Active(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            Dm_DonVi dm_DonVi = await db.Dm_DonVi.FindAsync(id);
            if (dm_DonVi == null)
            {
                //return HttpNotFound();
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                dm_DonVi.TrangThai = TrangThai.HoatDong;
                db.Entry(dm_DonVi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ReloadListDonVi();
            }
            return Json(new { msg = "ok" });
        }

        #region Event
        /// <summary>
        /// Lấy danh sách Trực thuộc - View Create, Edit (hoặc ds đơn vị - View Index) theo Nhóm đơn vị
        /// </summary>
        /// <param name="nhomDonVi_ID">ID của nhóm đơn vị</param>
        /// <param name="view">Tên view cần hiển thị</param>
        public ActionResult NhomDonVi_OnChange(int nhomDonVi_ID, string view)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                IEnumerable<Dm_DonVi> list = new List<Dm_DonVi>();

                if (view == "Index")
                {
                    str.AppendFormat("<option value=''>Tất cả</option>");
                    list = S4T_HaTinhBase.ListDonViByNhomDonVi(nhomDonVi_ID);
                }
                else
                {
                    str.Append("<option value='-1'></option>");
                    if (nhomDonVi_ID == DonVi.NhomDonViCapXa)
                        list = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong && o.NhomDonVi_ID == DonVi.NhomDonViCapHuyen).OrderBy(o => o.TenDonVi);
                    else if (nhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                        list = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong && o.NhomDonVi_ID == DonVi.NhomDonViCapTinh).OrderBy(o => o.TenDonVi);

                }
                
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        str.AppendFormat("<option value='{0}'>{1}</option>", item.DonVi_ID, item.TenDonVi);
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
        #endregion

        /// <summary>
        /// Update lại các biến static Đơn vị
        /// </summary>
        private void ReloadListDonVi()
        {
            MvcApplication.ListDonVi = db.Dm_DonVi.ToList();
            MvcApplication.SelectListDonVi = new SelectList(db.Dm_DonVi.Where(o => o.TrangThai == TrangThai.HoatDong), "DonVi_ID", "TenDonVi");
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
