using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Common
{
    public class TrangThai
    {
        public const int HoatDong = 1;
        public const int KhongHoatDong = 2;
    }

    public class TrangThaiNhapLieu
    {
        public const int ThemMoi = 14;
        public const int Sua = 15;
        public const int DaGui = 16;
        //public const int ChoDuyet = 17,
        public const int PheDuyet = 18;
    }

    public class TrangThaiLichNhapLieu
    {
        public const int KhongHoatDong = 0;
        public const int HoatDong = 1;
        public const int QuaHan = 2;
        public const int ChuaDenThoiDiem = 3;
    }

    public class LoaiDanhMuc
    {
        public const int NhomDonVi = 5; // Nhóm đơn vị
        public const int NhomDoiTuong = 10;
    }

    public class NhomDoiTuong
    {
        public const int SoTTTT = 31;
        public const int DonVi = 32;
    }

    public class PhanHe
    {
        public const int QuanLyThongTin = 20;
    }

    public class PhamVi
    {
        public const int CaNhan = 0;
        public const int PhongBan = 1;
    }

    public class TrangThaiDauViec
    {
        public const int HoanThanh = 1;
        public const int KetThuc = 2;
    }

    public class DonVi
    {
        public const int SoThongTinTruyenThong = 97;
        public const int NhomDonViCapHuyen = 12;
        public const int NhomDonViCapXa = 51;
        public const int NhomDonViCapTinh = 11;
    }

    public class LoaiChuyen
    {
        public const int GiaoViec = 0;
        public const int BaoCao = 1;
        public const int XinYKien = 2;
        public const int TraLoi = 3;
    }

    public class STTCapXuLy
    {
        public const int CapNhatHoSo = 1;
        public const int PhanHoi = -1;
        public const int XinYKien = 0;
        public const int DaPheDuyet = 5;
    }

    public enum ViewReport
    {
        Index,
        Create,
        Edit,
        Delete
    }

    /// <summary>
    /// Dùng cho thẩm định hồ sơ
    /// </summary>
    public class Permission
    {
        public bool isView { get; set; }
        public bool isEdit { get; set; }
        public bool isSuccess { get; set; } // Được quyền phê duyệt hồ sơ
    }

    public enum PermissionType
    {
        Deny = 0,
        Read = 1,
        Write = 2
    }

    public class LoaiBaoCao
    {
        public const string UngDungCNTT = "UngDungCNTT";
        public const string ChinhSach = "ChinhSach";
    }

    public class MaBaoCao
    {
        public static int[] BaoCaoSo =  { 10, 11, 12, 13, 14 };
        public static int[] BaoCaoHuyen = { 15, 16, 20, 21 };
    }

    public class Han
    {
        public const int HoanThanhDungHan = 1;
        public const int HoanThanhMuon = 0;
        public const int QuaHan = 2;
    }

    public class VanBanKhac {
        public const int VBTiepNhanHoSoKhac_ID = 5;
    }
}