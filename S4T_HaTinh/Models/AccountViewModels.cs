using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using S4T_HaTinh.App_GlobalResources;

namespace S4T_HaTinh.Models
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ?")]
        public bool RememberMe { get; set; }

        [Display(Name="Vai trò")]
        public string UserRolesName { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name="Tên đơn vị")]
        public string TenDonVi { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        [Display(Name="Tên chuyên viên")]
        public string HoVaTen { get; set; }

        public int NhomDonVi_ID { get; set; }
    }

    public class RegisterViewModel
    {
        //[Required]
        //[Display(Name = "Mã của user")]
        //public string Id { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Tên đăng nhập")]
        [StringLength(256, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringMinRange")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }

        // New Fields added to extend Application User class:

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        [Display(Name = "Họ và tên")]        
        public string HoVaTen { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Tên đơn vị")]
        public int DonVi_ID { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [EmailAddress( ErrorMessage="Phải nhập email đúng định dạng")]
        [StringLength(256, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        [Display(Name = "Vai trò")]
        public string RoleId { get; set; }

        // Return a pre-poulated instance of AppliationUser:
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                UserName = this.UserName,
                DonVi_ID = this.DonVi_ID,
                Email = this.Email,
                HoVaTen = this.HoVaTen,
                TrangThai = this.TrangThai
            };
            return user;
        }
    }

    public class EditUserViewModel
    {
        [Key]
        [Required]
        [Display(Name = "Mã của user")]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        // New Fields added to extend Application User class:

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Tên đơn vị")]
        public int DonVi_ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [EmailAddress(ErrorMessage="Phải nhập email đúng định dạng")]
        [StringLength(256, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        [Display(Name = "Vai trò")]
        public string RoleId { get; set; }

        public EditUserViewModel() { }

        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(sp_GetUserAndRole_Result user)
        {
            this.Id = user.Id;
            this.UserName = user.UserName;
            this.HoVaTen = user.HoVaTen;
            this.DonVi_ID = user.DonVi_ID;
            this.Email = user.Email;
            this.TrangThai = user.TrangThai;
            this.RoleId = user.RoleId;
        }

        public EditUserViewModel(ApplicationUser user)
        {
            this.UserName = user.UserName;
            this.HoVaTen = user.HoVaTen;
            this.DonVi_ID = user.DonVi_ID;
            this.Email = user.Email;
            this.TrangThai = user.TrangThai;
        }
    }

    public class SelectUserRolesViewModel
    {
        public SelectUserRolesViewModel()
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }


        // Enable initialization with an instance of ApplicationUser:
        public SelectUserRolesViewModel(ApplicationUser user)
            : this()
        {
            this.UserName = user.UserName;
            this.HoVaTen = user.HoVaTen;
            this.DonVi_ID = user.DonVi_ID;
            this.Email = user.Email;
            this.TrangThai = user.TrangThai;

            var Db = new ApplicationDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles;
            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }

            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            foreach (var userRole in user.Roles)
            {
                var _role = allRoles.FirstOrDefault(o => o.Id == userRole.RoleId);
                if (_role != null)
                {
                    var checkUserRole =
                        this.Roles.Find(r => r.RoleName == _role.Name);
                    checkUserRole.Selected = true;
                }
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Tên đơn vị")]
        public int DonVi_ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [EmailAddress( ErrorMessage="Phải nhập email đúng định dạng")]
        [StringLength(256, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "StringOutOfRange")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }

    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }
        public SelectRoleEditorViewModel(IdentityRole role)
        {
            this.RoleName = role.Name;
        }

        public bool Selected { get; set; }

        [Required]
        public string RoleName { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ModelBinders), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(100, ErrorMessage = "Độ dài mật khẩu tối thiểu cần lớn hơn {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không đúng")]
        public string ConfirmPassword { get; set; }
    }
}
