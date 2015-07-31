namespace S4T_HaTinh.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class HaTangNhanLucCNTT_Huyen
    {
        [Key]
        public int HaTangNhanLucCNTTHuyen_ID { get; set; }

        public int LichNhap_ID { get; set; }

        [Display(Name = "Đơn vị")]
        public int DonVi_ID { get; set; }

        [Display(Name = "Số trường Tiểu Học")]
        public int TieuHoc_SoTruong { get; set; }

        [Display(Name = "Môn tin học Trường Tiểu Học")]
        public int TieuHoc_TinHoc { get; set; }

        [Display(Name = "Ngành tin học Trường Tiểu Học")]
        public int TieuHoc_NganhCNTT { get; set; }

        [Display(Name = "Khoa tin học Trường Tiểu Học")]
        public int TieuHoc_KhoaCNTT { get; set; }

        [Display(Name = "Giáo viên CNTT Trường Tiểu Học")]
        public int TieuHoc_GVCNTT { get; set; }

        [Display(Name = "Tiến sĩ CNTT Trường Tiểu Học")]
        public int TieuHoc_TSCNTT { get; set; }

        [Display(Name = "Thạc sĩ Trường Tiểu Học")]
        public int TieuHoc_ThSCNTT { get; set; }

        [Display(Name = "Sinh viên tốt nghiệp Trường Tiểu Học")]
        public int TieuHoc_SVCNTT { get; set; }

        [Display(Name = "Số trường THCS")]
        public int THCS_SoTruong { get; set; }

        [Display(Name = "Môn tin học Trường THCS")]
        public int THCS_TinHoc { get; set; }

        [Display(Name = "Ngành tin học Trường THCS")]
        public int THCS_NganhCNTT { get; set; }

        [Display(Name = "Khoa tin học Trường THCS")]
        public int THCS_KhoaCNTT { get; set; }

        [Display(Name = "Giáo viên CNTT  Trường THCS")]
        public int THCS_GVCNTT { get; set; }

        [Display(Name = "Tiến sĩ CNTT Trường THCS")]
        public int THCS_TSCNTT { get; set; }

        [Display(Name = "Thạc sĩ Trường THCS")]
        public int THCS_ThSCNTT { get; set; }

        [Display(Name = "Sinh viên tốt nghiệp Trường THCS")]
        public int THCS_SVCNTT { get; set; }

        [Display(Name = "Số trường THPT")]
        public int THPT_SoTruong { get; set; }

        [Display(Name = "Môn tin học THPT")]
        public int THPT_TinHoc { get; set; }

        [Display(Name = "Ngành tin học THPT")]
        public int THPT_NganhCNTT { get; set; }

        [Display(Name = "Khoa tin học THPT")]
        public int THPT_KhoaCNTT { get; set; }

        [Display(Name = "Giáo viên CNTT THPT")]
        public int THPT_GVCNTT { get; set; }

        [Display(Name = "Tiến sĩ CNTT Trường THPT")]
        public int THPT_TSCNTT { get; set; }

        [Display(Name = "Thạc sĩ Trường THPT")]
        public int THPT_ThSCNTT { get; set; }

        [Display(Name = "Sinh viên tốt nghiệp THPT")]
        public int THPT_SVCNTT { get; set; }

        [Display(Name = "Số trường Trung cấp")]
        public int TrungCap_SoTruong { get; set; }

        [Display(Name = "Môn tin học Trung cấp")]
        public int TrungCap_TinHoc { get; set; }

        [Display(Name = "Ngành CNTT Trung cấp")]
        public int TrungCap_NganhCNTT { get; set; }

        [Display(Name = "Khoa CNTT Trường Trung cấp")]
        public int TrungCap_KhoaCNTT { get; set; }

        [Display(Name = "Giáo viên CNTT Trường Trung cấp")]
        public int TrungCap_GVCNTT { get; set; }

        [Display(Name = "Tiến sĩ Trường Trung cấp")]
        public int TrungCap_TSCNTT { get; set; }

        [Display(Name = "Thạc sĩ Trường Trung cấp")]
        public int TrungCap_ThSCNTT { get; set; }

        [Display(Name = "Sinh viên tốt nghiệp Trung cấp")]
        public int TrungCap_SVCNTT { get; set; }

        [Display(Name = "Số trường Cao Đẳng")]
        public int CaoDang_SoTruong { get; set; }

        [Display(Name = "Môn tin học Cao Đẳng")]
        public int CaoDang_MonTinHoc { get; set; }

        [Display(Name = "Ngành CNTT của Trường Cao Đẳng")]
        public int CaoDang_NganhCNTT { get; set; }

        [Display(Name = "Khoa CNTT của Trường Cao Đẳng")]
        public int CaoDang_KhoaCNTT { get; set; }

        [Display(Name = "Giáo viên CNTT của Trường Cao Đẳng")]
        public int CaoDang_GVCNTT { get; set; }

        [Display(Name = "Tiến sĩ của Trường Cao Đẳng")]
        public int CaoDang_TSCNTT { get; set; }

        [Display(Name = "Thạc sĩ của Trường Cao Đẳng")]
        public int CaoDang_ThSCNTT { get; set; }

        [Display(Name = "Sinh viên CNTT Cao Đẳng")]
        public int CaoDang_SVCNTT { get; set; }

        [Display(Name = "Số trường Đại Học")]
        public int DaiHoc_SoTruong { get; set; }

        [Display(Name = "Môn tin học Đại Học")]
        public int DaiHoc_MonTinHoc { get; set; }

        [Display(Name = "Ngành CNTT của Trường Đại Học")]
        public int DaiHoc_NganhCNTT { get; set; }

        [Display(Name = "Khoa CNTT của Trường Đại Học")]
        public int DaiHoc_KhoaCNTT { get; set; }

        [Display(Name = "Giáo viên CNTT của Trường Đại Học")]
        public int DaiHoc_GVCNTT { get; set; }

        [Display(Name = "Tiến sĩ CNTT của Trường Đại Học")]
        public int DaiHoc_TSCNTT { get; set; }

        [Display(Name = "Thạc sĩ CNTT của Trường Đại Học")]
        public int DaiHoc_ThSCNTT { get; set; }

        [Display(Name = "Sinh viên CNTT Đại Học")]
        public int DaiHoc_SVCNTT { get; set; }

        [Display(Name = "Số trường Khác")]
        public int Khac_SoTruong { get; set; }

        [Display(Name = "Môn tin học trường Khác")]
        public int Khac_MonTinHoc { get; set; }

        [Display(Name = "Ngành CNTT trường Khác")]
        public int Khac_NganhCNTT { get; set; }

        [Display(Name = "Khoa CNTT trường Khác")]
        public int Khac_KhoaCNTT { get; set; }

        [Display(Name = "Giáo viên CNTT trường Khác")]
        public int Khac_GVCNTT { get; set; }

        [Display(Name = "Tiến sĩ CNTT trường Khác")]
        public int Khac_TSCNTT { get; set; }

        [Display(Name = "Thạc sĩ CNTT trường Khác")]
        public int Khac_ThSCNTT { get; set; }

        [Display(Name = "Sinh viên tốt nghiệp trường Khác")]
        public int Khac_SVCNTT { get; set; }

        public int CBCT_Huyen_SoLuong { get; set; }

        public int CBCT_Huyen_TienSy { get; set; }

        public int CBCT_Huyen_ThacSy { get; set; }

        public int CBCT_Huyen_DHCQ { get; set; }

        public int CBCT_Huyen_DHKhongCQ { get; set; }

        public int CBCT_Huyen_HocThacSy { get; set; }

        public int CBCT_Huyen_NghienCuuSinh { get; set; }

        public int CBCT_Huyen_LuotTapHuan { get; set; }

        public int CBCT_Xa_SoLuong { get; set; }

        public int CBCT_Xa_TienSy { get; set; }

        public int CBCT_Xa_ThacSy { get; set; }

        public int CBCT_Xa_DHCQ { get; set; }

        public int CBCT_Xa_DHKhongCQ { get; set; }

        public int CBCT_Xa_HocThacSy { get; set; }

        public int CBCT_Xa_NghienCuuSinh { get; set; }

        public int CBCT_Xa_LuotTapHuan { get; set; }

        [Display(Name = "Số lượng CBCCVC Tiến sĩ CNTT biết sử dụng thành thạo máy tính UBND cấp Huyện")]
        public int UBND_ThanhThao { get; set; }

        public int UBND_ThamGiaTapHuan { get; set; }

        public int UBND_TSCNTT { get; set; }

        public int UBND_ThSCNTT { get; set; }

        public int UBND_HocThacSy { get; set; }

        public int UBND_NghienCuuSinh { get; set; }

        public int HuyenUy_ThanhThao { get; set; }

        public int HuyenUy_LuotTapHuan { get; set; }

        public int HuyenUy_TSCNTT { get; set; }

        public int HuyenUy_ThSCNTT { get; set; }

        public int HuyenUy_HocThacSy { get; set; }

        public int HuyenUy_NghienCuuSinh { get; set; }

        public int Xa_ThanhThao { get; set; }

        public int Xa_LuotTapHuan { get; set; }

        public int Xa_TSCNTT { get; set; }

        public int Xa_ThSCNTT { get; set; }

        public int Xa_HocThacSy { get; set; }

        public int Xa_NghienCuuSinh { get; set; }

        public int GiaoDuc_ThanhThao { get; set; }

        public int GiaoDuc_LuotTapHuan { get; set; }

        public int GiaoDuc_TSCNTT { get; set; }

        public int GiaoDuc_ThSCNTT { get; set; }

        public int GiaoDuc_HocThacSy { get; set; }

        public int GiaoDuc_NghienCuuSinh { get; set; }

        public int YTe_ThanhThao { get; set; }

        public int YTe_LuotTapHuan { get; set; }

        public int YTe_TSCNTT { get; set; }

        public int YTe_ThSCNTT { get; set; }

        public int YTe_HocThacSy { get; set; }

        public int YTe_NghienCuuSinh { get; set; }

        public int DVSN_ThanhThao { get; set; }

        public int DVSN_LuotTapHuan { get; set; }

        public int DVSN_TSCNTT { get; set; }

        public int DVSN_ThSCNTT { get; set; }

        public int DVSN_HocThacSy { get; set; }

        public int DVSN_NghienCuuSinh { get; set; }

        public int DN_ThanhThao { get; set; }

        public int DN_LuotTapHuan { get; set; }

        public int DN_TSCNTT { get; set; }

        public int DN_ThSCNTT { get; set; }

        public int DN_HocThacSy { get; set; }

        public int DN_NghienCuuSinh { get; set; }

        public int CBCCVC_Khac_ThanhThao { get; set; }

        public int CBCCVC_Khac_LuotTapHuan { get; set; }

        public int CBCCVC_Khac_TSCNTT { get; set; }

        public int CBCCVC_Khac_ThSCNTT { get; set; }

        public int CBCCVC_Khac_HocThacSy { get; set; }

        public int CBCCVC_Khac_NghienCuuSinh { get; set; }

        [Display(Name = "Tổng chi ngân sách cho đào tạo CNTT")]
        public decimal TongChiNganSach { get; set; }

        //[Display(Name = "Tổng mức đầu tư")]
        [Display(Name = "Tổng chi ngân sách cho đào tạo CNTT")]
        public decimal TongMucDauTu { get; set; }

        [Display(Name = "Trường nhập lại")]
        public string TruongNhapLai { get; set; }

        [Display(Name = "Trạng thái")]
        public byte Success { get; set; }
    }
}
