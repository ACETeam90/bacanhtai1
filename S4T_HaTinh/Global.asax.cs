using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using S4T_HaTinh.Models;
using System.Globalization;
using System.Threading;
using System.Data.Entity;
using S4T_HaTinh.Common;

namespace S4T_HaTinh
{
    public class MvcApplication : System.Web.HttpApplication
    {
        #region Các biến danh mục
        public static List<Ht_PhanHe> ListPhanHe = new S4T_HaTinhEntities().Ht_PhanHe.OrderBy(o => o.TrangThai).ThenBy(o => o.ThuTu).ToList();
        public static List<Dm_LoaiDanhMuc> ListLoaiDanhMuc = new S4T_HaTinhEntities().Dm_LoaiDanhMuc.ToList(); // List loại danh mục
        public static List<Dm_DonVi> ListDonVi = new S4T_HaTinhEntities().Dm_DonVi.ToList();
        public static SelectList SelectListDonVi = new SelectList(new S4T_HaTinhEntities().Dm_DonVi.Where(o => o.TrangThai == TrangThai.HoatDong), "DonVi_ID", "TenDonVi");
        public static List<AspNetRoles> ListRole = new S4T_HaTinhEntities().AspNetRoles.Where(o => o.Name != "Admin").ToList();

        /// <summary>
        /// Đợt báo cáo
        /// </summary>
        public static List<Dm_DanhMucChung> ListDotBaoCao()
        {
            var config = Convert.ToInt32(ConfigurationManager.AppSettings["DotBaoCao"]);
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.LoaiDanhMuc_ID == config);

            return list.ToList();
        }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public static List<Dm_DanhMucChung> ListTrangThai()
        {
            var config = Convert.ToInt32(ConfigurationManager.AppSettings["TrangThai"]);
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.LoaiDanhMuc_ID == config);
            
            return list.ToList();
        }

        /// <summary>
        /// Nhóm đơn vị
        /// </summary>
        public static List<Dm_DanhMucChung> ListNhomDonVi()
        {
            var config = Convert.ToInt32(ConfigurationManager.AppSettings["NhomDonVi"]);
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == config).OrderBy(o => o.TenDanhMuc);
            
            return list.ToList();
        }

        /// <summary>
        /// Nhóm đối tượng
        /// </summary>
        public static List<Dm_DanhMucChung> ListNhomDoiTuong()
        {
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == LoaiDanhMuc.NhomDoiTuong);
            
            return list.ToList();
        }

        /// <summary>
        /// Trạng thái nhập liệu của báo cáo
        /// </summary>
        public static List<Dm_DanhMucChung> ListTrangThaiNhapLieu()
        {
            var config = Convert.ToInt32(ConfigurationManager.AppSettings["TrangThaiNhapLieu"]);
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == config);
            return list.ToList();
        }

        /// <summary>
        /// Danh sách ngành nghề
        /// </summary>
        public static List<Dm_DanhMucChung> ListNganhNghe()
        {
            var config = Convert.ToInt32(ConfigurationManager.AppSettings["NganhNghe"]);
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == config);
            return list.ToList();
        }

        /// <summary>
        /// Danh sách loại hình doanh nghiệp
        /// </summary>
        public static List<Dm_DanhMucChung> ListLoaiHinhDoanhNghiep()
        {
            var config = Convert.ToInt32(ConfigurationManager.AppSettings["LoaiHinhDoanhNghiep"]);
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == config);
            return list.ToList();
        }

        /// <summary>
        /// Danh sách các báo cáo môi trường chính sách
        /// </summary>
        public static List<Dm_DanhMucChung> ListReportMTCS()
        {
            // LoaiDanhMuc_ID = 8 : report moi truong chinh sách
            var list = new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == 8);
            return list.ToList();
        }

        /// <summary>
        /// Danh sách cấp xử lý hồ sơ
        /// </summary>
        public static List<Dm_CapXuLy> ListCapXuLyHoSo()
        {
            var list = new S4T_HaTinhEntities().Dm_CapXuLy.Where(o => o.TrangThai == TrangThai.HoatDong).OrderBy(o => o.ThuTu);
            return list.ToList();
        }

        /// <summary>
        /// Danh sách loại dự án
        /// </summary>
        public static List<Dm_DanhMucChung> ListTrinhDoCNTT()
        {
            var config = ConfigurationManager.AppSettings["TrinhDoCNTT"];
            if(String.IsNullOrEmpty(config))
                return null;
            var num = 0;
            if(Int32.TryParse(config, out num))
                return new S4T_HaTinhEntities().Dm_DanhMucChung.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == num).ToList();
            return null;
        }

        /// <summary>
        /// Danh sách loại dự án
        /// </summary>
        public static List<Dm_LoaiDuAn> ListLoaiDuAn()
        {
            var list = new S4T_HaTinhEntities().Dm_LoaiDuAn.Where(o => o.TrangThai == TrangThai.HoatDong);
            return list.ToList();
        }

        /// <summary>
        /// SelectList Có / value = 1 và Không / value = 0
        /// </summary>
        public static SelectList SelectListCoKhong()
        {
            var _list = new List<SelectListItem>() { 
                new SelectListItem() { Value = "1", Text = "Có", Selected = true },
                new SelectListItem() { Value = "0", Text = "Không" }
            };
            return new SelectList(_list, "Value", "Text");
        }

        #endregion Các biến danh mục

        protected void Application_Start()
        {
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            //CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            CultureInfo newCulture = new CultureInfo("en-GB");
            newCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            newCulture.DateTimeFormat.DateSeparator = "/";
            //newCulture.NumberFormat.NumberDecimalDigits = 2;
            newCulture.NumberFormat.PercentDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<ApplicationDbContext>(null);

            ClientDataTypeModelValidatorProvider.ResourceClassKey = "ModelBinders";
            DefaultModelBinder.ResourceClassKey = "ModelBinders";
        }
    }
}
