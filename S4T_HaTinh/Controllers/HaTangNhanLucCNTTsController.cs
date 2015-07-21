using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Text;
using System.IO;
using Novacode;
using System.Web.Mvc;
using System.Globalization;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class HaTangNhanLucCNTTsController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();
        //private string currentController = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
        private Ht_PhanHeChucNang objChucNang = new S4T_HaTinhEntities().Ht_PhanHeChucNang.ToList().FirstOrDefault(o => o.ControllerName == System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller"));

        // GET: HaTangNhanLucCNTTs
        public async Task<ActionResult> Index()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            // Get ViewBag
            var listLichNhapByDonVi = db.Ht_LichNhapLieu.Where(o => o.TrangThai == TrangThai.HoatDong && o.DonVi_ID == user.DonVi_ID && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID);
            ViewBag.ListLichNhap = listLichNhapByDonVi;
            if (listLichNhapByDonVi.Any(o => o.ChucNang_ID == TrangThaiNhapLieu.ThemMoi))
                ViewBag.CreateLink = true;
            var objDonVi = db.Dm_DonVi.Find(user.DonVi_ID);
            ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";

            return View(await db.HaTangNhanLucCNTT.Where(o => o.DonVi_ID == user.DonVi_ID).ToListAsync());
        }

        /// <summary>
        /// Check trạng thái báo cáo theo lịch nhập liệu 
        /// </summary>
        /// <param name="user">object người dùng</param>
        /// <param name="status">Trạng thái của báo cáo</param>
        private string CheckReportStatus(ApplicationUser user, int status)
        {
            var _objLichNhapLieu = S4T_HaTinhBase.GetTrangThaiLichNhapLieu(user, objChucNang.PhanHeChucNang_ID);
            if (_objLichNhapLieu.TrangThai != TrangThaiLichNhapLieu.HoatDong)
                return ExceptionViewer.GetMessage(_objLichNhapLieu);
            else
            {
                if (status == TrangThaiNhapLieu.ThemMoi)
                {
                    // Kiểm tra đã tồn tại bản ghi trong database?
                    var objReport = db.HaTangNhanLucCNTT.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                        && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                        && o.Success != TrangThaiNhapLieu.PheDuyet);
                    return objReport == null ? "" : ExceptionViewer.GetMessage("LICH_NHAP_NOT_FOUND");
                }
                else
                {
                    // Kiểm tra đã tồn tại bản ghi trong database?
                    var objReport = db.HaTangNhanLucCNTT.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                        && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                        && o.Success == (byte)TrangThaiNhapLieu.Sua);
                    return objReport != null ? "" : ExceptionViewer.GetMessage("LICH_NHAP_NOT_FOUND");
                }
            }
        }

        /// <summary>
        /// Lấy các ViewBag
        /// </summary>
        private void GetViewBag(int donVi_ID)
        {
            // Đơn vị
            var objDonVi = db.Dm_DonVi.Find(donVi_ID);
            ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";
            var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                            && o.DonVi_ID == donVi_ID
                                                            && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                            && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
            if (objLichNhap != null)
            {
                ViewBag.TuNgay = objLichNhap.TuNgay;
                ViewBag.DenNgay = objLichNhap.DenNgay;
            }
            ViewBag.listInputRadio = ""; // Form ko dung radio thi de? tro^'ng
        }

        // GET: HaTangNhanLucCNTTs/Details/5
        public async Task<ActionResult> Details(int? id, int? donVi_ID, int? lichNhap_ID)
        {
            var returnUrl = Request.UrlReferrer;
            if (returnUrl != null)
                ViewBag.returnUrl = returnUrl.ToString();

            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny)
            {
                // Kiểm tra thêm quyền là Thẩm định báo cáo ?
                per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.UngDungCNTT);
                if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));
            }

            var obj = new HaTangNhanLucCNTT();
            if (id == null)
            {
                if (lichNhap_ID == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                else
                    obj = await db.HaTangNhanLucCNTT.FirstOrDefaultAsync(o => o.LichNhap_ID == lichNhap_ID
                                                                            && o.DonVi_ID == donVi_ID);
            }
            else
                obj = await db.HaTangNhanLucCNTT.FindAsync(id);

            GetViewBag(obj.DonVi_ID);

            if (obj == null)
                return HttpNotFound();

            return View(obj);
        }

        // GET: HaTangNhanLucCNTTs/Create
        public ActionResult Create()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            GetViewBag(user.DonVi_ID);
            var mess = CheckReportStatus(user, TrangThaiNhapLieu.ThemMoi);
            if (String.IsNullOrEmpty(mess))
            {
                HaTangNhanLucCNTT obj = new HaTangNhanLucCNTT();
                obj.Success = (byte)TrangThaiNhapLieu.ThemMoi;
                obj.DonVi_ID = user.DonVi_ID;

                return View(obj);
            }

            return Content(mess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "HaTangNhanLucCNTT_ID,DonVi_ID,FileUpload,TienSy_DV,ThacSy_DV,DaiHocChinhQuy_DV,DaiHocKhongChinhQuy_DV,LuotTapHuan_DV,SoLuongCanBo_DV,TienSy_DVTT,ThacSy_DVTT,DaiHocChinhQuy_DVTT,DaiHocKhongChinhQuy_DVTT,LuotTapHuan_DVTT,SoLuongCanBo_DVTT,ThanhThaoMayTinh_DV,LuotTapHuanThanhThaoMayTinh_DV,ThanhThaoMayTinh_DVTT,LuotTapHuanThanhThaoMayTinh_DVTT,Success,TruongNhapLai")] HaTangNhanLucCNTT haTangNhanLucCNTT)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                var mess = CheckReportStatus(user, TrangThaiNhapLieu.ThemMoi);
                if (String.IsNullOrEmpty(mess))
                {
                    var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                                          && o.DonVi_ID == haTangNhanLucCNTT.DonVi_ID
                                                                          && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                          && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);

                    // Đổi trạng thái nhập liệu
                    haTangNhanLucCNTT.Success = (byte)TrangThaiNhapLieu.DaGui;
                    haTangNhanLucCNTT.LichNhap_ID = objLichNhap.LichNhap_ID; // Add LichNhap_ID vào báo cáo
                    db.HaTangNhanLucCNTT.Add(haTangNhanLucCNTT);

                    // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                    //objLichNhap.BaoCao_ID = haTangNhanLucCNTT.HaTangNhanLucCNTT_ID;
                    objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                    db.Entry(objLichNhap).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                return Content(mess);
            }

            GetViewBag(user.DonVi_ID);
            return View(haTangNhanLucCNTT);
        }

        // GET: HaTangNhanLucCNTTs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            var mess = CheckReportStatus(user, TrangThaiNhapLieu.Sua);
            if (String.IsNullOrEmpty(mess))
            {
                GetViewBag(user.DonVi_ID);
                HaTangNhanLucCNTT haTangNhanLucCNTT = await db.HaTangNhanLucCNTT.FindAsync(id);
                if (haTangNhanLucCNTT == null)
                {
                    return HttpNotFound();
                }
                return View(haTangNhanLucCNTT);
            }

            return Content(mess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "HaTangNhanLucCNTT_ID,DonVi_ID,FileUpload,TienSy_DV,ThacSy_DV,DaiHocChinhQuy_DV,DaiHocKhongChinhQuy_DV,LuotTapHuan_DV,SoLuongCanBo_DV,TienSy_DVTT,ThacSy_DVTT,DaiHocChinhQuy_DVTT,DaiHocKhongChinhQuy_DVTT,LuotTapHuan_DVTT,SoLuongCanBo_DVTT,ThanhThaoMayTinh_DV,LuotTapHuanThanhThaoMayTinh_DV,ThanhThaoMayTinh_DVTT,LuotTapHuanThanhThaoMayTinh_DVTT,Success,TruongNhapLai,LichNhap_ID")] HaTangNhanLucCNTT haTangNhanLucCNTT)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                var mess = CheckReportStatus(user, TrangThaiNhapLieu.Sua);
                if (String.IsNullOrEmpty(mess))
                {
                    using (var context = new S4T_HaTinhEntities())
                    {
                        haTangNhanLucCNTT.Success = (byte)TrangThaiNhapLieu.DaGui;
                        //haTangNhanLucCNTT.TruongNhapLai = string.Empty; // Xóa hết các yêu cầu nhập lại dữ liệu
                        context.Entry(haTangNhanLucCNTT).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                    // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                    var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                                          && o.DonVi_ID == haTangNhanLucCNTT.DonVi_ID
                                                                          && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                          && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
                    objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                    db.Entry(objLichNhap).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                return Content(mess);
            }

            GetViewBag(user.DonVi_ID);
            return View(haTangNhanLucCNTT);
        }

        /// <summary>
        /// Kiểm tra dữ liệu 
        /// </summary>
        public ActionResult Check(int donVi_ID, int lichNhap_ID)
        {
            var returnUrl = Request.UrlReferrer;
            if (returnUrl != null)
                ViewBag.returnUrl = returnUrl.ToString();

            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.UngDungCNTT);
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            GetViewBag(donVi_ID); // Get ViewBag
            ViewBag.LichNhap_ID = lichNhap_ID;
            var haTangNhanLucCNTT = db.HaTangNhanLucCNTT.FirstOrDefault(o => o.DonVi_ID == donVi_ID
                                                                            && o.Success == (byte)TrangThaiNhapLieu.DaGui
                                                                            && o.LichNhap_ID == lichNhap_ID);
            //haTangNhanLucCNTT haTangNhanLucCNTT = await db.haTangNhanLucCNTT.FindAsync(objReport.);
            if (haTangNhanLucCNTT == null)
                return HttpNotFound();
            
            return View(haTangNhanLucCNTT);
        }

        /// <summary>
        /// Yêu cầu đơn vị nhập lại số liệu với các trường bị đánh dấu sai
        /// </summary>
        /// <param name="id">ID của báo cáo</param>
        /// <param name="lichNhap_ID">ID của lịch nhập liệu</param>
        /// <param name="truongNhapLai">mã của các trường dữ liệu cần nhập lại</param>
        /// <param name="tuNgay">ngày bắt đầu nhập</param>
        /// <param name="denNgay">ngày kết thúc nhập</param>
        public ActionResult NhapLaiRequest(int id, int lichNhap_ID, string truongNhapLai, string tuNgay, string denNgay)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.UngDungCNTT);
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            try
            {
                HaTangNhanLucCNTT haTangNhanLucCNTT = db.HaTangNhanLucCNTT.Find(id);
                if (haTangNhanLucCNTT == null)
                {
                    return HttpNotFound();
                }

                #region Kiểm tra thời gian nhập liệu
                DateTime _tuNgay;
                DateTime _denNgay;
                var msg = string.Empty;
                if (!DateTime.TryParseExact(tuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _tuNgay))
                    msg = "'Từ ngày' nhập sai định dạng";
                if (!DateTime.TryParseExact(denNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _denNgay))
                    msg = "'Đến ngày' nhập sai định dạng";

                if (_tuNgay.Date < DateTime.Now.Date)
                    msg = "Thời gian 'Từ ngày' không được nhỏ hơn thời gian hiện tại";
                if (_denNgay.Date < _tuNgay.Date)
                    msg = "Thời gian 'Đến ngày' không được nhỏ hơn thời gian 'Từ ngày'";

                if (!string.IsNullOrEmpty(msg))
                {
                    GetViewBag(haTangNhanLucCNTT.DonVi_ID); // Get ViewBag
                    ViewBag.LichNhap_ID = lichNhap_ID;
                    return Json(new { msg = msg });
                }
                #endregion Kiểm tra thời gian nhập liệu

                #region Update haTangNhanLucCNTT
                haTangNhanLucCNTT.TruongNhapLai = truongNhapLai;
                haTangNhanLucCNTT.Success = (byte)TrangThaiNhapLieu.Sua;
                db.Entry(haTangNhanLucCNTT).State = EntityState.Modified;
                #endregion

                #region Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                Ht_LichNhapLieu objLichNhapLieu = db.Ht_LichNhapLieu.Find(lichNhap_ID);
                objLichNhapLieu.ChucNang_ID = (byte)TrangThaiNhapLieu.Sua;
                if (!string.IsNullOrEmpty(tuNgay))
                    objLichNhapLieu.TuNgay = DateTime.ParseExact(tuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(denNgay))
                    objLichNhapLieu.DenNgay = DateTime.ParseExact(denNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                db.Entry(objLichNhapLieu).State = EntityState.Modified;
                db.SaveChanges();
                #endregion

                return Json(new { ok = "ok" });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }

        /// <summary>
        /// Duyệt báo cáo thành công
        /// </summary>
        /// <param name="id">ID của báo cáo</param>
        /// <param name="lichNhap_ID">ID của lịch nhập liệu</param>
        public ActionResult ConfirmReport(int id, int lichNhap_ID)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.UngDungCNTT);
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            try
            {
                HaTangNhanLucCNTT haTangNhanLucCNTT = db.HaTangNhanLucCNTT.Find(id);
                if (haTangNhanLucCNTT == null)
                {
                    return HttpNotFound();
                }

                #region Update haTangNhanLucCNTT
                haTangNhanLucCNTT.Success = (byte)TrangThaiNhapLieu.PheDuyet;
                haTangNhanLucCNTT.TruongNhapLai = string.Empty;
                db.Entry(haTangNhanLucCNTT).State = EntityState.Modified;
                #endregion

                #region Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                Ht_LichNhapLieu objLichNhapLieu = db.Ht_LichNhapLieu.Find(lichNhap_ID);
                objLichNhapLieu.ChucNang_ID = (byte)TrangThaiNhapLieu.PheDuyet;
                db.Entry(objLichNhapLieu).State = EntityState.Modified;
                db.SaveChanges();
                #endregion

                return Json(new { ok = "ok" });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }
        
        /// <summary>
        /// Xuất báo cáo ra word
        /// </summary>
        /// <param name="id">Mã báo cáo</param>
        public ActionResult ExportToWord(int id)
        {
            HaTangNhanLucCNTT obj = db.HaTangNhanLucCNTT.FirstOrDefault(o => o.HaTangNhanLucCNTT_ID == id);
            if (obj == null) return Content("Không tìm thấy báo cáo");

            string wordTemplate = ExportDoc.GetTemplateByChucNang(Server.MapPath("/Templates"), typeof(HaTangNhanLucCNTT));
            DocX doc = DocX.Load(wordTemplate);
            if (doc == null) return Content("Không tìm thấy mẫu báo cáo");
            
            foreach (var prop in obj.GetType().GetProperties())
            {
                string value = prop.GetValue(obj) != null ? prop.GetValue(obj).ToString() : "";
                doc.AddCustomProperty(new CustomProperty("@" + prop.Name, value ?? ""));
            }

            var storeStream = new MemoryStream();
            doc.SaveAs(storeStream);
            StringBuilder fileName = new StringBuilder();
            fileName.AppendFormat("{0}{1}{2}{3}{4}{5}_{6}", DateTime.Now.Second, DateTime.Now.Minute, DateTime.Now.Hour, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, "HaTangNhanLuc.docx");
            return File(storeStream.ToArray(), "application/x-msworks", fileName.ToString());
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
