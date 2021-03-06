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
    
    public partial class Ht_User
    {
        [Key]
        public int User_ID { get; set; }
        public int LoaiUser_ID { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]

        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [Display(Name = "Đơn vị")]
        public int DonVi_ID { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Quyền hạn")]
        public string QuyenHan { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }
    }
}
