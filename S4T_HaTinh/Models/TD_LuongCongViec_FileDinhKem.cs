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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class TD_LuongCongViec_FileDinhKem
    {
        [Key]
        public int FileHoSoDinhKem_ID { get; set; }
        public int LuongCongViec_ID { get; set; }

        [Display(Name = "Tên file")]
        public string TenFile { get; set; }
        public string TenHienThi { get; set; }

        [Display(Name = "Đường dẫn")]
        public string DuongDan { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        [Display(Name = "Trạng thái")]
        public Nullable<int> TrangThai { get; set; }

        [Display(Name = "Tên văn bản")]
        public string TenVanBan { get; set; }

        [Display(Name = "Số thứ tự")]
        public Nullable<int> STT { get; set; }
        public string MaHoSo { get; set; }
    }
}
