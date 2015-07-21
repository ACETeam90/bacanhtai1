using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data.Entity.Validation;
using System.Configuration;
using S4T_HaTinh.Common;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Controllers
{
    [Authorize]
    public class Ht_UserController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();
        private ApplicationUserManager _userManager;

        /// <summary>
        /// Hiển thị danh sách Cán bộ chuyên trách CNTT các đơn vị
        /// </summary>
        /// <param name="status">status = null: Khi load page lần đầu sẽ trả ra View trắng</param>
        /// <param name="ddlNhomDonVi">Mã nhóm đơn vị</param>
        /// <param name="ddlDonViCap1">Mã nhóm đơn vị cấp trên</param>
        /// <param name="ddlDonVi">Mã đơn vị</param>
        /// <param name="roleName">Vai trò người dùng</param>
        /// <param name="trangThai">Trạng thái account</param>
        public ActionResult ListUser(int? status, int? ddlDonVi, string roleName, int? trangThai)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            GetViewBag(NhomDoiTuong.DonVi);
            
            var objRole = MvcApplication.ListRole.FirstOrDefault(o => o.Name == roleName && o.NhomDoiTuong_ID == NhomDoiTuong.DonVi);
            var role_ID = objRole == null ? null : objRole.Id;
            var list = db.sp_GetUserByDonVi_Role(null, null, ddlDonVi, role_ID, trangThai == null ? TrangThai.HoatDong : trangThai).Where(o => o.DonVi_ID != DonVi.SoThongTinTruyenThong);

            return View(list);
        }

        /// <summary>
        /// Hiển thị danh sách Chuyên viên Sở TTTT
        /// </summary>
        /// <param name="roleName">vai trò người dùng</param>
        /// <param name="trangThai">trạng thái account</param>
        public ActionResult ListChuyenVienSo(string roleName, int? trangThai)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            GetViewBag(NhomDoiTuong.SoTTTT);
            

            var objRole = MvcApplication.ListRole.FirstOrDefault(o => o.Name == roleName && o.NhomDoiTuong_ID == NhomDoiTuong.SoTTTT);
            var role_ID = objRole == null ? null : objRole.Id;
            var list = db.sp_GetUserByDonVi_Role(null, null, DonVi.SoThongTinTruyenThong, role_ID, trangThai == null ? TrangThai.HoatDong : trangThai);

            return View(list);
        }

        /// <summary>
        /// Lấy các ViewBag trên view
        /// </summary>
        private void GetViewBag(int type)
        {
            //ViewBag.ListDonViCap1 = listDonVi.Where(o => o.DonViCap1_ID == -1); // Lấy list đơn vị cấp trên

            var action = Request.RequestContext.RouteData.GetRequiredString("action");
            if (action == "Edit")
                ViewBag.ListDonVi = MvcApplication.ListDonVi.OrderBy(o => o.TenDonVi);
            else
                ViewBag.ListDonVi = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong).OrderBy(o => o.TenDonVi).ToList();

            if (type == NhomDoiTuong.SoTTTT)
            {
                ViewBag.ListRole = MvcApplication.ListRole.Where(o => o.NhomDoiTuong_ID == NhomDoiTuong.SoTTTT).OrderBy(o => o.TenHienThi);
                ViewBag.DonViSoThongTinTruyenThong = DonVi.SoThongTinTruyenThong;
            }
            else
            {
                ViewBag.ListRole = MvcApplication.ListRole.Where(o => o.NhomDoiTuong_ID == NhomDoiTuong.DonVi).OrderBy(o => o.TenHienThi);
            }
        }

        public ActionResult Create(int? donViSoTTTT,string vi)
        {
            ViewBag.URLPrevious = vi;
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            var obj = new RegisterViewModel();
            
            if (donViSoTTTT != null)
            {
                obj.DonVi_ID = donViSoTTTT.Value;
                GetViewBag(NhomDoiTuong.SoTTTT);
            }else
                GetViewBag(NhomDoiTuong.DonVi);

            return View(obj);
        }

        private bool Validate(ApplicationUser user)
        {
            if (user.DonVi_ID == DonVi.SoThongTinTruyenThong)
            {
                var isExist = db.AspNetUsers.Any(o => o.UserName.Trim().ToUpper() == user.UserName.Trim().ToUpper());
                if (isExist)
                    ModelState.AddModelError("", "Đã tồn tại account: " + user.UserName);
            }
            else
            {
                var isExist = db.AspNetUsers.Any(o => o.HoVaTen.Trim().ToUpper() == user.HoVaTen.Trim().ToUpper() && o.DonVi_ID == user.DonVi_ID);
                if (isExist)
                    ModelState.AddModelError("", "Đã tồn tại tên chuyên viên: " + user.HoVaTen + " trong đơn vị này");
            }
            if (ModelState.IsValid)
                return true;
            else
                return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserName,Password, HoVaTen,DonVi_ID,Email,TrangThai,RoleId")] RegisterViewModel ht_User)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            ht_User.TrangThai = TrangThai.HoatDong;
            if (ht_User.DonVi_ID != DonVi.SoThongTinTruyenThong)
            {
                ModelState.Remove("UserName");
                ModelState.Remove("Password");
                var guid = Guid.NewGuid().ToString();
                ht_User.UserName = ht_User.HoVaTen + "_" + ht_User.DonVi_ID + "_" + guid;
                ht_User.Password = ht_User.HoVaTen + "_" + guid;
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser(ht_User);
                if (Validate(user)){
                    try
                    {
                        user.UserName = user.UserName.TrimEnd();
                        var result = await UserManager.CreateAsync(user, ht_User.Password);
                        if (result.Succeeded)
                        {
                            // Tạo quyền cho user
                            var objUserRole = new AspNetUserRoles();
                            objUserRole.RoleId = ht_User.RoleId;
                            objUserRole.UserId = user.Id;
                            db.AspNetUserRoles.Add(objUserRole);
                            await db.SaveChangesAsync();

                            // Tự động đăng nhập sau khi tạo acc
                            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            // Send an email with this link
                            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                            if (ht_User.DonVi_ID == DonVi.SoThongTinTruyenThong)
                                return RedirectToAction("ListChuyenVienSo");
                            else
                                return RedirectToAction("ListUser");
                        }
                        else
                            AddErrors(result);
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var sb = new StringBuilder();

                        foreach (var failure in ex.EntityValidationErrors)
                        {
                            sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                            foreach (var error in failure.ValidationErrors)
                            {
                                sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                sb.AppendLine();
                            }
                        }

                        throw new DbEntityValidationException(
                            "Entity Validation Failed - errors follow:\n" +
                            sb.ToString(), ex
                        ); // Add the original exception as the innerException
                    }
                }
            }

            if (ht_User.DonVi_ID == DonVi.SoThongTinTruyenThong)
            {
                GetViewBag(NhomDoiTuong.SoTTTT);
            }
            else
                GetViewBag(NhomDoiTuong.DonVi);

            return View(ht_User); 
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult Edit(string id, string vi)
        {
            ViewBag.URLPrevious = vi;
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            var obj = db.sp_GetUserAndRole(id).FirstOrDefault();
            var ht_User = new EditUserViewModel(obj);
            if (ht_User == null)
            {
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            if (ht_User.DonVi_ID == DonVi.SoThongTinTruyenThong)
            {
                GetViewBag(NhomDoiTuong.SoTTTT);
            }
            else
                GetViewBag(NhomDoiTuong.DonVi);

            return View(ht_User);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserName,HoVaTen,DonVi_ID,Email,TrangThai,RoleId")] EditUserViewModel ht_User)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            try
            {
                ModelState.Remove("UserName");
                ModelState.Remove("DonVi_ID");
                ModelState.Remove("RoleId");

                var objUserOld = db.AspNetUsers.FirstOrDefault(o => o.Id == ht_User.Id);
                if (objUserOld == null) return JavaScript("Không tìm thấy thông tin người dùng");
                ht_User.DonVi_ID = objUserOld.DonVi_ID;
                if (ModelState.IsValid)
                {
                    objUserOld.HoVaTen = ht_User.HoVaTen;
                    objUserOld.Email = ht_User.Email;
                    objUserOld.TrangThai = ht_User.TrangThai;
                    db.Entry(objUserOld).State = EntityState.Modified;

                    var objUserRoleOld = db.AspNetUserRoles.FirstOrDefault(o => o.UserId == ht_User.Id);
                    if (objUserRoleOld != null && !objUserRoleOld.RoleId.Equals(ht_User.RoleId))
                    {
                        IdentityManager mana = new IdentityManager();

                        // Xóa role cũ cho User
                        mana.ClearUserRoles(ht_User.Id);

                        // Add role mới cho User
                        var roleName = db.AspNetRoles.FirstOrDefault(o => o.Id == ht_User.RoleId).Name;
                        mana.AddUserToRole(ht_User.Id, roleName);
                    }
                    await db.SaveChangesAsync();
                }
                else
                {
                    if (ht_User.DonVi_ID == DonVi.SoThongTinTruyenThong){
                        GetViewBag(NhomDoiTuong.SoTTTT);
                    }
                    else
                        GetViewBag(NhomDoiTuong.DonVi);
                    return View(ht_User);
                }
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
            if (ht_User.DonVi_ID == DonVi.SoThongTinTruyenThong)
                return RedirectToAction("ListChuyenVienSo");
            else
                return RedirectToAction("ListUser");
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            }
            AspNetUsers user = await db.AspNetUsers.FindAsync(id);
            if (user == null)
            {
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            }
            else
            {
                user.TrangThai = TrangThai.KhongHoatDong;
                db.Entry(user).State = EntityState.Modified;
                //db.Ht_User.Remove(ht_User);
                await db.SaveChangesAsync();
            }
            return Json(new { msg = "ok" });
        }
        
        public ActionResult ResetPassword(string id, string userName)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (string.IsNullOrEmpty(userName)) return JavaScript("Không tìm thấy mã nhân viên");
            var obj = new ResetPasswordViewModel
            {
                Id = id,
                UserName = userName
            };
            return View(obj);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.ConfirmPassword);
            var user = db.AspNetUsers.FirstOrDefault(o => o.Id == model.Id);
            user.PasswordHash = hashedNewPassword;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();
            ViewBag.Message = "Đổi mật khẩu thành công";
            return View(model);
        }

        #region Event
        /// <summary>
        /// Lấy danh sách đơn vị cấp 2 theo Nhóm đơn vị và đơn vị cấp trên
        /// </summary>
        /// <param name="ddlNhomDonVi">ID của nhóm đơn vị</param>
        /// <param name="ddlDonViCap1">ID của đơn vị cấp trên - nếu ko chọn thì sẽ hiển thị cả DV cấp 1 và 2</param>
        public ActionResult NhomDonVi_OnChange(Nullable<int> ddlDonViCap1)
        {
            try
            {
                var list = (IEnumerable<Dm_DonVi>) db.Dm_DonVi;
                //if(ddlNhomDonVi != null)
                //    list = list.Where(o =>o.TrangThai == TrangThai.HoatDong &&  o.NhomDonVi_ID == ddlNhomDonVi);
                if (ddlDonViCap1 != null)
                    list = list.Where(o => o.TrangThai == TrangThai.HoatDong && o.DonViCap1_ID == ddlDonViCap1);

                var objDonViCap1 = db.Dm_DonVi.FirstOrDefault(o => o.DonVi_ID == ddlDonViCap1);
                var str = new StringBuilder().AppendFormat("<option value='{0}'>{1}</option>",objDonViCap1.DonVi_ID, objDonViCap1.TenDonVi);
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        str.AppendFormat("<option value='{0}'>{1}</option>", item.DonVi_ID, item.TenDonVi);
                    }
                }
                return Json(new { danhSach = str.ToString() });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }
        
        /// <summary>
        /// Lấy danh sách đơn vị cấp 2 theo Nhóm đơn vị và đơn vị cấp trên
        /// </summary>
        /// <param name="ddlNhomDonVi">ID của nhóm đơn vị</param>
        /// <param name="ddlDonViCap1">ID của đơn vị cấp trên - nếu ko chọn thì sẽ hiển thị cả DV cấp 1 và 2</param>
        public ActionResult DonVi_OnChange(Nullable<int> donVi_ID)
        {
            try
            {
                var objDonVi = MvcApplication.ListDonVi.FirstOrDefault(o => o.DonVi_ID == donVi_ID);
                if (objDonVi != null)
                {
                    if (objDonVi.NhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                    {
                        var configRoleUser = ConfigurationManager.AppSettings["RoleUserCapHuyen"];
                        if (String.IsNullOrEmpty(configRoleUser))
                            return Json(new { msg = "Không tìm thấy mã người dùng cấp Huyện" });

                        var objRole = db.AspNetRoles.FirstOrDefault(o => o.Id == configRoleUser);

                        if (objRole != null)
                            return Json(new { RoleName = objRole.TenHienThi, Role_ID = configRoleUser });
                    }
                    else
                    {
                        var configRoleUser = ConfigurationManager.AppSettings["RoleUserCapSo"];
                        if (String.IsNullOrEmpty(configRoleUser))
                            return Json(new { msg = "Không tìm thấy mã người dùng cấp Tỉnh" });

                        var objRole = db.AspNetRoles.FirstOrDefault(o => o.Id == configRoleUser);

                        if (objRole != null)
                            return Json(new { RoleName = objRole.TenHienThi, Role_ID = configRoleUser });
                    }

                    return Json(new { msg = "Không tìm thấy mã vai trò người dùng" });
                }

                return Json(new { msg = "Không tìm thấy mã đơn vị" });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
