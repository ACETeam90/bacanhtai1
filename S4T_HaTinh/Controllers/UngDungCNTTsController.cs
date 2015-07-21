using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using KellermanSoftware.CompareNetObjects;
using System.Data.Entity.Validation;
using System.Transactions;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using Novacode;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class UngDungCNTTsController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();
        private string listInputRadio = "QLVB,QLNhanSu,QLTaiChinhKeToan,QLTaiSanCong,QLKhieuNai,ChuKySo,MotCuaDienTu,";
        private Ht_PhanHeChucNang objChucNang = new S4T_HaTinhEntities().Ht_PhanHeChucNang.ToList().FirstOrDefault(o => o.ControllerName == System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller"));

        // GET: UngDungCNTTs
        public ActionResult Index()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));
            
            var objDonVi = db.Dm_DonVi.Find(user.DonVi_ID);
            ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";

            var listLichNhapByDonVi = db.Ht_LichNhapLieu.Where(o => o.TrangThai == TrangThai.HoatDong && o.DonVi_ID == user.DonVi_ID && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID);
            ViewBag.ListLichNhap = listLichNhapByDonVi;
            if (listLichNhapByDonVi.Any(o => o.ChucNang_ID == TrangThaiNhapLieu.ThemMoi))
                ViewBag.CreateLink = true;

            return View(db.UngDungCNTT.Where(o => o.DonVi_ID == objDonVi.DonVi_ID).ToList());
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
                    var objReport = db.UngDungCNTT.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                        && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                        && o.Success != TrangThaiNhapLieu.PheDuyet);
                    return objReport == null ? "" : ExceptionViewer.GetMessage("LICH_NHAP_NOT_FOUND");
                }
                else
                {
                    // Kiểm tra đã tồn tại bản ghi trong database?
                    var objReport = db.UngDungCNTT.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                        && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                        && o.Success == (byte)TrangThaiNhapLieu.Sua);
                    return objReport != null ? "" : ExceptionViewer.GetMessage("LICH_NHAP_NOT_FOUND");
                }
            }
        }

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
            ViewBag.listInputRadio = listInputRadio;
        }

        public ActionResult _CreateOrUpdate(UngDungCNTT obj)
        {
            return View(obj);
        }

        // GET: UngDungCNTTs/Details/5
        public async Task<ActionResult> Details(int? id, int? donVi_ID, int? lichNhap_ID)
        {
            var previousUrl = Request.UrlReferrer;
            if (previousUrl != null)
                ViewBag.PreviousUrl = previousUrl.ToString();

            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny)
            {
                // Kiểm tra thêm quyền là Thẩm định báo cáo ?
                per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.UngDungCNTT);
                if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));
            }

            var ungDungCNTT = new UngDungCNTT();
            if (id == null)
            {
                if (lichNhap_ID == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                else
                    ungDungCNTT = await db.UngDungCNTT.FirstOrDefaultAsync(o => o.LichNhap_ID == lichNhap_ID
                                                                            && o.DonVi_ID == donVi_ID);
            }
            else                
                ungDungCNTT = await db.UngDungCNTT.FindAsync(id);

            GetViewBag(ungDungCNTT.DonVi_ID);

            if (ungDungCNTT == null)
                return HttpNotFound();

            // Tạo TempData lưu list PMCN
            var list = db.UngDungCNTT_PMQLCN.Where(o => o.UngDungCNTT_ID == id);
            if (list.Any())
                ungDungCNTT.ListPMQLCN = list.ToList();
            else
                ungDungCNTT.ListPMQLCN = new List<UngDungCNTT_PMQLCN>();
            
            return View(ungDungCNTT);
        }
        
        // GET: UngDungCNTTs/Create
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
                UngDungCNTT obj = new UngDungCNTT();
                obj.Success = (byte)TrangThaiNhapLieu.ThemMoi;
                obj.DonVi_ID = user.DonVi_ID;
                obj.ListPMQLCN = new List<UngDungCNTT_PMQLCN>();

                // Tạo TempData lưu list PMCN
                TempData["UngDungCNTT_PMQLCN_" + user.Id] = obj.ListPMQLCN;

                return View(obj);
            }

            return Content(mess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UngDungCNTT ungDungCNTT)
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
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                                                && o.DonVi_ID == ungDungCNTT.DonVi_ID
                                                                                && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                                && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);

                            // Đổi trạng thái nhập liệu
                            ungDungCNTT.Success = (byte)TrangThaiNhapLieu.DaGui;
                            ungDungCNTT.LichNhap_ID = objLichNhap.LichNhap_ID; // Add LichNhap_ID vào báo cáo

                            db.UngDungCNTT.Add(ungDungCNTT);

                            // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                            //objLichNhap.BaoCao_ID = haTangKyThuatCNTT.HaTangKyThuatCNTT_ID;
                            objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                            db.Entry(objLichNhap).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            // Lưu phần mềm quản lý chuyên ngành
                            List<UngDungCNTT_PMQLCN> listPMCN = (List<UngDungCNTT_PMQLCN>)TempData["UngDungCNTT_PMQLCN_" + user.Id];
                            if (listPMCN.Any())
                            {
                                foreach (var item in listPMCN)
                                {
                                    //var obj = new UngDungCNTT_PMQLCN
                                    //{
                                    //    UngDungCNTT_ID = ungDungCNTT.UngDungCNTT_ID,
                                    //    Guid = item.Guid,
                                    //    TenPhanMem = item.TenPhanMem,
                                    //    LienThongSBN = item.LienThongSBN,
                                    //    LienThongUBNDCapHuyen = item.LienThongUBNDCapHuyen,
                                    //    LienThongUBNDCapXa = item.LienThongUBNDCapXa,
                                    //    LienThongDVTT = item.LienThongDVTT
                                    //};
                                    item.UngDungCNTT_ID = ungDungCNTT.UngDungCNTT_ID;
                                    db.UngDungCNTT_PMQLCN.Add(item);
                                    await db.SaveChangesAsync();
                                }
                            }

                            scope.Complete();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            var exc = new ExceptionViewer();
                            ViewBag.Mess = exc.GetError(ex);
                            scope.Dispose();
                            GetViewBag(user.DonVi_ID);
                            return View();
                        }
                    }
                    return RedirectToAction("Index");
                }
                return Content(mess);
            }

            GetViewBag(user.DonVi_ID);
            return View(ungDungCNTT);
        }

        // GET: UngDungCNTTs/Edit/5
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
                UngDungCNTT ungDungCNTT = await db.UngDungCNTT.FindAsync(id);
                if (ungDungCNTT == null)
                {
                    return HttpNotFound();
                }
                
                // Tạo TempData lưu list PMCN
                var list = db.UngDungCNTT_PMQLCN.Where(o => o.UngDungCNTT_ID == id);
                ungDungCNTT.ListPMQLCN = list.Any() ? list.ToList() : new List<UngDungCNTT_PMQLCN>();
                TempData["UngDungCNTT_PMQLCN_" + user.Id] = ungDungCNTT.ListPMQLCN;

                if (ungDungCNTT.TruongNhapLai.Contains("ListPMQLCN"))
                    ViewBag.ListPMQLCNRequestChange = true;

                return View(ungDungCNTT);
            }
            
            return Content(mess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UngDungCNTT ungDungCNTT)
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
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            using (var context = new S4T_HaTinhEntities())
                            {
                                ungDungCNTT.Success = (byte)TrangThaiNhapLieu.DaGui;
                                //ungDungCNTT.TruongNhapLai = string.Empty; // Xóa hết các yêu cầu nhập lại dữ liệu
                                context.Entry(ungDungCNTT).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                            var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                                                  && o.DonVi_ID == ungDungCNTT.DonVi_ID
                                                                                  && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                                  && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
                            objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                            db.Entry(objLichNhap).State = EntityState.Modified;
                            db.SaveChanges();

                            // Cập nhật List PMCN
                            List<UngDungCNTT_PMQLCN> listPMCN = (List<UngDungCNTT_PMQLCN>)TempData["UngDungCNTT_PMQLCN_" + user.Id];
                            if (listPMCN.Any())
                            {
                                bool isAdd = false;
                                var listInDb = db.UngDungCNTT_PMQLCN.Where(o => o.UngDungCNTT_ID == ungDungCNTT.UngDungCNTT_ID);
                                if (listPMCN.Count() > listInDb.Count()) isAdd = true;
                                foreach (UngDungCNTT_PMQLCN item in listPMCN)
                                {
                                    UngDungCNTT_PMQLCN objInDb = listInDb.FirstOrDefault(o => o.Id == item.Id && o.Guid == item.Guid);

                                    // Thêm mới nếu ko tìm thấy trong db
                                    if (objInDb == null && isAdd)
                                    {
                                        item.UngDungCNTT_ID = ungDungCNTT.UngDungCNTT_ID;
                                        db.UngDungCNTT_PMQLCN.Add(item);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        //This is the comparison class
                                        CompareLogic compareLogic = new CompareLogic();
                                        ComparisonResult result = compareLogic.Compare(objInDb, item);

                                        //These will be different, write out the differences
                                        if (!result.AreEqual)
                                        {
                                            objInDb.LienThongDVTT = item.LienThongDVTT;
                                            objInDb.LienThongSBN = item.LienThongSBN;
                                            objInDb.LienThongUBNDCapHuyen = item.LienThongUBNDCapHuyen;
                                            objInDb.LienThongUBNDCapXa = item.LienThongUBNDCapXa;
                                            objInDb.TenPhanMem = item.TenPhanMem;
                                            db.Entry(objInDb).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }

                            scope.Complete();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            var exc = new ExceptionViewer();
                            ViewBag.Mess = exc.GetError(ex);
                            scope.Dispose();
                            GetViewBag(user.DonVi_ID);
                            return View();
                        }                        
                    }
                    return RedirectToAction("Index");
                }

                return Content(mess);
            }

            GetViewBag(user.DonVi_ID);
            return View(ungDungCNTT);
        }

        public ActionResult _ListPMCN(List<UngDungCNTT_PMQLCN> ListPMQLCN)
        {
            ViewBag.HideMenu = true;
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            return PartialView(ListPMQLCN);
        }

        public ActionResult ListPMCN(int id)
        {
            ViewBag.HideMenu = true;
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            //var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            //if (per != PermissionType.Read) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            List<UngDungCNTT_PMQLCN> list = db.UngDungCNTT_PMQLCN.Where(o => o.UngDungCNTT_ID == id).ToList();

            return PartialView("_ListPMCN",list);
        }

        public ActionResult _CreateOrUpdatePMCN(string guid)
        {
            ViewBag.HideMenu = true;
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (!string.IsNullOrEmpty(guid))
            {
                List<UngDungCNTT_PMQLCN> list = (List<UngDungCNTT_PMQLCN>)TempData["UngDungCNTT_PMQLCN_" + user.Id];
                TempData["UngDungCNTT_PMQLCN_" + user.Id] = list;
                var obj = list.FirstOrDefault(o => o.Guid == guid);
                if (obj == null)
                    return HttpNotFound();
                return View(obj);
            }
            return View();
        }

        [HttpPost]
        public ActionResult _CreateOrUpdatePMCN(UngDungCNTT_PMQLCN obj)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            List<UngDungCNTT_PMQLCN> list = (List<UngDungCNTT_PMQLCN>) TempData["UngDungCNTT_PMQLCN_" + user.Id];

            // Insert
            if (String.IsNullOrEmpty(obj.Guid))
            {
                obj.Guid = Guid.NewGuid().ToString();
            }
            // Update
            else
            {
                UngDungCNTT_PMQLCN objRemove = list.FirstOrDefault(o => o.Guid == obj.Guid);
                list.Remove(objRemove);
            }
            list.Add(obj);

            TempData["UngDungCNTT_PMQLCN_" + user.Id] = list;

            // Close popup và refresh form main
            ViewBag.RefreshPage = true;
            ViewBag.btnId = "btnRefreshPMCN";

            return View();
        }

        public ActionResult RefreshPMCN()
        {
            var list = (List<UngDungCNTT_PMQLCN>) TempData["UngDungCNTT_PMQLCN_" + S4T_HaTinhBase.GetUserSession().Id];
            TempData["UngDungCNTT_PMQLCN_" + S4T_HaTinhBase.GetUserSession().Id] = list;
            if (list.Any())
            {
                StringBuilder sb = new StringBuilder();
                int i = 1;
                foreach (UngDungCNTT_PMQLCN item in list)
                {
                    string link = Url.Action("_CreateOrUpdatePMCN", "UngDungCNTTs", new { guid = item.Guid });
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", i);
                    sb.AppendFormat("<td>{0}</td>", item.TenPhanMem);
                    sb.AppendFormat("<td>{0}</td>", item.LienThongSBN);
                    sb.AppendFormat("<td>{0}</td>", item.LienThongUBNDCapHuyen);
                    sb.AppendFormat("<td>{0}</td>", item.LienThongUBNDCapXa);
                    sb.AppendFormat("<td>{0}</td>", item.LienThongDVTT);
                    sb.AppendFormat("<td><button type='button' class='k-button k-button-icontext k-grid-edit' onclick=\"javascript:OpenWindow('{0}', 626, 275, true); return false;\"><span class='k-icon k-edit'></span>Sửa</button></td>", link);
                    sb.Append("</tr>");
                    i++;
                }
                return Json(new { danhSach = sb.ToString() });
            }
            return Json(new { msg = "Không tìm thấy danh sách phần mềm chuyên ngành" });
        }

        /// <summary>
        /// Kiểm tra dữ liệu 
        /// </summary>
        public ActionResult Check(int donVi_ID, int lichNhap_ID)
        {
            var returnUrl = Request.UrlReferrer;
            if (returnUrl != null)
                ViewBag.returnUrl = returnUrl.ToString();

            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.UngDungCNTT);
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            GetViewBag(donVi_ID); // Get ViewBag
            ViewBag.LichNhap_ID = lichNhap_ID;
            var ungDungCNTT = db.UngDungCNTT.FirstOrDefault(o => o.DonVi_ID == donVi_ID
                                                                            && o.Success == (byte)TrangThaiNhapLieu.DaGui
                                                                            && o.LichNhap_ID == lichNhap_ID);
            //haTangKyThuatCNTT haTangKyThuatCNTT = await db.HaTangKyThuatCNTT.FindAsync(objReport.);
            if (ungDungCNTT == null)
                return HttpNotFound();

            // Tạo TempData lưu list PMCN
            var list = db.UngDungCNTT_PMQLCN.Where(o => o.UngDungCNTT_ID == ungDungCNTT.UngDungCNTT_ID);
            ungDungCNTT.ListPMQLCN = list.Any() ? list.ToList() : new List<UngDungCNTT_PMQLCN>();
            TempData["UngDungCNTT_PMQLCN_" + user.Id] = ungDungCNTT.ListPMQLCN;
            
            return View(ungDungCNTT);
        }

        /// <summary>
        /// Yêu cầu đơn vị nhập lại số liệu với các trường bị đánh dấu sai
        /// </summary>
        /// <param name="id">ID của báo cáo</param>
        /// <param name="lichNhap_ID">ID của lịch nhập liệu</param>
        /// <param name="truongNhapLai">mã của các trường dữ liệu cần nhập lại</param>
        /// <param name="tuNgay">ngày bắt đầu nhập</param>
        /// <param name="denNgay">ngày kết thúc nhập</param>
        public async Task<ActionResult> NhapLaiRequest(int id, int lichNhap_ID, string truongNhapLai, string tuNgay, string denNgay)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.UngDungCNTT);
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            try
            {
                UngDungCNTT ungdungCNTT = await db.UngDungCNTT.FindAsync(id);
                if (ungdungCNTT == null)
                {
                    return HttpNotFound();
                }

                #region Kiểm tra thời gian nhập liệu
                DateTime _tuNgay;
                DateTime _denNgay;
                var msg = string.Empty;
                if (!DateTime.TryParseExact(tuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _tuNgay))
                    msg = "'Từ ngày' nhập sai định dạng";
                if (!DateTime.TryParseExact(denNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _denNgay))
                    msg =  "'Đến ngày' nhập sai định dạng";

                if (_tuNgay.Date < DateTime.Now.Date)
                    msg = "Thời gian 'Từ ngày' không được nhỏ hơn thời gian hiện tại";
                if (_denNgay.Date < _tuNgay.Date)
                    msg = "Thời gian 'Đến ngày' không được nhỏ hơn thời gian 'Từ ngày'";

                if (!string.IsNullOrEmpty(msg))
                {
                    GetViewBag(ungdungCNTT.DonVi_ID); // Get ViewBag
                    ViewBag.LichNhap_ID = lichNhap_ID;
                    return Json(new { msg = msg });
                }
                #endregion Kiểm tra thời gian nhập liệu

                using (var context = new S4T_HaTinhEntities())
                {
                    #region Update ungdungCNTT
                    ungdungCNTT.TruongNhapLai = truongNhapLai;
                    ungdungCNTT.Success = (byte)TrangThaiNhapLieu.Sua;
                    context.Entry(ungdungCNTT).State = EntityState.Modified;
                    #endregion

                    #region Đổi trạng thái nhập liệu và ngày nhập trong bảng lịch nhập liệu
                    var objLichNhapLieu = await context.Ht_LichNhapLieu.FindAsync(lichNhap_ID);
                    objLichNhapLieu.ChucNang_ID = (byte)TrangThaiNhapLieu.Sua;
                    if (!string.IsNullOrEmpty(tuNgay))
                        objLichNhapLieu.TuNgay = _tuNgay;
                    if (!string.IsNullOrEmpty(denNgay))
                        objLichNhapLieu.DenNgay = _denNgay;
                    context.Entry(objLichNhapLieu).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    #endregion
                }

                return Json(new { ok = "ok" });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }

        /// <summary>
        /// Duyệt báo cáo thành công
        /// </summary>
        /// <param name="id">ID của báo cáo</param>
        /// <param name="lichNhap_ID">ID của lịch nhập liệu</param>
        public async Task<ActionResult> ConfirmReport(int id, int lichNhap_ID)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermissionAdmin(LoaiBaoCao.UngDungCNTT);
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            try
            {
                UngDungCNTT ungdungCNTT = await db.UngDungCNTT.FindAsync(id);
                if (ungdungCNTT == null)
                {
                    return HttpNotFound();
                }

                #region Update haTangKyThuatCNTT
                ungdungCNTT.Success = (byte)TrangThaiNhapLieu.PheDuyet;
                ungdungCNTT.TruongNhapLai = string.Empty;
                db.Entry(ungdungCNTT).State = EntityState.Modified;
                #endregion

                #region Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                var objLichNhapLieu = await db.Ht_LichNhapLieu.FindAsync(lichNhap_ID);
                objLichNhapLieu.ChucNang_ID = (byte)TrangThaiNhapLieu.PheDuyet;
                db.Entry(objLichNhapLieu).State = EntityState.Modified;
                await db.SaveChangesAsync();
                #endregion

                return Json(new { ok = "ok" });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }

        /// <summary>
        /// Xuất báo cáo ra word
        /// </summary>
        /// <param name="id">Mã báo cáo</param>
        public ActionResult ExportToWord(int id)
        {
            UngDungCNTT obj = db.UngDungCNTT.FirstOrDefault(o => o.UngDungCNTT_ID == id);
            if (obj == null) return Content("Không tìm thấy báo cáo");

            string wordTemplate = ExportDoc.GetTemplateByChucNang(Server.MapPath("/Templates"), typeof(UngDungCNTT));
            DocX doc = DocX.Load(wordTemplate);
            if (doc == null) return Content("Không tìm thấy mẫu báo cáo");

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.Name == "ListPMQLCN") continue;
                string value = prop.GetValue(obj) != null ? prop.GetValue(obj).ToString() : "";

                // Đổi dữ liệu trường True/false
                if (listInputRadio.Contains(prop.Name))
                {
                    value = value == "1" ? "Có" : "Không";
                }
                doc.AddCustomProperty(new CustomProperty("@" + prop.Name, value ?? ""));
            }

            #region Add table Phần mềm quản lý chuyên ngành
            IEnumerable<UngDungCNTT_PMQLCN> list = db.UngDungCNTT_PMQLCN.Where(o => o.UngDungCNTT_ID == id);
            if (list.Any())
            {
                PropertyInfo[] props = list.First().GetType().GetProperties();
                int numCol = props.Length - 2; // Bỏ cột Guid , UngDungCNTT_ID
                int numRow = list.Count();

                // Add a Table into the document.  
                Novacode.Table table = doc.AddTable(numRow + 1,numCol);

                //table.Design = TableDesign.ColorfulGridAccent2;  
                table.Design = TableDesign.ColorfulList;
                table.Alignment = Alignment.center;

                // Add header table
                table.Rows[0].Cells[0].Paragraphs[0].Append("STT");
                table.Rows[0].Cells[1].Paragraphs[0].Append("Phần mềm");
                table.Rows[0].Cells[2].Paragraphs[0].Append("SBN");
                table.Rows[0].Cells[3].Paragraphs[0].Append("UBND cấp Huyện");
                table.Rows[0].Cells[4].Paragraphs[0].Append("UBND cấp Xã");
                table.Rows[0].Cells[5].Paragraphs[0].Append("Đơn vị trực thuộc");

                // Add row table
                int i = 1;
                foreach(var item in list)
                {
                    table.Rows[i].Cells[0].Paragraphs[0].Append(i.ToString());
                    table.Rows[i].Cells[1].Paragraphs[0].Append(item.TenPhanMem);
                    table.Rows[i].Cells[2].Paragraphs[0].Append(item.LienThongSBN.ToString());
                    table.Rows[i].Cells[3].Paragraphs[0].Append(item.LienThongUBNDCapHuyen.ToString());
                    table.Rows[i].Cells[4].Paragraphs[0].Append(item.LienThongUBNDCapXa.ToString());
                    table.Rows[i].Cells[5].Paragraphs[0].Append(item.LienThongDVTT.ToString());
                    i++;
                }

                Paragraph p1 = doc.InsertParagraph();

                // Insert the Table after Paragraph 1.
                p1.InsertTableAfterSelf(table);
                
                //doc.Save();
            }            
            #endregion Add table Phần mềm quản lý chuyên ngành

            var storeStream = new MemoryStream();
            doc.SaveAs(storeStream);
            StringBuilder fileName = new StringBuilder();
            fileName.AppendFormat("{0}{1}{2}{3}{4}{5}_{6}", DateTime.Now.Second, DateTime.Now.Minute, DateTime.Now.Hour, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, "UngDungCNTT.docx");
            return File(storeStream.ToArray(), "application/x-msworks", fileName.ToString());
        }

        private string GetAttributeDisplayName(PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(
                typeof(DisplayNameAttribute), true);
            if (atts.Length == 0)
                return null;
            return (atts[0] as DisplayNameAttribute).DisplayName;
        }

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
