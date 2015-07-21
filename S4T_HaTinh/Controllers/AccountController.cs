using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web.Security;
using System.Collections.Generic;
//using System.Security.Cryptography;
using System.Web.Helpers;
using System.Text;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //var te = UserManager.PasswordHasher.HashPassword("123456");
            //var bo = UserManager.PasswordHasher.VerifyHashedPassword("", "123456");
            //var test = Crypto.HashPassword("123456");
            //var test1 = Crypto.SHA1("123456");
            //var test2 = Crypto.SHA256("123456");
            //var test3 = Crypto.VerifyHashedPassword("", "123456");
            GetViewBag();
            ViewBag.ReturnUrl = returnUrl;
            return View("Login", "_Layout_Null");
        }

        private void GetViewBag()
        {
            var db = new S4T_HaTinhEntities();
            // Lấy danh sách các đơn vị cha
            var listNhomDonVi = db.Dm_DanhMucChung.Where(o => o.TrangThai == TrangThai.HoatDong && o.LoaiDanhMuc_ID == LoaiDanhMuc.NhomDonVi);
            var str = new StringBuilder();
            if (listNhomDonVi.Any())
            {
                str.Append("<ul>");
                foreach (var item in listNhomDonVi)
                {
                    var count = db.Dm_DonVi.Count(o => o.NhomDonVi_ID == item.DanhMuc_ID);
                    str.AppendFormat("<li><a name='DonViCapDuoi' class='child' id='{2}' href='#'>{0} <small>{1} Đơn vị</small></a>", item.TenDanhMuc, count, item.DanhMuc_ID);
                    str.AppendFormat("<div id='divDonViLogin_{0}'></div></li>", item.DanhMuc_ID);
                }
                str.Append("</ul>");
            }
            ViewBag.NhomDonVi = str.ToString();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var result = new SignInStatus();
            ViewBag.HideMenu = true;
            // Kiểm tra theo loại vai trò
            #region Nếu là người dùng (Tỉnh hoặc Huyện)
            if (model.UserRolesName.Equals("User")) // Kiểm tra login cho đơn vị
            {
                TempData["TabLoginDonVi"] = "loginDonVi";
                TempData["TabLoginDonVi_ID"] = model.NhomDonVi_ID;
                ViewBag.TabLoginDonVi_TenDonVi = model.TenDonVi;
                TempData["TabLoginDonVi_TenDonVi"] = model.TenDonVi;

                if (string.IsNullOrEmpty(model.HoVaTen))
                {
                    ModelState.AddModelError("HoVaTen", "Tên chuyên viên không được để trống");
                }
                if (string.IsNullOrEmpty(model.TenDonVi))
                {
                    ModelState.AddModelError("TenDonVi", "Tên đơn vị không được để trống");
                }
                ModelState.Remove("UserName");
                ModelState.Remove("Password");

                if (!ModelState.IsValid)
                {
                    GetViewBag();
                    return View("Login","_Layout_Null", model);
                }

                var objDonVi = db.Dm_DonVi.FirstOrDefault(o => o.TenDonVi.Trim().ToLower().Equals(model.TenDonVi.Trim().ToLower()));
                if (objDonVi == null)
                {
                    ModelState.AddModelError("TenDonVi", "Không tìm thấy đơn vị");
                    GetViewBag();
                    return View("Login", "_Layout_Null", model);
                }
                var user = UserManager.Users.FirstOrDefault(o => o.HoVaTen.Trim().ToLower() == model.HoVaTen.Trim().ToLower() && o.DonVi_ID == objDonVi.DonVi_ID);
                if (user == null)
                {
                    ModelState.AddModelError("HoVaTen", "Tên chuyên viên không đúng");
                    GetViewBag();
                    return View("Login", "_Layout_Null", model);
                }
                
                // Kiểm tra trạng thái Account đang bị khóa (không hoạt động)
                if (user.TrangThai == TrangThai.KhongHoatDong)
                {
                    ModelState.AddModelError("HoVaTen", "Tài khoản đang bị khóa");
                    GetViewBag();
                    return View("Login", "_Layout_Null", model);
                }
                                
                // Kiểm tra nhân viên có thuộc đơn vị ?
                if (!user.DonVi_ID.Equals(objDonVi.DonVi_ID))
                {
                    ModelState.AddModelError("HoVaTen", "Chuyên viên không thuộc đơn vị");
                    GetViewBag();
                    return View("Login", "_Layout_Null", model);
                }

                // Trường hợp đăng nhập, layout gọi login để lấy Session
                if (String.IsNullOrEmpty(returnUrl))
                {
                    await SignInManager.SignInAsync(user, model.RememberMe, model.RememberMe);

                    // Lưu người dùng vào session
                    Session.Add("User", user as ApplicationUser);
                    Session.Add("HoVaTenUser", user.HoVaTen);
                    GetPhanHeChucNangByUser(user);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    // Kiểm tra đơn vị có đang đc quyền nhập dữ liệu không
                    var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.DonVi_ID == objDonVi.DonVi_ID && o.TuNgay <= DateTime.Now
                                                                        && o.DenNgay >= DateTime.Now);
                    if (objLichNhap == null)
                        ModelState.AddModelError("TenDonVi", "Đơn vị chưa được nhập dữ liệu");
                    else //login thành công
                    {
                        await SignInManager.SignInAsync(user, model.RememberMe, model.RememberMe);

                        // Lưu người dùng vào session
                        Session.Add("User", user as ApplicationUser);
                        Session.Add("HoVaTenUser", user.HoVaTen);
                        GetPhanHeChucNangByUser(user);
                        return RedirectToLocal(returnUrl);
                    }
                }
            }
            #endregion Nếu là người dùng (Tỉnh hoặc Huyện)

            #region Nếu loại vai trò là Admin, Chuyên viên, Văn thư
            else
            {
                ModelState.Remove("TenDonVi");
                ModelState.Remove("HoVaTen");
                if (!ModelState.IsValid)
                {
                    GetViewBag();
                    return View("Login", "_Layout_Null", model);
                }

                // Kiểm tra trạng thái Account đang bị khóa (không hoạt động)
                var user = UserManager.Users.FirstOrDefault(o => o.UserName == model.UserName.Trim());
                if (user == null)
                {
                    ModelState.AddModelError("UserName", "Tài khoản nhập không đúng");
                    GetViewBag();
                    return View("Login", "_Layout_Null", model);
                }
                if (user.TrangThai == TrangThai.KhongHoatDong)
                {
                    ModelState.AddModelError("UserName", "Tài khoản đang bị khóa");
                    GetViewBag();
                    return View("Login", "_Layout_Null", model);
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            }
            #endregion Nếu loại vai trò là Admin, Chuyên viên, Văn thư

            switch (result)
            {
                case SignInStatus.Success:
                    // Tạo session cho người dùng
                    var user = UserManager.Users.FirstOrDefault(o => o.UserName == model.UserName.Trim());
                    if (user != null)
                    {
                        Session.Add("User", user as ApplicationUser);
                        Session.Add("HoVaTenUser", user.HoVaTen);
                    }
                    GetPhanHeChucNangByUser(user);

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("Password", "Sai mật khẩu truy cập");
                    return View("Login", "_Layout_Null", model);
            }
        }

        [AllowAnonymous]
        public ActionResult LoginRequest(string returnUrl)
        {
            GetViewBag();
            ViewBag.ReturnUrl = returnUrl;
            return View("LoginRequest", "_Layout_Null");
        }

        /// <summary>
        /// Lấy danh sách chức năng theo quyền của user
        /// </summary>
        /// <param name="user">object Người dùng</param>
        private void GetPhanHeChucNangByUser(ApplicationUser user)
        {
            var objRole = user.Roles.First();
            if (objRole != null)
            {
                var listChucNang = db.sp_GetChucNangByRole(objRole.RoleId).OrderBy(o => o.ThuTu).ToList();
                if (listChucNang.Any())
                {
                    Session.Add("ChucNang", listChucNang);
                } 
            }            
        }

        /// <summary>
        /// Lấy danh sách đơn vị cấp dưới
        /// </summary>
        /// <param name="id">Mã của nhóm đơn vị</param>
        [AllowAnonymous]
        public ActionResult GetDonViCapDuoi(int id)
        {
            var db = new S4T_HaTinhEntities();
            var listDonVi = db.Dm_DonVi.Where(o => o.TrangThai == TrangThai.HoatDong && o.NhomDonVi_ID == id);
            var str = new StringBuilder();
            if (listDonVi.Any())
            {
                foreach (var item in listDonVi)
                {
                    str.AppendFormat("<option value='{0}'>{0}</option>", item.TenDonVi);
                }
            }
            return Json(new { danhSach = str.ToString() });
        }

        /// <summary>
        /// Kiểm tra Session Time Out
        /// </summary>
        public ActionResult CheckSessionTimeout()
        {
            if (System.Web.HttpContext.Current.Session["User"] + "" == "")
            {
                TempData["msg"] = "Phiên đăng nhập của bạn đã hết, vui lòng đăng nhập lại !";
                return Json("0");
            }
            return Json("1");
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var Db = new ApplicationDbContext();
            var users = Db.Users;
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            return View(model);
        }
        
        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles(string id)
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == id);
            var model = new SelectUserRolesViewModel(user);
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        idManager.AddUserToRole(user.Id, role.RoleName);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }
                
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
        
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            System.Web.HttpContext.Current.Session["User"] = null;
            System.Web.HttpContext.Current.Session["LeftMenu"] = null;
            System.Web.HttpContext.Current.Session["ChucNang"] = null;
            System.Web.HttpContext.Current.Session["HoVaTenUser"] = null;
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
    }
}