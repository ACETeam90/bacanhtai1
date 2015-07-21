using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Text;
using System.Configuration;
using System.Reflection;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;
//using System.Transactions;

namespace S4T_HaTinh.Controllers
{
    public class ToChucChinhSachCNTTsController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();
        private Ht_PhanHeChucNang objChucNang = new S4T_HaTinhEntities().Ht_PhanHeChucNang.ToList().FirstOrDefault(o => o.ControllerName == System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller"));

        /// <summary>
        /// Lấy các ViewBag
        /// </summary>
        private void GetViewBag(int donVi_ID)
        {
            // Đơn vị
            var objDonVi = db.Dm_DonVi.Find(donVi_ID);
            ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";
            var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                            && o.DonVi_ID == donVi_ID
                                                            && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                            && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
            if (objLichNhap != null)
            {
                ViewBag.TuNgay = objLichNhap.TuNgay;
                ViewBag.DenNgay = objLichNhap.DenNgay;
            }
        }

        public async Task<ActionResult> Index()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] = null;
            
            var listLichNhapByDonVi = db.Ht_LichNhapLieu.Where(o => o.TrangThai == TrangThai.HoatDong && o.DonVi_ID == user.DonVi_ID && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID);
            ViewBag.ListLichNhap = listLichNhapByDonVi;
            if (listLichNhapByDonVi.Any(o => o.ChucNang_ID == TrangThaiNhapLieu.ThemMoi))
                ViewBag.CreateLink = true;

