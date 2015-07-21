using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using S4T_HaTinh.Common;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Controllers
{
    public class ImportController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        //Chuỗi kiểm tra dữ liệu của file có phải là file excel không
        private string excelContentType = "application/vnd.ms-excel";
        //Chuỗi kiểm tra với excel 2010
        private string excel2010ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private void GetViewBag()
        {
            ViewBag.ListPhanHeChucNang = db.Ht_PhanHeChucNang.Where(o => o.TrangThai == TrangThai.HoatDong && o.PhanHe_ID == PhanHe.QuanLyThongTin).OrderBy(o => o.TenChucNang);

            // Đợt báo cáo
            ViewBag.ListDotBaoCao = MvcApplication.ListDotBaoCao().Where(o => o.TrangThai == TrangThai.HoatDong);
        }

        // GET: Import/Create
        public ActionResult Import()
        {
            GetViewBag();
            return View();
        }

        // POST: Import/Create
        [HttpPost]
        public ActionResult Import(ImportModel obj)
        {
            try
            {
                // Kiểm tra đã submit file lên
                if (Request.Files.Count == 0)
                {
                    ViewBag.Mess = "Không tìm thấy file";
                }
                else
                {
                    var file = Request.Files[0];
                    if (file.ContentType != excelContentType && file.ContentType != excel2010ContentType)
                    {
                        ViewBag.Mess = "Vui lòng chọn file excel";
                    }
                    else
                    {
                        var pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
                        if (string.IsNullOrEmpty(pathSource)) return Content("Không tìm thấy đường dẫn file");

                        // Lưu đường dẫn file theo mã hồ sơ
                        var duongDan = pathSource + "/ImportReport/";
                        if (!Directory.Exists(duongDan)) Directory.CreateDirectory(duongDan);

                        //Post file lên thư mục tạm (Temp)
                        string path = Path.Combine(duongDan, file.FileName);
                        file.SaveAs(path);

                        //Sau khi upload xong= > đọc dữ liệu trong file này
                        //Code này là lấy đường dẫn của file excel bỏ vào connection string (có thể tham khảo trên trang ConnectionStrings.com)
                        string excelConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;", path);
                        //"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;";
                        OleDbConnection connection = new OleDbConnection();
                        connection.ConnectionString = excelConnectionString;
                        OleDbCommand command = new OleDbCommand("Select * from [Sheet1$]", connection);
                        connection.Open();
                        DbDataReader dr = command.ExecuteReader();

                        //Sau khi lấy dữ liệu vào dr => Insert vào trong SQL Server
                        //Lấy chuỗi kết nối tới SQL server, database là DBImportDataFromExcel
                        // Integrated Security=True
                        //string sqlConnectionString = String.Format(@"Data Source={0};Initial Catalog={1};{2}", "(local)\\SQLSERVER", "Test", "user id=sa;          password=123456");
                        var sqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                        // Lấy tên bảng dữ liệu
                        //var objPhanHeChucNang = db.Ht_PhanHeChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == obj.PhanHeChucNang);
                        //var lastIndex = objPhanHeChucNang.ControllerName.Substring(objPhanHeChucNang.ControllerName.Length - 1, 1);
                        //if (lastIndex == "s")
                        //    objPhanHeChucNang.ControllerName = objPhanHeChucNang.ControllerName.Substring(0, objPhanHeChucNang.ControllerName.Length - 1);

                        // Ép kiểu sang object
                        var dt = new DataTable();
                        dt.Load(dr);

                        connection.Dispose();

                        ImportToDB(dt, obj);
                        ViewBag.Mess = "Nhập dữ liệu thành công";

                        // import dữ liệu vào table trong database
                        //SqlBulkCopy bulkInsert = new SqlBulkCopy(sqlConnectionString);
                        //bulkInsert.DestinationTableName = objPhanHeChucNang.ControllerName;//Tên bảng muốn chèn dữ liệu vào
                        //bulkInsert.WriteToServer(dr);
                    }
                }
                GetViewBag();
                return View();
            }
            catch (DbEntityValidationException ex)
            {
                var exc = new ExceptionViewer();
                ViewBag.Mess = exc.GetError(ex);
                GetViewBag();
                return View();
            }
        }

        private void ImportToDB(DataTable dt, ImportModel obj)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                switch (obj.PhanHeChucNang)
                {
                    case 10: // Hạ tầng kỹ thuật
                        List<HaTangKyThuatCNTT> listHaTang = dt.DataTableToList<HaTangKyThuatCNTT>();

                        // Xóa dữ liệu cũ
                        if (obj.DeleteFirst)
                        {
                            var listDonVi_ID = listHaTang.Select(o => o.DonVi_ID);

                            // Lấy các lịch nhập đã được tạo
                            var listLich = db.Ht_LichNhapLieu.Where(o => o.PhanHe_ID == 10 && o.DotBaoCao_ID == obj.DotBaoCao && o.Nam == obj.Nam && o.ChucNang_ID == TrangThaiNhapLieu.DaGui && listDonVi_ID.Contains(o.DonVi_ID));
                            var listLichID = listLich.Select(o => o.LichNhap_ID);
                            db.Ht_LichNhapLieu.RemoveRange(listLich);

                            var listBaoCaoRemove = db.HaTangKyThuatCNTT.Where(o => listLichID.Contains(o.LichNhap_ID));
                            db.HaTangKyThuatCNTT.RemoveRange(listBaoCaoRemove);
                        }
                        
                        foreach (var item in listHaTang)
                        {
                            // Tạo lịch nhập
                            var objLich = new Ht_LichNhapLieu
                            {
                                ChucNang_ID = TrangThaiNhapLieu.DaGui,
                                DenNgay = obj.DenNgay,
                                DonVi_ID = item.DonVi_ID,
                                DotBaoCao_ID = obj.DotBaoCao,
                                Nam = obj.Nam,
                                PhanHe_ID = obj.PhanHeChucNang,
                                TrangThai = TrangThai.HoatDong,
                                TuNgay = obj.TuNgay
                            };
                            db.Ht_LichNhapLieu.Add(objLich);
                            db.SaveChanges();

                            // Import bản ghi báo cáo vào db
                            item.LichNhap_ID = objLich.LichNhap_ID;
                            item.Success = Convert.ToByte(TrangThaiNhapLieu.DaGui);
                            db.HaTangKyThuatCNTT.Add(item);
                            db.SaveChanges();
                        }
                        break;

                    case 11: // Hạ tầng nhân lực CNTT
                        List<HaTangNhanLucCNTT> listHaTangNhanLuc = dt.DataTableToList<HaTangNhanLucCNTT>();

                        // Xóa dữ liệu cũ
                        if (obj.DeleteFirst)
                        {
                            var listDonVi_ID = listHaTangNhanLuc.Select(o => o.DonVi_ID);

                            // Lấy các lịch nhập đã được tạo
                            var listLich = db.Ht_LichNhapLieu.Where(o => o.PhanHe_ID == 11 && o.DotBaoCao_ID == obj.DotBaoCao && o.Nam == obj.Nam && o.ChucNang_ID == TrangThaiNhapLieu.DaGui && listDonVi_ID.Contains(o.DonVi_ID));
                            var listLichID = listLich.Select(o => o.LichNhap_ID);
                            db.Ht_LichNhapLieu.RemoveRange(listLich);

                            var listBaoCaoRemove = db.HaTangNhanLucCNTT.Where(o => listLichID.Contains(o.LichNhap_ID));
                            db.HaTangNhanLucCNTT.RemoveRange(listBaoCaoRemove);
                        }

                        foreach (var item in listHaTangNhanLuc)
                        {
                            // Tạo lịch nhập
                            var objLich = new Ht_LichNhapLieu
                            {
                                ChucNang_ID = TrangThaiNhapLieu.DaGui,
                                DenNgay = obj.DenNgay,
                                DonVi_ID = item.DonVi_ID,
                                DotBaoCao_ID = obj.DotBaoCao,
                                Nam = obj.Nam,
                                PhanHe_ID = obj.PhanHeChucNang,
                                TrangThai = TrangThai.HoatDong,
                                TuNgay = obj.TuNgay
                            };
                            db.Ht_LichNhapLieu.Add(objLich);
                            db.SaveChanges();

                            // Import bản ghi báo cáo vào db
                            item.LichNhap_ID = objLich.LichNhap_ID;
                            item.Success = Convert.ToByte(TrangThaiNhapLieu.DaGui);
                            db.HaTangNhanLucCNTT.Add(item);
                            db.SaveChanges();
                        }
                        break;

                    case 12: // Ứng dụng CNTT
                        List<UngDungCNTT> listUngDungCNTT = dt.DataTableToList<UngDungCNTT>();

                        // Xóa dữ liệu cũ
                        if (obj.DeleteFirst)
                        {
                            var listDonVi_ID = listUngDungCNTT.Select(o => o.DonVi_ID);

                            // Lấy các lịch nhập đã được tạo
                            var listLich = db.Ht_LichNhapLieu.Where(o => o.PhanHe_ID == 12 && o.DotBaoCao_ID == obj.DotBaoCao && o.Nam == obj.Nam && o.ChucNang_ID == TrangThaiNhapLieu.DaGui && listDonVi_ID.Contains(o.DonVi_ID));
                            var listLichID = listLich.Select(o => o.LichNhap_ID);
                            db.Ht_LichNhapLieu.RemoveRange(listLich);

                            var listBaoCaoRemove = db.UngDungCNTT.Where(o => listLichID.Contains(o.LichNhap_ID));
                            db.UngDungCNTT.RemoveRange(listBaoCaoRemove);
                        }

                        foreach (var item in listUngDungCNTT)
                        {
                            // Tạo lịch nhập
                            var objLich = new Ht_LichNhapLieu
                            {
                                ChucNang_ID = TrangThaiNhapLieu.DaGui,
                                DenNgay = obj.DenNgay,
                                DonVi_ID = item.DonVi_ID,
                                DotBaoCao_ID = obj.DotBaoCao,
                                Nam = obj.Nam,
                                PhanHe_ID = obj.PhanHeChucNang,
                                TrangThai = TrangThai.HoatDong,
                                TuNgay = obj.TuNgay
                            };
                            db.Ht_LichNhapLieu.Add(objLich);
                            db.SaveChanges();

                            // Import bản ghi báo cáo vào db
                            item.LichNhap_ID = objLich.LichNhap_ID;
                            item.Success = Convert.ToByte(TrangThaiNhapLieu.DaGui);
                            db.UngDungCNTT.Add(item);
                            db.SaveChanges();
                        }
                        break;

                    case 14: // Cổng thông tin điện tử
                        List<CongThongTinDienTu> listCongThongTinDienTu = dt.DataTableToList<CongThongTinDienTu>();

                        // Xóa dữ liệu cũ
                        if (obj.DeleteFirst)
                        {
                            var listDonVi_ID = listCongThongTinDienTu.Select(o => o.DonVi_ID);

                            // Lấy các lịch nhập đã được tạo
                            var listLich = db.Ht_LichNhapLieu.Where(o => o.PhanHe_ID == 14 && o.DotBaoCao_ID == obj.DotBaoCao && o.Nam == obj.Nam && o.ChucNang_ID == TrangThaiNhapLieu.DaGui && listDonVi_ID.Contains(o.DonVi_ID));
                            var listLichID = listLich.Select(o => o.LichNhap_ID);
                            db.Ht_LichNhapLieu.RemoveRange(listLich);

                            var listBaoCaoRemove = db.CongThongTinDienTu.Where(o => listLichID.Contains(o.LichNhap_ID));
                            db.CongThongTinDienTu.RemoveRange(listBaoCaoRemove);
                        }

                        foreach (var item in listCongThongTinDienTu)
                        {
                            // Tạo lịch nhập
                            var objLich = new Ht_LichNhapLieu
                            {
                                ChucNang_ID = TrangThaiNhapLieu.DaGui,
                                DenNgay = obj.DenNgay,
                                DonVi_ID = item.DonVi_ID,
                                DotBaoCao_ID = obj.DotBaoCao,
                                Nam = obj.Nam,
                                PhanHe_ID = obj.PhanHeChucNang,
                                TrangThai = TrangThai.HoatDong,
                                TuNgay = obj.TuNgay
                            };
                            db.Ht_LichNhapLieu.Add(objLich);
                            db.SaveChanges();

                            // Import bản ghi báo cáo vào db
                            item.LichNhap_ID = objLich.LichNhap_ID;
                            item.Success = Convert.ToByte(TrangThaiNhapLieu.DaGui);
                            db.CongThongTinDienTu.Add(item);
                            db.SaveChanges();
                        }
                        break;

                    case 15: // Hạ tầng kỹ thuật CNTT cấp Huyện
                        List<HaTangKyThuatCNTT_Huyen> listHaTangKyThuatCNTT_Huyen = dt.DataTableToList<HaTangKyThuatCNTT_Huyen>();

                        // Xóa dữ liệu cũ
                        if (obj.DeleteFirst)
                        {
                            var listDonVi_ID = listHaTangKyThuatCNTT_Huyen.Select(o => o.DonVi_ID);

                            // Lấy các lịch nhập đã được tạo
                            var listLich = db.Ht_LichNhapLieu.Where(o => o.PhanHe_ID == 15 && o.DotBaoCao_ID == obj.DotBaoCao && o.Nam == obj.Nam && o.ChucNang_ID == TrangThaiNhapLieu.DaGui && listDonVi_ID.Contains(o.DonVi_ID));
                            var listLichID = listLich.Select(o => o.LichNhap_ID);
                            db.Ht_LichNhapLieu.RemoveRange(listLich);

                            var listBaoCaoRemove = db.HaTangKyThuatCNTT_Huyen.Where(o => listLichID.Contains(o.LichNhap_ID));
                            db.HaTangKyThuatCNTT_Huyen.RemoveRange(listBaoCaoRemove);
                        }

                        foreach (var item in listHaTangKyThuatCNTT_Huyen)
                        {
                            // Tạo lịch nhập
                            var objLich = new Ht_LichNhapLieu
                            {
                                ChucNang_ID = TrangThaiNhapLieu.DaGui,
                                DenNgay = obj.DenNgay,
                                DonVi_ID = item.DonVi_ID,
                                DotBaoCao_ID = obj.DotBaoCao,
                                Nam = obj.Nam,
                                PhanHe_ID = obj.PhanHeChucNang,
                                TrangThai = TrangThai.HoatDong,
                                TuNgay = obj.TuNgay
                            };
                            db.Ht_LichNhapLieu.Add(objLich);
                            db.SaveChanges();

                            // Import bản ghi báo cáo vào db
                            item.LichNhap_ID = objLich.LichNhap_ID;
                            db.HaTangKyThuatCNTT_Huyen.Add(item);
                            db.SaveChanges();
                        }
                        break;

                    case 16: // Hạ tầng nhân lực CNTT cấp Huyện
                        List<HaTangNhanLucCNTT_Huyen> listHaTangNhanLucCNTT_Huyen = dt.DataTableToList<HaTangNhanLucCNTT_Huyen>();

                        // Xóa dữ liệu cũ
                        if (obj.DeleteFirst)
                        {
                            var listDonVi_ID = listHaTangNhanLucCNTT_Huyen.Select(o => o.DonVi_ID);

                            // Lấy các lịch nhập đã được tạo
                            var listLich = db.Ht_LichNhapLieu.Where(o => o.PhanHe_ID == 16 && o.DotBaoCao_ID == obj.DotBaoCao && o.Nam == obj.Nam && o.ChucNang_ID == TrangThaiNhapLieu.DaGui && listDonVi_ID.Contains(o.DonVi_ID));
                            var listLichID = listLich.Select(o => o.LichNhap_ID);
                            db.Ht_LichNhapLieu.RemoveRange(listLich);

                            var listBaoCaoRemove = db.HaTangNhanLucCNTT_Huyen.Where(o => listLichID.Contains(o.LichNhap_ID));
                            db.HaTangNhanLucCNTT_Huyen.RemoveRange(listBaoCaoRemove);
                        }

                        foreach (var item in listHaTangNhanLucCNTT_Huyen)
                        {
                            // Tạo lịch nhập
                            var objLich = new Ht_LichNhapLieu
                            {
                                ChucNang_ID = TrangThaiNhapLieu.DaGui,
                                DenNgay = obj.DenNgay,
                                DonVi_ID = item.DonVi_ID,
                                DotBaoCao_ID = obj.DotBaoCao,
                                Nam = obj.Nam,
                                PhanHe_ID = obj.PhanHeChucNang,
                                TrangThai = TrangThai.HoatDong,
                                TuNgay = obj.TuNgay
                            };
                            db.Ht_LichNhapLieu.Add(objLich);
                            db.SaveChanges();

                            // Import bản ghi báo cáo vào db
                            item.LichNhap_ID = objLich.LichNhap_ID;
                            item.Success = Convert.ToByte(TrangThaiNhapLieu.DaGui);
                            db.HaTangNhanLucCNTT_Huyen.Add(item);
                            db.SaveChanges();
                        }
                        break;

                    case 20: // Cổng Thông Tin Điện Tử Cấp Huyện
                        List<CongThongTinDienTu_Huyen> listCongThongTinDienTu_Huyen = dt.DataTableToList<CongThongTinDienTu_Huyen>();

                        // Xóa dữ liệu cũ
                        if (obj.DeleteFirst)
                        {
                            var listDonVi_ID = listCongThongTinDienTu_Huyen.Select(o => o.DonVi_ID);

                            // Lấy các lịch nhập đã được tạo
                            var listLich = db.Ht_LichNhapLieu.Where(o => o.PhanHe_ID == 20 && o.DotBaoCao_ID == obj.DotBaoCao && o.Nam == obj.Nam && o.ChucNang_ID == TrangThaiNhapLieu.DaGui && listDonVi_ID.Contains(o.DonVi_ID));
                            var listLichID = listLich.Select(o => o.LichNhap_ID);
                            db.Ht_LichNhapLieu.RemoveRange(listLich);

                            var listBaoCaoRemove = db.CongThongTinDienTu_Huyen.Where(o => listLichID.Contains(o.LichNhap_ID));
                            db.CongThongTinDienTu_Huyen.RemoveRange(listBaoCaoRemove);
                        }

                        foreach (var item in listCongThongTinDienTu_Huyen)
                        {
                            // Tạo lịch nhập
                            var objLich = new Ht_LichNhapLieu
                            {
                                ChucNang_ID = TrangThaiNhapLieu.DaGui,
                                DenNgay = obj.DenNgay,
                                DonVi_ID = item.DonVi_ID,
                                DotBaoCao_ID = obj.DotBaoCao,
                                Nam = obj.Nam,
                                PhanHe_ID = obj.PhanHeChucNang,
                                TrangThai = TrangThai.HoatDong,
                                TuNgay = obj.TuNgay
                            };
                            db.Ht_LichNhapLieu.Add(objLich);
                            db.SaveChanges();

                            // Import bản ghi báo cáo vào db
                            item.LichNhap_ID = objLich.LichNhap_ID;
                            item.Success = Convert.ToByte(TrangThaiNhapLieu.DaGui);
                            db.CongThongTinDienTu_Huyen.Add(item);
                            db.SaveChanges();
                        }
                        break;

                    case 21: // Ứng Dụng CNTT Cấp Huyện
                        List<UngDungCNTT_Huyen> listUngDungCNTT_Huyen = dt.DataTableToList<UngDungCNTT_Huyen>();

                        // Xóa dữ liệu cũ
                        if (obj.DeleteFirst)
                        {
                            var listDonVi_ID = listUngDungCNTT_Huyen.Select(o => o.DonVi_ID);

                            // Lấy các lịch nhập đã được tạo
                            var listLich = db.Ht_LichNhapLieu.Where(o => o.PhanHe_ID == 21 && o.DotBaoCao_ID == obj.DotBaoCao && o.Nam == obj.Nam && o.ChucNang_ID == TrangThaiNhapLieu.DaGui && listDonVi_ID.Contains(o.DonVi_ID));
                            var listLichID = listLich.Select(o => o.LichNhap_ID);
                            db.Ht_LichNhapLieu.RemoveRange(listLich);

                            var listBaoCaoRemove = db.UngDungCNTT_Huyen.Where(o => listLichID.Contains(o.LichNhap_ID));
                            db.UngDungCNTT_Huyen.RemoveRange(listBaoCaoRemove);
                        }

                        foreach (var item in listUngDungCNTT_Huyen)
                        {
                            // Tạo lịch nhập
                            var objLich = new Ht_LichNhapLieu
                            {
                                ChucNang_ID = TrangThaiNhapLieu.DaGui,
                                DenNgay = obj.DenNgay,
                                DonVi_ID = item.DonVi_ID,
                                DotBaoCao_ID = obj.DotBaoCao,
                                Nam = obj.Nam,
                                PhanHe_ID = obj.PhanHeChucNang,
                                TrangThai = TrangThai.HoatDong,
                                TuNgay = obj.TuNgay
                            };
                            db.Ht_LichNhapLieu.Add(objLich);
                            db.SaveChanges();

                            // Import bản ghi báo cáo vào db
                            item.LichNhap_ID = objLich.LichNhap_ID;
                            item.Success = Convert.ToByte(TrangThaiNhapLieu.DaGui);
                            db.UngDungCNTT_Huyen.Add(item);
                            db.SaveChanges();
                        }
                        break;
                }
                scope.Complete();
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

    public class ImportModel
    {
        [Display(Name = "Báo cáo")]
        public int PhanHeChucNang { get; set; }

        [Display(Name = "Năm")]
        public int Nam { get; set; }

        [Display(Name = "Đợt báo cáo")]
        public int DotBaoCao { get; set; }

        [Display(Name = "Từ ngày")]
        public DateTime TuNgay { get; set; }

        [Display(Name = "Đến ngày")]
        public DateTime DenNgay { get; set; }

        public bool DeleteFirst { get; set; }
    }
}
