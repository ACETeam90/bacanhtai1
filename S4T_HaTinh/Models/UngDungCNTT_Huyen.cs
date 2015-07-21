namespace S4T_HaTinh.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UngDungCNTT_Huyen
    {
        [Key]
        public int UngDungCNTTHuyen_ID { get; set; }

        public int LichNhap_ID { get; set; }

        [Display(Name = "Đơn vị")]
        public int DonVi_ID { get; set; }

        [Display(Name = "Quản lý văn bản và điều hành trên môi trường mạng")]
        public byte HasQuanLyVanBan { get; set; }

        [Display(Name = "Quản lý nhân sự")]
        public byte HasQuanLyNhanSu { get; set; }

        [Display(Name = "Quản lý tài chính - kế toán")]
        public byte HasTaiChinhKeToan { get; set; }

        [Display(Name = "Quản lý công sản")]
        public byte HasQuanLyCongSan { get; set; }

        [Display(Name = "Quản lý thanh tra, khiếu nại, tố cáo")]
        public byte HasQuanLyThanhTraKhieuNai { get; set; }

        [Display(Name = "Ứng dụng chữ ký số")]
        public byte HasChuKySo { get; set; }

        [Display(Name = "Một cửa điện tử")]
        public byte HasMotCuaDienTu { get; set; }

        [Display(Name = "Số TTHC được đưa vào một cửa điện tử")]
        public int SoTTHC_MotCuaDienTu { get; set; }

        [Display(Name = "-	Tên phần mềm/ CSDL")]
        [StringLength(250)]
        public string TenPhanMem { get; set; }

        [Display(Name = "-	Chức năng")]
        [StringLength(250)]
        public string ChucNang { get; set; }

        [Display(Name = "-	Phạm vi sử dụng")]
        [StringLength(250)]
        public string PhamViSuDung { get; set; }

        [Display(Name = "-	Bắt đầu sử dụng từ (tháng/năm)")]
        public DateTime? BatDauSuDung { get; set; }

        [Display(Name = "-	Đơn vị lập trình phần mềm")]
        [StringLength(250)]
        public string DonViLapTrinhPhanMem { get; set; }

        [Display(Name = "OpenOffice")]
        public byte HasOpenOffice { get; set; }

        [Display(Name = "Mozilla ThunderBird")]
        public byte HasMozilla { get; set; }

        [Display(Name = "Mozilla FireFox")]
        public byte HasFirefox { get; set; }

        [Display(Name = "Unikey")]
        public byte HasUnikey { get; set; }

        [Display(Name = "Hệ điều hành PMNM")]
        public byte HasHeDieuHanhPMNM { get; set; }

        [Display(Name = "Máy chủ cài hệ điều hành PMNM")]
        public byte HasMayChuCaiHeDieuHanhPMNM { get; set; }

        [Display(Name = "Tổng số CBCC tại UBND cấp huyện dùng thư điện tử Mail.hatinh.gov.vn")]
        public int TongSoCBCCCapHuyenDungMail { get; set; }

        [Display(Name = "Tổng số CBCC tại UBND cấp xã dùng thư điện tử Mail.hatinh.gov.vn")]
        public int TongSoCBCCCapXaDungMail { get; set; }

        public int MamNon_CongTTDT { get; set; }

        public int MamNon_QLTaiChinh { get; set; }

        public int MamNon_QLCongSan { get; set; }

        public int MamNon_QLGiangDay { get; set; }

        public int MamNon_QLHSSV { get; set; }

        public int MamNon_QLThuVien { get; set; }

        public int MamNon_DeTaiKH { get; set; }

        public int MamNon_QLThi { get; set; }

        public int TieuHoc_CongTTDT { get; set; }

        public int TieuHoc_QLTaiChinh { get; set; }

        public int TieuHoc_QLCongSan { get; set; }

        public int TieuHoc_QLGiangDay { get; set; }

        public int TieuHoc_QLHSSV { get; set; }

        public int TieuHoc_QLThuVien { get; set; }

        public int TieuHoc_DeTaiKH { get; set; }

        public int TieuHoc_QLThi { get; set; }

        public int THCS_CongTTDT { get; set; }

        public int THCS_QLTaiChinh { get; set; }

        public int THCS_CongSan { get; set; }

        public int THCS_QLGiangDay { get; set; }

        public int THCS_QLHSSV { get; set; }

        public int THCS_QLThuVien { get; set; }

        public int THCS_DeTaiKH { get; set; }

        public int THCS_QLThi { get; set; }

        public int THPT_CongTTDT { get; set; }

        public int THPT_QLTaiChinh { get; set; }

        public int THPT_CongSan { get; set; }

        public int THPT_QLGiangDay { get; set; }

        public int THPT_QLHSSV { get; set; }

        public int THPT_QLThuVien { get; set; }

        public int THPT_DeTaiKH { get; set; }

        public int THPT_QLThi { get; set; }

        public int TrungCap_CongTTDT { get; set; }

        public int TrungCap_QLTaiChinh { get; set; }

        public int TrungCap_CongSan { get; set; }

        public int TrungCap_QLGiangDay { get; set; }

        public int TrungCap_QLHSSV { get; set; }

        public int TrungCap_QLThuVien { get; set; }

        public int TrungCap_DeTaiKH { get; set; }

        public int TrungCap_QLThi { get; set; }

        public int CaoDang_CongTTDT { get; set; }

        public int CaoDang_QLTaiChinh { get; set; }

        public int CaoDang_CongSan { get; set; }

        public int CaoDang_QLGiangDay { get; set; }

        public int CaoDang_QLHSSV { get; set; }

        public int CaoDang_QLThuVien { get; set; }

        public int CaoDang_DeTaiKH { get; set; }

        public int CaoDang_QLThi { get; set; }

        public int DaiHoc_CongTTDT { get; set; }

        public int DaiHoc_QLTaiChinh { get; set; }

        public int DaiHoc_CongSan { get; set; }

        public int DaiHoc_QLGiangDay { get; set; }

        public int DaiHoc_QLHSSV { get; set; }

        public int DaiHoc_QLThuVien { get; set; }

        public int DaiHoc_DeTaiKH { get; set; }

        public int DaiHoc_QLThi { get; set; }

        public int Khac_CongTTDT { get; set; }

        public int Khac_QLTaiChinh { get; set; }

        public int Khac_CongSan { get; set; }

        public int Khac_QLGiangDay { get; set; }

        public int Khac_QLHSSV { get; set; }

        public int Khac_QLThuVien { get; set; }

        public int Khac_DeTaiKH { get; set; }

        public int Khac_QLThi { get; set; }

        public int TramXa_CongTTDT { get; set; }

        public int TramXa_QLTC { get; set; }

        public int TramXa_QLNhanSu { get; set; }

        public int TramXa_QLBenhAn { get; set; }

        public int TramXa_QLTacNghiep { get; set; }

        public int TramXa_QLThuoc { get; set; }

        public int TramXa_QLGiuongBenh { get; set; }

        public int TramXa_QLCongSan { get; set; }

        public int TramXa_KhamTuXa { get; set; }

        public int BenhVien_CongTTDT { get; set; }

        public int BenhVien_QLTC { get; set; }

        public int BenhVien_QLNhanSu { get; set; }

        public int BenhVien_QLBenhAn { get; set; }

        public int BenhVien_QLTacNghiep { get; set; }

        public int BenhVien_QLThuoc { get; set; }

        public int BenhVien_QLGiuongBenh { get; set; }

        public int BenhVien_QLCongSan { get; set; }

        public int BenhVien_KhamTuXa { get; set; }

        public int DuPhong_CongTTDT { get; set; }

        public int DuPhong_QLTC { get; set; }

        public int DuPhong_QLNhanSu { get; set; }

        public int DuPhong_QLBenhAn { get; set; }

        public int DuPhong_QLTacNghiep { get; set; }

        public int DuPhong_QLThuoc { get; set; }

        public int DuPhong_QLGiuongBenh { get; set; }

        public int DuPhong_QLCongSan { get; set; }

        public int DuPhong_KhamTuXa { get; set; }

        public int SotRet_CongTTDT { get; set; }

        public int SotRet_QLTC { get; set; }

        public int SotRet_QLNhanSu { get; set; }

        public int SotRet_QLBenhAn { get; set; }

        public int SotRet_QLTacNghiep { get; set; }

        public int SotRet_QLThuoc { get; set; }

        public int SotRet_QLGiuongBenh { get; set; }

        public int SotRet_QLCongSan { get; set; }

        public int SotRet_KhamTuXa { get; set; }

        public int HIVAIDS_CongTTDT { get; set; }

        public int HIVAIDS_QLTC { get; set; }

        public int HIVAIDS_QLNhanSu { get; set; }

        public int HIVAIDS_QLBenhAn { get; set; }

        public int HIVAIDS_QLTacNghiep { get; set; }

        public int HIVAIDS_QLThuoc { get; set; }

        public int HIVAIDS_QLGiuongBenh { get; set; }

        public int HIVAIDS_QLCongSan { get; set; }

        public int HIVAIDS_KhamTuXa { get; set; }

        public int SinhSan_CongTTDT { get; set; }

        public int SinhSan_QLTC { get; set; }

        public int SinhSan_QLNhanSu { get; set; }

        public int SinhSan_QLBenhAn { get; set; }

        public int SinhSan_QLTacNghiep { get; set; }

        public int SinhSan_QLThuoc { get; set; }

        public int SinhSan_QLGiuongBenh { get; set; }

        public int SinhSan_QLCongSan { get; set; }

        public int SinhSan_KhamTuXa { get; set; }

        public int DaLieu_CongTTDT { get; set; }

        public int DaLieu_QLTC { get; set; }

        public int DaLieu_QLNhanSu { get; set; }

        public int DaLieu_QLBenhAn { get; set; }

        public int DaLieu_QLTacNghiep { get; set; }

        public int DaLieu_QLThuoc { get; set; }

        public int DaLieu_QLGiuongBenh { get; set; }

        public int DaLieu_QLCongSan { get; set; }

        public int DaLieu_KhamTuXa { get; set; }

        public int Mat_CongTTDT { get; set; }

        public int Mat_QLTC { get; set; }

        public int Mat_QLNhanSu { get; set; }

        public int Mat_QLBenhAn { get; set; }

        public int Mat_QLTacNghiep { get; set; }

        public int Mat_QLThuoc { get; set; }

        public int Mat_QLGiuongBenh { get; set; }

        public int Mat_QLCongSan { get; set; }

        public int Mat_KhamTuXa { get; set; }

        public int DKTinh_CongTTDT { get; set; }

        public int DKTinh_QLTC { get; set; }

        public int DKTinh_QLNhanSu { get; set; }

        public int DKTinh_QLBenhAn { get; set; }

        public int DKTinh_QLTacNghiep { get; set; }

        public int DKTinh_QLThuoc { get; set; }

        public int DKTinh_QLGiuongBenh { get; set; }

        public int DKTinh_QLCongSan { get; set; }

        public int DKTinh_KhamTuXa { get; set; }

        public int DieuDuong_CongTTDT { get; set; }

        public int DieuDuong_QLTC { get; set; }

        public int DieuDuong_QLNhanSu { get; set; }

        public int DieuDuong_QLBenhAn { get; set; }

        public int DieuDuong_QLTacNghiep { get; set; }

        public int DieuDuong_QLThuoc { get; set; }

        public int DieuDuong_QLGiuongBenh { get; set; }

        public int DieuDuong_QLCongSan { get; set; }

        public int DieuDuong_KhamTuXa { get; set; }

        public int LaoPhoi_CongTTDT { get; set; }

        public int LaoPhoi_QLTC { get; set; }

        public int LaoPhoi_QLNhanSu { get; set; }

        public int LaoPhoi_QLBenhAn { get; set; }

        public int LaoPhoi_QLTacNghiep { get; set; }

        public int LaoPhoi_QLThuoc { get; set; }

        public int LaoPhoi_QLGiuongBenh { get; set; }

        public int LaoPhoi_QLCongSan { get; set; }

        public int LaoPhoi_KhamTuXa { get; set; }

        public int YHoc_CongTTDT { get; set; }

        public int YHoc_QLTC { get; set; }

        public int YHoc_QLNhanSu { get; set; }

        public int YHoc_QLBenhAn { get; set; }

        public int YHoc_QLTacNghiep { get; set; }

        public int YHoc_QLThuoc { get; set; }

        public int YHoc_GiuongBenh { get; set; }

        public int YHoc_QLCongSan { get; set; }

        public int YHoc_KhamTuXa { get; set; }

        public int TamThan_CongTTDT { get; set; }

        public int TamThan_QLTC { get; set; }

        public int TamThan_QLNhanSu { get; set; }

        public int TamThan_QLBenhAn { get; set; }

        public int TamThan_QLTacNghiep { get; set; }

        public int TamThan_QLThuoc { get; set; }

        public int TamThan_QLGiuongBenh { get; set; }

        public int TamThan_QLCongSan { get; set; }

        public int TamThan_KhamTuXa { get; set; }

        [Display(Name = "Các ứng dụng tại Chi cục an toàn vệ sinh thực phẩm")]
        [StringLength(500)]
        public string UD_AnToanVeSinhTP { get; set; }

        [Display(Name = "Các ứng dụng tại Chi cục Dân số -KHH gia đình")]
        [StringLength(500)]
        public string UD_KHHGiaDinh { get; set; }

        [Display(Name = "Các ứng dụng tại TT TT-GD sức khỏe")]
        [StringLength(500)]
        public string UD_TTSucKhoe { get; set; }

        [Display(Name = "Các ứng dụng tại TT Kiểm nghiệm DP-Mỹ phẩm")]
        [StringLength(500)]
        public string UD_DPMyPham { get; set; }

        [Display(Name = "Các ứng dụng tại TT Giám định Y khoa")]
        [StringLength(500)]
        public string UD_GiamDinhYKhoa { get; set; }

        [Display(Name = "Các ứng dụng tại TT Giám định Pháp y")]
        [StringLength(500)]
        public string UD_GiamDinhPhapY { get; set; }

        public int SuNghiep_KeToan { get; set; }

        public int SuNghiep_ERP { get; set; }

        public int SuNghiep_QLVatTu { get; set; }

        public int SuNghiep_Luong { get; set; }

        public int SuNghiep_BanHang { get; set; }

        public int SuNghiep_GDDTTinh { get; set; }

        public int SuNghiep_Khac { get; set; }

        public int CNTT_KeToan { get; set; }

        public int CNTT_ERP { get; set; }

        public int CNTT_QLVatTu { get; set; }

        public int CNTT_Luong { get; set; }

        public int CNTT_BanHang { get; set; }

        public int CNTT_GDDTTinh { get; set; }

        public int CNTT_Khac { get; set; }

        public int Khac_KeToan { get; set; }

        public int Khac_ERP { get; set; }

        public int Khac_QLVatTu { get; set; }

        public int Khac_Luong { get; set; }

        public int Khac_BanHang { get; set; }

        public int Khac_GDDTTinh { get; set; }

        public int Khac_Khac { get; set; }

        [Display(Name = "Tổng mức đầu tư")]
        public decimal TongMucDauTu { get; set; }

        [Display(Name = "Trường nhập lại")]
        public string TruongNhapLai { get; set; }

        [Display(Name = "Trạng thái")]
        public byte Success { get; set; }

        public byte HasQuanLyVanBan_Xa { get; set; }
        public byte HasQuanLyNhanSu_Xa { get; set; }
        public byte HasTaiChinhKeToan_Xa { get; set; }
        public byte HasQuanLyCongSan_Xa { get; set; }
        public byte HasQuanLyThanhTraKhieuNai_Xa { get; set; }
        public byte HasChuKySo_Xa { get; set; }
        public byte HasMotCuaDienTu_Xa { get; set; }
        public byte SoTTHC_MotCuaDienTu_Xa { get; set; }
        public int SoXaLienThongPhanMem { get; set; }
        public int SoVBDi { get; set; }
        public int SoVBDen { get; set; }
        public int TongSoVBDi { get; set; }
        public int TongSoVBDen { get; set; }
    }
}
