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
    
    public partial class Dm_CapXuLy
    {
        [Key]
        [Display(Name="Mã cấp")]
        public int Cap_ID { get; set; }

        [Display(Name="Tên cấp")]
        public string TenCap { get; set; }

        [Display(Name = "Thứ tự")]
        public Nullable<int> ThuTu { get; set; }

        [Display(Name = "Người xử lý")]
        public string NguoiXuLy { get; set; }

        [Display(Name = "Trạng thái")]
        public byte TrangThai { get; set; }
    }
}
