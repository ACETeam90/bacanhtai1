using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Data;
using System.IO;
using System.Data.Entity.Validation;
using S4T_HaTinh.Common;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Controllers
{
    public class ThongKeController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: ThongKe
        public ActionResult Index(int? NhomDonVi, int? PhanHeChucNang, int? Nam, int? DotBaoCao, int? TrangThai)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            GetViewBag(NhomDonVi);
            ViewBag.TrangThai = TrangThai;

            return View();
        }

        private void GetViewBag(int? nhomDonVi_ID)
        {
            Dm_DonVi objDonVi = new Dm_DonVi();
            IEnumerable<Dm_DonVi> listDonVi = new List<Dm_DonVi>();

            // Nhóm đơn vị
            ViewBag.ListNhomDonVi = MvcApplication.ListNhomDonVi(); 

            // Đợt báo cáo
            ViewBag.ListDotBaoCao = MvcApplication.ListDotBaoCao().Where(o => o.TrangThai == TrangThai.HoatDong);

            if (nhomDonVi_ID == null)
            {
                var firstNhomDonVi = MvcApplication.ListNhomDonVi().Where(o => o.DanhMuc_ID != S4T_HaTinh.Common.DonVi.NhomDonViCapXa).FirstOrDefault();
                listDonVi = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong && o.NhomDonVi_ID == firstNhomDonVi.DanhMuc_ID);
            }
            else
            {
                listDonVi = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong && o.NhomDonVi_ID == nhomDonVi_ID);
            }

            objDonVi = listDonVi.FirstOrDefault();

            // Danh mục phân hệ nhập liệu
            if (objDonVi != null)
            {
                // Danh mục phân hệ nhập liệu
                if (objDonVi.NhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                    ViewBag.ListPhanHeChucNang = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && MaBaoCao.BaoCaoHuyen.Contains(o.PhanHeChucNang_ID)).OrderBy(o => o.TenChucNang).ToList();
                else
                    ViewBag.ListPhanHeChucNang = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && MaBaoCao.BaoCaoSo.Contains(o.PhanHeChucNang_ID)).OrderBy(o => o.TenChucNang).ToList();
            }

            //ViewBag.SelectListDonVi = new SelectList(listDonVi, "DonVi_ID", "TenDonVi");
        }

        // GET: ThongKe/Details/5
        public ActionResult ReportView(ReportModel model, int trangThai)
        {
            if (model.PhanHeChucNang == 0)
                return Content("Chưa chọn báo cáo"); // PartialView();

            try
            {
                switch (model.PhanHeChucNang)
                {
                    case 10: // Hạ tầng kỹ thuật
                        var listHaTangKyThuat = db.sp_GetHaTangKyThuat(model.NhomDonVi,model.DotBaoCao,model.Nam,TrangThaiNhapLieu.PheDuyet);
                        ViewBag.SoCBCC = listHaTangKyThuat.Sum(m => m.SoCBCC);
                        ViewBag.SoDonViTrucThuoc = listHaTangKyThuat.Sum(m => m.SoDonViTrucThuoc);
                        ViewBag.SoCB_DVTT = listHaTangKyThuat.Sum(m => m.SoCB_DVTT);
                        ViewBag.MayBan = listHaTangKyThuat.Sum(m => m.MayBan);
                        ViewBag.LapTop = listHaTangKyThuat.Sum(m => m.LapTop);
                        ViewBag.MayChu = listHaTangKyThuat.Sum(m => m.MayChu);
                        ViewBag.MayNoiMangInternet = listHaTangKyThuat.Sum(m => m.MayNoiMangInternet);
                        ViewBag.VirusCoBanQuyen = listHaTangKyThuat.Sum(m => m.VirusCoBanQuyen);
                        ViewBag.MayChieu = listHaTangKyThuat.Sum(m => m.MayChieu);
                        ViewBag.MayScan = listHaTangKyThuat.Sum(m => m.MayScan);
                        ViewBag.MayIn = listHaTangKyThuat.Sum(m => m.MayIn);
                        ViewBag.MayAnh = listHaTangKyThuat.Sum(m => m.MayAnh);
                        ViewBag.MayQuayPhim = listHaTangKyThuat.Sum(m => m.MayQuayPhim);
                        ViewBag.MayTinhBang = listHaTangKyThuat.Sum(m => m.MayTinhBang);
                        ViewBag.MayBan_DVTT = listHaTangKyThuat.Sum(m => m.MayBan_DVTT);
                        ViewBag.LapTop_DVTT = listHaTangKyThuat.Sum(m => m.LapTop_DVTT);
                        ViewBag.MayChu_DVTT = listHaTangKyThuat.Sum(m => m.MayChu_DVTT);
                        ViewBag.MayNoiMangInternet_DVTT = listHaTangKyThuat.Sum(m => m.MayNoiMangInternet_DVTT);
                        ViewBag.VirusCoBanQuyen_DVTT = listHaTangKyThuat.Sum(m => m.VirusCoBanQuyen_DVTT);
                        ViewBag.MayChieu_DVTT = listHaTangKyThuat.Sum(m => m.MayChieu_DVTT);
                        ViewBag.MayScan_DVTT = listHaTangKyThuat.Sum(m => m.MayScan_DVTT);
                        ViewBag.MayIn_DVTT = listHaTangKyThuat.Sum(m => m.MayIn_DVTT);
                        ViewBag.MayAnh_DVTT = listHaTangKyThuat.Sum(m => m.MayAnh_DVTT);
                        ViewBag.MayQuayPhim_DVTT = listHaTangKyThuat.Sum(m => m.MayQuayPhim_DVTT);
                        ViewBag.MayTinhBang_DVTT = listHaTangKyThuat.Sum(m => m.MayTinhBang_DVTT);
                        ViewBag.ADSL = listHaTangKyThuat.Sum(m => m.ADSL);
                        ViewBag.LeasedLine = listHaTangKyThuat.Sum(m => m.LeasedLine);
                        ViewBag.CapQuang = listHaTangKyThuat.Sum(m => m.CapQuang);
                        ViewBag.MangChuyenDung = listHaTangKyThuat.Sum(m => m.MangChuyenDung);
                        ViewBag.DungLuong_ADSL = listHaTangKyThuat.Sum(m => m.DungLuong_ADSL);
                        ViewBag.DungLuong_CapQuang = listHaTangKyThuat.Sum(m => m.DungLuong_CapQuang);
                        ViewBag.DungLuong_Leased = listHaTangKyThuat.Sum(m => m.DungLuong_Leased);
                        ViewBag.DungLuong_MangChuyenDung = listHaTangKyThuat.Sum(m => m.DungLuong_MangChuyenDung);
                        ViewBag.ManHinh_LichCongTac = listHaTangKyThuat.Sum(m => m.ManHinh_LichCongTac);
                        ViewBag.ManHinh_TraCuuTTHC = listHaTangKyThuat.Sum(m => m.ManHinh_TraCuuTTHC);
                        ViewBag.TongMucDauTu = listHaTangKyThuat.Sum(m => m.TongMucDauTu);
                        return PartialView("HaTangKyThuat", listHaTangKyThuat);
                        break;

                    case 11: // Hạ tầng nhân lực CNTT
                        var listHaTangNhanLuc = db.sp_GetHaTangNhanLuc(model.NhomDonVi,model.DotBaoCao,model.Nam,TrangThaiNhapLieu.PheDuyet);
                        ViewBag.SoCBCC = listHaTangNhanLuc.Sum(m => m.SoCBCC);
                        ViewBag.SoDonViTrucThuoc = listHaTangNhanLuc.Sum(m => m.SoDonViTrucThuoc);
                        ViewBag.SoCB_DVTT = listHaTangNhanLuc.Sum(m => m.SoCB_DVTT);
                        ViewBag.TienSy_DV = listHaTangNhanLuc.Sum(m => m.TienSy_DV);
                        ViewBag.ThacSy_DV = listHaTangNhanLuc.Sum(m => m.ThacSy_DV);
                        ViewBag.DaiHocChinhQuy_DV = listHaTangNhanLuc.Sum(m => m.DaiHocChinhQuy_DV);
                        ViewBag.DaiHocKhongChinhQuy_DV = listHaTangNhanLuc.Sum(m => m.DaiHocKhongChinhQuy_DV);
                        ViewBag.LuotTapHuan_DV = listHaTangNhanLuc.Sum(m => m.LuotTapHuan_DV);
                        ViewBag.SoLuongCanBo_DV = listHaTangNhanLuc.Sum(m => m.SoLuongCanBo_DV);
                        ViewBag.TienSy_DVTT = listHaTangNhanLuc.Sum(m => m.TienSy_DVTT);
                        ViewBag.ThacSy_DVTT = listHaTangNhanLuc.Sum(m => m.ThacSy_DVTT);
                        ViewBag.DaiHocChinhQuy_DVTT = listHaTangNhanLuc.Sum(m => m.DaiHocChinhQuy_DVTT);
                        ViewBag.DaiHocKhongChinhQuy_DVTT = listHaTangNhanLuc.Sum(m => m.DaiHocKhongChinhQuy_DVTT);
                        ViewBag.LuotTapHuan_DVTT = listHaTangNhanLuc.Sum(m => m.LuotTapHuan_DVTT);
                        ViewBag.SoLuongCanBo_DVTT = listHaTangNhanLuc.Sum(m => m.SoLuongCanBo_DVTT);
                        ViewBag.ThanhThaoMayTinh_DV = listHaTangNhanLuc.Sum(m => m.ThanhThaoMayTinh_DV);
                        ViewBag.LuotTapHuanThanhThaoMayTinh_DV = listHaTangNhanLuc.Sum(m => m.LuotTapHuanThanhThaoMayTinh_DV);
                        ViewBag.ThanhThaoMayTinh_DVTT = listHaTangNhanLuc.Sum(m => m.ThanhThaoMayTinh_DVTT);
                        ViewBag.LuotTapHuanThanhThaoMayTinh_DVTT = listHaTangNhanLuc.Sum(m => m.LuotTapHuanThanhThaoMayTinh_DVTT);
                        ViewBag.TongMucDauTu = listHaTangNhanLuc.Sum(m => m.TongMucDauTu);
                        return PartialView("HaTangNhanLuc", listHaTangNhanLuc);
                        break;

                    case 12: // Ứng dụng CNTT
                        var listUngDungCNTT = db.sp_GetUngDungCNTT(model.NhomDonVi,model.DotBaoCao,model.Nam,TrangThaiNhapLieu.PheDuyet);
                        ViewBag.SoCBCC = listUngDungCNTT.Sum(m => m.SoCBCC);
                        ViewBag.SoDonViTrucThuoc = listUngDungCNTT.Sum(m => m.SoDonViTrucThuoc);
                        ViewBag.SoCB_DVTT = listUngDungCNTT.Sum(m => m.SoCB_DVTT);
                        ViewBag.SoTTHC = listUngDungCNTT.Sum(m => m.SoTTHC);
                        ViewBag.DV_LienThong = listUngDungCNTT.Sum(m => m.DV_LienThong);
                        ViewBag.VBDi = listUngDungCNTT.Sum(m => m.VBDi);
                        ViewBag.VBDen = listUngDungCNTT.Sum(m => m.VBDen);
                        ViewBag.TongSo_VBDi = listUngDungCNTT.Sum(m => m.TongSo_VBDi);
                        ViewBag.TongSo_VBDen = listUngDungCNTT.Sum(m => m.TongSo_VBDen);
                        ViewBag.SoEmail = listUngDungCNTT.Sum(m => m.SoEmail);
                        ViewBag.OpenOffice = listUngDungCNTT.Sum(m => m.OpenOffice);
                        ViewBag.MozillaThunderBird = listUngDungCNTT.Sum(m => m.MozillaThunderBird);
                        ViewBag.MozillaFireFox = listUngDungCNTT.Sum(m => m.MozillaFireFox);
                        ViewBag.Unikey = listUngDungCNTT.Sum(m => m.Unikey);
                        ViewBag.PMNM = listUngDungCNTT.Sum(m => m.PMNM);
                        ViewBag.PMNM_MayChu = listUngDungCNTT.Sum(m => m.PMNM_MayChu);
                        ViewBag.TongMucDauTu = listUngDungCNTT.Sum(m => m.TongMucDauTu);
                        return PartialView("UngDungCNTT", listUngDungCNTT);
                        break;

                    case 14: // Cổng thông tin điện tử
                        var listCongTTDT = db.sp_GetCongThongTinDienTu(model.NhomDonVi,model.DotBaoCao,model.Nam,TrangThaiNhapLieu.PheDuyet);
                        ViewBag.SoCBCC = listCongTTDT.Sum(m => m.SoCBCC);
                        ViewBag.SoDonViTrucThuoc = listCongTTDT.Sum(m => m.SoDonViTrucThuoc);
                        ViewBag.SoCB_DVTT = listCongTTDT.Sum(m => m.SoCB_DVTT);
                        ViewData["VanBanChiDao"] = listCongTTDT.Sum(m => m.VanBanChiDao);
                        ViewData["VanBanChiDao_DangTai"] = listCongTTDT.Sum(m => m.VanBanChiDao_DangTai);
                        ViewData["TTHC_DonVi"] = listCongTTDT.Sum(m => m.TTHC_DonVi);
                        ViewData["DVC1"] = listCongTTDT.Sum(m => m.DVC1);
                        ViewData["DVC2"] = listCongTTDT.Sum(m => m.DVC2);
                        ViewData["DVC3"] = listCongTTDT.Sum(m => m.DVC3);
                        ViewData["TongMucDauTu"] = listCongTTDT.Sum(m => m.TongMucDauTu);
                        return PartialView("CongThongTinDienTu", listCongTTDT);
                        break;

                    case 15: // Hạ tầng kỹ thuật CNTT cấp Huyện

                        break;

                    case 16: // Hạ tầng nhân lực CNTT cấp Huyện

                        break;

                    case 20: // Cổng Thông Tin Điện Tử Cấp Huyện

                        break;

                    case 21: // Ứng Dụng CNTT Cấp Huyện

                        break;
                    default:
                        return PartialView("ToChucChinhSachCNTT");
                        break;
                }
            }
            catch (DbEntityValidationException ex)
            {
                var exc = new ExceptionViewer();
                exc.GetError(ex);
            }
            
            return PartialView();
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
