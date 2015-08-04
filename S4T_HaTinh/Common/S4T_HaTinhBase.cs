using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Common
{
    public class ObjectLichNhapLieu
    {
        public int TrangThai { get; set; }
        public Ht_LichNhapLieu LichNhapLieu { get; set; }
    }
    
    public class S4T_HaTinhBase : Controller
    {
        /// <summary>
        /// Lấy session đăng nhập của người dùng
        /// </summary>
        public static ApplicationUser GetUserSession()
        {
            return System.Web.HttpContext.Current.Session["User"] as ApplicationUser;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập chức năng
        /// </summary>
        /// <param name="controllerName">Tên controller user truy cập</param>
        public static PermissionType CheckPermission(string controllerName)
        {
            if (System.Web.HttpContext.Current.Session["ChucNang"] == null)
                return PermissionType.Deny;
            var listChucNang = (List<sp_GetChucNangByRole_Result>)System.Web.HttpContext.Current.Session["ChucNang"];
            var objChucNang = listChucNang.FirstOrDefault(o => o.ControllerName.Trim().ToUpper() == controllerName.ToUpper());
            if (objChucNang != null)
                return objChucNang.IsEdit == 1 ? PermissionType.Write : (objChucNang.IsView == 1 ? PermissionType.Read : PermissionType.Deny);
            else
                return PermissionType.Deny;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập xem và yêu cầu nhập lại các báo cáo của đơn vị
        /// </summary>
        /// <param name="loaiVanBan">loaiVanBan: NhapLieu (báo cáo về ứng dụng CNTT), loaiVanBan: ChinhSach (tổ chức chính sách)</param>
        public static PermissionType CheckPermissionAdmin(string loaiVanBan)
        {
            if (System.Web.HttpContext.Current.Session["ChucNang"] == null)
                return PermissionType.Deny;
            var listChucNang = (List<sp_GetChucNangByRole_Result>)System.Web.HttpContext.Current.Session["ChucNang"];
            if (loaiVanBan == "UngDungCNTT")
            {
                var objChucNang = listChucNang.FirstOrDefault(o => o.ControllerName == "HT_QuanLyNhapLieu");
                if(objChucNang != null)
                    return objChucNang.IsEdit == 1 ? PermissionType.Write : (objChucNang.IsView == 1 ? PermissionType.Read : PermissionType.Deny);
                else
                    return PermissionType.Deny;
            }
            else
            {
                var objChucNang = listChucNang.FirstOrDefault(o => o.ControllerName == "ToChucChinhSachCNTTs");
                if (objChucNang != null)
                    return objChucNang.IsEdit == 1 ? PermissionType.Write : (objChucNang.IsView == 1 ? PermissionType.Read : PermissionType.Deny);
                else
                    return PermissionType.Deny;
            }
        }

        /// <summary>
        /// get content type
        /// </summary>
        /// <param name="fileName">tên file</param>
        public static string GetContentType(string fileName)
        {
            string contentType = "application/octetstream";
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                string ext = extension.ToLower();
                var registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (registryKey != null && registryKey.GetValue("Content Type") != null)
                    contentType = registryKey.GetValue("Content Type").ToString();
            }
            return contentType;
        }

        /// <summary>
        /// Lấy trạng thái nhập liệu báo cáo của đơn vị
        /// </summary>
        /// <param name="user">object người dùng</param>
        /// <param name="phanHeChucNang_ID">ID của phân hệ</param>
        public static ObjectLichNhapLieu GetTrangThaiLichNhapLieu(ApplicationUser user, int phanHeChucNang_ID)
        {
            var obj = new ObjectLichNhapLieu();
            var objLichNhap = new S4T_HaTinhEntities().Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                                && o.DonVi_ID == user.DonVi_ID
                                                                && o.PhanHe_ID == phanHeChucNang_ID
                                                                && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
            if (objLichNhap != null)
            {
                obj.LichNhapLieu = objLichNhap;
                if (objLichNhap.TuNgay.Date <= DateTime.Now.Date && objLichNhap.DenNgay.Date >= DateTime.Now.Date)
                {
                    obj.TrangThai = TrangThaiLichNhapLieu.HoatDong;
                }
                else if (objLichNhap.TuNgay.Date > DateTime.Now.Date)
                {
                    obj.TrangThai = TrangThaiLichNhapLieu.ChuaDenThoiDiem;
                }
                else
                {
                    obj.TrangThai = TrangThaiLichNhapLieu.QuaHan;
                }
            }
            else
            {
                obj.TrangThai = TrangThaiLichNhapLieu.KhongHoatDong;
            }
            return obj;
        }

        /// <summary>
        /// Lấy danh sách đơn vị cấp Xã
        /// </summary>
        public static IEnumerable<Dm_DonVi> ListDonViByNhomDonVi(int nhomDonVi_ID)
        {
            return MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong && o.NhomDonVi_ID == nhomDonVi_ID).OrderBy(o => o.TenDonVi);
        }

        /// <summary>
        /// Lấy danh sách hình thức quản lý (Thẩm định hồ sơ)
        /// </summary>
        public static IEnumerable<Dm_DanhMucChung> ListHinhThucQuanLy(int? trangThai)
        {
            string strConfig = ConfigurationManager.AppSettings["HinhThucQuanLy"];
            return ListObject(strConfig, trangThai);
        }

        /// <summary>
        /// Lấy danh sách nhóm dự án (Thẩm định hồ sơ)
        /// </summary>
        public static IEnumerable<Dm_DanhMucChung> ListNhomDuAn(int? trangThai)
        {
            string strConfig = ConfigurationManager.AppSettings["NhomDuAn"];
            return ListObject(strConfig, trangThai);
        }

        /// <summary>
        /// Lấy danh sách tính chất dự án (Thẩm định hồ sơ)
        /// </summary>
        public static IEnumerable<Dm_DanhMucChung> ListTinhChatDuAn(int? trangThai)
        {
            string strConfig = ConfigurationManager.AppSettings["TinhChatDuAn"];
            return ListObject(strConfig, trangThai);
        }

        private static IEnumerable<Dm_DanhMucChung> ListObject(string strConfig, int? trangThai)
        {
            if (string.IsNullOrEmpty(strConfig)) return null;
            int config = Convert.ToInt32(strConfig);
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.LoaiDanhMuc_ID == config);
            if (trangThai != null)
                return list.Where(o => o.TrangThai == trangThai);
            return list;
        }

        public static IEnumerable<Ht_PhanHeChucNang> GetListBaoCao(int? capDV, int? trangThai)
        {
            var list = new S4T_HaTinhEntities().Ht_PhanHeChucNang.Where(o => MaBaoCao.BaoCaoSo.Contains(o.PhanHeChucNang_ID) || MaBaoCao.BaoCaoHuyen.Contains(o.PhanHeChucNang_ID));
            if(trangThai != null)
                list = list.Where(o => o.TrangThai == trangThai);
            list = capDV == DonVi.NhomDonViCapHuyen ? list.Where(o => MaBaoCao.BaoCaoHuyen.Contains(o.PhanHeChucNang_ID)) 
                : capDV == DonVi.NhomDonViCapTinh ? list.Where(o => MaBaoCao.BaoCaoSo.Contains(o.PhanHeChucNang_ID)) 
                : list;

            return list;
        }
    }
}