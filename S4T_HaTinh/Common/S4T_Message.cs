using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace S4T_HaTinh.Common
{
    public class S4T_Message
    {
        public static string LichNhapLieuMessage(int trangThai)
        {
            return trangThai == TrangThaiLichNhapLieu.ChuaDenThoiDiem ? "Chưa đến thời điểm nhập liệu"
                             : trangThai == TrangThaiLichNhapLieu.QuaHan ? "Quá thời hạn nhập liệu"
                             : trangThai == TrangThaiLichNhapLieu.KhongHoatDong ? "Không tìm thấy yêu cầu nhập liệu"
                             : "";
        } 
    }
}