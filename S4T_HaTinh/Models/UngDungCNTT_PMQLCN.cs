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
    using S4T_HaTinh.App_GlobalResources;

    public partial class UngDungCNTT_PMQLCN
    {
        [Key]
        public int Id { get; set; }

        public string Guid { get; set; }

        public int UngDungCNTT_ID { get; set; }

        [Display(Name = "Tên phần mềm")]
        public string TenPhanMem { get; set; }

        [Display(Name = "Liên thông Sở ban ngành")]
        public int LienThongSBN { get; set; }

        [Display(Name = "Liên thông UBND cấp Huyện")]
        public int LienThongUBNDCapHuyen { get; set; }

        [Display(Name = "Liên thông UBND cấp Xã")]
        public int LienThongUBNDCapXa { get; set; }

        [Display(Name = "Liên thông Đơn vị trực thuộc")]
        public int LienThongDVTT { get; set; }
    }
}