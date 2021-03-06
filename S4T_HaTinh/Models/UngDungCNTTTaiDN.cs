//------------------------------------------------------------------------------
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
    
    public partial class UngDungCNTTTaiDN
    {
        [Key]
        public int UngDungCNTTTaiDN_ID { get; set; }
        public int DonVi_ID { get; set; }
        public string FileUpload { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        public string DiaChi { get; set; }
        public string NguoiDaiDien { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        public string SoDienThoai { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        public string NganhNgheKinhDoanh { get; set; }
        public byte LAN { get; set; }
        public int LoaiHinhDoanhNghiep { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public decimal DoanhThu { get; set; }
        public int SoMayTinh { get; set; }
        public int PhanMemDangUngDung { get; set; }
        public int SoWebsite { get; set; }
        public string KienNghi { get; set; }
        public byte Success { get; set; }
        public string TruongNhapLai { get; set; }
        public int LichNhap_ID { get; set; }
    }
}
