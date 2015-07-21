using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;
using System.Globalization;

namespace S4T_HaTinh.Controllers
{
    public class TinhHinhSXDNController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();
        //private string currentController = "TinhHinhSXDN";//System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");        
        private Ht_PhanHeChucNang objChucNang = new S4T_HaTinhEntities().Ht_PhanHeChucNang.FirstOrDefault(o => o.ControllerName == "TinhHinhSXDN");

        // GET: TinhHinhSXDNs
        public async Task<ActionResult> Index()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/TinhHinhSXDN/Index" });
            }
            else
            {
                var objDonVi = db.Dm_DonVi.Find(user.DonVi_ID);
                ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";
                ViewBag.CreateLink = CheckCreateStatus(user) == true ? "<a href='/TinhHinhSXDN/Create'>Tạo mới</a>" : "";
                ViewBag.ListLichNhap = db.Ht_LichNhapLieu.Where(o => o.DonVi_ID == user.DonVi_ID && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID);
                return View(await db.TinhHinhSXDN.ToListAsync());
            }
        }

        /// <summary>
        /// Check trạng thái "Tạo mới" theo lịch nhập liệu 
        /// </summary>
        /// <param name="user">object người dùng</param>
        private bool CheckCreateStatus(ApplicationUser user)
        {
            var _objLichNhapLieu = S4T_HaTinhBase.GetTrangThaiLichNhapLieu(user, objChucNang.PhanHeChucNang_ID);
            if (_objLichNhapLieu.TrangThai == TrangThaiLichNhapLieu.HoatDong)
            {
                // Kiểm tra đã tồn tại bản ghi trong database?
                //var customStatus = new[] { TrangThaiNhapLieu.DaGui, TrangThaiNhapLieu.Sua, TrangThaiNhapLieu.ThanhCong};
                var objReport = db.TinhHinhSXDN.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                                                       && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                                                       && o.Success != TrangThaiNhapLieu.PheDuyet);
                return objReport == null ? true : false;
            }
            return false;
        }

        /// <summary>
        /// Check trạng thái "Sửa" theo lịch nhập liệu 
        /// </summary>
        /// <param name="user">object người dùng</param>
        private bool CheckEditStatus(ApplicationUser user)
        {
            var _objLichNhapLieu = S4T_HaTinhBase.GetTrangThaiLichNhapLieu(user, objChucNang.PhanHeChucNang_ID);
            if (_objLichNhapLieu.TrangThai == TrangThaiLichNhapLieu.HoatDong)
            {
                // Kiểm tra đã tồn tại bản ghi trong database?
                var objReport = db.TinhHinhSXDN.FirstOrDefault(o => o.DonVi_ID == user.DonVi_ID
                                                                       && o.LichNhap_ID == _objLichNhapLieu.LichNhapLieu.LichNhap_ID
                                                                       && o.Success == (byte)TrangThaiNhapLieu.Sua);
                return objReport != null ? true : false;
            }
            return false;
        }

        /// <summary>
        /// Lấy các ViewBag
        /// </summary>
        private void GetViewBag(ApplicationUser user)
        {
            // Đơn vị
            var objDonVi = db.Dm_DonVi.Find(user.DonVi_ID);
            ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";
            var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.TrangThai == TrangThai.HoatDong
                                                            && o.DonVi_ID == user.DonVi_ID
                                                            && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                            && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
            if (objLichNhap != null)
            {
                ViewBag.TuNgay = objLichNhap.TuNgay;
                ViewBag.DenNgay = objLichNhap.DenNgay;
            }
            ViewBag.listInputRadio = ""; // Form ko dung radio thi de? tro^'ng
        }

        // GET: TinhHinhSXDNs/Details/5
        public async Task<ActionResult> Details(int? id, int? donVi_ID, int? lichNhap_ID)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/TinhHinhSXDN/Details" });
            }
            else
            {
                var obj = new TinhHinhSXDN();
                if (id == null)
                {
                    if (lichNhap_ID == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    else
                        obj = await db.TinhHinhSXDN.FirstOrDefaultAsync(o => o.LichNhap_ID == lichNhap_ID
                                                                                && o.DonVi_ID == donVi_ID);
                }
                else
                    obj = await db.TinhHinhSXDN.FindAsync(id);

                var objDonVi = db.Dm_DonVi.FirstOrDefault(o => o.DonVi_ID == obj.DonVi_ID);
                ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";

                if (obj == null)
                    return HttpNotFound();

                return View(obj);
            }
        }

        // GET: TinhHinhSXDNs/Create
        public ActionResult Create()
        {
            var user = S4T_HaTinhBase.GetUserSession();
            GetViewBag(user);
            if (CheckCreateStatus(user))
            {
                TinhHinhSXDN obj = new TinhHinhSXDN();
                obj.Success = (byte)TrangThaiNhapLieu.ThemMoi;
                obj.DonVi_ID = user.DonVi_ID;

                return View(obj);
            }
            else
                return View();
        }

        // POST: TinhHinhSXDNs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TinhHinhSXDN tinhHinhSXDN)
        {
            if (ModelState.IsValid)
            {
                var user = S4T_HaTinhBase.GetUserSession();
                if (CheckCreateStatus(user))
                {
                    var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.DonVi_ID == tinhHinhSXDN.DonVi_ID
                                                                          && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                          && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);

                    // Đổi trạng thái nhập liệu
                    tinhHinhSXDN.Success = (byte)TrangThaiNhapLieu.DaGui;
                    tinhHinhSXDN.LichNhap_ID = objLichNhap.LichNhap_ID; // Add LichNhap_ID vào báo cáo
                    db.TinhHinhSXDN.Add(tinhHinhSXDN);
                    await db.SaveChangesAsync();

                    // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                    //objLichNhap.BaoCao_ID = haTangNhanLucCNTT.HaTangNhanLucCNTT_ID;
                    objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                    db.Entry(objLichNhap).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }

            return View(tinhHinhSXDN);
        }

        // GET: TinhHinhSXDNs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/TinhHinhSXDNs/Index" });
            else
            {
                if (CheckEditStatus(user))
                {
                    GetViewBag(user);
                    TinhHinhSXDN tinhHinhSXDN = await db.TinhHinhSXDN.FindAsync(id);
                    if (tinhHinhSXDN == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tinhHinhSXDN);
                }
            }
            return View();
        }

        // POST: TinhHinhSXDNs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TinhHinhSXDN tinhHinhSXDN)
        {
            if (ModelState.IsValid)
            {
                var user = S4T_HaTinhBase.GetUserSession();
                if (CheckEditStatus(user))
                {
                    using (var context = new S4T_HaTinhEntities())
                    {
                        tinhHinhSXDN.Success = (byte)TrangThaiNhapLieu.DaGui;
                        //haTangNhanLucCNTT.TruongNhapLai = string.Empty; // Xóa hết các yêu cầu nhập lại dữ liệu
                        context.Entry(tinhHinhSXDN).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                    // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                    var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.DonVi_ID == tinhHinhSXDN.DonVi_ID
                                                                          && o.PhanHe_ID == objChucNang.PhanHeChucNang_ID
                                                                          && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet);
                    objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                    db.Entry(objLichNhap).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            return View(tinhHinhSXDN);
        }

        /// <summary>
        /// Kiểm tra dữ liệu 
        /// </summary>
        [Authorize(Roles = "Admin,ChuyenVien")]
        public ActionResult Check(int donVi_ID, int lichNhap_ID)
        {
            var user = S4T_HaTinhBase.GetUserSession();
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/TinhHinhSXDNs/Check?donVi_ID=" + donVi_ID + "&lichNhap_ID=" + lichNhap_ID });
            }
            else
            {
                GetViewBag(user); // Get ViewBag
                ViewBag.LichNhap_ID = lichNhap_ID;
                var tinhHinhSXDN = db.TinhHinhSXDN.FirstOrDefault(o => o.DonVi_ID == donVi_ID
                                                                               && o.Success == (byte)TrangThaiNhapLieu.DaGui
                                                                               && o.LichNhap_ID == lichNhap_ID);
                //haTangNhanLucCNTT haTangNhanLucCNTT = await db.haTangNhanLucCNTT.FindAsync(objReport.);
                if (tinhHinhSXDN == null)
                {
                    return HttpNotFound();
                }
                return View(tinhHinhSXDN);
            }
        }

        [Authorize(Roles = "Admin,ChuyenVien")]
        public ActionResult NhapLaiRequest(int id, int lichNhap_ID, string truongNhapLai, string tuNgay, string denNgay)
        {
            try
            {
                TinhHinhSXDN tinhHinhSXDN = db.TinhHinhSXDN.Find(id);
                if (tinhHinhSXDN == null)
                {
                    return HttpNotFound();
                }

                #region Update tinhHinhSXDN
                tinhHinhSXDN.TruongNhapLai = truongNhapLai;
                tinhHinhSXDN.Success = (byte)TrangThaiNhapLieu.Sua;
                db.Entry(tinhHinhSXDN).State = EntityState.Modified;
                db.SaveChanges();
                #endregion

                #region Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                Ht_LichNhapLieu objLichNhapLieu = db.Ht_LichNhapLieu.Find(lichNhap_ID);
                objLichNhapLieu.ChucNang_ID = (byte)TrangThaiNhapLieu.Sua;
                if (!string.IsNullOrEmpty(tuNgay))
                    objLichNhapLieu.TuNgay = DateTime.ParseExact(tuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(denNgay))
                    objLichNhapLieu.DenNgay = DateTime.ParseExact(denNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                db.Entry(objLichNhapLieu).State = EntityState.Modified;
                db.SaveChanges();
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
        /// Duyệt báo cáo thành công
        /// </summary>
        /// <param name="id">ID của báo cáo</param>
        /// <param name="lichNhap_ID">ID của lịch nhập liệu</param>
        [Authorize(Roles = "Admin,ChuyenVien")]
        public ActionResult ConfirmReport(int id, int lichNhap_ID)
        {
            try
            {
                TinhHinhSXDN tinhHinhSXDN = db.TinhHinhSXDN.Find(id);
                if (tinhHinhSXDN == null)
                {
                    return HttpNotFound();
                }

                #region Update tinhHinhSXDN
                tinhHinhSXDN.Success = (byte)TrangThaiNhapLieu.PheDuyet;
                tinhHinhSXDN.TruongNhapLai = string.Empty;
                db.Entry(tinhHinhSXDN).State = EntityState.Modified;
                db.SaveChanges();
                #endregion

                #region Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                Ht_LichNhapLieu objLichNhapLieu = db.Ht_LichNhapLieu.Find(lichNhap_ID);
                objLichNhapLieu.ChucNang_ID = (byte)TrangThaiNhapLieu.PheDuyet;
                db.Entry(objLichNhapLieu).State = EntityState.Modified;
                db.SaveChanges();
                #endregion

                return Json(new { ok = "ok" });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
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
