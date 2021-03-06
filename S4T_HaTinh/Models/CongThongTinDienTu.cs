﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace S4T_HaTinh.Models
{
    using S4T_HaTinh.App_GlobalResources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class CongThongTinDienTu
    {
        [Key]
        public int CongThongTinDienTu_ID { get; set; }

        [Display(Name = "Đơn vị")]
        public int DonVi_ID { get; set; }

        public int LichNhap_ID { get; set; }

        [Display(Name = "Công nghệ lõi")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string CongNgheLoi { get; set; }

        [Display(Name = "Ngôn ngữ lập trình")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string NgonNguLapTrinh { get; set; }

        [Display(Name = "Hệ quản trị CSDL")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string CSDL { get; set; }

        [Display(Name = "Đơn vị thực hiện")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string DonViThucHien { get; set; }

        [Display(Name = "Nâng cấp trong năm")]
        public int NangCapTrongNam { get; set; }

        [Display(Name = "Nội dung nâng cấp")]
        [StringLength(500, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string NoiDung { get; set; }

        [Display(Name = "Đơn vị thực hiện")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string DonViThucHien2 { get; set; }

        [Display(Name = "Thông tin được cung cấp và cập nhật đầy đủ trên Website/Portal")]
        [StringLength(500, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string Website_Portal { get; set; }

        [Display(Name = "Thông tin về sơ đồ, cơ cấu tổ chức bộ máy cơ quan")]
        public byte CoCauToChuc { get; set; }

        [Display(Name = "Chức năng, nhiệm vụ, quyền hạn của cơ quan và các đơn vị trực thuộc")]
        public byte ChucNang_DVTT { get; set; }

        [Display(Name = "Giới thiệu tóm lược sự hình thành và phát triển của cơ quan")]
        public byte LichSuHinhThanh { get; set; }

        [Display(Name = "Thông tin tóm tắt (họ tên, số điện thoại, địa chỉ thư điện tử chính thức) và nhiệm vụ đảm nhiệm của lãnh đạo cơ quan")]
        public byte ThongTinTomTat { get; set; }

        [Display(Name = "Thông tin giao dịch chính thức của cơ quan (địa chỉ, điện thoại, fax, thư điện tử chính thức)")]
        public byte ThongTinGiaoDich { get; set; }

        [Display(Name = "Thông tin liên hệ của CBCC có thẩm quyền (họ tên, chức vụ, điện thoại, thư điện tử chính thức)")]
        public byte ThongTinLienHe { get; set; }

        [Display(Name = "Đăng tải bản đồ địa giới hành chính huyện (đối với UBND cấp huyện); đăng tải hoặc có liên kết đến bản đồ địa giới hành chính tỉnh (đối với sở, ban, ngành):")]
        public byte BanDoDiaGioi { get; set; }

        [Display(Name = "Thông tin thống kê về ngành, lĩnh vực (đối với sở, ban, ngành) hoặc thông tin thống kê của địa phương (đối với UBND cấp huyện)")]
        public byte ThongTinThongKe { get; set; }

        [Display(Name = "Tuần suất cập nhật")]
        public byte TanSuatCapNhat { get; set; }

        [Display(Name = "Tổng số văn bản chỉ đạo điều hành của đơn vị")]
        public int VanBanChiDao { get; set; }

        [Display(Name = "Tổng số văn bản chỉ đạo điều hành được đăng tải")]
        public int VanBanChiDao_DangTai { get; set; }

        [Display(Name = "Đăng lịch làm việc của lãnh đạo cơ quan")]
        public byte LichLamViec { get; set; }

        [Display(Name = "Có chuyên trang hoặc chuyên mục về phổ biến văn bản QPPL")]
        public byte PhoBienVanBan { get; set; }

        [Display(Name = "Số tin, bài viết phổ biến chính sách pháp luật đăng tải")]
        public byte Tin_BaiViet { get; set; }

        [Display(Name = "Có chuyên mục hoặc chuyên trang về chiến lược, quy hoạch, kế hoạch,… ")]
        public byte ChuyenMuc_ChuyenTrang { get; set; }

        [Display(Name = "Số lượng Chiến lược, Quy hoạch của ngành hoặc địa phương đã đăng tải")]
        public byte ChienLuoc_QuyHoach { get; set; }

        [Display(Name = "Số lượng Kế hoạch phát triển của ngành hoặc địa phương đã đăng tải")]
        public byte KeHoachPhatTrien { get; set; }

        [Display(Name = "Chuyên trang về văn bản quy phạm pháp luật ")]
        public byte VanBanQuyPham { get; set; }

        [Display(Name = "Số văn bản quy phạm pháp luật đã đăng tải")]
        public byte VanBanQuyPham_DangTai { get; set; }

        [Display(Name = "Liên kết đến các chuyên trang quản lý văn bản quy phạm pháp luật khác")]
        public byte ChuyenTrang_QuanLy { get; set; }

        [Display(Name = "Cho phép tải về các văn bản quy phạm pháp luật")]
        public byte VanBanQuyPham_TaiVe { get; set; }

        [Display(Name = "Có chuyên trang hoặc hạng mục đầu tư, mua sắm")]
        public byte HangMucDauTu { get; set; }

        [Display(Name = "Số dự án, hạng mục đầu tư, đấu thầu mua sắm công được đăng tải")]
        public byte SoDuAn { get; set; }

        [Display(Name = "Đăng những thông tin tối thiểu của mỗi dự án, hạng mục đầu tư, đấu thầu mua sắm công (tên dự án, mục tiêu chính, lĩnh vực, loại dự án, thời gian thực hiện, nguồn vốn, tình trạng dự án)")]
        public byte ThongTinToiThieu { get; set; }

        [Display(Name = "Có chuyên trang hoặc chuyên mục về đề tài khoa học")]
        public byte DeTaiKhoaHoc { get; set; }

        [Display(Name = "Số lượng đề tài khoa học được đăng tải với đầy đủ danh mục (bao gồm: mã số, tên, cấp quản lý, lĩnh vực, đơn vị chủ trì, thời gian thực hiện, tổng hợp, báo cáo kết quả)")]
        public byte SoLuongDeTaiKH_DangTai { get; set; }

        [Display(Name = "Có chuyên trang hoặc chuyên mục góp ý")]
        public byte GopY { get; set; }

        [Display(Name = "Số dự thảo văn bản quy phạm pháp luật cần xin ý kiến đăng tải")]
        public byte YKienDangTai { get; set; }

        [Display(Name = "Cung cấp các thông tin và chức năng; toàn văn nội dung vấn đề cần xin ý kiến; thời hạn tiếp nhận ý kiến góp ý; xem nội dung các ý kiến góp ý; nhận ý kiến góp ý mới; địa chỉ, thư điện tử của cơ quan, đơn vị tiếp nhận ý kiến góp ý của văn bản xin ý kiến")]
        public byte CungCapThongTin { get; set; }

        [Display(Name = "Chức năng tìm kiếm và tìm kiếm được đầy đủ nội dung thông tin, bài cần tìm")]
        public byte ChucNangTimKiem { get; set; }

        [Display(Name = "Sơ đồ website thể hiện cây cấu trúc các hạng mục thông tin của trang TTĐT; đảm bảo liên kết đúng tới các mục thông tin hoặc chức năng tương ứng")]
        public byte SoDoWebsite { get; set; }

        [Display(Name = "Đăng câu hỏi, trả lời trong mục trao đổi- hỏi đáp đối với những vấn đề có liên quan chung")]
        public byte TraoDoi_HoiDap { get; set; }

        [Display(Name = "Cung cấp dữ liệu đặc tả theo quy định cho mỗi tin bài")]
        public byte DuLieuDacTa { get; set; }

        [Display(Name = "Sử dụng bộ mã ký tự chữ Việt Unicode theo tiêu chuẩn TCVN 6909:2001")]
        public byte Unicode { get; set; }

        [Display(Name = "Khả năng tương thích với nhiều trình duyệt")]
        public byte TuongThichTrinhDuyet { get; set; }

        [Display(Name = "Liên kết tới website các đơn vị trực thuộc hoặc các cơ quan liên quan")]
        public byte LienKetWebsite { get; set; }

        [Display(Name = "Chức năng hỗ trợ người khuyết tật tiếp cận thông tin")]
        public byte NguoiKhuyetTat { get; set; }

        [Display(Name = "Tên miền Cổng Thông tin điện tử")]
        public byte TenMien_CTTDT { get; set; }

        [Display(Name = "Tổng số TTHC phải giải quyết tại đơn vị")]
        public int TTHC_DonVi { get; set; }

        [Display(Name = "Tổng số DVC mức 1 được cung cấp trên Cổng TTĐT")]
        public int DVC1 { get; set; }

        [Display(Name = "Tổng số DVC mức 2 được cung cấp trên Cổng TTĐT")]
        public int DVC2 { get; set; }

        [Display(Name = "Tổng số DVC mức 3 trở lên được cung cấp trên Cổng TTĐT")]
        public int DVC3 { get; set; }

        [Display(Name = "Ban hành quyết định thành lập Ban biên tập đúng quy định")]
        public byte BanBienTap { get; set; }

        [Display(Name = "Bố trí chuyên viên quản trị kỹ thuật")]
        public byte QuanTriKyThuat { get; set; }

        [Display(Name = "Bố trí nhân lực xử lý dịch vụ công trực tuyến")]
        public byte DichVuCongTT { get; set; }

        [Display(Name = "Tập huấn, đào tạo cán bộ Ban biên tập và chuyên viên quản trị trong năm")]
        public byte TapHuanDaoTao { get; set; }

        [Display(Name = "Thực hiện các biện pháp kỹ thuật để bảo đảm an toàn thông tin và dữ liệu trên Cổng Thông tin điện tử")]
        public byte BienPhapKyThuat { get; set; }

        [Display(Name = "Xây dựng giải pháp hiệu quả chống lại các tấn công gây mất an toàn thông tin của Cổng Thông tin điện tử")]
        public byte GiaiPhapHieuQua { get; set; }

        [Display(Name = "Xây dựng phương án dự phòng khắc phục sự cố bảo đảm hệ thống Cổng Thông tin điện tử hoạt động liên tục ở mức tối đa")]
        public byte PhuongAnDuPhong { get; set; }

        [Display(Name = "Ban hành quy chế phối hợp giữa các đơn vị trong cơ quan để cung cấp và xử lý thông tin")]
        public byte BanHanhQuyChe { get; set; }

        [Display(Name = "Ban hành quy chế hoạt động của Cổng TTĐT")]
        public byte BanHanhQuyChe_TTDT { get; set; }

        [Display(Name = "Thực hiện chế độ báo cáo định kỳ hàng năm về tình hình hoạt động của Cổng TTĐT")]
        public byte BaoCaoHangNam_TTDT { get; set; }

        [Display(Name = "Cập nhật thông tin theo quy định tại Điều 17 Nghị định số 43/2011/NĐ-CP")]
        public byte CapNhatThongTin { get; set; }

        [Display(Name = "Nội dung các thông tin đăng tải trên Cổng TTĐT")]
        public byte NoiDungThongTin { get; set; }

        [Display(Name = "Thực hiện quy định khác của Nghị định 72/2013/NĐ-CP về bản quyền thông tin đăng tải")]
        public byte QuyDinhKhac { get; set; }

        [Display(Name = "Tổng số các đơn vị trực thuộc có cổng/trang thông tin điện tử")]
        public int SoDVCoCTTDT { get; set; }

        [Display(Name = "Tổng mức đầu tư")]
        public decimal TongMucDauTu { get; set; }

        [Display(Name = "Trạng thái")]
        public byte Success { get; set; }

        [Display(Name = "Trường nhập lại")]
        //[StringLength(500, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string TruongNhapLai { get; set; }
    }
}
