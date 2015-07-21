using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Novacode;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class CongThongTinDienTusController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();
        //private string currentController = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
        private string listInputRadio = "CoCauToChuc,ChucNang_DVTT,LichSuHinhThanh,ThongTinTomTat,ThongTinGiaoDich,ThongTinLienHe,BanDoDiaGioi,ThongTinThongKe,TanSuatCapNhat,LichLamViec,PhoBienVanBan,Tin_BaiViet,ChuyenMuc_ChuyenTrang,ChienLuoc_QuyHoach,KeHoachPhatTrien,VanBanQuyPham,VanBanQuyPham_DangTai,ChuyenTrang_QuanLy,VanBanQuyPham_TaiVe,HangMucDauTu,SoDuAn,ThongTinToiThieu,DeTaiKhoaHoc,SoLuongDeTaiKH_DangTai,GopY,YKienDangTai,CungCapThongTin,ChucNangTimKiem,SoDoWebsite,TraoDoi_HoiDap,DuLieuDacTa,Unicode,TuongThichTrinhDuyet,LienKetWebsite,NguoiKhuyetTat,TenMien_CTTDT,BanBienTap,QuanTriKyThuat,DichVuCongTT,TapHuanDaoTao,BienPhapKyThuat,GiaiPhapHieuQua,PhuongAnDuPhong,BanHanhQuyChe,BanHanhQuyChe_TTDT,BaoCaoHangNam_TTDT,CapNhatThongTin,NoiDungThongTin,QuyDinhKhac,";
        private Ht_PhanHeChucNang objChucNang = new S4T_HaTinhEntities().Ht_PhanHeChucNang.ToList().FirstOrDefault(o => o.ControllerName == System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller"));

        // GET: HaTangNhanLucCNTTs
        public async Task<ActionResult> Index()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            var objDonVi = db.Dm_DonVi.Find(user.DonVi_ID);
            ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";

            var listLichNhapByDonVi = db.Ht_LichNhapLieu.Where(o => o.TrangThai == TrangThai.HoatDong && o.DonVi_ID == user.DonVi_ID && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID);
            ViewBag.ListLichNhap = listLichNhapByDonVi;
            if (listLichNhapByDonVi.Any(o => o.ChucNang_ID == TrangThaiNhapLieu.ThemMoi))
                ViewBag.CreateLink = true;

            return View(await db.CongThongTinDienTu.Where(o => o.DonVi_ID == user.DonVi_ID).ToListAsync());
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
                    var objReport = db.CongThongTinDienTu.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                        && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                        && o.Success != TrangThaiNhapLieu.PheDuyet);
                    return objReport == null ? "" : ExceptionViewer.GetMessage("LICH_NHAP_NOT_FOUND");
                }
                else
                {
                    // Kiểm tra đã tồn tại bản ghi trong database?
                    var objReport = db.CongThongTinDienTu.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
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
            ViewBag.listInputRadio = listInputRadio;
        }

        // GET: CongThongTinDienTus/Details/5
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

            var congThongTinDienTu = new CongThongTinDienTu();
            if (id == null)
            {
                if (lichNhap_ID == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                else
                    congThongTinDienTu = await db.CongThongTinDienTu.FirstOrDefaultAsync(o => o.LichNhap_ID == lichNhap_ID
                                                                            && o.DonVi_ID == donVi_ID);
            }
            else
                congThongTinDienTu = await db.CongThongTinDienTu.FindAsync(id);

            GetViewBag(congThongTinDienTu.DonVi_ID);

            if (congThongTinDienTu == null)
                return HttpNotFound();
            
            return View(congThongTinDienTu);
        }

        // GET: CongThongTinDienTus/Create
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
                CongThongTinDienTu obj = new CongThongTinDienTu();
                obj.Success = (byte)TrangThaiNhapLieu.ThemMoi;
                obj.DonVi_ID = user.DonVi_ID;

                return View(obj);
            }
            
            return Content(mess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CongThongTinDienTu congThongTinDienTu)
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
                                                                          && o.DonVi_ID == congThongTinDienTu.DonVi_ID
                                                                          && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                          && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);

                    // Đổi trạng thái nhập liệu
                    congThongTinDienTu.Success = (byte)TrangThaiNhapLieu.DaGui;
                    congThongTinDienTu.LichNhap_ID = objLichNhap.LichNhap_ID; // Add LichNhap_ID vào báo cáo
                    db.CongThongTinDienTu.Add(congThongTinDienTu);

                    // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                    //objLichNhap.BaoCao_ID = congThongTinDienTu.CongThongTinDienTu_ID;
                    objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                    db.Entry(objLichNhap).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                return Content(mess);
            }

            GetViewBag(user.DonVi_ID);
            return View(congThongTinDienTu);
        }

        // GET: CongThongTinDienTus/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var mess = CheckReportStatus(user, TrangThaiNhapLieu.Sua);
            if (String.IsNullOrEmpty(mess))
            {
                GetViewBag(user.DonVi_ID);
                CongThongTinDienTu congThongTinDienTu = await db.CongThongTinDienTu.FindAsync(id);
                if (congThongTinDienTu == null)
                {
                    return HttpNotFound();
                }
                return View(congThongTinDienTu);
            }

            return Content(mess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CongThongTinDienTu congThongTinDienTu)
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
                    try
                    {
                        using (var context = new S4T_HaTinhEntities())
                        {
                            congThongTinDienTu.Success = (byte)TrangThaiNhapLieu.DaGui;
                            //haTangNhanLucCNTT.TruongNhapLai = string.Empty; // Xóa hết các yêu cầu nhập lại dữ liệu
                            context.Entry(congThongTinDienTu).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                        }

                        // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                        var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                                          && o.DonVi_ID == congThongTinDienTu.DonVi_ID
                                                                              && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                              && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
                        objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                        db.Entry(objLichNhap).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        var exv = new ExceptionViewer(ex);
                        return Json(new { 
                            //msg = exv.GetErrorMessage(ex.Message) 
                            msg = exv.GetErrorMessage("Có lỗi dữ liệu xảy ra") 
                        });
                    }
                }

                return Content(mess);
            }

            GetViewBag(user.DonVi_ID);
            return View(congThongTinDienTu);
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
            var congThongTinDienTu = db.CongThongTinDienTu.FirstOrDefault(o => o.DonVi_ID == donVi_ID
                                                                            && o.Success == (byte)TrangThaiNhapLieu.DaGui
                                                                            && o.LichNhap_ID == lichNhap_ID);
            //haTangNhanLucCNTT haTangNhanLucCNTT = await db.haTangNhanLucCNTT.FindAsync(objReport.);
            if (congThongTinDienTu == null)
                return HttpNotFound();
            
            return View(congThongTinDienTu);
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
                CongThongTinDienTu congThongTinDienTu = db.CongThongTinDienTu.Find(id);
                if (congThongTinDienTu == null)
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
                    GetViewBag(congThongTinDienTu.DonVi_ID); // Get ViewBag
                    ViewBag.LichNhap_ID = lichNhap_ID;
                    return Json(new { msg = msg });
                }
                #endregion Kiểm tra thời gian nhập liệu

                #region Update congThongTinDienTu
                congThongTinDienTu.TruongNhapLai = truongNhapLai;
                congThongTinDienTu.Success = (byte)TrangThaiNhapLieu.Sua;
                db.Entry(congThongTinDienTu).State = EntityState.Modified;
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
                CongThongTinDienTu congThongTinDienTu = db.CongThongTinDienTu.Find(id);
                if (congThongTinDienTu == null)
                {
                    return HttpNotFound();
                }

                #region Update congThongTinDienTu
                congThongTinDienTu.Success = (byte)TrangThaiNhapLieu.PheDuyet;
                congThongTinDienTu.TruongNhapLai = string.Empty;
                db.Entry(congThongTinDienTu).State = EntityState.Modified;
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
            CongThongTinDienTu obj = db.CongThongTinDienTu.FirstOrDefault(o => o.CongThongTinDienTu_ID == id);
            if (obj == null) return Content("Không tìm thấy báo cáo");

            string wordTemplate = ExportDoc.GetTemplateByChucNang(Server.MapPath("/Templates"), typeof(CongThongTinDienTu));
            DocX doc = DocX.Load(wordTemplate);
            if (doc == null) return Content("Không tìm thấy mẫu báo cáo");

            foreach (var prop in obj.GetType().GetProperties())
            {
                string value = prop.GetValue(obj) != null ? prop.GetValue(obj).ToString() : "";

                // Đổi dữ liệu trường True/false
                if (listInputRadio.Contains(prop.Name))
                {
                    value = ConvertAttributeValue.MultiChoiceValue(value);
                }
                doc.AddCustomProperty(new CustomProperty("@" + prop.Name, value ?? ""));
            }

            var storeStream = new MemoryStream();
            doc.SaveAs(storeStream);
            StringBuilder fileName = new StringBuilder();
            fileName.AppendFormat("{0}{1}{2}{3}{4}{5}_{6}", DateTime.Now.Second, DateTime.Now.Minute, DateTime.Now.Hour, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, "CongThongTinDienTu.docx");
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
