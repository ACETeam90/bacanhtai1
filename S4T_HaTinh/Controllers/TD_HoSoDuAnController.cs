using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Configuration;
using System.Transactions;
using System.Data.Entity.Validation;
using S4T_HaTinh.Common;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Controllers
{
    public class TD_HoSoDuAnController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();
        private string pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
        private string phuLucHoSoKhac = ConfigurationManager.AppSettings["PhuLucHoSoKhac"];

        /// <summary>
        /// Kiểm tra quyền truy cập vào chức năng và quyền sửa
        /// </summary>
        private Permission CheckPermission(ApplicationUser user)
        {
            var per = new Permission();
            var listQuyenXuLy = db.Dm_CapXuLy.Where(o => o.NguoiXuLy.Contains(user.UserName));
            if (listQuyenXuLy.Any())
            {
                per.isView = true;

                // Neu co vai tro cap nhat ho so
                if (listQuyenXuLy.Any(o => o.ThuTu == STTCapXuLy.CapNhatHoSo))
                    per.isEdit = true;

                // Nếu có vai trò phê duyệt hồ sơ : Giám đốc
                var objThamSoSep = db.Ht_ThamSoHeThong.FirstOrDefault(o => o.MaThamSo == "GiamDoc_ID");
                if (objThamSoSep.GiaTriThamSo.Contains(user.Id))
                    per.isSuccess = true;
            }
            return per;
        }

        private void GetViewBag(ViewReport typeView)
        {
            // List Hình thức quản lý
            var items = new List<SelectListItem>();
            var listHinhThucQuanLy = typeView == ViewReport.Index ? S4T_HaTinhBase.ListHinhThucQuanLy(null) : S4T_HaTinhBase.ListHinhThucQuanLy(TrangThai.HoatDong);
            if (!listHinhThucQuanLy.Any())
                items.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
            else
            {
                var listItem = listHinhThucQuanLy.Select(o => new SelectListItem()
                {
                    Text = o.TenDanhMuc,
                    Value = o.DanhMuc_ID.ToString()
                });
                items.AddRange(listItem);
            }
            ViewBag.HinhThucQuanLy = items;

            // List Nhóm dự án
            items = new List<SelectListItem>();
            var listNhomDuAn = typeView == ViewReport.Index ? S4T_HaTinhBase.ListNhomDuAn(null) : S4T_HaTinhBase.ListNhomDuAn(TrangThai.HoatDong);
            if (!listNhomDuAn.Any())
                items.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
            else
            {
                var listItem = listNhomDuAn.Select(o => new SelectListItem()
                {
                    Text = o.TenDanhMuc,
                    Value = o.DanhMuc_ID.ToString()
                });
                items.AddRange(listItem);
            }
            ViewBag.NhomDuAn = items;

            // List Tính chất dự án
            items = new List<SelectListItem>();
            var listTinhChatDuAn = typeView == ViewReport.Index ? S4T_HaTinhBase.ListTinhChatDuAn(null) : S4T_HaTinhBase.ListTinhChatDuAn(TrangThai.HoatDong);
            if (!listTinhChatDuAn.Any())
                items.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
            else
            {
                var listItem = listTinhChatDuAn.Select(o => new SelectListItem()
                {
                    Text = o.TenDanhMuc,
                    Value = o.DanhMuc_ID.ToString()
                });
                items.AddRange(listItem);
            }
            ViewBag.TinhChatDuAn = items;
        }

        #region File đính kèm
        public ActionResult SaveFile(IEnumerable<HttpPostedFileBase> files,int id)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path. We are only interested in the file name.
                    string fileName = Path.GetFileName(file.FileName);
                    
                    // Lưu đường dẫn file theo mã hồ sơ
                    string duongDan = pathSource + "/TD_HoSoDuAn/Upload/" + User.Identity.GetUserId();
                    if (!Directory.Exists(duongDan)) Directory.CreateDirectory(duongDan);
                    //string fileName = obj.MaHoSo + "_" + Guid.NewGuid().ToString() + "_" + User.Identity.GetUserId() + "_" + Path.GetFileName(file.FileName);

                    // Lưu file vào thư mục tạm của hệ thống
                    string path = Path.Combine(duongDan, fileName);
                    file.SaveAs(path);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames, int id)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    string fileName = Path.GetFileName(fullName);
                    
                    // Lưu đường dẫn file theo mã hồ sơ
                    string duongDan = pathSource + "/TD_HoSoDuAn/Upload/" + User.Identity.GetUserId();
                    if (!Directory.Exists(duongDan)) return Content("");
                    var physicalPath = Path.Combine(duongDan, fileName);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult _ListFile(List<TD_FileHoSoDinhKem> list)
        {
            ViewBag.HideMenu = true;

            // Bỏ 3 file Văn bản quyết định sau khi display để hiện chỉ các văn bản Phụ lục bên dưới
            var listVBQD = GetListVBQuyetDinhPheDuyet();
            var listVanBan_ID = listVBQD.Select(o => o.VanBan_ID);
            list = list.Where(o => !listVanBan_ID.Contains(o.VanBan_ID)).ToList();

            return PartialView("_ListFile", list);
        }

        public ActionResult _CreateOrUpdateFile(int? id, string tenFile, string maHoSo, int? vanBan_ID)
        {
            ViewBag.HideMenu = true;
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            TD_FileHoSoDinhKem obj = new TD_FileHoSoDinhKem
            {
                MaHoSo = maHoSo,
                VanBan_ID = (vanBan_ID == null || vanBan_ID == 0) ? 0 : vanBan_ID.Value
            };
            if ((id != null && id != 0) || !string.IsNullOrEmpty(tenFile))
            {
                List<TD_FileHoSoDinhKem> list = (List<TD_FileHoSoDinhKem>)TempData["TD_FileHoSoDinhKem_" + user.Id];
                TempData["TD_FileHoSoDinhKem_" + user.Id] = list;
                obj = list.FirstOrDefault(o => o.FileHoSoDinhKem_ID == id);
                if (obj == null)
                {
                    obj = list.FirstOrDefault(o => o.TenFile == tenFile);
                    if (obj == null)
                        return HttpNotFound();
                }
            }
            else
            {
                int phuLuc = string.IsNullOrEmpty(phuLucHoSoKhac) ? 0 : Convert.ToInt32(phuLucHoSoKhac);
                if (obj.VanBan_ID != phuLuc && string.IsNullOrEmpty(obj.TrichYeu))
                {
                    List<View_Loai_Nhom_VanBan> list = GetListVBQuyetDinhPheDuyet();
                    var objVB = list.FirstOrDefault(o => o.VanBan_ID == obj.VanBan_ID);
                    obj.TrichYeu = objVB == null ? "" : objVB.TenVanBan;
                }
            }

            return View(obj);
        }

        private List<View_Loai_Nhom_VanBan> GetListVBQuyetDinhPheDuyet()
        {
            List<View_Loai_Nhom_VanBan> list = new List<View_Loai_Nhom_VanBan>();
            if (Session["VBQuyetDinhPheDuyet"] == null)
            {
                list = db.View_Loai_Nhom_VanBan.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaNhomVanBan == "QuyetDinhPheDuyet").ToList();
                if(!list.Any())
                    return new List<View_Loai_Nhom_VanBan>();

                Session["VBQuyetDinhPheDuyet"] = list;
            }
            else
                list = (List<View_Loai_Nhom_VanBan>)Session["VBQuyetDinhPheDuyet"];

            return list;
        }

        [HttpPost]
        public ActionResult _CreateOrUpdateFile(TD_FileHoSoDinhKem obj)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            List<TD_FileHoSoDinhKem> list = (List<TD_FileHoSoDinhKem>)TempData["TD_FileHoSoDinhKem_" + user.Id];
            if (list == null) list = new List<TD_FileHoSoDinhKem>();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file.ContentLength > 0)
                        {
                            var maxSize = 16384 * 1024; // in Byte

                            // Kiểm tra giới hạn dung lượng file upload cho phép
                            if (file.ContentLength > maxSize)
                            {
                                ModelState.AddModelError("", "Dung lượng file vượt quá dung lượng cho phép tải lên");
                                return View(obj);
                            }

                            if (string.IsNullOrEmpty(pathSource)) return Content("Không tìm thấy đường dẫn file");

                            // Xóa file tạm trong db
                            if (!string.IsNullOrEmpty(obj.TenFile))
                            {
                                TD_FileHoSoDinhKem objRemove = list.FirstOrDefault(o => o.TenFile == obj.TenFile);
                                list.Remove(objRemove);
                                System.IO.File.Delete(objRemove.DuongDan);
                            }

                            // Lưu đường dẫn file theo mã hồ sơ
                            string duongDan = pathSource + "/TD_HoSoDuAn/Upload/" + User.Identity.GetUserId();
                            if (!Directory.Exists(duongDan)) Directory.CreateDirectory(duongDan);
                            string fileName = obj.MaHoSo + "_" + Guid.NewGuid().ToString() + "_" + User.Identity.GetUserId() + "_" + Path.GetFileName(file.FileName);

                            // Lưu file vào thư mục tạm của hệ thống
                            string path = Path.Combine(duongDan, fileName);
                            file.SaveAs(path);

                            #region Lưu đường dẫn, thông tin file vào db
                            obj.DuongDan = path;
                            obj.TenFile = fileName;
                            obj.TenHienThi = Path.GetFileName(file.FileName);
                            obj.TrangThai = TrangThai.HoatDong;
                            
                            if (obj.VanBan_ID == 0)
                            {
                                ViewBag.btnId = "btnRefreshListFile";
                                string phuLucHoSoKhac = ConfigurationManager.AppSettings["PhuLucHoSoKhac"];
                                obj.VanBan_ID = string.IsNullOrEmpty(phuLucHoSoKhac) ? 0 : Convert.ToInt32(phuLucHoSoKhac);
                            }
                            else
                            {
                                ViewBag.btnId = "btnRefreshListFileQuyetDinh";
                            }

                            list.Add(obj);

                            TempData["TD_FileHoSoDinhKem_" + user.Id] = list;

                            scope.Complete();
                            #endregion Lưu đường dẫn, thông tin file vào db
                        }
                        else
                        {
                            ViewBag.Mess = "<span style=\"color:red;\">Chưa chọn file</span>";
                            return View(obj);
                        }
                    }
                    else
                    {
                        ViewBag.Mess = "<span style=\"color:red;\">Chưa chọn file</span>";
                        return View(obj);
                    }

                    // Close popup và refresh form main
                    ViewBag.RefreshPage = true;
                }
                catch (DbEntityValidationException ex)
                {
                    var exc = new ExceptionViewer();
                    ViewBag.Mess = exc.GetError(ex);
                    scope.Dispose();
                }
            }

            return View(obj);
        }

        public ActionResult RefreshFileQuyetDinh(string maHoSo)
        {
            var list = (List<TD_FileHoSoDinhKem>)TempData["TD_FileHoSoDinhKem_" + S4T_HaTinhBase.GetUserSession().Id];
            TempData["TD_FileHoSoDinhKem_" + S4T_HaTinhBase.GetUserSession().Id] = list;
            int intPhuLucHoSoKhac = string.IsNullOrEmpty(phuLucHoSoKhac) ? 0 : Convert.ToInt32(phuLucHoSoKhac);
            list = list.Where(o => o.VanBan_ID != intPhuLucHoSoKhac).ToList();

            var listVBQD = GetListVBQuyetDinhPheDuyet();
            if (listVBQD.Any() && list.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var temp in listVBQD)
                {
                    var item = list.FirstOrDefault(o => o.VanBan_ID == temp.VanBan_ID);
                    if (item != null)
                    {
                        string link = Url.Action("_CreateOrUpdateFile", "TD_HoSoDuAn", new { id = item.FileHoSoDinhKem_ID, tenFile = item.TenFile, vanBan_ID = item.VanBan_ID });
                        sb.Append("<tr>");
                        sb.AppendFormat("<td>{0}</td>", item.TrichYeu);
                        sb.AppendFormat("<td>{0}</td>", item.SoKyHieu);
                        if (item.FileHoSoDinhKem_ID == 0)
                        {
                            //sb.AppendFormat("<td>{0}</td>", item.TenHienThi);
                            sb.AppendFormat("<td>{0}</td>", item.TenHienThi);
                            sb.AppendFormat("<td><button type='button' class='k-button k-button-icontext k-grid-edit' onclick=\"javascript:OpenWindow('{0}', 543, 230, true); return false;\"><span class='k-icon k-edit'></span>Chọn file khác</button></td>", link);
                        }
                        else
                        {
                            sb.AppendFormat("<td><a href=\"/TD_HoSoDuAn/Download?id={0}&prefixName=TD_HoSoDuAn\">{1}</a></td>", item.FileHoSoDinhKem_ID, item.TenHienThi);
                            sb.AppendFormat("<td></td>");
                        }

                        sb.Append("</tr>");
                    }
                    else
                    {
                        string link = Url.Action("_CreateOrUpdateFile", "TD_HoSoDuAn", new { maHoSo = maHoSo, vanBan_ID = temp.VanBan_ID });
                        sb.Append("<tr>");
                        sb.AppendFormat("<td>{0}</td>",temp.TenVanBan);
                        sb.AppendFormat("<td>{0}</td>", temp.SoKyHieu);
                        sb.Append("<td>(Không có file)</td>");
                        sb.AppendFormat("<td><button type='button' class='k-button k-button-icontext k-grid-edit' onclick=\"javascript:OpenWindow('{0}', 543, 230, true); return false;\"><span class='k-icon k-edit'></span>Chọn file</button></td>", link);
                    }
                }
                    
                return Json(new { danhSach = sb.ToString() });
            }
            return Json(new { msg = "Không tìm thấy danh sách file đính kèm" });
        }

        public ActionResult RefreshListFile()
        {
            var list = (List<TD_FileHoSoDinhKem>)TempData["TD_FileHoSoDinhKem_" + S4T_HaTinhBase.GetUserSession().Id];
            TempData["TD_FileHoSoDinhKem_" + S4T_HaTinhBase.GetUserSession().Id] = list;
            var listVBQD = GetListVBQuyetDinhPheDuyet();
            var listVanBan_ID = listVBQD.Select(o => o.VanBan_ID);
            list = list.Where(o => !listVanBan_ID.Contains(o.VanBan_ID)).ToList();

            if (list.Any())
            {
                StringBuilder sb = new StringBuilder();
                int i = 1;
                foreach (TD_FileHoSoDinhKem item in list)
                {
                    string link = Url.Action("_CreateOrUpdateFile", "TD_HoSoDuAn", new { id = item.FileHoSoDinhKem_ID, tenFile = item.TenFile });
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", i);
                    //sb.AppendFormat("<td>{0}</td>", item.TenVanBan);
                    //sb.AppendFormat("<td>{0}</td>", item.SoKyHieu);
                    //sb.AppendFormat("<td>{0}</td>", item.TrichYeu);
                    if (item.FileHoSoDinhKem_ID == 0)
                    {
                        sb.AppendFormat("<td>{0}</td>",item.TenHienThi);
                        sb.AppendFormat("<td><button type='button' class='k-button k-button-icontext k-grid-edit' onclick=\"javascript:OpenWindow('{0}', 626, 275, true); return false;\"><span class='k-icon k-edit'></span>Sửa</button></td>", link);
                    }
                    else
                    {
                        sb.AppendFormat("<td><a href=\"/TD_HoSoDuAn/Download?id={0}&prefixName=TD_HoSoDuAn\">{1}</a></td>", item.FileHoSoDinhKem_ID, item.TenHienThi);
                        sb.AppendFormat("<td><a href=\"/TD_HoSoDuAn/Download?id={0}&prefixName=TD_HoSoDuAn\">Tải về</a></td>", item.FileHoSoDinhKem_ID);
                    }

                    sb.Append("</tr>");
                    i++;
                }
                return Json(new { danhSach = sb.ToString() });
            }
            return Json(new { msg = "Không tìm thấy danh sách file đính kèm" });
        }
        #endregion File đính kèm

        #region Thông tin chung
        public ActionResult ThongTinChung(string maHoSo, string tenDuAn, string tongMucDauTu, int? ddltinhChatDuAn)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (per.isView)
            {
                if (per.isEdit)
                    ViewBag.CapNhatHoSo = "Yes";
                GetViewBag(ViewReport.Index);

                // chỉ lấy các hồ sơ có Cap_ID = 5 (Phê duyệt) lý (Max STT = 1)
                if (String.IsNullOrEmpty(maHoSo) && String.IsNullOrEmpty(tenDuAn) && String.IsNullOrEmpty(tongMucDauTu))
                {
                    var list = db.sp_HoSoDuAnWithLuongXuLy(null, null, null, null, null, null, TrangThai.HoatDong).Where(o => o.Cap_ID == STTCapXuLy.DaPheDuyet);
                    if (ddltinhChatDuAn != null)
                        list = list.Where(o => o.TinhChatDuAn_ID == ddltinhChatDuAn);
                    return View(list);
                }

                else
                {
                    var list = db.sp_HoSoDuAnWithLuongXuLy(maHoSo.Trim(), tenDuAn.Trim(), tongMucDauTu.Trim(), null, null, null, TrangThai.HoatDong).Where(o => o.Cap_ID == STTCapXuLy.DaPheDuyet);
                    if (ddltinhChatDuAn != null)
                        list = list.Where(o => o.TinhChatDuAn_ID == ddltinhChatDuAn);
                    return View(list);
                }
            }
            else
                return View();
        }

        public ActionResult UpdateThongTinChung(int id)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isEdit)
                return Content("Bạn không có quyền thực hiện chức năng này");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_HoSoDuAn tD_HoSoDuAn = db.TD_HoSoDuAn.Find(id);
            if (tD_HoSoDuAn == null)
            {
                return HttpNotFound();
            }
            GetViewBag(ViewReport.Edit);

            // Lấy các file đã upload lên server
            //string strPhuLucHoSoKhac = ConfigurationManager.AppSettings["PhuLucHoSoKhac"];
            //int intPhuLucHoSoKhac = string.IsNullOrEmpty(strPhuLucHoSoKhac) ? 0 : Convert.ToInt32(strPhuLucHoSoKhac);
            var listFile = db.TD_FileHoSoDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == tD_HoSoDuAn.MaHoSo);
            if (listFile.Any())
                tD_HoSoDuAn.ListFile = listFile.ToList();
            else
                tD_HoSoDuAn.ListFile = new List<TD_FileHoSoDinhKem>();

            //Lưu temp data
            TempData["TD_FileHoSoDinhKem_" + user.Id] = tD_HoSoDuAn.ListFile;

            ViewBag.TenDuAn = tD_HoSoDuAn.TenDuAn;
            ViewBag.ChuDauTu = tD_HoSoDuAn.ChuDauTu;
            var listVBQuyetDinh = db.View_Loai_Nhom_VanBan.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaNhomVanBan == "QuyetDinhPheDuyet").ToList();
            Session["VBQuyetDinhPheDuyet"] = listVBQuyetDinh;
            ViewBag.ListVBQuyetDinh = listVBQuyetDinh.Any() ? listVBQuyetDinh.ToList() : new List<View_Loai_Nhom_VanBan>();

            return View(tD_HoSoDuAn);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdateThongTinChung(TD_HoSoDuAn tD_HoSoDuAn, List<TD_FileHoSoDinhKem> listFile, HttpPostedFileBase upload_0, HttpPostedFileBase upload_1, HttpPostedFileBase upload_2)
        public ActionResult UpdateThongTinChung(TD_HoSoDuAn tD_HoSoDuAn, List<TD_FileHoSoDinhKem> listFile, List<HttpPostedFileBase> listFileUp)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isEdit)
                return Content("Bạn không có quyền thực hiện chức năng này");

            var obj = db.TD_HoSoDuAn.FirstOrDefault(o => o.HoSoDuAn_ID == tD_HoSoDuAn.HoSoDuAn_ID);
            ModelState.Remove("TenDuAn");
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        // Mapping obj in db
                        obj.str1 = tD_HoSoDuAn.str1;
                        obj.NhaThau = tD_HoSoDuAn.NhaThau;
                        obj.MucTieu = obj.MucTieu;
                        obj.QuyMo = tD_HoSoDuAn.QuyMo;
                        obj.DiaDiem = tD_HoSoDuAn.DiaDiem;
                        obj.NguonVon = tD_HoSoDuAn.NguonVon;
                        obj.HinhThucQuanLy_ID = tD_HoSoDuAn.HinhThucQuanLy_ID;
                        obj.HinhThucQuanLyKhac = tD_HoSoDuAn.HinhThucQuanLyKhac;
                        obj.int1 = tD_HoSoDuAn.int1;
                        obj.int2 = tD_HoSoDuAn.int2;
                        obj.QuyTrinhQuanLy = tD_HoSoDuAn.QuyTrinhQuanLy;
                        obj.NhomDuAn_ID = tD_HoSoDuAn.NhomDuAn_ID;
                        obj.LoaiDuAn_ID = tD_HoSoDuAn.LoaiDuAn_ID;
                        obj.TinhChatDuAn_ID = tD_HoSoDuAn.TinhChatDuAn_ID;
                        obj.TinhTrangThucHien = tD_HoSoDuAn.TinhTrangThucHien;
                        obj.TongMucDauTu = tD_HoSoDuAn.TongMucDauTu;
                        obj.ChiPhiXayLap = tD_HoSoDuAn.ChiPhiXayLap;
                        obj.ChiPhiThietBi = tD_HoSoDuAn.ChiPhiThietBi;
                        obj.ChiPhiQuanLy = tD_HoSoDuAn.ChiPhiQuanLy;
                        obj.ChiPhiTuVan = tD_HoSoDuAn.ChiPhiTuVan;
                        obj.ChiPhiKhac = tD_HoSoDuAn.ChiPhiKhac;
                        obj.ChiPhiDuPhong = tD_HoSoDuAn.ChiPhiDuPhong;
                        obj.NganSachTrungUong = tD_HoSoDuAn.NganSachTrungUong;
                        obj.NganSachTinh = tD_HoSoDuAn.NganSachTinh;
                        obj.NganSachDonVi = tD_HoSoDuAn.NganSachDonVi;
                        obj.NguonKhac = tD_HoSoDuAn.NguonKhac;
                        obj.HangMucTrienKhai = tD_HoSoDuAn.HangMucTrienKhai;
                        obj.ThoiGianHoanThanh = tD_HoSoDuAn.ThoiGianHoanThanh;
                        obj.SanPhamHangMuc = tD_HoSoDuAn.SanPhamHangMuc;
                        obj.KinhPhiThucHienHangMuc = tD_HoSoDuAn.KinhPhiThucHienHangMuc;
                        obj.KetQuaDauTu = tD_HoSoDuAn.KetQuaDauTu;

                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                        // Lưu file vào hệ thống
                        List<TD_FileHoSoDinhKem> list = (List<TD_FileHoSoDinhKem>)TempData["TD_FileHoSoDinhKem_" + user.Id];
                        if (list.Any())
                        {
                            TempData["TD_FileHoSoDinhKem_" + user.Id] = list;
                            string duongDanVatLy = pathSource + "/TD_HoSoDuAn";
                            string phuLucHoSoKhac = ConfigurationManager.AppSettings["PhuLucHoSoKhac"];
                            int intPhuLucHoSoKhac = string.IsNullOrEmpty(phuLucHoSoKhac) ? 0 : Convert.ToInt32(phuLucHoSoKhac);
                            if (!Directory.Exists(duongDanVatLy))
                                Directory.CreateDirectory(duongDanVatLy);

                            var listFileInDb = db.TD_FileHoSoDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == obj.MaHoSo && o.VanBan_ID == intPhuLucHoSoKhac);

                            for (var i = 0; i < list.Count; i++)
                            {
                                // Insert
                                if (list[i].FileHoSoDinhKem_ID == 0)
                                {
                                    string path = Path.Combine(duongDanVatLy, list[i].TenFile);

                                    // To copy a file to another location and 
                                    // overwrite the destination file if it already exists.
                                    System.IO.File.Copy(list[i].DuongDan, path, true);

                                    // đổi đường dẫn file
                                    list[i].DuongDan = path;

                                    // Lưu obj file vào db
                                    db.TD_FileHoSoDinhKem.Add(list[i]);
                                    db.SaveChanges();
                                }
                                //Update
                                else
                                {

                                }
                            }
                        }

                        // Lưu các file quyết định phê duyệt (nếu có đính kèm)
                        if(Request.Files.Count > 0)
                        {
                            if (string.IsNullOrEmpty(pathSource)) return Content("Không tìm thấy đường dẫn file");

                            //var duongDan = pathSource + "/TD_HoSoDuAn/NewUpload/" + User.Identity.GetUserId();
                            //if (!Directory.Exists(duongDan))
                            //    Directory.CreateDirectory(duongDan);

                            var duongDanVatLy = pathSource + "/TD_HoSoDuAn";
                            if (!Directory.Exists(duongDanVatLy))
                                Directory.CreateDirectory(duongDanVatLy);
                            
                            var indexFile = 0;
                            var stt = db.TD_FileHoSoDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == tD_HoSoDuAn.MaHoSo).Max(o => o.STT);
                            if (stt == null || stt == 0)
                                stt = 1;

                            for (int i = 0; i < Request.Files.Count; i++)
                            {
                                var file = Request.Files[i];
                                if (file.ContentLength > 0)
                                {
                                    // Tạo tên file lưu trên hệ thống
                                    var fileName = tD_HoSoDuAn.HoSoDuAn_ID + "_" + tD_HoSoDuAn.LoaiDuAn_ID + "_" + Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);

                                    // Lưu file lên hệ thống
                                    var path = Path.Combine(duongDanVatLy, fileName);
                                    file.SaveAs(path);

                                    // Tạo object TD_FileHoSoDinhKem                                    
                                    var objFileDinhKem = new TD_FileHoSoDinhKem
                                    {
                                        DuongDan = path,
                                        SoKyHieu = listFile[indexFile].SoKyHieu,
                                        MaHoSo = tD_HoSoDuAn.MaHoSo,
                                        STT = stt,
                                        TenFile = fileName,
                                        TenHienThi = Path.GetFileName(file.FileName),
                                        TrichYeu = listFile[indexFile].TrichYeu,
                                        VanBan_ID = listFile[indexFile].VanBan_ID,
                                        TrangThai = TrangThai.HoatDong
                                    };

                                    // Lưu thông tin file vào db
                                    db.TD_FileHoSoDinhKem.Add(objFileDinhKem);
                                    stt++; // Cộng số thứ tự file thêm 1 nếu đã lưu 1 file thành công
                                }
                                indexFile++;
                            }
                        }

                        db.SaveChanges();
                        scope.Complete();

                        // Xóa file đã đính kèm trước đó đi trong thư mục
                        if (Directory.Exists(pathSource + "/TD_HoSoDuAn/Upload/" + User.Identity.GetUserId()))
                        {
                            string[] filesPath = Directory.GetFiles(pathSource + "/TD_HoSoDuAn/Upload/" + User.Identity.GetUserId());
                            if (filesPath.Length > 0)
                            {
                                foreach (string item in filesPath)
                                {
                                    if (!string.IsNullOrEmpty(item))
                                        System.IO.File.Delete(item);
                                }
                            }
                        }

                        return RedirectToAction("ThongTinChung");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        scope.Dispose();
                        var exc = new ExceptionViewer();
                        exc.GetError(ex);
                    }
                }
            }
            GetModelStateError(ViewData);
            ViewBag.TenDuAn = obj.TenDuAn;
            ViewBag.ChuDauTu = obj.ChuDauTu;
            var listVBQuyetDinh = db.View_Loai_Nhom_VanBan.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaNhomVanBan == "QuyetDinhPheDuyet");
            ViewBag.ListVBQuyetDinh = listVBQuyetDinh.Any() ? listVBQuyetDinh.ToList() : new List<View_Loai_Nhom_VanBan>();
            var _listFile = db.TD_FileHoSoDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == tD_HoSoDuAn.MaHoSo);
            if (_listFile.Any())
                tD_HoSoDuAn.ListFile = _listFile.ToList();
            else
                tD_HoSoDuAn.ListFile = new List<TD_FileHoSoDinhKem>();
            GetViewBag(ViewReport.Edit);
            return View(tD_HoSoDuAn);
        }
        #endregion Thông tin chung

        private void GetModelStateError(ViewDataDictionary viewData)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    sb.AppendFormat("<span>{0}</span>", error.ErrorMessage);
                }
            }
            ViewBag.Mess = sb.ToString();
        }

        #region Tình trạng dự án
        public ActionResult TinhTrangDuAn(string maHoSo, string tenDuAn, string tongMucDauTu, int? ddltinhChatDuAn)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            
            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (per.isView)
            {
                if (per.isEdit)
                    ViewBag.CapNhatHoSo = "Yes";
                GetViewBag(ViewReport.Index);

                // chỉ lấy các hồ sơ có Cap_ID = 5 (Phê duyệt) lý (Max STT = 1)
                if (String.IsNullOrEmpty(maHoSo) && String.IsNullOrEmpty(tenDuAn) && String.IsNullOrEmpty(tongMucDauTu))
                {
                    var list = db.sp_HoSoDuAnWithLuongXuLy(null, null, null, null, null, null, TrangThai.HoatDong).Where(o => o.Cap_ID == STTCapXuLy.DaPheDuyet);
                    if (ddltinhChatDuAn != null)
                        list = list.Where(o => o.TinhChatDuAn_ID == ddltinhChatDuAn);
                    return View(list);
                }

                else
                {
                    var list = db.sp_HoSoDuAnWithLuongXuLy(maHoSo.Trim(), tenDuAn.Trim(), tongMucDauTu.Trim(), null, null, null, TrangThai.HoatDong).Where(o => o.Cap_ID == STTCapXuLy.DaPheDuyet);
                    if (ddltinhChatDuAn != null)
                        list = list.Where(o => o.TinhChatDuAn_ID == ddltinhChatDuAn);
                    return View(list);
                }
            }
            else
                return View();
        }

        public ActionResult UpdateTinhTrangDuAn(int id)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isEdit)
                return Content("Bạn không có quyền thực hiện chức năng này");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_HoSoDuAn tD_HoSoDuAn = db.TD_HoSoDuAn.Find(id);
            if (tD_HoSoDuAn == null)
            {
                return HttpNotFound();
            }
            GetViewBag(ViewReport.Edit);

            return View(tD_HoSoDuAn);
        }

        [HttpPost]
        public ActionResult UpdateTinhTrangDuAn(TD_HoSoDuAn tD_HoSoDuAn)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isEdit)
                return Content("Bạn không có quyền thực hiện chức năng này");

            var obj = db.TD_HoSoDuAn.FirstOrDefault(o => o.HoSoDuAn_ID == tD_HoSoDuAn.HoSoDuAn_ID);
            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Mapping obj in db
                        obj.TinhTrangThucHien = tD_HoSoDuAn.TinhTrangThucHien;
                        obj.str2 = tD_HoSoDuAn.str2;
                        obj.str3 = tD_HoSoDuAn.str3;

                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                        dbContextTransaction.Commit();
                        return RedirectToAction("TinhTrangDuAn");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbContextTransaction.Rollback();
                        var exc = new ExceptionViewer();
                        exc.GetError(ex);
                    }
                }
            }
            GetViewBag(ViewReport.Edit);
            GetModelStateError(ViewData);
            return View(tD_HoSoDuAn);
        }
        #endregion Tình trạng dự án

        // GET: TD_HoSoDuAn
        public ActionResult Index(string maHoSo, string tenDuAn, string tongMucDauTu)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (per.isView)
            {
                if (per.isEdit)
                    ViewBag.CapNhatHoSo = "Yes";
                GetViewBag(ViewReport.Index);

                if (String.IsNullOrEmpty(maHoSo) && String.IsNullOrEmpty(tenDuAn) && String.IsNullOrEmpty(tongMucDauTu))
                    return View(db.sp_HoSoDuAnWithLuongXuLy(null, null, null, null, null, user.UserName, TrangThai.HoatDong).OrderByDescending(o => o.NgayHopLe));
                else
                {
                    var list = db.sp_HoSoDuAnWithLuongXuLy(maHoSo.Trim(), tenDuAn.Trim(), tongMucDauTu.Trim(), null, null, user.UserName, TrangThai.HoatDong).OrderByDescending(o => o.NgayHopLe);
                    return View(list);
                }
            }
            else
                return View();
        }

        /// <summary>
        /// Danh sách hồ sơ chưa xử lý
        /// </summary>
        public ActionResult HoSoChuaXuLy(string maHoSo, string tenDuAn, string tongMucDauTu)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (per.isView)
            {
                if (per.isEdit)
                    ViewBag.CapNhatHoSo = "Yes";
                GetViewBag(ViewReport.Index);

                // STT = 1 : Tạo mới, chưa gửi xử lý
                if (String.IsNullOrEmpty(maHoSo) && String.IsNullOrEmpty(tenDuAn) && String.IsNullOrEmpty(tongMucDauTu))
                    return View(db.sp_HoSoDuAnWithLuongXuLy(null, null, null, null, null, null, TrangThai.HoatDong).Where(o => o.STT == 1).ToList());
                else
                    return View(db.sp_HoSoDuAnWithLuongXuLy(maHoSo.Trim(), tenDuAn.Trim(), tongMucDauTu.Trim(), null, null, null, TrangThai.HoatDong).Where(o => o.STT == 1).ToList());
            }
            else
                return View();
        }

        /// <summary>
        /// Danh sách hồ sơ đang xử lý của User đăng nhập, loại ra các hồ sơ chưa đc phê duyệt
        /// </summary>
        public ActionResult ThamDinhHoSo(string maHoSo, string tenDuAn, string tongMucDauTu)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (per.isView)
            {
                GetViewBag(ViewReport.Index);

                // loại ra các hồ sơ chưa xử lý (Max STT = 1) và hồ sơ chưa đc phê duyệt
                if (String.IsNullOrEmpty(maHoSo) && String.IsNullOrEmpty(tenDuAn) && String.IsNullOrEmpty(tongMucDauTu))
                    return View(db.sp_HoSoDuAnWithLuongXuLy(null, null, null, null, null, user.UserName, TrangThai.HoatDong).Where(o => o.Cap_ID != STTCapXuLy.DaPheDuyet).ToList());
                else
                    return View(db.sp_HoSoDuAnWithLuongXuLy(maHoSo.Trim(), tenDuAn.Trim(), tongMucDauTu.Trim(), null, null, user.UserName, TrangThai.HoatDong).Where(o => o.Cap_ID != STTCapXuLy.DaPheDuyet).ToList());
            }
            else
                return View();
        }

        /// <summary>
        /// Danh sách hồ sơ đang xử lý của Giám đốc đăng nhập, dùng để Phê duyệt hồ sơ, loại ra các hồ sơ chưa đc phê duyệt
        /// </summary>
        public ActionResult PheDuyetHoSo(string maHoSo, string tenDuAn, string tongMucDauTu)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (per.isView)
            {
                GetViewBag(ViewReport.Index);

                if (String.IsNullOrEmpty(maHoSo) && String.IsNullOrEmpty(tenDuAn) && String.IsNullOrEmpty(tongMucDauTu))
                    return View(db.sp_HoSoDuAnWithLuongXuLy(null, null, null, null, null, user.UserName, TrangThai.HoatDong).Where(o => o.Cap_ID != STTCapXuLy.DaPheDuyet).ToList());
                else
                    return View(db.sp_HoSoDuAnWithLuongXuLy(maHoSo.Trim(), tenDuAn.Trim(), tongMucDauTu.Trim(), null, null, user.UserName, TrangThai.HoatDong).Where(o => o.Cap_ID != STTCapXuLy.DaPheDuyet).ToList());
            }
            else
                return View();
        }

        /// <summary>
        /// Danh sách hồ sơ đang xử lý, loại ra các hồ sơ chưa xử lý (Max STT = 1) và hồ sơ chưa đc phê duyệt
        /// </summary>
        public ActionResult HoSoDangXuLy(string maHoSo)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (per.isView)
            {
                if (per.isEdit)
                    ViewBag.CapNhatHoSo = "Yes";
                GetViewBag(ViewReport.Index);

                // loại ra các hồ sơ chưa xử lý (Max STT = 1) và hồ sơ chưa đc phê duyệt
                if (String.IsNullOrEmpty(maHoSo))
                    return View(db.sp_HoSoDuAnWithLuongXuLy(null, null, null, null, null, null, TrangThai.HoatDong).Where(o => o.STT != 1 && o.Cap_ID != STTCapXuLy.DaPheDuyet).ToList());
                else
                    return View(db.sp_HoSoDuAnWithLuongXuLy(maHoSo.Trim(), null, null, null, null, null, TrangThai.HoatDong).Where(o => o.STT != 1 && o.Cap_ID != STTCapXuLy.DaPheDuyet).ToList());
            }
            else
                return View();
        }

        /// <summary>
        /// Danh sách hồ sơ đã xử lý, chỉ lấy các hồ sơ có Cap_ID = 5 (Phê duyệt)
        /// </summary>
        public ActionResult HoSoDaXuLy(string maHoSo)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (per.isView)
            {
                if (per.isEdit)
                    ViewBag.CapNhatHoSo = "Yes";
                GetViewBag(ViewReport.Index);

                // chỉ lấy các hồ sơ có Cap_ID = 5 (Phê duyệt) lý (Max STT = 1)
                if (String.IsNullOrEmpty(maHoSo))
                    return View(db.sp_HoSoDuAnWithLuongXuLy(null, null, null, null, null, null, TrangThai.HoatDong).Where(o => o.Cap_ID == STTCapXuLy.DaPheDuyet).ToList());
                else
                    return View(db.sp_HoSoDuAnWithLuongXuLy(maHoSo.Trim(), null, null, null, null, null, TrangThai.HoatDong).Where(o => o.Cap_ID == STTCapXuLy.DaPheDuyet).ToList());
            }
            else
                return View();
        }

        // GET: TD_HoSoDuAn/Details/5
        public ActionResult Details(int id, string vi)
        {
            ViewBag.URLPrevious = vi;
            //if (Request.UrlReferrer != null)
            //{
            //    ViewBag.URLPrevious = Request.UrlReferrer.ToString();
            //    TempData["URLPrevious"] = Request.UrlReferrer.ToString();
            //}

            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isView)
                return Content("Bạn không có quyền truy cập chức năng này");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_HoSoDuAn tD_HoSoDuAn = db.TD_HoSoDuAn.Find(id);
            if (tD_HoSoDuAn == null)
            {
                return HttpNotFound();
            }
            GetViewBag(ViewReport.Index);

            // Lấy các file đã upload lên server
            var listFile = db.TD_FileHoSoDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == tD_HoSoDuAn.MaHoSo);
            if (listFile.Any())
                tD_HoSoDuAn.ListFile = listFile.ToList();

            // Lấy luồng xử lý cuối cùng
            //var objLuongCongViecCuoi = db.TD_LuongCongViec.Where(o => o.MaHoSo == tD_HoSoDuAn.MaHoSo).OrderByDescending(o => o.STT).FirstOrDefault();
            var objLuongCVCuoi = db.sp_GetMaxLuongCongViecByMaHoSo(tD_HoSoDuAn.MaHoSo).FirstOrDefault();
            ViewData["LuongCongViec_ID"] = objLuongCVCuoi.LuongCongViec_ID;
            tD_HoSoDuAn.str5 = objLuongCVCuoi.NguoiNhan;
            //tD_HoSoDuAn.Cap_ID = objLuongCVCuoi.Cap_ID;
            if (objLuongCVCuoi.NguoiNhan == user.UserName && objLuongCVCuoi.Cap_ID != STTCapXuLy.DaPheDuyet)
            {
                // Kiểm tra quyền phê duyệt hồ sơ : Giám đốc
                if (per.isSuccess)
                    ViewData["IsGiamDoc"] = true;

                // Kiểm tra quyền xử lý hồ sơ
                if (objLuongCVCuoi.Cap_ID == STTCapXuLy.CapNhatHoSo)
                    ViewData["ShowDivButton"] = true;
                else
                {
                    ViewData["ShowDivXuLy"] = true;
                    if (user.UserName == objLuongCVCuoi.NguoiNhan)
                    {
                        ViewData["NguoiGui"] = db.AspNetUsers.FirstOrDefault(o => o.UserName == objLuongCVCuoi.NguoiGui).HoVaTen;
                        ViewData["NoiDung"] = objLuongCVCuoi.NoiDung;
                        ViewData["NgayHenTra"] = objLuongCVCuoi.NgayHenTra.ToString("dd/MM/yyyy");

                        var objFileDinhKemLuongCV = db.TD_LuongCongViec_FileDinhKem.FirstOrDefault(o => o.LuongCongViec_ID == objLuongCVCuoi.LuongCongViec_ID);
                        if (objFileDinhKemLuongCV != null)
                            ViewData["HrefFileDinhKem"] = "<a href='/TD_HoSoDuAn/Download?id=" + objFileDinhKemLuongCV.FileHoSoDinhKem_ID + "&prefixName=TD_LuongCongViec_FileDinhKem'>" + objFileDinhKemLuongCV.TenHienThi + "</a>";
                    }
                }
            }

            return View(tD_HoSoDuAn);
        }

        // GET: TD_HoSoDuAn/Create
        public ActionResult Create()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isEdit)
                return Content("Bạn không có quyền thực hiện chức năng này");

            var obj = new TD_HoSoDuAn();
            obj.ListFile = new List<TD_FileHoSoDinhKem>();

            // Số ngày giải quyết
            var objLoaiDuAn = db.Dm_LoaiDuAn.FirstOrDefault();
            if (objLoaiDuAn != null)
            {
                obj.SoNgayGiaiQuyet = objLoaiDuAn.SoNgayGiaiQuyet;
                obj.NgayTra = DateTime.Now.AddDays(obj.SoNgayGiaiQuyet).Date;
            }

            obj.ListFile = new List<TD_FileHoSoDinhKem>();
            var objVanBanMacDinh = db.TD_CauHinh.FirstOrDefault(o => o.MaCauHinh == "VanBanMacDinhTiepNhanHoSo");
            if (objVanBanMacDinh != null)
            {
                String[] vanBanID = objVanBanMacDinh.NoiDung.Split(',');
                foreach (string vbId in vanBanID)
                {
                    try
                    {
                        int iVanBanID = int.Parse(vbId);
                        var objVanBan = db.VanBan.FirstOrDefault(o => o.VanBan_ID == iVanBanID);

                        obj.ListFile.Add(new TD_FileHoSoDinhKem
                        {
                            TrichYeu = objVanBan.TenVanBan,
                            VanBan_ID = objVanBan.VanBan_ID
                        });
                    }
                    catch (Exception)
                    {

                    }

                }

            }
            
            // Lấy tên file văn bản và ghi chú đc cấu hình sẵn
            //var listLoaiDuAn = MvcApplication.ListLoaiDuAn();
            //if (listLoaiDuAn != null)
            //{
            //    var duAn_ID = listLoaiDuAn.FirstOrDefault().LoaiDuAn_ID;
            //    var listLoaiFile = db.Ht_FileHoSo_LoaiDuAn.Where(o => o.LoaiDuAn_ID == duAn_ID);
            //    if (listLoaiFile != null)
            //    {
            //        foreach (var item in listLoaiFile)
            //        {
            //            obj.ListFile.Add(new TD_FileHoSoDinhKem
            //            {
            //                TenVanBan = item.TenFile,
            //                GhiChu = item.GhiChu
            //            });
            //        }
            //    }
            //}

            //Lấy số file cấu hình max
            var objMaxFileCauHinh = db.TD_CauHinh.FirstOrDefault(o => o.MaCauHinh == "MaxFileThamDinh");
            if (objMaxFileCauHinh != null)
                ViewBag.MaxFileThamDinh = Convert.ToInt32(objMaxFileCauHinh.NoiDung);

            GetViewBag(ViewReport.Create);
            return View(obj);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(TD_HoSoDuAn tD_HoSoDuAn, List<TD_FileHoSoDinhKem> listFile)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isEdit)
                return Content("Bạn không có quyền thực hiện chức năng này");

            ModelState.Remove("MaHoSo");
            ModelState.Remove("TuNgay");
            ModelState.Remove("DenNgay");
            tD_HoSoDuAn.TuNgay = DateTime.Now;
            tD_HoSoDuAn.DenNgay = DateTime.Now;
            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Lưu tạm để tạo số biên nhận
                        db.TD_HoSoDuAn.Add(tD_HoSoDuAn);
                        db.SaveChanges();

                        // update lại số biên nhận
                        var maHoSo = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + tD_HoSoDuAn.LoaiDuAn_ID + tD_HoSoDuAn.HoSoDuAn_ID;
                        tD_HoSoDuAn.MaHoSo = maHoSo;
                        db.Entry(tD_HoSoDuAn).State = EntityState.Modified;

                        // Tạo luồng công việc đầu tiên là tạo mới
                        var objLuongCV = new TD_LuongCongViec
                        {
                            Cap_ID = STTCapXuLy.CapNhatHoSo,
                            MaHoSo = maHoSo,
                            NgayGui = DateTime.Now,
                            NgayHenTra = DateTime.Now,
                            NguoiGui = user.UserName,
                            NguoiNhan = user.UserName,
                            NoiDung = "Tạo mới",
                            STT = 1,
                            TrangThai = TrangThai.HoatDong
                        };
                        db.TD_LuongCongViec.Add(objLuongCV);

                        // lưu file từ thư mục tạm vào thư mục chính
                        if (Request.Files.Count == 0)
                        {
                            //return Content("Không có file"); 
                        }
                        else
                        {
                            var pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
                            if (string.IsNullOrEmpty(pathSource)) return Content("Không tìm thấy đường dẫn file");

                            //var duongDan = pathSource + "/TD_HoSoDuAn/NewUpload/" + User.Identity.GetUserId();
                            //if (!Directory.Exists(duongDan))
                            //    Directory.CreateDirectory(duongDan);

                            var duongDanVatLy = pathSource + "/TD_HoSoDuAn";
                            if (!Directory.Exists(duongDanVatLy))
                                Directory.CreateDirectory(duongDanVatLy);

                            var indexFile = 0;
                            var stt = 1;
                            for (int i = 0; i < Request.Files.Count; i++)
                            {
                                var file = Request.Files[i];
                                if (file.ContentLength > 0)
                                {
                                    // Tạo tên file lưu trên hệ thống
                                    var fileName = tD_HoSoDuAn.HoSoDuAn_ID + "_" + tD_HoSoDuAn.LoaiDuAn_ID + "_" + Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);

                                    // Lưu file lên hệ thống
                                    var path = Path.Combine(duongDanVatLy, fileName);
                                    file.SaveAs(path);

                                    // Tạo object TD_FileHoSoDinhKem                                    
                                    var objFileDinhKem = new TD_FileHoSoDinhKem
                                    {
                                        DuongDan = path,
                                        SoKyHieu = listFile[indexFile].SoKyHieu,
                                        MaHoSo = maHoSo,
                                        STT = stt,
                                        TenFile = fileName,
                                        TenHienThi = Path.GetFileName(file.FileName),
                                        TrichYeu = listFile[indexFile].TrichYeu,
                                        VanBan_ID = listFile[indexFile].VanBan_ID,
                                        TrangThai = TrangThai.HoatDong
                                    };

                                    // Lưu thông tin file vào db
                                    db.TD_FileHoSoDinhKem.Add(objFileDinhKem);
                                    stt++; // Cộng số thứ tự file thêm 1 nếu đã lưu 1 file thành công
                                }
                                indexFile++;
                            }
                        }

                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbContextTransaction.Rollback();
                        var exc = new ExceptionViewer();
                        exc.GetError(ex);
                    }
                }
            }
            tD_HoSoDuAn.ListFile = listFile;
            GetViewBag(ViewReport.Create);
            GetModelStateError(ViewData);
            return View(tD_HoSoDuAn);
        }

        // GET: TD_HoSoDuAn/Edit/5
        public ActionResult Edit(int id)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isEdit)
                return Content("Bạn không có quyền thực hiện chức năng này");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_HoSoDuAn tD_HoSoDuAn = db.TD_HoSoDuAn.Find(id);
            if (tD_HoSoDuAn == null)
            {
                return HttpNotFound();
            }
            GetViewBag(ViewReport.Edit);

            // Nếu không phải Trưởng phòng, Giám đốc thì hồ sơ đang xử lý không có quyền sửa            
            var objLuongCongViecCuoi = db.sp_GetMaxLuongCongViecByMaHoSo(tD_HoSoDuAn.MaHoSo).FirstOrDefault();
            if (objLuongCongViecCuoi.Cap_ID != STTCapXuLy.CapNhatHoSo)
            {
                var listSep = db.Ht_ThamSoHeThong.Where(o => o.MaThamSo == "TruongPhong_ID" || o.MaThamSo == "GiamDoc_ID").Select(o => o.GiaTriThamSo);
                if (!listSep.Contains(User.Identity.GetUserId()))
                    return Content("Hồ sơ đang xử lý. Bạn không có quyền sửa hồ sơ");
            }

            // Nếu hồ sơ đã phê duyệt thì không được phép sửa
            if (objLuongCongViecCuoi.Cap_ID == STTCapXuLy.DaPheDuyet)
                return Content("Hồ sơ đã phê duyệt. Bạn không được quyền sửa");

            //var objSep = db.Ht_ThamSoHeThong.First(o => o.MaThamSo == "SepID");            
            //if (!objSep.GiaTriThamSo.Contains(User.Identity.GetUserId()) && tD_HoSoDuAn.Cap_ID != 1)
            //    return Content("Hồ sơ đang xử lý. Bạn không có quyền sửa hồ sơ");

            // Lấy các file đã upload lên server
            var listFileInDb = db.TD_FileHoSoDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == tD_HoSoDuAn.MaHoSo).ToList();
            if (listFileInDb.Any())
                tD_HoSoDuAn.ListFile = listFileInDb;
            else
                tD_HoSoDuAn.ListFile = new List<TD_FileHoSoDinhKem>();

            var objVanBanMacDinh = db.TD_CauHinh.FirstOrDefault(o => o.MaCauHinh == "VanBanMacDinhTiepNhanHoSo");

            // Nếu có cấu hình
            if(objVanBanMacDinh != null)
            {
                String[] listVBStr = objVanBanMacDinh.NoiDung.Split(',');
                int[] listVBInts = Array.ConvertAll(listVBStr, s => int.Parse(s));

                // Lấy các văn bản được cấu hình
                List<VanBan> listVB = db.VanBan.Where(o => o.TrangThai == TrangThai.HoatDong && listVBInts.Contains(o.VanBan_ID)).ToList();

                // Bỏ các văn bản đã được đính kèm lên và add thêm văn bản chưa đc đính kèm
                foreach(var item in listVB)
                {
                    if (!listFileInDb.Any(o => o.VanBan_ID == item.VanBan_ID))
                        tD_HoSoDuAn.ListFile.Add(new TD_FileHoSoDinhKem
                        {
                            TrichYeu = item.TenVanBan,
                            VanBan_ID = item.VanBan_ID
                        });
                }
            }

            //Lấy số file cấu hình max
            var objMaxFileCauHinh = db.TD_CauHinh.FirstOrDefault(o => o.MaCauHinh == "MaxFileThamDinh");
            if (objMaxFileCauHinh != null)
                ViewBag.MaxFileThamDinh = Convert.ToInt32(objMaxFileCauHinh.NoiDung);

            return View(tD_HoSoDuAn);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(TD_HoSoDuAn tD_HoSoDuAn, List<TD_FileHoSoDinhKem> listFile)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            // Kiểm tra quyền truy cập và cập nhật
            var per = CheckPermission(user);
            if (!per.isEdit)
                return Content("Bạn không có quyền thực hiện chức năng này");

            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(tD_HoSoDuAn).State = EntityState.Modified;
                        tD_HoSoDuAn.TuNgay = DateTime.Now;
                        tD_HoSoDuAn.DenNgay = DateTime.Now;
                        //db.SaveChanges();

                        // Lưu file vào hệ thống
                        //if (Request.Files.Count > 0)
                        if (listFile.Any())
                        {
                            var pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
                            if (string.IsNullOrEmpty(pathSource)) return Content("Không tìm thấy đường dẫn file");
                            var maxSTT = db.TD_FileHoSoDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == tD_HoSoDuAn.MaHoSo).Max(o => o.STT);
                            var duongDanVatLy = pathSource + "/TD_HoSoDuAn";
                            if (!Directory.Exists(duongDanVatLy))
                                Directory.CreateDirectory(duongDanVatLy);

                            var listFileInDb = db.TD_FileHoSoDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == tD_HoSoDuAn.MaHoSo);
                            var count = Request.Files.Count;
                            for (var i = 0; i < listFile.Count; i++)
                            {
                                var id = listFile[i].FileHoSoDinhKem_ID;

                                // File chưa được lưu trong database
                                if (id <= 0 || id == null)
                                {
                                    // Nếu có file thì sẽ tạo mới
                                    if (count > 0)
                                    {
                                        var file = Request.Files["file_" + i];
                                        if (file != null)
                                        {
                                            // Tạo tên file lưu trên hệ thống
                                            var fileName = tD_HoSoDuAn.HoSoDuAn_ID + "_" + tD_HoSoDuAn.LoaiDuAn_ID + "_" + Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);

                                            // Tạo đường dẫn vật lý cho file
                                            var path = Path.Combine(duongDanVatLy, fileName);

                                            // Tạo object TD_FileHoSoDinhKem
                                            var objFileDinhKem = new TD_FileHoSoDinhKem
                                            {
                                                DuongDan = path,
                                                GhiChu = listFile[i].GhiChu,
                                                MaHoSo = tD_HoSoDuAn.MaHoSo,
                                                VanBan_ID = listFile[i].VanBan_ID,
                                                TenFile = fileName,
                                                TenHienThi = Path.GetFileName(file.FileName),
                                                TenVanBan = listFile[i].TenVanBan,
                                                TrangThai = TrangThai.HoatDong
                                            };

                                            // Lưu thông tin file vào db
                                            db.TD_FileHoSoDinhKem.Add(objFileDinhKem);
                                            //db.SaveChanges();

                                            // Lưu file lên hệ thống
                                            file.SaveAs(path);
                                        }
                                    }
                                    else
                                    {
                                        // Không có file thì ko làm gì
                                    }
                                }
                                // Có file đã được lưu trong database
                                else
                                {
                                    var objFileOld = listFileInDb.FirstOrDefault(o => o.FileHoSoDinhKem_ID == id);

                                    if (objFileOld != null)
                                    {
                                        // Không up file khác lên thay thì cập nhật lại thông tin Tên văn bản, Ghi chú
                                        if (count == 0)
                                        {
                                            objFileOld.TenVanBan = !string.IsNullOrEmpty(listFile[i].TenVanBan) ? listFile[i].TenVanBan.Trim() : "";
                                            objFileOld.GhiChu = !string.IsNullOrEmpty(listFile[i].GhiChu) ? listFile[i].GhiChu.Trim() : "";
                                            db.Entry(objFileOld).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            var file = Request.Files["file_" + i];

                                            // Up file khác lên thay thế file cũ
                                            if (file != null)
                                            {
                                                // Tạo tên file lưu trên hệ thống
                                                var fileName = tD_HoSoDuAn.HoSoDuAn_ID + "_" + tD_HoSoDuAn.LoaiDuAn_ID + "_" + Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);

                                                // Tạo đường dẫn vật lý cho file
                                                var path = Path.Combine(duongDanVatLy, fileName);

                                                // Tạo object TD_FileHoSoDinhKem
                                                var objFileDinhKem = new TD_FileHoSoDinhKem
                                                {
                                                    DuongDan = path,
                                                    GhiChu = listFile[i].GhiChu,
                                                    MaHoSo = tD_HoSoDuAn.MaHoSo,
                                                    //STT = stt,
                                                    TenFile = fileName,
                                                    TenHienThi = Path.GetFileName(file.FileName),
                                                    TenVanBan = listFile[i].TenVanBan,
                                                    TrangThai = TrangThai.HoatDong
                                                };

                                                // Nếu đổi file khác thì đổi trạng thái của file cũ, và tạo 1 object mới
                                                if (listFile[i].FileHoSoDinhKem_ID != -1)
                                                {
                                                    //var id = listFile[i].FileHoSoDinhKem_ID;
                                                    //var objFileOld = db.TD_FileHoSoDinhKem.FirstOrDefault(o => o.FileHoSoDinhKem_ID == id);
                                                    objFileOld.TrangThai = TrangThai.KhongHoatDong;
                                                    db.Entry(objFileOld).State = EntityState.Modified;
                                                    //db.SaveChanges();

                                                    objFileDinhKem.STT = objFileOld.STT;
                                                }
                                                // Nếu lưu thêm file lên hệ thống
                                                else
                                                {
                                                    maxSTT++;
                                                    objFileDinhKem.STT = maxSTT;
                                                }

                                                // Lưu thông tin file vào db
                                                db.TD_FileHoSoDinhKem.Add(objFileDinhKem);
                                                //db.SaveChanges();

                                                // Lưu file lên hệ thống
                                                file.SaveAs(path);
                                            }
                                            // Không up file khác lên thay thì cập nhật lại thông tin Tên văn bản, Ghi chú
                                            else
                                            {
                                                objFileOld.TenVanBan = !string.IsNullOrEmpty(listFile[i].TenVanBan) ? listFile[i].TenVanBan.Trim() : "";
                                                objFileOld.GhiChu = !string.IsNullOrEmpty(listFile[i].GhiChu) ? listFile[i].GhiChu.Trim() : "";
                                                db.Entry(objFileOld).State = EntityState.Modified;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Không tìm thấy file trong database
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return RedirectToAction("Index");
                        //return Json(new { ok = "ok" });
                        //return View();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbContextTransaction.Rollback();
                        var exc = new ExceptionViewer();
                        exc.GetError(ex);
                    }
                }
            }
            GetViewBag(ViewReport.Edit);
            GetModelStateError(ViewData);
            return View(tD_HoSoDuAn);
        }

        // GET: TD_HoSoDuAn/Delete/
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TD_HoSoDuAn tD_HoSoDuAn = db.TD_HoSoDuAn.Find(id);

            if (tD_HoSoDuAn == null)
            {
                return HttpNotFound();
            }
            // Kiểm tra xem hồ sơ này đã từng được xử lý chưa? Nếu đã có luồng xử lý thì ko cho xóa
            var countLuongXuLy = db.TD_LuongCongViec.Count(o => o.MaHoSo == tD_HoSoDuAn.MaHoSo);
            if (countLuongXuLy <= 1)
            {
                tD_HoSoDuAn.TrangThai = TrangThai.KhongHoatDong;
                db.Entry(tD_HoSoDuAn).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg = "ok" });
            }
            else
                return Json(new { msg = "Hồ sơ này đã từng được xử lý nên không thể xóa" });
        }

        public ActionResult Up(int? id, int stt)
        {
            //ViewBag.TenFile = TempData["TenVanBan_" + User.Identity.GetUserId()];
            //ViewBag.STT = TempData["STT_" + User.Identity.GetUserId()];
            var obj = new TD_FileHoSoDinhKem();
            if (id != null)
                obj = db.TD_FileHoSoDinhKem.FirstOrDefault(o => o.FileHoSoDinhKem_ID == id);
            if (obj.STT == null)
                obj.STT = stt;
            return View(obj);
        }

        /// <summary>
        /// Upload file lên thư mục tạm của hệ thống
        /// </summary>
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "UploadFile")]
        public ActionResult Up(TD_FileHoSoDinhKem objFile)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            }
            else
            {
                // Kiểm tra đã submit file lên
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    var pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
                    if (string.IsNullOrEmpty(pathSource)) return Content("Không tìm thấy đường dẫn file");
                    var duongDan = pathSource + "/TD_HoSoDuAn/NewUpload/" + User.Identity.GetUserId() + "/" + objFile.STT;

                    if (!Directory.Exists(duongDan))
                        Directory.CreateDirectory(duongDan);

                    // lấy tên file
                    var fileName = Path.GetFileName(file.FileName);

                    // Xóa file đã đính kèm trước đó đi trong thư mục
                    string[] filesPath = System.IO.Directory.GetFiles(duongDan);
                    if (filesPath.Length > 0)
                    {
                        foreach (string item in filesPath)
                        {
                            if (!string.IsNullOrEmpty(item))
                                System.IO.File.Delete(item);
                        }
                    }

                    // Lưu file vào thư mục tạm của hệ thống
                    var path = Path.Combine(duongDan, fileName);
                    file.SaveAs(path);
                    ViewBag.FileName = fileName; // truyền lại tên file ra ngoài view

                    #region Lưu đường dẫn, thông tin file vào TempData
                    objFile.TrangThai = TrangThai.HoatDong;
                    objFile.TenHienThi = fileName;
                    var list = new List<TD_FileHoSoDinhKem>();
                    if (Session["TD_HoSoDuAn_" + User.Identity.GetUserId()] == null)
                    {
                        list.Add(objFile);
                    }
                    else
                    {
                        list = (List<TD_FileHoSoDinhKem>)Session["TD_HoSoDuAn_" + User.Identity.GetUserId()];

                        // Nếu đã có thì xóa trong list và tạo mới 
                        var obj = list.Find(o => o.STT == objFile.STT);
                        if (obj != null)
                            list.Remove(obj);
                        list.Add(objFile);
                    }
                    Session["TD_HoSoDuAn_" + User.Identity.GetUserId()] = list;
                    #endregion Lưu đường dẫn, thông tin file vào TempData
                }

                // redirect back to the index action to show the form once again
                ViewBag.Status = "status=\"OK\";";
            }
            //return Json(new { msg = "ok" });
            return View();
        }

        public ActionResult LoadListFileHoSo()
        {
            if (Session["TD_HoSoDuAn_" + User.Identity.GetUserId()] != null)
            {
                var str = new StringBuilder();
                var list = (List<TD_FileHoSoDinhKem>)Session["TD_HoSoDuAn_" + User.Identity.GetUserId()];
                //Session["TD_HoSoDuAn_" + User.Identity.GetUserId()] = list;
                var i = 1;
                foreach (var item in list)
                {
                    str.Append("<tr>");
                    str.AppendFormat("<td id='tenVanBan_{0}'>{1}</td>", i, item.TenVanBan);
                    str.AppendFormat("<td id='ghiChu_{0}'>{1}</td>", i, item.GhiChu);
                    str.AppendFormat("<td id='tenFile_{0}'><a href='/TD_HoSoDuAn/Download?id={1}&prefixName=TD_HoSoDuAn'>{2}</a></td>", i, item.FileHoSoDinhKem_ID, item.TenHienThi);
                    str.AppendFormat("<td><a href='/TD_HoSoDuAn/Up?id={0}&stt={1}' id='ShowPopUp' >Chọn file</a></td>", item.FileHoSoDinhKem_ID, i);
                    str.Append("</tr>");
                    i++;
                }
                return Json(new { table = str.ToString() });
            }
            return Json(new { ok = "ok" });
        }

        /// <summary>
        /// Tính ngày trả = Ngày nhận + số ngày giải quyết
        /// </summary>
        public ActionResult AddDateToNgayTra(string ngayNhan, int soNgayGiaiQuyet)
        {
            DateTime ngayTra;
            if (soNgayGiaiQuyet == null)
                return Json(new { msg = "Số ngày giải quyết không đúng" });
            if (DateTime.TryParseExact(ngayNhan, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out ngayTra))
                ngayTra = ngayTra.AddDays(soNgayGiaiQuyet);
            else
                return Json(new { msg = "Ngày nhận không đúng định dạng cho phép" });

            return Json(new { ngayTra = ngayTra.ToString("dd/MM/yyyy") });
        }

        /// <summary>
        /// Lưu tên file cần up và số thứ tự vào TempData để chuyển sang View Up
        /// </summary>
        /// <param name="tenFile">tên file cần up</param>
        /// <param name="stt">số thứ tự</param>
        public ActionResult SaveTempData(string tenVanBan, int stt, string ghiChu, int? fileID)
        {
            var objFile = new TD_FileHoSoDinhKem();
            if (fileID != null)
            {
                var id = Convert.ToInt32(fileID);
                objFile = db.TD_FileHoSoDinhKem.FirstOrDefault(o => o.FileHoSoDinhKem_ID == id);
            }
            else
            {
                objFile.TenVanBan = tenVanBan;
                objFile.STT = stt;
                objFile.GhiChu = ghiChu;
            }
            TempData["FileDinhKem_" + User.Identity.GetUserId()] = objFile;
            return Json(new { msg = "ok" });
        }

        public ActionResult Forward(string maHoSo, string prefix, string ngayHenTra)
        {
            ViewBag.HideMenu = true;
            if (string.IsNullOrEmpty(maHoSo)) return Content("Không tìm thấy mã hồ sơ");
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return Content("Phiên đăng nhập đã hết. Mời bạn đăng nhập lại!");

            // Kiểm tra người xử lý cuối có phải người gửi
            var objLuongCVCuoi = db.sp_GetMaxLuongCongViecByMaHoSo(maHoSo).FirstOrDefault();
            if (objLuongCVCuoi == null) return Content("Không tìm thấy luồng công việc");
            if (objLuongCVCuoi.NguoiNhan != user.UserName) return Content("Bạn không phải người xử lý cuối của hồ sơ");

            var obj = new TD_LuongCongViec
            {
                LuongCha_ID = objLuongCVCuoi.LuongCongViec_ID,
                MaHoSo = maHoSo,
                Prefix = prefix
            };

            if (prefix == "Reply")
            {
                obj.NguoiNhan = objLuongCVCuoi.NguoiGui;
                obj.Cap_ID = STTCapXuLy.PhanHoi;
            }
            GetViewBag_Forward(prefix, user);
            ViewBag.NgayHenTra = ngayHenTra;

            // Lấy luồng công việc cha
            obj.LuongCha_ID = objLuongCVCuoi.LuongCha_ID == null ? obj.LuongCongViec_ID : obj.LuongCha_ID;
            obj.STT = objLuongCVCuoi.STT + 1;

            return View(obj);
        }

        private void GetViewBag_Forward(string prefix, ApplicationUser user)
        {
            if (prefix == "Reply")
            {
                ViewBag.DisableNguoiYeuCau = true;
            }
            else
            {
                var listUser = db.AspNetUsers.Where(o => o.TrangThai == TrangThai.HoatDong && o.DonVi_ID == DonVi.SoThongTinTruyenThong && o.UserName != user.UserName).Select(o => new { o.UserName, o.HoVaTen }).ToList();
                ViewBag.NguoiNhan = listUser;
            }
        }

        [HttpPost]
        public ActionResult Forward(TD_LuongCongViec obj)
        {
            if (String.IsNullOrEmpty(obj.MaHoSo)) return Content("Không tìm thấy mã hồ sơ");
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            obj.NguoiGui = user.UserName;
            obj.NgayGui = DateTime.Now;
            obj.NgayHenTra = DateTime.Now;

            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra có tồn tại người nhận
                        var isNguoiNhan = db.AspNetUsers.Any(o => o.UserName.ToUpper() == obj.NguoiNhan.Trim().ToUpper());
                        if (!isNguoiNhan)
                        {
                            ModelState.AddModelError("NguoiNhan", "Không tồn tại người nhận " + obj.NguoiNhan + " trong hệ thống");
                            GetViewBag_Forward(obj.Prefix, user);
                            ViewBag.HideMenu = true;
                            return View(obj);
                        }

                        #region Cập nhật trạng thái hồ sơ cho luồng công việc
                        var objCap = GetCapXuLy(obj.NguoiNhan);
                        obj.Cap_ID = objCap == null ? STTCapXuLy.XinYKien : objCap.ThuTu.Value; // Trạng thái "Xin ý kiến" nếu không tìm thấy người trong phòng ban xử lý
                        #endregion Cập nhật trạng thái hồ sơ cho luồng công việc

                        #region Tạo luồng công việc
                        obj.TrangThai = TrangThai.HoatDong;
                        db.TD_LuongCongViec.Add(obj);
                        db.SaveChanges();
                        #endregion Tạo luồng công việc

                        #region Lưu file đính kèm theo luồng chuyển vào db
                        if (Request.Files.Count > 0)
                        {
                            var file = Request.Files[0];
                            if (file.ContentLength > 0)
                            {
                                var maxSize = 16384 * 1024; // in Byte

                                // Kiểm tra giới hạn dung lượng file upload cho phép
                                if (file.ContentLength > maxSize)
                                {
                                    ModelState.AddModelError("", "Dung lượng file vượt quá dung lượng cho phép tải lên");
                                    ViewBag.HideMenu = true;
                                    return View(obj);
                                }

                                var pathSource = ConfigurationManager.AppSettings["FolderUploadPath"];
                                if (string.IsNullOrEmpty(pathSource)) return Content("Không tìm thấy đường dẫn file");

                                // Lưu đường dẫn file theo mã hồ sơ
                                var duongDan = pathSource + "/TD_LuongCongViec_FileDinhKem/" + obj.MaHoSo;
                                if (!Directory.Exists(duongDan)) Directory.CreateDirectory(duongDan);
                                var fileName = obj.MaHoSo + "_" + obj.LuongCongViec_ID + "_" + obj.STT + "_" + User.Identity.GetUserId() + "_" + Path.GetFileName(file.FileName);

                                // Lưu file vào thư mục tạm của hệ thống
                                var path = Path.Combine(duongDan, fileName);
                                file.SaveAs(path);

                                #region Lưu đường dẫn, thông tin file vào db
                                var objFile = new TD_LuongCongViec_FileDinhKem();
                                objFile.LuongCongViec_ID = obj.LuongCongViec_ID;
                                objFile.MaHoSo = obj.MaHoSo;
                                objFile.TenFile = fileName;
                                objFile.TenHienThi = Path.GetFileName(file.FileName);
                                objFile.DuongDan = path;
                                objFile.TrangThai = TrangThai.HoatDong;
                                objFile.STT = 1;
                                db.TD_LuongCongViec_FileDinhKem.Add(objFile);
                                db.SaveChanges();

                                #endregion Lưu đường dẫn, thông tin file vào db
                            }
                        }
                        #endregion Lưu file đính kèm theo luồng chuyển vào db

                        // redirect back to the index action to show the form once again
                        ViewBag.Status = "status=\"OK\";";

                        dbContextTransaction.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        ViewBag.Status = "status=\"ERROR\";";
                        dbContextTransaction.Rollback();
                        var exc = new ExceptionViewer();
                        exc.GetError(ex);
                    }
                }
            }

            GetViewBag_Forward(obj.Prefix, user);
            ViewBag.HideMenu = true;
            return View(obj);
        }

        public ActionResult QuaTrinhXuLy(string maHoSo)
        {
            ViewBag.HideMenu = true;

            if (string.IsNullOrEmpty(maHoSo)) return Content("Không tìm thấy mã hồ sơ");
            var listLuongCV = db.TD_LuongCongViec.Where(o => o.MaHoSo == maHoSo).OrderBy(o => o.STT);

            // Lấy danh sách file đính kèm
            var listFile = db.TD_LuongCongViec_FileDinhKem.Where(o => o.TrangThai == TrangThai.HoatDong && o.MaHoSo == maHoSo);
            ViewBag.ListFileDinhKem = listFile;
            return View(listLuongCV);
        }

        /// <summary>
        /// Phê duyệt hồ sơ , chỉ có quyền giám đốc là có
        /// </summary>
        /// <param name="maHoSo"></param>
        /// <returns></returns>
        public ActionResult PheDuyet(string maHoSo)
        {
            if (String.IsNullOrEmpty(maHoSo)) return Json(new { msg = "Không tìm thấy mã hồ sơ" });
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            }
            else
            {
                // Kiểm tra quyền phê duyệt hồ sơ
                var per = CheckPermission(user);
                if (!per.isSuccess) return Content("Bạn không có quyền thực hiện chức năng này");

                var objHoSo = db.TD_HoSoDuAn.FirstOrDefault(o => o.MaHoSo == maHoSo);
                if (objHoSo == null) return Json(new { msg = "Không tìm thấy mã hồ sơ" });
                else
                {
                    try
                    {
                        var objLuongCVCuoi = db.sp_GetMaxLuongCongViecByMaHoSo(maHoSo).FirstOrDefault();
                        var luongCha_ID = objLuongCVCuoi.LuongCha_ID;
                        var sttMax = objLuongCVCuoi.STT;
                        var objLuongCV = new TD_LuongCongViec
                        {
                            Cap_ID = STTCapXuLy.DaPheDuyet,
                            LuongCha_ID = luongCha_ID,
                            MaHoSo = maHoSo,
                            NgayGui = DateTime.Now,
                            NgayHenTra = DateTime.Now,
                            NguoiGui = user.UserName,
                            NguoiNhan = user.UserName,
                            NoiDung = "Đã phê duyệt",
                            STT = sttMax + 1,
                            TrangThai = TrangThai.HoatDong
                        };
                        db.TD_LuongCongViec.Add(objLuongCV);
                        db.SaveChanges();
                        return Json(new { ok = "ok" });
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var exc = new ExceptionViewer();
                        exc.GetError(ex);
                        return Json(new { msg = "Có lỗi trong quá trình xử lý" });
                    }
                }
            }
        }

        public Dm_CapXuLy GetCapXuLy(string user_ID)
        {
            var listCap = db.Dm_CapXuLy.Where(o => o.Cap_ID < 5).OrderByDescending(o => o.ThuTu);
            if (listCap.Any())
            {
                foreach (var item in listCap)
                {
                    if (item.NguoiXuLy.Contains(user_ID))
                        return item;
                }
            }
            return null;
        }

        public ActionResult Download(int? id, string prefixName)
        {
            if (id == null) return Content("Không tìm thấy file trên server!");
            var filePath = "";
            var fileName = "";
            if (prefixName == "TD_HoSoDuAn")
            {
                var objFile = db.TD_FileHoSoDinhKem.FirstOrDefault(o => o.FileHoSoDinhKem_ID == id);
                filePath = objFile.DuongDan;
                fileName = objFile.TenHienThi;
            }
            else
            {
                var objFile = db.TD_LuongCongViec_FileDinhKem.FirstOrDefault(o => o.FileHoSoDinhKem_ID == id);
                filePath = objFile.DuongDan;
                fileName = objFile.TenHienThi;
            }
            var contentType = S4T_HaTinhBase.GetContentType(filePath);
            if (System.IO.File.Exists(filePath))
                return File(new FileStream(filePath, FileMode.Open), contentType, fileName);

            return Content("Không tìm thấy file trên server!");
        }

        #region Event
        /// <summary>
        /// Tự động lấy số ngày giải quyết theo loại dự án
        /// </summary>
        /// <param name="loaiDuAn_ID">Mã loại dự án</param>
        public ActionResult LoaiDuAn_OnChange(int loaiDuAn_ID)
        {
            var objLoaiDuAn = db.Dm_LoaiDuAn.FirstOrDefault(o => o.LoaiDuAn_ID == loaiDuAn_ID);
            if (objLoaiDuAn == null)
                return Json(new { value = "null" });
            else
                return Json(new { SoNgayGiaiQuyet = objLoaiDuAn.SoNgayGiaiQuyet });
        }
        #endregion Event

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