            return View(await db.ToChucChinhSachCNTT.Where(o => o.DonVi_ID == user.DonVi_ID).ToListAsync());
        }

        /// <summary>
        /// Quản lý các báo cáo môi trường chính sách của đơn vị
        /// </summary>
        public ActionResult List()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.ChinhSach);
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));
            
            return View();
        }

        /// <summary>
        /// Check trạng thái báo cáo theo lịch nhập liệu 
        /// </summary>
        /// <param name="user">object người dùng</param>
        /// <param name="status">Trạng thái của báo cáo</param>
        private string CheckReportStatus(ApplicationUser user, int status)
        {
            var _objLichNhapLieu = S4T_HaTinhBase.GetTrangThaiLichNhapLieu(user, objChucNang.PhanHeChucNang_ID);
            if (_objLichNhapLieu.TrangThai != TrangThaiLichNhapLieu.HoatDong)
                return ExceptionViewer.GetMessage(_objLichNhapLieu);
            else
            {
                if (status == TrangThaiNhapLieu.ThemMoi)
                {
                    // Kiểm tra đã tồn tại bản ghi trong database?
                    var objReport = db.ToChucChinhSachCNTT.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                        && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                        && o.Success != TrangThaiNhapLieu.PheDuyet);
                    return objReport == null ? "" : ExceptionViewer.GetMessage("LICH_NHAP_NOT_FOUND");
                }
                else
                {
                    // Kiểm tra đã tồn tại bản ghi trong database?
                    var objReport = db.ToChucChinhSachCNTT.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                        && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                        && o.Success == (byte)TrangThaiNhapLieu.Sua);
                    return objReport != null ? "" : ExceptionViewer.GetMessage("LICH_NHAP_NOT_FOUND");
                }
            }
        }

        public async Task<ActionResult> Details(int? id, int? donVi_ID, int? lichNhap_ID)
        {
            var returnUrl = Request.UrlReferrer;
            if (returnUrl != null)
                ViewBag.returnUrl = returnUrl.ToString();

            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny)
            {
                // Kiểm tra thêm quyền là Thẩm định báo cáo ?
                per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.ChinhSach);
                if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));
            }

            ToChucChinhSachCNTT toChucChinhSachCNTT = new ToChucChinhSachCNTT();
            if (id == null)
            {
                if (lichNhap_ID == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                else
                    toChucChinhSachCNTT = await db.ToChucChinhSachCNTT.FirstOrDefaultAsync(o => o.LichNhap_ID == lichNhap_ID
                                                                                                && o.DonVi_ID == donVi_ID);
            }
            else
                toChucChinhSachCNTT = await db.ToChucChinhSachCNTT.FindAsync(id);

            if (toChucChinhSachCNTT == null)
                return HttpNotFound();
                
            // Lấy danh sách file đã đc up lên
            TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] = db.Ht_FileDinhKem.Where(o => o.PhanHeChucNang_ID == objChucNang.PhanHeChucNang_ID && o.BaoCao_ID == toChucChinhSachCNTT.ToChucChinhSachCNTT_ID).ToList();
            
            return View(toChucChinhSachCNTT);
        }

        // GET: ToChucChinhSachCNTTs/Create
        public ActionResult Create()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            GetViewBag(user.DonVi_ID);
            
            var mess = CheckReportStatus(user, TrangThaiNhapLieu.ThemMoi);
            if (String.IsNullOrEmpty(mess))
            {
                ToChucChinhSachCNTT obj = new ToChucChinhSachCNTT();
                obj.Success = (byte)TrangThaiNhapLieu.ThemMoi;
                obj.DonVi_ID = user.DonVi_ID;
                TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] = null;

                return View(obj);
            }

            return Content(mess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[MultipleButton(Name = "action", Argument = "Create")]
        public async Task<ActionResult> Create(ToChucChinhSachCNTT toChucChinhSachCNTT)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                var mess = CheckReportStatus(user, TrangThaiNhapLieu.ThemMoi);
                if (String.IsNullOrEmpty(mess))
                {
                    try
                    {
                        var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.DonVi_ID == toChucChinhSachCNTT.DonVi_ID
                                                                          && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                          && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);

                        // Đổi trạng thái nhập liệu
                        toChucChinhSachCNTT.Success = (byte)TrangThaiNhapLieu.DaGui;
                        toChucChinhSachCNTT.LichNhap_ID = objLichNhap.LichNhap_ID; // Add LichNhap_ID vào báo cáo
                        db.ToChucChinhSachCNTT.Add(toChucChinhSachCNTT);
                        await db.SaveChangesAsync();

                        // Lưu file vào thư mục dữ liệu chính
                        if (TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] != null)
                        {
                            var listObjFile = (List<Ht_FileDinhKem>)TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()];

                            var pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
                            if (string.IsNullOrEmpty(pathSource)) return JavaScript("Không tìm thấy đường dẫn file");

                            var duongDan = pathSource + "/ToChucChinhSachCNTT/NewUpload/" + User.Identity.GetUserId();
                            var duongDanVatLy = pathSource + "/ToChucChinhSachCNTT";
                            string[] filesPath = System.IO.Directory.GetFiles(duongDan);

                            if (filesPath.Any())
                            {
                                foreach (var item in listObjFile)
                                {
                                    item.BaoCao_ID = toChucChinhSachCNTT.ToChucChinhSachCNTT_ID;
                                    item.PhanHeChucNang_ID = objChucNang.PhanHeChucNang_ID;
                                    item.DonVi_ID = toChucChinhSachCNTT.DonVi_ID;
                                    item.HieuLuc = TrangThai.HoatDong;

                                    var fileName = toChucChinhSachCNTT.ToChucChinhSachCNTT_ID + "_" + item.LoaiVanBan_ID + "_" + User.Identity.GetUserId() + "_" +  item.TenHienThi;
                                    var pathFile = duongDanVatLy + "/" + fileName;

                                    //copy file vào thư mục chính
                                    System.IO.File.Copy(item.DuongDan, pathFile, true);

                                    item.DuongDan = pathFile;
                                    item.TenFile = fileName;
                                    item.TrangThai = TrangThai.HoatDong;
                                    db.Ht_FileDinhKem.Add(item);
                                }
                            }
                            //await db.SaveChangesAsync();
                        }
                        else
                        {
                            //return JavaScript("Không tìm thấy file");
                        }

                        // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                        objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                        db.Entry(objLichNhap).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        // Xóa dữ liệu trong thư mục tạm
                        DeleteTempUploadFile();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        var exv = new ExceptionViewer(ex);
                        return Json(new { msg = exv.GetErrorMessage(ex.Message) });
                    }                 
                }

                return Content(mess);
            }

            return View(toChucChinhSachCNTT);
        }

        // GET: ToChucChinhSachCNTTs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            var mess = CheckReportStatus(user, TrangThaiNhapLieu.Sua);
            if (String.IsNullOrEmpty(mess))
            {
                GetViewBag(user.DonVi_ID);
                ToChucChinhSachCNTT toChucChinhSachCNTT = await db.ToChucChinhSachCNTT.FindAsync(id);

                // Lấy danh sách file đã đc up lên
                TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] = db.Ht_FileDinhKem.Where(o => o.PhanHeChucNang_ID == objChucNang.PhanHeChucNang_ID && o.BaoCao_ID == toChucChinhSachCNTT.ToChucChinhSachCNTT_ID).ToList();

                if (toChucChinhSachCNTT == null)
                {
                    return HttpNotFound();
                }
                return View(toChucChinhSachCNTT);
            }
            
            return Content(mess);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ToChucChinhSachCNTT toChucChinhSachCNTT)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (ModelState.IsValid)
            {
                var mess = CheckReportStatus(user, TrangThaiNhapLieu.Sua);
                if (String.IsNullOrEmpty(mess))
                {
                    using (var context = new S4T_HaTinhEntities())
                    {
                        toChucChinhSachCNTT.Success = (byte)TrangThaiNhapLieu.DaGui;
                        //haTangKyThuatCNTT.TruongNhapLai = string.Empty; // Xóa hết các yêu cầu nhập lại dữ liệu
                        context.Entry(toChucChinhSachCNTT).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                    // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                    var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.DonVi_ID == toChucChinhSachCNTT.DonVi_ID
                                                                          && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                          && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
                    objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                    db.Entry(objLichNhap).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                return Content(mess);
            }

            return View(toChucChinhSachCNTT);
        }

        /// <summary>
        /// Load danh sách file đính kèm trong báo cáo
        /// </summary>
        /// <param name="id">mã của báo cáo</param>
        public ActionResult LoadListFile(int? id)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) if (user == null) return RedirectToAction("Login", "Account");
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny)
            {
                // Kiểm tra thêm quyền là Thẩm định báo cáo ?
                per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.ChinhSach);
                if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));
            }

            var listReport = MvcApplication.ListReportMTCS();
            var str = new StringBuilder();
            var showInfoFile = string.Empty;
            try
            {
                if (listReport.Any())
                {
                    var list = new List<Ht_FileDinhKem>();

                    // Load lại list file đã đính kèm lên thư mục tạm của hệ thống
                    if (TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] != null)
                        list = (List<Ht_FileDinhKem>)TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()];
                    else
                        // Load list file đã đc lưu vào hệ thống (View: Edit, Details)
                        list = db.Ht_FileDinhKem.Where(o => o.BaoCao_ID == id && o.PhanHeChucNang_ID == objChucNang.PhanHeChucNang_ID).ToList();

                    TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] = list;

                    // Hiển thị 2 cột ẩn
                    showInfoFile = list.Any() ? "Show" : ""; 

                    foreach (var item in listReport)
                    {
                        str.Append("<tr>");
                        str.AppendFormat("<td> {0} </td>", item.TenDanhMuc);
                        var objFile = list.FirstOrDefault(o => o.LoaiVanBan_ID == item.DanhMuc_ID);
                        if (objFile != null)
                        {
                            str.AppendFormat("<td>{0}</td>",objFile.SoVanBan);
                            str.AppendFormat("<td><a href='/HT_FileDinhKem/Download?tenFile={0}&chucNang_ID={1}'>{2}</a></td>", objFile.TenFile, objFile.PhanHeChucNang_ID, objFile.TenHienThi);

                            /* Lưu nhiều file
                            str.Append("<td><ul style='list-style:none;display: table-cell'");
                            foreach (var file in list.Where(o => o.LoaiVanBan_ID == item.DanhMuc_ID))
                            {
                                str.AppendFormat("<li><a href='/HT_FileDinhKem/Download?tenFile={0}&chucNang_ID={1}'>{2}</a></li>", file.TenFile, file.PhanHeChucNang_ID, file.TenHienThi);
                            }
                            str.Append("</ul></td>");
                            */
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(showInfoFile))
                            {
                                str.Append("<td></td>");
                                str.Append("<td></td>");
                            }
                        }
                        str.AppendFormat("<td><a href='javascript:void(0);' class='opendialog' onclick='OpenDialog({0});' loaivanban='{0}'>Chọn file</a></td>", @item.DanhMuc_ID);
                        str.Append("</tr>");
                    }
                }

                return Json(new { danhSachFile = str.ToString(), ShowInfoFile = showInfoFile });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(ex.Message) });
            }             
        }

        public ActionResult Up(int loaiVanBan)
        {
            ViewBag.HideMenu = true;

            if (loaiVanBan == null)
            {
                return HttpNotFound();
            }

            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            Ht_FileDinhKem obj = new Ht_FileDinhKem();
            obj.LoaiVanBan_ID = loaiVanBan;

            return View(obj);
        }

        /// <summary>
        /// Upload file lên thư mục tạm của hệ thống
        /// </summary>
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "UploadFile")]
        public ActionResult Up(Ht_FileDinhKem objFile)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = "/ToChucChinhSachCNTTs/Index" });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));
            
            // Kiểm tra đã submit file lên
            if (Request.Files.Count > 0)
            {
                var pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
                if (string.IsNullOrEmpty(pathSource)) return JavaScript("Không tìm thấy đường dẫn file");

                var duongDan = pathSource + "/ToChucChinhSachCNTT/NewUpload/" + User.Identity.GetUserId();
                if (!Directory.Exists(duongDan))
                    Directory.CreateDirectory(duongDan);

                var list = new List<Ht_FileDinhKem>();
                if (TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] != null)
                    list = (List<Ht_FileDinhKem>)TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()];

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);

                    // Lưu file vào thư mục tạm của hệ thống
                    var path = Path.Combine(duongDan, fileName);
                    file.SaveAs(path);

                    #region Lưu đường dẫn, thông tin file vào TempData
                    var _file = new Ht_FileDinhKem
                    {
                        LoaiVanBan_ID = objFile.LoaiVanBan_ID,
                        SoVanBan = objFile.SoVanBan,
                        NgayBanHanh = objFile.NgayBanHanh,
                        PhanHeChucNang_ID = objChucNang.PhanHeChucNang_ID,
                        DuongDan = path,
                        HieuLuc = TrangThai.HoatDong,
                        TenHienThi = fileName,
                        TenFile = objFile.LoaiVanBan_ID + "_" + User.Identity.GetUserId() + "_" + fileName,
                        TrangThai = TrangThai.HoatDong
                    };

                    var fileOld = list.FirstOrDefault(o => o.LoaiVanBan_ID == objFile.LoaiVanBan_ID);
                    
                    //Xóa file cũ trong list
                    list.Remove(fileOld);

                    //Add file mới vào thay
                    list.Add(_file); 
                    #endregion Lưu đường dẫn, thông tin file vào TempData
                }
                TempData[objChucNang.ControllerName + "_" + User.Identity.GetUserId()] = list;
            }

            // redirect back to the index action to show the form once again
            ViewBag.Status = "status=\"OK\";";
            return View();
        }

        /// <summary>
        /// Xóa thư mục lưu trữ file tạm
        /// </summary>
        private bool DeleteTempUploadFile()
        {
            try
            {
                var pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
                if (string.IsNullOrEmpty(pathSource)) return false;
                var duongDan = pathSource + "/ToChucChinhSachCNTT/NewUpload/" + User.Identity.GetUserId();
                if (!Directory.Exists(duongDan))
                    return true;
                
                // Xóa thư mục trong folder
                string[] folderPath = System.IO.Directory.GetDirectories(duongDan);
                if (folderPath.Length > 0)
                {
                    foreach (string file in folderPath)
                    {
                        if (!string.IsNullOrEmpty(file))
                            Directory.Delete(file, true);
                    }
                }

                // Xóa file đã đính kèm trước đó đi trong thư mục
                string[] filesPath = System.IO.Directory.GetFiles(duongDan);
                if (filesPath.Length > 0)
                {
                    foreach (string file in filesPath)
                    {
                        if (!string.IsNullOrEmpty(file))
                            System.IO.File.Delete(file);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return false;
            }
        }

        #region Event
        /// <summary>
        /// Lấy danh sách đơn vị cấp trên theo Nhóm đơn vị
        /// </summary>
        /// <param name="nhomDonVi_ID">ID của nhóm đơn vị</param>
        public ActionResult NhomDonVi_OnChange(int nhomDonVi_ID)
        {
            try
            {
                var list = db.Dm_DonVi.Where(o => o.NhomDonVi_ID == nhomDonVi_ID);
                var str = new StringBuilder();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        str.AppendFormat("<option value='{0}' >{1}</option>", item.DonVi_ID, item.TenDonVi);
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
        /// Lọc danh sách các báo cáo theo đơn vị
        /// </summary>
        /// <param name="donVi_ID">Mã đơn vị</param>
        public ActionResult GetReportByDonVi(int donVi_ID)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            try
            {
                var listLichNhap = db.Ht_LichNhapLieu.Where(o => o.DonVi_ID == donVi_ID && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID);

                if (listLichNhap.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var item in listLichNhap)
                    {
                        var objDanhMuc = MvcApplication.ListTrangThaiNhapLieu().FirstOrDefault(o => o.DanhMuc_ID == item.ChucNang_ID);
                        sb.Append("<tr>");
                        sb.AppendFormat("<td>{0}</td>", "Tổ chức chính sách");
                        sb.AppendFormat("<td>{0}</td>", item.TuNgay.ToShortDateString());
                        sb.AppendFormat("<td>{0}</td>", item.DenNgay.ToShortDateString());
                        sb.AppendFormat("<td>{0}</td>", objDanhMuc == null ? "" : objDanhMuc.TenDanhMuc);

                        // Trạng thái: Chờ duyệt
                        //if (item.ChucNang_ID == TrangThaiNhapLieu.DaGui)
                        //{
                        //    // Duyệt báo cáo
                        //    sb.AppendFormat("<td><a href='/{0}/Check?donVi_ID={1}&lichNhap_ID={2}'>Kiểm tra</a></td>", objChucNang.ControllerName, item.DonVi_ID, item.LichNhap_ID);
                        //}
                        //else if (item.ChucNang_ID == TrangThaiNhapLieu.PheDuyet || item.ChucNang_ID == TrangThaiNhapLieu.Sua)
                        //{
                        //    // Xem báo cáo
                        //    sb.AppendFormat("<td><a href='/{0}/Details?donVi_ID={1}&lichNhap_ID={2}'>Xem</a></td>", objChucNang.ControllerName, item.DonVi_ID, item.LichNhap_ID);
                        //}
                        sb.AppendFormat("<td><a href='/{0}/Details?donVi_ID={1}&lichNhap_ID={2}'>Xem</a></td>", objChucNang.ControllerName, item.DonVi_ID, item.LichNhap_ID);
                        sb.Append("</tr>");
                    }
                    return Json(new { danhSach = sb.ToString() });
                }
                else
                {
                    return Json(new { danhSach = "Không có dữ liệu" });
                }
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }
        #endregion

        //[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        //public class MultipleButtonAttribute : ActionNameSelectorAttribute
        //{
        //    public string Name { get; set; }
        //    public string Argument { get; set; }

        //    public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        //    {
        //        var isValidName = false;
        //        var keyValue = string.Format("{0}:{1}", Name, Argument);
        //        var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

        //        if (value != null)
        //        {
        //            controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
        //            isValidName = true;
        //        }

        //        return isValidName;
        //    }
        //}

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
