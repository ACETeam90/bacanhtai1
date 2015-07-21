using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Text;
using System.Web.Mvc;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class Ht_QuanLyNhapLieuController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();        

        // GET: Ht_CauHinhUpload
        public ActionResult Index(int? donVi_ID, int? phanHe_ID, int? trangThai)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            GetViewBag();

            if (donVi_ID == null && phanHe_ID == null && trangThai == null)
                return View();

            var listLichNhap = (IEnumerable<sp_LichNhapLieuWithOption_Result>) db.sp_LichNhapLieuWithOption(donVi_ID, phanHe_ID, trangThai).OrderBy(o => o.TuNgay);
            return View(listLichNhap);
        }

        /// <summary>
        /// Lấy ViewBag All
        /// </summary>
        private void GetViewBag()
        {
            if (MvcApplication.ListDonVi.Count() > 0)
                ViewBag.ListDonVi = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong).ToList();

            // Danh mục phân hệ nhập liệu
            var dmPhanHeChucNangQLTT = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && o.PhanHe_ID == PhanHe.QuanLyThongTin).OrderBy(o => o.TenChucNang).ToList();
            if (dmPhanHeChucNangQLTT.Count() > 0)
                ViewBag.ListPhanHeChucNang = dmPhanHeChucNangQLTT;
        }

        /// <summary>
        /// Lọc danh sách các báo cáo theo đơn vị
        /// </summary>
        /// <param name="donVi_ID">Mã đơn vị</param>
        public ActionResult GetReportByDonVi(int donVi_ID, int? phanHe_ID, int? trangThaiNhapLieu)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            try
            {
                var listLichNhap = db.Ht_LichNhapLieu.Where(o => o.TrangThai == TrangThai.HoatDong && o.DonVi_ID == donVi_ID);
                var listPhanHeChucNang = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && o.PhanHe_ID == PhanHe.QuanLyThongTin).ToList();
                var objPhanHe = new Ht_PhanHeChucNang();

                if (trangThaiNhapLieu != null)
                    listLichNhap = listLichNhap.Where(o => o.ChucNang_ID == trangThaiNhapLieu);
                if (phanHe_ID != null)
                {
                    objPhanHe = listPhanHeChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == phanHe_ID);
                    listLichNhap = listLichNhap.Where(o => o.PhanHe_ID == phanHe_ID);
                }

                if (listLichNhap.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var item in listLichNhap)
                    {
                        if(phanHe_ID == null)
                            objPhanHe = listPhanHeChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == item.PhanHe_ID);

                        var objDanhMuc = MvcApplication.ListTrangThaiNhapLieu().FirstOrDefault(o => o.DanhMuc_ID == item.ChucNang_ID);
                        sb.Append("<tr>");
                        sb.AppendFormat("<td>{0}</td>", objPhanHe == null ? "" : objPhanHe.TenChucNang);
                        sb.AppendFormat("<td>{0}</td>", item.TuNgay.ToShortDateString());
                        sb.AppendFormat("<td>{0}</td>", item.DenNgay.ToShortDateString());
                        sb.AppendFormat("<td>{0}</td>", objDanhMuc == null ? "" : objDanhMuc.TenDanhMuc);

                        // Trạng thái: Chờ duyệt
                        if (item.ChucNang_ID == TrangThaiNhapLieu.DaGui)
                        {
                            // Duyệt báo cáo
                            sb.AppendFormat("<td><a href='/{0}/Check?donVi_ID={1}&lichNhap_ID={2}'>Kiểm tra</a></td>", objPhanHe.ControllerName, item.DonVi_ID, item.LichNhap_ID);
                        }
                        else if (item.ChucNang_ID == TrangThaiNhapLieu.PheDuyet || item.ChucNang_ID == TrangThaiNhapLieu.Sua)
                        {
                            // Xem báo cáo
                            sb.AppendFormat("<td><a href='/{0}/Details?donVi_ID={1}&lichNhap_ID={2}'>Xem</a></td>", objPhanHe.ControllerName, item.DonVi_ID, item.LichNhap_ID);
                        }
                        else
                            sb.Append("<td></td>");
                        sb.Append("</tr>");
                    }
                    return Json(new { danhSach = sb.ToString() });
                }
                else
                {
                    return Json(new { danhSach = "Không có dữ liệu" });
                }
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
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
