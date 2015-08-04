using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Models
{
    public class ReportModel
    {
        [Display(Name = "Nhóm đơn vị")]
        public int NhomDonVi { get; set; }

        [Display(Name = "Báo cáo")]
        public int PhanHeChucNang { get; set; }

        [Display(Name = "Năm")]
        public int Nam { get; set; }

        [Display(Name = "Đợt báo cáo")]
        public int DotBaoCao { get; set; }

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        public object ListObject { get; set; }
    }
}