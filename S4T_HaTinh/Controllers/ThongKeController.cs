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
using System.Text;

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

        public ActionResult ChangeListBaoCao(int nhomDonVi_ID)
        {
            //var objDonVi = MvcApplication.ListDonVi.FirstOrDefault(o => o.DonVi_ID == donVi_ID);
            var listPhanHeChucNang = new List<Ht_PhanHeChucNang>();

            // Danh mục phân hệ nhập liệu
            if (nhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                listPhanHeChucNang = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && MaBaoCao.BaoCaoHuyen.Contains(o.PhanHeChucNang_ID)).OrderBy(o => o.TenChucNang).ToList();
            else
                listPhanHeChucNang = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && MaBaoCao.BaoCaoSo.Contains(o.PhanHeChucNang_ID)).OrderBy(o => o.TenChucNang).ToList();

            var str = new StringBuilder();

            if (listPhanHeChucNang.Any())
            {
                foreach (var item in listPhanHeChucNang)
                {
                    str.AppendFormat("<option value='{0}'>{1}</option>", item.PhanHeChucNang_ID, item.TenChucNang);
                }
                return Json(new { danhSach = str.ToString() });
            }

            return Json(new { msg = "Không tìm thấy danh mục báo cáo" });
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
                        var listHaTangKyThuat = db.sp_GetHaTangKyThuat(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
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
                        var listHaTangNhanLuc = db.sp_GetHaTangNhanLuc(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
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
                        var listUngDungCNTT = db.sp_GetUngDungCNTT(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
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
                        var listCongTTDT = db.sp_GetCongThongTinDienTu(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
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
                        var listHaTangKyThuatCapHuyen = db.sp_GetHaTangKyThuatCapHuyen(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
                        ViewBag.SoCBCC = listHaTangKyThuatCapHuyen.Sum(m => m.SoCBCC);
                        ViewBag.SoDonViTrucThuoc = listHaTangKyThuatCapHuyen.Sum(m => m.SoDonViTrucThuoc);
                        ViewBag.SoCB_DVTT = listHaTangKyThuatCapHuyen.Sum(m => m.SoCB_DVTT);
                        ViewBag.ADSL = listHaTangKyThuatCapHuyen.Sum(m => m.ADSL);
                        ViewBag.LeadedLine = listHaTangKyThuatCapHuyen.Sum(m => m.LeadedLine);
                        ViewBag.CapQuang = listHaTangKyThuatCapHuyen.Sum(m => m.CapQuang);
                        ViewBag.Khac = listHaTangKyThuatCapHuyen.Sum(m => m.Khac);
                        ViewBag.UBND_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.UBND_MayBan);
                        ViewBag.UBND_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.UBND_LapTop);
                        ViewBag.UBND_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.UBND_MayChu);
                        ViewBag.UBND_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.UBND_VirusBanQuyen);
                        ViewBag.UBND_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.UBND_Internet);
                        ViewBag.UBND_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.UBND_MangCD);
                        ViewBag.HuyenUy_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.HuyenUy_MayBan);
                        ViewBag.HuyenUy_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.HuyenUy_LapTop);
                        ViewBag.HuyenUy_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.HuyenUy_MayChu);
                        ViewBag.HuyenUy_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.HuyenUy_VirusBanQuyen);
                        ViewBag.HuyenUy_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.HuyenUy_Internet);
                        ViewBag.HuyenUy_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.HuyenUy_MangCD);
                        ViewBag.Xa_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.Xa_MayBan);
                        ViewBag.Xa_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.Xa_LapTop);
                        ViewBag.Xa_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.Xa_MayChu);
                        ViewBag.Xa_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.Xa_VirusBanQuyen);
                        ViewBag.Xa_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.Xa_Internet);
                        ViewBag.Xa_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.Xa_MangCD);
                        ViewBag.GiaoDuc_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.GiaoDuc_MayBan);
                        ViewBag.GiaoDuc_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.GiaoDuc_LapTop);
                        ViewBag.GiaoDuc_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.GiaoDuc_MayChu);
                        ViewBag.GiaoDuc_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.GiaoDuc_VirusBanQuyen);
                        ViewBag.GiaoDuc_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.GiaoDuc_Internet);
                        ViewBag.GiaoDuc_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.GiaoDuc_MangCD);
                        ViewBag.YTe_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.YTe_MayBan);
                        ViewBag.YTe_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.YTe_LapTop);
                        ViewBag.YTe_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.YTe_MayChu);
                        ViewBag.YTe_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.YTe_VirusBanQuyen);
                        ViewBag.YTe_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.YTe_Internet);
                        ViewBag.YTe_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.YTe_MangCD);
                        ViewBag.Khac_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.Khac_MayBan);
                        ViewBag.Khac_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.Khac_LapTop);
                        ViewBag.Khac_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.Khac_MayChu);
                        ViewBag.Khac_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.Khac_VirusBanQuyen);
                        ViewBag.Khac_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.Khac_Internet);
                        ViewBag.Khac_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.Khac_MangCD);
                        ViewBag.VHX_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.VHX_MayBan);
                        ViewBag.VHX_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.VHX_LapTop);
                        ViewBag.VHX_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.VHX_MayChu);
                        ViewBag.VHX_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.VHX_VirusBanQuyen);
                        ViewBag.VHX_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.VHX_Internet);
                        ViewBag.VHX_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.VHX_MangCD);
                        ViewBag.DoanhNghiep_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.DoanhNghiep_MayBan);
                        ViewBag.DoanhNghiep_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.DoanhNghiep_LapTop);
                        ViewBag.DoanhNghiep_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.DoanhNghiep_MayChu);
                        ViewBag.DoanhNghiep_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.DoanhNghiep_VirusBanQuyen);
                        ViewBag.DoanhNghiep_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.DoanhNghiep_Internet);
                        ViewBag.DoanhNghiep_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.DoanhNghiep_MangCD);
                        ViewBag.HoGiaDinh_MayBan = listHaTangKyThuatCapHuyen.Sum(m => m.HoGiaDinh_MayBan);
                        ViewBag.HoGiaDinh_LapTop = listHaTangKyThuatCapHuyen.Sum(m => m.HoGiaDinh_LapTop);
                        ViewBag.HoGiaDinh_MayChu = listHaTangKyThuatCapHuyen.Sum(m => m.HoGiaDinh_MayChu);
                        ViewBag.HoGiaDinh_VirusBanQuyen = listHaTangKyThuatCapHuyen.Sum(m => m.HoGiaDinh_VirusBanQuyen);
                        ViewBag.HoGiaDinh_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.HoGiaDinh_Internet);
                        ViewBag.HoGiaDinh_MangCD = listHaTangKyThuatCapHuyen.Sum(m => m.HoGiaDinh_MangCD);
                        ViewBag.KhongKetNoiInternet = listHaTangKyThuatCapHuyen.Sum(m => m.KhongKetNoiInternet);
                        ViewBag.BuuDienXa = listHaTangKyThuatCapHuyen.Sum(m => m.BuuDienXa);
                        ViewBag.BuuDienXa_Internet = listHaTangKyThuatCapHuyen.Sum(m => m.BuuDienXa_Internet);
                        ViewBag.BuuDienXa_DaiLyInternet = listHaTangKyThuatCapHuyen.Sum(m => m.BuuDienXa_DaiLyInternet);
                        ViewBag.SoCuocHopCapHuyen = listHaTangKyThuatCapHuyen.Sum(m => m.SoCuocHopCapHuyen);
                        ViewBag.SoCuocHopCapXa = listHaTangKyThuatCapHuyen.Sum(m => m.SoCuocHopCapXa);
                        ViewBag.TongChiNganSach = listHaTangKyThuatCapHuyen.Sum(m => m.TongChiNganSach);
                        ViewBag.TongMucDauTu = listHaTangKyThuatCapHuyen.Sum(m => m.TongMucDauTu);
                        return PartialView("HaTangKyThuatCapHuyen", listHaTangKyThuatCapHuyen);
                        break;

                    case 16: // Hạ tầng nhân lực CNTT cấp Huyện
                        var listHaTangNhanLucCapHuyen = db.sp_GetHaTangNhanLucCapHuyen(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
                        ViewBag.TieuHoc_SoTruong = listHaTangNhanLucCapHuyen.Sum(m => m.TieuHoc_SoTruong);
                        ViewBag.TieuHoc_TinHoc = listHaTangNhanLucCapHuyen.Sum(m => m.TieuHoc_TinHoc);
                        ViewBag.TieuHoc_NganhCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TieuHoc_NganhCNTT);
                        ViewBag.TieuHoc_KhoaCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TieuHoc_KhoaCNTT);
                        ViewBag.TieuHoc_GVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TieuHoc_GVCNTT);
                        ViewBag.TieuHoc_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TieuHoc_TSCNTT);
                        ViewBag.TieuHoc_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TieuHoc_ThSCNTT);
                        ViewBag.TieuHoc_SVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TieuHoc_SVCNTT);
                        ViewBag.THCS_SoTruong = listHaTangNhanLucCapHuyen.Sum(m => m.THCS_SoTruong);
                        ViewBag.THCS_TinHoc = listHaTangNhanLucCapHuyen.Sum(m => m.THCS_TinHoc);
                        ViewBag.THCS_NganhCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THCS_NganhCNTT);
                        ViewBag.THCS_KhoaCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THCS_KhoaCNTT);
                        ViewBag.THCS_GVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THCS_GVCNTT);
                        ViewBag.THCS_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THCS_TSCNTT);
                        ViewBag.THCS_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THCS_ThSCNTT);
                        ViewBag.THCS_SVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THCS_SVCNTT);
                        ViewBag.THPT_SoTruong = listHaTangNhanLucCapHuyen.Sum(m => m.THPT_SoTruong);
                        ViewBag.THPT_TinHoc = listHaTangNhanLucCapHuyen.Sum(m => m.THPT_TinHoc);
                        ViewBag.THPT_NganhCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THPT_NganhCNTT);
                        ViewBag.THPT_KhoaCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THPT_KhoaCNTT);
                        ViewBag.THPT_GVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THPT_GVCNTT);
                        ViewBag.THPT_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THPT_TSCNTT);
                        ViewBag.THPT_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THPT_ThSCNTT);
                        ViewBag.THPT_SVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.THPT_SVCNTT);
                        ViewBag.TrungCap_SoTruong = listHaTangNhanLucCapHuyen.Sum(m => m.TrungCap_SoTruong);
                        ViewBag.TrungCap_TinHoc = listHaTangNhanLucCapHuyen.Sum(m => m.TrungCap_TinHoc);
                        ViewBag.TrungCap_NganhCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TrungCap_NganhCNTT);
                        ViewBag.TrungCap_KhoaCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TrungCap_KhoaCNTT);
                        ViewBag.TrungCap_GVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TrungCap_GVCNTT);
                        ViewBag.TrungCap_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TrungCap_TSCNTT);
                        ViewBag.TrungCap_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TrungCap_ThSCNTT);
                        ViewBag.TrungCap_SVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.TrungCap_SVCNTT);
                        ViewBag.CaoDang_SoTruong = listHaTangNhanLucCapHuyen.Sum(m => m.CaoDang_SoTruong);
                        ViewBag.CaoDang_MonTinHoc = listHaTangNhanLucCapHuyen.Sum(m => m.CaoDang_MonTinHoc);
                        ViewBag.CaoDang_NganhCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.CaoDang_NganhCNTT);
                        ViewBag.CaoDang_KhoaCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.CaoDang_KhoaCNTT);
                        ViewBag.CaoDang_GVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.CaoDang_GVCNTT);
                        ViewBag.CaoDang_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.CaoDang_TSCNTT);
                        ViewBag.CaoDang_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.CaoDang_ThSCNTT);
                        ViewBag.CaoDang_SVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.CaoDang_SVCNTT);
                        ViewBag.DaiHoc_SoTruong = listHaTangNhanLucCapHuyen.Sum(m => m.DaiHoc_SoTruong);
                        ViewBag.DaiHoc_MonTinHoc = listHaTangNhanLucCapHuyen.Sum(m => m.DaiHoc_MonTinHoc);
                        ViewBag.DaiHoc_NganhCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DaiHoc_NganhCNTT);
                        ViewBag.DaiHoc_KhoaCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DaiHoc_KhoaCNTT);
                        ViewBag.DaiHoc_GVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DaiHoc_GVCNTT);
                        ViewBag.DaiHoc_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DaiHoc_TSCNTT);
                        ViewBag.DaiHoc_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DaiHoc_ThSCNTT);
                        ViewBag.DaiHoc_SVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DaiHoc_SVCNTT);
                        ViewBag.Khac_SoTruong = listHaTangNhanLucCapHuyen.Sum(m => m.Khac_SoTruong);
                        ViewBag.Khac_MonTinHoc = listHaTangNhanLucCapHuyen.Sum(m => m.Khac_MonTinHoc);
                        ViewBag.Khac_NganhCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.Khac_NganhCNTT);
                        ViewBag.Khac_KhoaCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.Khac_KhoaCNTT);
                        ViewBag.Khac_GVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.Khac_GVCNTT);
                        ViewBag.Khac_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.Khac_TSCNTT);
                        ViewBag.Khac_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.Khac_ThSCNTT);
                        ViewBag.Khac_SVCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.Khac_SVCNTT);
                        ViewBag.CBCT_Huyen_SoLuong = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Huyen_SoLuong);
                        ViewBag.CBCT_Huyen_TienSy = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Huyen_TienSy);
                        ViewBag.CBCT_Huyen_ThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Huyen_ThacSy);
                        ViewBag.CBCT_Huyen_DHCQ = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Huyen_DHCQ);
                        ViewBag.CBCT_Huyen_DHKhongCQ = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Huyen_DHKhongCQ);
                        ViewBag.CBCT_Huyen_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Huyen_HocThacSy);
                        ViewBag.CBCT_Huyen_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Huyen_NghienCuuSinh);
                        ViewBag.CBCT_Huyen_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Huyen_LuotTapHuan);
                        ViewBag.CBCT_Xa_SoLuong = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Xa_SoLuong);
                        ViewBag.CBCT_Xa_TienSy = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Xa_TienSy);
                        ViewBag.CBCT_Xa_ThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Xa_ThacSy);
                        ViewBag.CBCT_Xa_DHCQ = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Xa_DHCQ);
                        ViewBag.CBCT_Xa_DHKhongCQ = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Xa_DHKhongCQ);
                        ViewBag.CBCT_Xa_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Xa_HocThacSy);
                        ViewBag.CBCT_Xa_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Xa_NghienCuuSinh);
                        ViewBag.CBCT_Xa_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.CBCT_Xa_LuotTapHuan);
                        ViewBag.UBND_ThanhThao = listHaTangNhanLucCapHuyen.Sum(m => m.UBND_ThanhThao);
                        ViewBag.UBND_ThamGiaTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.UBND_ThamGiaTapHuan);
                        ViewBag.UBND_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.UBND_TSCNTT);
                        ViewBag.UBND_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.UBND_ThSCNTT);
                        ViewBag.UBND_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.UBND_HocThacSy);
                        ViewBag.UBND_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.UBND_NghienCuuSinh);
                        ViewBag.HuyenUy_ThanhThao = listHaTangNhanLucCapHuyen.Sum(m => m.HuyenUy_ThanhThao);
                        ViewBag.HuyenUy_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.HuyenUy_LuotTapHuan);
                        ViewBag.HuyenUy_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.HuyenUy_TSCNTT);
                        ViewBag.HuyenUy_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.HuyenUy_ThSCNTT);
                        ViewBag.HuyenUy_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.HuyenUy_HocThacSy);
                        ViewBag.HuyenUy_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.HuyenUy_NghienCuuSinh);
                        ViewBag.Xa_ThanhThao = listHaTangNhanLucCapHuyen.Sum(m => m.Xa_ThanhThao);
                        ViewBag.Xa_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.Xa_LuotTapHuan);
                        ViewBag.Xa_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.Xa_TSCNTT);
                        ViewBag.Xa_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.Xa_ThSCNTT);
                        ViewBag.Xa_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.Xa_HocThacSy);
                        ViewBag.Xa_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.Xa_NghienCuuSinh);
                        ViewBag.GiaoDuc_ThanhThao = listHaTangNhanLucCapHuyen.Sum(m => m.GiaoDuc_ThanhThao);
                        ViewBag.GiaoDuc_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.GiaoDuc_LuotTapHuan);
                        ViewBag.GiaoDuc_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.GiaoDuc_TSCNTT);
                        ViewBag.GiaoDuc_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.GiaoDuc_ThSCNTT);
                        ViewBag.GiaoDuc_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.GiaoDuc_HocThacSy);
                        ViewBag.GiaoDuc_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.GiaoDuc_NghienCuuSinh);
                        ViewBag.YTe_ThanhThao = listHaTangNhanLucCapHuyen.Sum(m => m.YTe_ThanhThao);
                        ViewBag.YTe_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.YTe_LuotTapHuan);
                        ViewBag.YTe_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.YTe_TSCNTT);
                        ViewBag.YTe_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.YTe_ThSCNTT);
                        ViewBag.YTe_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.YTe_HocThacSy);
                        ViewBag.YTe_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.YTe_NghienCuuSinh);
                        ViewBag.DVSN_ThanhThao = listHaTangNhanLucCapHuyen.Sum(m => m.DVSN_ThanhThao);
                        ViewBag.DVSN_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.DVSN_LuotTapHuan);
                        ViewBag.DVSN_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DVSN_TSCNTT);
                        ViewBag.DVSN_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DVSN_ThSCNTT);
                        ViewBag.DVSN_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.DVSN_HocThacSy);
                        ViewBag.DVSN_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.DVSN_NghienCuuSinh);
                        ViewBag.DN_ThanhThao = listHaTangNhanLucCapHuyen.Sum(m => m.DN_ThanhThao);
                        ViewBag.DN_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.DN_LuotTapHuan);
                        ViewBag.DN_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DN_TSCNTT);
                        ViewBag.DN_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.DN_ThSCNTT);
                        ViewBag.DN_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.DN_HocThacSy);
                        ViewBag.DN_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.DN_NghienCuuSinh);
                        ViewBag.CBCCVC_Khac_ThanhThao = listHaTangNhanLucCapHuyen.Sum(m => m.CBCCVC_Khac_ThanhThao);
                        ViewBag.CBCCVC_Khac_LuotTapHuan = listHaTangNhanLucCapHuyen.Sum(m => m.CBCCVC_Khac_LuotTapHuan);
                        ViewBag.CBCCVC_Khac_TSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.CBCCVC_Khac_TSCNTT);
                        ViewBag.CBCCVC_Khac_ThSCNTT = listHaTangNhanLucCapHuyen.Sum(m => m.CBCCVC_Khac_ThSCNTT);
                        ViewBag.CBCCVC_Khac_HocThacSy = listHaTangNhanLucCapHuyen.Sum(m => m.CBCCVC_Khac_HocThacSy);
                        ViewBag.CBCCVC_Khac_NghienCuuSinh = listHaTangNhanLucCapHuyen.Sum(m => m.CBCCVC_Khac_NghienCuuSinh);
                        ViewBag.TongChiNganSach = listHaTangNhanLucCapHuyen.Sum(m => m.TongChiNganSach);
                        ViewBag.TongMucDauTu = listHaTangNhanLucCapHuyen.Sum(m => m.TongMucDauTu);
                        return PartialView("HaTangNhanLucCapHuyen", listHaTangNhanLucCapHuyen);
                        break;

                    case 20: // Cổng Thông Tin Điện Tử Cấp Huyện
                        var listCongTTDTCapHuyen = db.sp_GetCongThongTinDienTuCapHuyen(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
                        ViewBag.SoCBCC = listCongTTDTCapHuyen.Sum(m => m.SoCBCC);
                        ViewBag.SoDonViTrucThuoc = listCongTTDTCapHuyen.Sum(m => m.SoDonViTrucThuoc);
                        ViewBag.SoCB_DVTT = listCongTTDTCapHuyen.Sum(m => m.SoCB_DVTT);

                        var listsum = db.sp_GetCongThongTinDienTuCapHuyen(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
                        sp_GetCongThongTinDienTuCapHuyen_Result objSum = new sp_GetCongThongTinDienTuCapHuyen_Result
                        {
                            SoSubsiteCapXa = listCongTTDTCapHuyen.Sum(o => o.SoSubsiteCapXa),
                            TTCD_TongSoVBChiDao_DonVi = listCongTTDTCapHuyen.Sum(o => o.TTCD_TongSoVBChiDao_DonVi),
                            TTCD_TongSoVBChiDao_DieuHanh = listCongTTDTCapHuyen.Sum(o => o.TTCD_TongSoVBChiDao_DieuHanh),
                            TongSoTTHC_DV = listCongTTDTCapHuyen.Sum(o => o.TongSoTTHC_DV),
                            TongSoDVC_Muc1 = listCongTTDTCapHuyen.Sum(o => o.TongSoDVC_Muc1),
                            TongSoDVC_Muc2 = listCongTTDTCapHuyen.Sum(o => o.TongSoDVC_Muc2),
                            TongSoDVC_Muc3 = listCongTTDTCapHuyen.Sum(o => o.TongSoDVC_Muc3),
                            TongSoDVTT_CoCongTTDT = listCongTTDTCapHuyen.Sum(o => o.TongSoDVTT_CoCongTTDT),
                            SoDNSXKD_PhanCung = listCongTTDTCapHuyen.Sum(o => o.SoDNSXKD_PhanCung),
                            SoDNSXKD_PhanMem = listCongTTDTCapHuyen.Sum(o => o.SoDNSXKD_PhanMem),
                            SoDNCungCapDVNDSo = listCongTTDTCapHuyen.Sum(o => o.SoDNCungCapDVNDSo),
                            TongDoanhThu = listCongTTDTCapHuyen.Sum(o => o.TongDoanhThu),
                            TongMucDauTu = listCongTTDTCapHuyen.Sum(o => o.TongMucDauTu)
                        };
                        ViewBag.ObjectSum = objSum;
                        return PartialView("CongThongTinDienTuCapHuyen", listCongTTDTCapHuyen);
                        break;

                    case 21: // Ứng Dụng CNTT Cấp Huyện
                        var listUngDungCNTTCapHuyen = db.sp_GetUngDungCNTTCapHuyen(model.NhomDonVi, model.DotBaoCao, model.Nam, TrangThaiNhapLieu.PheDuyet);
                        ViewBag.SoCBCC = listUngDungCNTTCapHuyen.Sum(m => m.SoCBCC);
                        ViewBag.SoDonViTrucThuoc = listUngDungCNTTCapHuyen.Sum(m => m.SoDonViTrucThuoc);
                        ViewBag.SoCB_DVTT = listUngDungCNTTCapHuyen.Sum(m => m.SoCB_DVTT);
                        ViewBag.SoTTHC_MotCuaDienTu = listUngDungCNTTCapHuyen.Sum(m => m.SoTTHC_MotCuaDienTu);
                        ViewBag.SoVBDi = listUngDungCNTTCapHuyen.Sum(m => m.SoVBDi);
                        ViewBag.SoVBDen = listUngDungCNTTCapHuyen.Sum(m => m.SoVBDen);
                        ViewBag.TongSoVBDi = listUngDungCNTTCapHuyen.Sum(m => m.TongSoVBDi);
                        ViewBag.TongSoVBDen = listUngDungCNTTCapHuyen.Sum(m => m.TongSoVBDen);
                        ViewBag.TongSoCBCCCapHuyenDungMail = listUngDungCNTTCapHuyen.Sum(m => m.TongSoCBCCCapHuyenDungMail);
                        ViewBag.TongSoCBCCCapXaDungMail = listUngDungCNTTCapHuyen.Sum(m => m.TongSoCBCCCapXaDungMail);
                        ViewBag.MamNon_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.MamNon_CongTTDT);
                        ViewBag.MamNon_QLTaiChinh = listUngDungCNTTCapHuyen.Sum(m => m.MamNon_QLTaiChinh);
                        ViewBag.MamNon_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.MamNon_QLCongSan);
                        ViewBag.MamNon_QLGiangDay = listUngDungCNTTCapHuyen.Sum(m => m.MamNon_QLGiangDay);
                        ViewBag.MamNon_QLHSSV = listUngDungCNTTCapHuyen.Sum(m => m.MamNon_QLHSSV);
                        ViewBag.MamNon_QLThuVien = listUngDungCNTTCapHuyen.Sum(m => m.MamNon_QLThuVien);
                        ViewBag.MamNon_DeTaiKH = listUngDungCNTTCapHuyen.Sum(m => m.MamNon_DeTaiKH);
                        ViewBag.MamNon_QLThi = listUngDungCNTTCapHuyen.Sum(m => m.MamNon_QLThi);
                        ViewBag.TieuHoc_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.TieuHoc_CongTTDT);
                        ViewBag.TieuHoc_QLTaiChinh = listUngDungCNTTCapHuyen.Sum(m => m.TieuHoc_QLTaiChinh);
                        ViewBag.TieuHoc_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.TieuHoc_QLCongSan);
                        ViewBag.TieuHoc_QLGiangDay = listUngDungCNTTCapHuyen.Sum(m => m.TieuHoc_QLGiangDay);
                        ViewBag.TieuHoc_QLHSSV = listUngDungCNTTCapHuyen.Sum(m => m.TieuHoc_QLHSSV);
                        ViewBag.TieuHoc_QLThuVien = listUngDungCNTTCapHuyen.Sum(m => m.TieuHoc_QLThuVien);
                        ViewBag.TieuHoc_DeTaiKH = listUngDungCNTTCapHuyen.Sum(m => m.TieuHoc_DeTaiKH);
                        ViewBag.TieuHoc_QLThi = listUngDungCNTTCapHuyen.Sum(m => m.TieuHoc_QLThi);
                        ViewBag.THCS_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.THCS_CongTTDT);
                        ViewBag.THCS_QLTaiChinh = listUngDungCNTTCapHuyen.Sum(m => m.THCS_QLTaiChinh);
                        ViewBag.THCS_CongSan = listUngDungCNTTCapHuyen.Sum(m => m.THCS_CongSan);
                        ViewBag.THCS_QLGiangDay = listUngDungCNTTCapHuyen.Sum(m => m.THCS_QLGiangDay);
                        ViewBag.THCS_QLHSSV = listUngDungCNTTCapHuyen.Sum(m => m.THCS_QLHSSV);
                        ViewBag.THCS_QLThuVien = listUngDungCNTTCapHuyen.Sum(m => m.THCS_QLThuVien);
                        ViewBag.THCS_DeTaiKH = listUngDungCNTTCapHuyen.Sum(m => m.THCS_DeTaiKH);
                        ViewBag.THCS_QLThi = listUngDungCNTTCapHuyen.Sum(m => m.THCS_QLThi);
                        ViewBag.THPT_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.THPT_CongTTDT);
                        ViewBag.THPT_QLTaiChinh = listUngDungCNTTCapHuyen.Sum(m => m.THPT_QLTaiChinh);
                        ViewBag.THPT_CongSan = listUngDungCNTTCapHuyen.Sum(m => m.THPT_CongSan);
                        ViewBag.THPT_QLGiangDay = listUngDungCNTTCapHuyen.Sum(m => m.THPT_QLGiangDay);
                        ViewBag.THPT_QLHSSV = listUngDungCNTTCapHuyen.Sum(m => m.THPT_QLHSSV);
                        ViewBag.THPT_QLThuVien = listUngDungCNTTCapHuyen.Sum(m => m.THPT_QLThuVien);
                        ViewBag.THPT_DeTaiKH = listUngDungCNTTCapHuyen.Sum(m => m.THPT_DeTaiKH);
                        ViewBag.THPT_QLThi = listUngDungCNTTCapHuyen.Sum(m => m.THPT_QLThi);
                        ViewBag.TrungCap_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.TrungCap_CongTTDT);
                        ViewBag.TrungCap_QLTaiChinh = listUngDungCNTTCapHuyen.Sum(m => m.TrungCap_QLTaiChinh);
                        ViewBag.TrungCap_CongSan = listUngDungCNTTCapHuyen.Sum(m => m.TrungCap_CongSan);
                        ViewBag.TrungCap_QLGiangDay = listUngDungCNTTCapHuyen.Sum(m => m.TrungCap_QLGiangDay);
                        ViewBag.TrungCap_QLHSSV = listUngDungCNTTCapHuyen.Sum(m => m.TrungCap_QLHSSV);
                        ViewBag.TrungCap_QLThuVien = listUngDungCNTTCapHuyen.Sum(m => m.TrungCap_QLThuVien);
                        ViewBag.TrungCap_DeTaiKH = listUngDungCNTTCapHuyen.Sum(m => m.TrungCap_DeTaiKH);
                        ViewBag.TrungCap_QLThi = listUngDungCNTTCapHuyen.Sum(m => m.TrungCap_QLThi);
                        ViewBag.CaoDang_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.CaoDang_CongTTDT);
                        ViewBag.CaoDang_QLTaiChinh = listUngDungCNTTCapHuyen.Sum(m => m.CaoDang_QLTaiChinh);
                        ViewBag.CaoDang_CongSan = listUngDungCNTTCapHuyen.Sum(m => m.CaoDang_CongSan);
                        ViewBag.CaoDang_QLGiangDay = listUngDungCNTTCapHuyen.Sum(m => m.CaoDang_QLGiangDay);
                        ViewBag.CaoDang_QLHSSV = listUngDungCNTTCapHuyen.Sum(m => m.CaoDang_QLHSSV);
                        ViewBag.CaoDang_QLThuVien = listUngDungCNTTCapHuyen.Sum(m => m.CaoDang_QLThuVien);
                        ViewBag.CaoDang_DeTaiKH = listUngDungCNTTCapHuyen.Sum(m => m.CaoDang_DeTaiKH);
                        ViewBag.CaoDang_QLThi = listUngDungCNTTCapHuyen.Sum(m => m.CaoDang_QLThi);
                        ViewBag.DaiHoc_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.DaiHoc_CongTTDT);
                        ViewBag.DaiHoc_QLTaiChinh = listUngDungCNTTCapHuyen.Sum(m => m.DaiHoc_QLTaiChinh);
                        ViewBag.DaiHoc_CongSan = listUngDungCNTTCapHuyen.Sum(m => m.DaiHoc_CongSan);
                        ViewBag.DaiHoc_QLGiangDay = listUngDungCNTTCapHuyen.Sum(m => m.DaiHoc_QLGiangDay);
                        ViewBag.DaiHoc_QLHSSV = listUngDungCNTTCapHuyen.Sum(m => m.DaiHoc_QLHSSV);
                        ViewBag.DaiHoc_QLThuVien = listUngDungCNTTCapHuyen.Sum(m => m.DaiHoc_QLThuVien);
                        ViewBag.DaiHoc_DeTaiKH = listUngDungCNTTCapHuyen.Sum(m => m.DaiHoc_DeTaiKH);
                        ViewBag.DaiHoc_QLThi = listUngDungCNTTCapHuyen.Sum(m => m.DaiHoc_QLThi);
                        ViewBag.Khac_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.Khac_CongTTDT);
                        ViewBag.Khac_QLTaiChinh = listUngDungCNTTCapHuyen.Sum(m => m.Khac_QLTaiChinh);
                        ViewBag.Khac_CongSan = listUngDungCNTTCapHuyen.Sum(m => m.Khac_CongSan);
                        ViewBag.Khac_QLGiangDay = listUngDungCNTTCapHuyen.Sum(m => m.Khac_QLGiangDay);
                        ViewBag.Khac_QLHSSV = listUngDungCNTTCapHuyen.Sum(m => m.Khac_QLHSSV);
                        ViewBag.Khac_QLThuVien = listUngDungCNTTCapHuyen.Sum(m => m.Khac_QLThuVien);
                        ViewBag.Khac_DeTaiKH = listUngDungCNTTCapHuyen.Sum(m => m.Khac_DeTaiKH);
                        ViewBag.Khac_QLThi = listUngDungCNTTCapHuyen.Sum(m => m.Khac_QLThi);
                        ViewBag.TramXa_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_CongTTDT);
                        ViewBag.TramXa_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_QLTC);
                        ViewBag.TramXa_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_QLNhanSu);
                        ViewBag.TramXa_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_QLBenhAn);
                        ViewBag.TramXa_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_QLTacNghiep);
                        ViewBag.TramXa_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_QLThuoc);
                        ViewBag.TramXa_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_QLGiuongBenh);
                        ViewBag.TramXa_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_QLCongSan);
                        ViewBag.TramXa_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.TramXa_KhamTuXa);
                        ViewBag.BenhVien_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_CongTTDT);
                        ViewBag.BenhVien_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_QLTC);
                        ViewBag.BenhVien_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_QLNhanSu);
                        ViewBag.BenhVien_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_QLBenhAn);
                        ViewBag.BenhVien_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_QLTacNghiep);
                        ViewBag.BenhVien_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_QLThuoc);
                        ViewBag.BenhVien_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_QLGiuongBenh);
                        ViewBag.BenhVien_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_QLCongSan);
                        ViewBag.BenhVien_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.BenhVien_KhamTuXa);
                        ViewBag.DuPhong_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_CongTTDT);
                        ViewBag.DuPhong_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_QLTC);
                        ViewBag.DuPhong_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_QLNhanSu);
                        ViewBag.DuPhong_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_QLBenhAn);
                        ViewBag.DuPhong_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_QLTacNghiep);
                        ViewBag.DuPhong_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_QLThuoc);
                        ViewBag.DuPhong_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_QLGiuongBenh);
                        ViewBag.DuPhong_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_QLCongSan);
                        ViewBag.DuPhong_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.DuPhong_KhamTuXa);
                        ViewBag.SotRet_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_CongTTDT);
                        ViewBag.SotRet_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_QLTC);
                        ViewBag.SotRet_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_QLNhanSu);
                        ViewBag.SotRet_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_QLBenhAn);
                        ViewBag.SotRet_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_QLTacNghiep);
                        ViewBag.SotRet_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_QLThuoc);
                        ViewBag.SotRet_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_QLGiuongBenh);
                        ViewBag.SotRet_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_QLCongSan);
                        ViewBag.SotRet_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.SotRet_KhamTuXa);
                        ViewBag.HIVAIDS_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_CongTTDT);
                        ViewBag.HIVAIDS_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_QLTC);
                        ViewBag.HIVAIDS_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_QLNhanSu);
                        ViewBag.HIVAIDS_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_QLBenhAn);
                        ViewBag.HIVAIDS_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_QLTacNghiep);
                        ViewBag.HIVAIDS_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_QLThuoc);
                        ViewBag.HIVAIDS_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_QLGiuongBenh);
                        ViewBag.HIVAIDS_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_QLCongSan);
                        ViewBag.HIVAIDS_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.HIVAIDS_KhamTuXa);
                        ViewBag.SinhSan_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_CongTTDT);
                        ViewBag.SinhSan_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_QLTC);
                        ViewBag.SinhSan_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_QLNhanSu);
                        ViewBag.SinhSan_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_QLBenhAn);
                        ViewBag.SinhSan_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_QLTacNghiep);
                        ViewBag.SinhSan_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_QLThuoc);
                        ViewBag.SinhSan_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_QLGiuongBenh);
                        ViewBag.SinhSan_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_QLCongSan);
                        ViewBag.SinhSan_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.SinhSan_KhamTuXa);
                        ViewBag.DaLieu_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_CongTTDT);
                        ViewBag.DaLieu_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_QLTC);
                        ViewBag.DaLieu_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_QLNhanSu);
                        ViewBag.DaLieu_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_QLBenhAn);
                        ViewBag.DaLieu_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_QLTacNghiep);
                        ViewBag.DaLieu_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_QLThuoc);
                        ViewBag.DaLieu_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_QLGiuongBenh);
                        ViewBag.DaLieu_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_QLCongSan);
                        ViewBag.DaLieu_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.DaLieu_KhamTuXa);
                        ViewBag.Mat_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.Mat_CongTTDT);
                        ViewBag.Mat_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.Mat_QLTC);
                        ViewBag.Mat_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.Mat_QLNhanSu);
                        ViewBag.Mat_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.Mat_QLBenhAn);
                        ViewBag.Mat_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.Mat_QLTacNghiep);
                        ViewBag.Mat_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.Mat_QLThuoc);
                        ViewBag.Mat_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.Mat_QLGiuongBenh);
                        ViewBag.Mat_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.Mat_QLCongSan);
                        ViewBag.Mat_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.Mat_KhamTuXa);
                        ViewBag.DKTinh_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_CongTTDT);
                        ViewBag.DKTinh_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_QLTC);
                        ViewBag.DKTinh_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_QLNhanSu);
                        ViewBag.DKTinh_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_QLBenhAn);
                        ViewBag.DKTinh_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_QLTacNghiep);
                        ViewBag.DKTinh_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_QLThuoc);
                        ViewBag.DKTinh_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_QLGiuongBenh);
                        ViewBag.DKTinh_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_QLCongSan);
                        ViewBag.DKTinh_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.DKTinh_KhamTuXa);
                        ViewBag.DieuDuong_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_CongTTDT);
                        ViewBag.DieuDuong_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_QLTC);
                        ViewBag.DieuDuong_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_QLNhanSu);
                        ViewBag.DieuDuong_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_QLBenhAn);
                        ViewBag.DieuDuong_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_QLTacNghiep);
                        ViewBag.DieuDuong_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_QLThuoc);
                        ViewBag.DieuDuong_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_QLGiuongBenh);
                        ViewBag.DieuDuong_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_QLCongSan);
                        ViewBag.DieuDuong_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.DieuDuong_KhamTuXa);
                        ViewBag.LaoPhoi_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_CongTTDT);
                        ViewBag.LaoPhoi_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_QLTC);
                        ViewBag.LaoPhoi_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_QLNhanSu);
                        ViewBag.LaoPhoi_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_QLBenhAn);
                        ViewBag.LaoPhoi_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_QLTacNghiep);
                        ViewBag.LaoPhoi_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_QLThuoc);
                        ViewBag.LaoPhoi_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_QLGiuongBenh);
                        ViewBag.LaoPhoi_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_QLCongSan);
                        ViewBag.LaoPhoi_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.LaoPhoi_KhamTuXa);
                        ViewBag.YHoc_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_CongTTDT);
                        ViewBag.YHoc_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_QLTC);
                        ViewBag.YHoc_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_QLNhanSu);
                        ViewBag.YHoc_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_QLBenhAn);
                        ViewBag.YHoc_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_QLTacNghiep);
                        ViewBag.YHoc_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_QLThuoc);
                        ViewBag.YHoc_GiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_GiuongBenh);
                        ViewBag.YHoc_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_QLCongSan);
                        ViewBag.YHoc_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.YHoc_KhamTuXa);
                        ViewBag.TamThan_CongTTDT = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_CongTTDT);
                        ViewBag.TamThan_QLTC = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_QLTC);
                        ViewBag.TamThan_QLNhanSu = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_QLNhanSu);
                        ViewBag.TamThan_QLBenhAn = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_QLBenhAn);
                        ViewBag.TamThan_QLTacNghiep = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_QLTacNghiep);
                        ViewBag.TamThan_QLThuoc = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_QLThuoc);
                        ViewBag.TamThan_QLGiuongBenh = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_QLGiuongBenh);
                        ViewBag.TamThan_QLCongSan = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_QLCongSan);
                        ViewBag.TamThan_KhamTuXa = listUngDungCNTTCapHuyen.Sum(m => m.TamThan_KhamTuXa);
                        ViewBag.SuNghiep_KeToan = listUngDungCNTTCapHuyen.Sum(m => m.SuNghiep_KeToan);
                        ViewBag.SuNghiep_ERP = listUngDungCNTTCapHuyen.Sum(m => m.SuNghiep_ERP);
                        ViewBag.SuNghiep_QLVatTu = listUngDungCNTTCapHuyen.Sum(m => m.SuNghiep_QLVatTu);
                        ViewBag.SuNghiep_Luong = listUngDungCNTTCapHuyen.Sum(m => m.SuNghiep_Luong);
                        ViewBag.SuNghiep_BanHang = listUngDungCNTTCapHuyen.Sum(m => m.SuNghiep_BanHang);
                        ViewBag.SuNghiep_GDDTTinh = listUngDungCNTTCapHuyen.Sum(m => m.SuNghiep_GDDTTinh);
                        ViewBag.SuNghiep_Khac = listUngDungCNTTCapHuyen.Sum(m => m.SuNghiep_Khac);
                        ViewBag.CNTT_KeToan = listUngDungCNTTCapHuyen.Sum(m => m.CNTT_KeToan);
                        ViewBag.CNTT_ERP = listUngDungCNTTCapHuyen.Sum(m => m.CNTT_ERP);
                        ViewBag.CNTT_QLVatTu = listUngDungCNTTCapHuyen.Sum(m => m.CNTT_QLVatTu);
                        ViewBag.CNTT_Luong = listUngDungCNTTCapHuyen.Sum(m => m.CNTT_Luong);
                        ViewBag.CNTT_BanHang = listUngDungCNTTCapHuyen.Sum(m => m.CNTT_BanHang);
                        ViewBag.CNTT_GDDTTinh = listUngDungCNTTCapHuyen.Sum(m => m.CNTT_GDDTTinh);
                        ViewBag.CNTT_Khac = listUngDungCNTTCapHuyen.Sum(m => m.CNTT_Khac);
                        ViewBag.Khac_KeToan = listUngDungCNTTCapHuyen.Sum(m => m.Khac_KeToan);
                        ViewBag.Khac_ERP = listUngDungCNTTCapHuyen.Sum(m => m.Khac_ERP);
                        ViewBag.Khac_QLVatTu = listUngDungCNTTCapHuyen.Sum(m => m.Khac_QLVatTu);
                        ViewBag.Khac_Luong = listUngDungCNTTCapHuyen.Sum(m => m.Khac_Luong);
                        ViewBag.Khac_BanHang = listUngDungCNTTCapHuyen.Sum(m => m.Khac_BanHang);
                        ViewBag.Khac_GDDTTinh = listUngDungCNTTCapHuyen.Sum(m => m.Khac_GDDTTinh);
                        ViewBag.Khac_Khac = listUngDungCNTTCapHuyen.Sum(m => m.Khac_Khac);
                        ViewBag.TongMucDauTu = listUngDungCNTTCapHuyen.Sum(m => m.TongMucDauTu);
                        ViewBag.SoXaLienThongPhanMem = listUngDungCNTTCapHuyen.Sum(m => m.SoXaLienThongPhanMem);
                        return PartialView("UngDungCNTTCapHuyen", listUngDungCNTTCapHuyen);
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
