namespace S4T_HaTinh.Models
{
    using System.ComponentModel.DataAnnotations;
    //using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class CongThongTinDienTu_Huyen
    {
        [Key]
        public int CongThongTinDienTuHuyen_ID { get; set; }

        public int LichNhap_ID { get; set; }

        [Display(Name = "Đơn vị")]
        public int DonVi_ID { get; set; }

        [Display(Name = "Trang thông tin điện tử (Website)")]
        [StringLength(250)]
        public string WebTTDT { get; set; }

        [Display(Name = "Cổng thông tin điện tử (Portal)")]
        [StringLength(250)]
        public string CongTTDT { get; set; }

        [Display(Name = "Hệ quản trị cơ sở dữ liệu")]
        [StringLength(250)]
        public string HeQuanTriCSDL { get; set; }

        [Display(Name = "Công nghệ lõi")]
        [StringLength(250)]
        public string CongNgheLoi { get; set; }

        [Display(Name = "Ngôn ngữ lập trình")]
        [StringLength(250)]
        public string NgonNguLapTrinh { get; set; }

        [Display(Name = "Số Subsite cấp xã")]
        public int SoSubsiteCapXa { get; set; }

        [Display(Name = "Thông tin về sơ đồ, cơ cấu tổ chức bộ máy cơ quan")]
        public byte TTGT_SoDo_CoCau { get; set; }

        [Display(Name = "Chức năng, nhiệm vụ, quyền hạn của cơ quan và các đơn vị trực thuộc")]
        public byte TTGT_ChucNang_NhiemVu { get; set; }

        [Display(Name = "Giới thiệu tóm lược sự hình thành và phát triển của cơ quan")]
        public byte TTGT_HinhThanh_PhatTrien { get; set; }

        [Display(Name = "Thông tin tóm tắt (họ tên, số điện thoại, địa chỉ thư điện tử chính thức) và nhiệm vụ đảm nhiệm của lãnh đạo cơ quan")]
        public byte TTGT_ThongTinTomTat { get; set; }

        [Display(Name = "Thông tin giao dịch chính thức của cơ quan (địa chỉ, điện thoại, fax, thư điện tử chính thức):")]
        public byte TTGT_ThongTinGiaoDich { get; set; }

        [Display(Name = "Thông tin liên hệ của CBCC có thẩm quyền (họ tên, chức vụ, điện thoại, thư điện tử chính thức):")]
        public byte TTGT_ThongTinLienHe { get; set; }

        [Display(Name = "Đăng tải bản đồ địa giới hành chính huyện (đối với UBND cấp huyện); đăng tải hoặc có liên kết đến bản đồ địa giới hành chính tỉnh (đối với sở, ban, ngành)")]
        public byte TTGT_BanDoDiaGioi { get; set; }

        [Display(Name = "Thông tin thống kê về ngành, lĩnh vực (đối với sở, ban, ngành) hoặc thông tin thống kê của địa phương (đối với UBND cấp huyện)")]
        public byte TTGT_ThongTinThongKe { get; set; }

        [Display(Name = "Tuần suất cập nhật ")]
        public byte TanSuatCapNhat { get; set; }

        [Display(Name = "Tổng số văn bản chỉ đạo điều hành của đơn vị")]
        public int TTCD_TongSoVBChiDao_DonVi { get; set; }

        [Display(Name = "Tổng số văn bản chỉ đạo điều hành được đăng tải")]
        public int TTCD_TongSoVBChiDao_DieuHanh { get; set; }

        [Display(Name = "Đăng lịch làm việc của lãnh đạo cơ quan")]
        public byte TTCD_DangLichLamViec { get; set; }

        [Display(Name = "Có chuyên trang hoặc chuyên mục về phổ biến văn bản QPPL")]
        public byte TTTT_ChuyenTrangVBQPPL { get; set; }

        [Display(Name = "Số tin, bài viết phổ biến chính sách pháp luật đăng tải")]
        public byte TTTT_TinBai_DangTaiPL { get; set; }

        [Display(Name = "Có chuyên mục hoặc chuyên trang về chiến lược, quy hoạch, kế hoạch,… ")]
        public byte CLDH_ChuyenMuc_ChienLuoc { get; set; }

        [Display(Name = "Số lượng Chiến lược, Quy hoạch của ngành hoặc địa phương đã đăng tải")]
        public byte CLDH_SoChienLuoc { get; set; }

        [Display(Name = "Số lượng Kế hoạch phát triển của ngành hoặc địa phương đã đăng tải")]
        public byte CLDH_KeHoachPhatTrien { get; set; }

        [Display(Name = "Chuyên trang về văn bản quy phạm pháp luật ")]
        public byte VBQP_ChuyenTrang_QPPL { get; set; }

        [Display(Name = "Số văn bản quy phạm pháp luật đã đăng tải")]
        public byte VBQP_SoVBQPPL_DangTai { get; set; }

        [Display(Name = "Liên kết đến các chuyên trang quản lý văn bản quy phạm pháp luật khác")]
        public byte VBQP_LienKetQLVBQPPL { get; set; }

        [Display(Name = "Cho phép tải về các văn bản quy phạm pháp luật")]
        public byte VBQP_ChoPhepTaiVBQPPL { get; set; }

        [Display(Name = "Có chuyên trang hoặc hạng mục đầu tư, mua sắm")]
        public byte TTDA_DauTu_ChuyenTrang { get; set; }

        [Display(Name = "Số dự án, hạng mục đầu tư, đấu thầu mua sắm công được đăng tải")]
        public byte TTDA_SoDauTu_DuAnDuocDang { get; set; }

        [Display(Name = "Đăng những thông tin tối thiểu của mỗi dự án, hạng mục đầu tư, đấu thầu mua sắm công (tên dự án, mục tiêu chính, lĩnh vực, loại dự án, thời gian thực hiện, nguồn vốn, tình trạng dự án):")]
        public byte TTDA_DangTaiTTToiThieu { get; set; }

        [Display(Name = "Có chuyên trang hoặc chuyên mục về đề tài khoa học ")]
        public byte DTKH_ChuyenTrang { get; set; }

        [Display(Name = "Số lượng đề tài khoa học được đăng tải với đầy đủ danh mục (bao gồm: mã số, tên, cấp quản lý, lĩnh vực, đơn vị chủ trì, thời gian thực hiện, tổng hợp, báo cáo kết quả)")]
        public byte DTKH_DuocDangTai { get; set; }

        [Display(Name = "Có chuyên trang hoặc chuyên mục góp ý ")]
        public byte GopY_ChuyenTrang { get; set; }

        [Display(Name = "Số dự thảo văn bản quy phạm pháp luật cần xin ý kiến đã đăng tải")]
        public byte GopY_SoDuThaoXinYKien { get; set; }

        [Display(Name = "Cung cấp các thông tin và chức năng; toàn văn nội dung vấn đề cần xin ý kiến; thời hạn tiếp nhận ý kiến góp ý; xem nội dung các ý kiến góp ý; nhận ý kiến góp ý mới; địa chỉ, thư điện tử của cơ quan, đơn vị tiếp nhận ý kiến góp ý của văn bản xin ý kiến")]
        public byte GopY_CungCapTT { get; set; }

        [Display(Name = "Chức năng tìm kiếm và tìm kiếm được đầy đủ nội dung thông tin, bài cần tìm")]
        public byte KTTT_ChucNangTimKiem { get; set; }

        [Display(Name = "Sơ đồ website thể hiện cây cấu trúc các hạng mục thông tin của trang TTĐT; đảm bảo liên kết đúng tới các mục thông tin hoặc chức năng tương ứng")]
        public byte KTTT_SoDoWeb { get; set; }

        [Display(Name = "Đăng câu hỏi, trả lời trong mục trao đổi- hỏi đáp đối với những vấn đề có liên quan chung")]
        public byte KTTT_DangCauHoi_TraLoi { get; set; }

        [Display(Name = "Cung cấp dữ liệu đặc tả theo quy định cho mỗi tin bài")]
        public byte KTTT_DuLieuDacTa { get; set; }

        [Display(Name = "Sử dụng bộ mã ký tự chữ Việt Unicode theo tiêu chuẩn TCVN 6909:2001")]
        public byte KTTT_MaUnicode { get; set; }

        [Display(Name = "Khả năng tương thích với nhiều trình duyệt")]
        public byte KTTT_KhaNangTuongThich { get; set; }

        [Display(Name = "Liên kết tới website các đơn vị trực thuộc hoặc các cơ quan liên quan")]
        public byte KTTT_LienKetWeb { get; set; }

        [Display(Name = "Chức năng hỗ trợ người khuyết tật tiếp cận thông tin")]
        public byte KTTT_HoTro_NguoiKhuyetTat { get; set; }

        [Display(Name = "Tên miền Cổng Thông tin điện tử")]
        public byte KTTT_TenMien_CongTTDT { get; set; }

        [Display(Name = "Tổng số TTHC phải giải quyết tại đơn vị")]
        public int TongSoTTHC_DV { get; set; }

        [Display(Name = "Tổng số DVC mức 1 được cung cấp trên Cổng TTĐT")]
        public int TongSoDVC_Muc1 { get; set; }

        [Display(Name = "Tổng số DVC mức 2 được cung cấp trên Cổng TTĐT")]
        public int TongSoDVC_Muc2 { get; set; }

        [Display(Name = "Tổng số DVC mức 3 được cung cấp trên Cổng TTĐT")]
        public int TongSoDVC_Muc3 { get; set; }

        [Display(Name = "Ban hành quyết định thành lập Ban biên tập đúng quy định")]
        public byte DBNL_ThanhLap_BanBienTap { get; set; }

        [Display(Name = "Bố trí chuyên viên quản trị kỹ thuật")]
        public byte DBNL_BoTri_QuanTriKyThuat { get; set; }

        [Display(Name = "Bố trí nhân lực xử lý dịch vụ công trực tuyến")]
        public byte DBNL_BoTri_XuLyDVCong { get; set; }

        [Display(Name = "Tập huấn, đào tạo cán bộ Ban biên tập và chuyên viên quản trị trong năm")]
        public byte DBNL_TapHuan_DaoTaoCanBo { get; set; }

        [Display(Name = "Thực hiện các biện pháp kỹ thuật để bảo đảm an toàn thông tin và dữ liệu trên Cổng Thông tin điện tử")]
        public byte ATTT_DamBaoAnToanTTDuLieu { get; set; }

        [Display(Name = "Xây dựng giải pháp hiệu quả chống lại các tấn công gây mất an toàn thông tin của Cổng Thông tin điện tử")]
        public byte ATTT_XayDungGiaiPhap { get; set; }

        [Display(Name = "Xây dựng phương án dự phòng khắc phục sự cố bảo đảm hệ thống Cổng Thông tin điện tử hoạt động liên tục ở mức tối đa")]
        public byte ATTT_XDPhuongAnDuPhong { get; set; }

        [Display(Name = "Ban hành quy chế phối hợp giữa các đơn vị trong cơ quan để cung cấp và xử lý thông tin")]
        public byte CongTTDT_BanHanhQuyChePhoiHop { get; set; }

        [Display(Name = "Ban hành quy chế hoạt động của Cổng TTĐT")]
        public byte CongTTDT_BanHanhQuyCheHoatDong { get; set; }

        [Display(Name = "Thực hiện chế độ báo cáo định kỳ hàng năm về tình hình hoạt động của Cổng TTĐT")]
        public byte CongTTDT_BaoCaoDinhKy_Nam { get; set; }

        [Display(Name = "Cập nhật thông tin theo quy định tại Điều 17 Nghị định số 43/2011/NĐ-CP")]
        public byte DangTai_CapNhatThongTin { get; set; }

        [Display(Name = "Nội dung các thông tin đăng tải trên Cổng TTĐT")]
        public byte DangTai_NoiDungThongTin { get; set; }

        [Display(Name = "Thực hiện quy định khác của Nghị định 72/2013/NĐ-CP về bản quyền thông tin đăng tải")]
        public byte DangTai_QuyDinhKhac { get; set; }

        [Display(Name = "Tổng số các đơn vị trực thuộc có cổng/trang thông tin điện tử")]
        public int TongSoDVTT_CoCongTTDT { get; set; }

        [Display(Name = "Số doanh nghiệp SX-KD phần cứng")]
        public int SoDNSXKD_PhanCung { get; set; }

        [Display(Name = "Số doanh nghiệp SX-KD phần mềm")]
        public int SoDNSXKD_PhanMem { get; set; }

        [Display(Name = "Số DN cung cấp DV nội dung số")]
        public int SoDNCungCapDVNDSo { get; set; }

        [Display(Name = "Danh sách DN CNTT-ĐT")]
        [StringLength(250)]
        public string DanhSachDNCNTT_DT { get; set; }

        [Display(Name = "Tổng doanh thu")]
        public decimal TongDoanhThu { get; set; }

        [Display(Name = "Tổng mức đầu tư")]
        public decimal TongMucDauTu { get; set; }

        [Display(Name = "Trường nhập lại")]
        public string TruongNhapLai { get; set; }

        [Display(Name = "Trạng thái")]
        public byte Success { get; set; }
    }
}
