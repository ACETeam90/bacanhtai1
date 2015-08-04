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
using System.Data.Entity.Validation;
using System.Text;
using System.Transactions;
using S4T_HaTinh.Models;
using S4T_HaTinh.Common;

namespace S4T_HaTinh.Controllers
{
    public class Ht_LichNhapLieuController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: Ht_LichNhapLieu
        public ActionResult Index(string status, string ddlDonVi, string ddlPhanHe, string ddlTrangThaiNhapLieu, int? namBaoCao, string ddlDotBaoCao)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per == PermissionType.Deny) return Content(ExceptionViewer.GetMessage("VIEW_NOT_PERMISSION"));

            var list = db.Ht_LichNhapLieu.Where(o => o.TrangThai == TrangThai.HoatDong);

            if (list != null && namBaoCao != null)
                list = list.Where(o => o.Nam == namBaoCao);
            
            int trangThai;
            int donVi_ID = 0;
            int phanhe_ID;
            int dotBaoCao;
            if (list != null && !String.IsNullOrEmpty(ddlDotBaoCao) && int.TryParse(ddlDotBaoCao, out dotBaoCao))
                list = list.Where(o => o.DotBaoCao_ID == dotBaoCao);
            if (list != null && !String.IsNullOrEmpty(ddlDonVi) && int.TryParse(ddlDonVi, out donVi_ID))
                list = list.Where(o => o.DonVi_ID == donVi_ID);

            if (list != null && !String.IsNullOrEmpty(ddlPhanHe) && int.TryParse(ddlPhanHe, out phanhe_ID))
                list = list.Where(o => o.PhanHe_ID == phanhe_ID);

            if (list != null && !String.IsNullOrEmpty(ddlTrangThaiNhapLieu) && int.TryParse(ddlTrangThaiNhapLieu, out trangThai))
                list = list.Where(o => o.ChucNang_ID == trangThai);

            if (string.IsNullOrEmpty(status))
                list = list.Where(o => o.ChucNang_ID == TrangThaiNhapLieu.ThemMoi);

            GetViewBag(ViewReport.Index, donVi_ID, null);

            return View(list);
        }

        private void GetViewBag(ViewReport type, int donVi_ID, int? nhomDonVi_ID)
        {
            var objDonVi = new Dm_DonVi();
            IEnumerable<Dm_DonVi> listDonVi = new List<Dm_DonVi>();

            if (type == ViewReport.Index)
            {
                listDonVi = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong);
                
                if (donVi_ID == 0)
                {
                    objDonVi = listDonVi.FirstOrDefault();
                    ViewBag.ListPhanHeChucNang = S4T_HaTinhBase.GetListBaoCao(null, TrangThai.HoatDong).OrderBy(o => o.TenChucNang);
                }
                else
                {
                    objDonVi = MvcApplication.ListDonVi.FirstOrDefault(o => o.DonVi_ID == donVi_ID);

                    // Danh mục phân hệ nhập liệu
                    if (objDonVi != null && objDonVi.NhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                        ViewBag.ListPhanHeChucNang = S4T_HaTinhBase.GetListBaoCao(DonVi.NhomDonViCapHuyen, TrangThai.HoatDong).OrderBy(o => o.TenChucNang);
                    else if (objDonVi != null)
                        ViewBag.ListPhanHeChucNang = S4T_HaTinhBase.GetListBaoCao(DonVi.NhomDonViCapTinh, TrangThai.HoatDong).OrderBy(o => o.TenChucNang);
                }
            }
            else if (type == ViewReport.Create)
            {
                // Nhóm đơn vị
                ViewBag.ListNhomDonVi = MvcApplication.ListNhomDonVi(); 

                if (nhomDonVi_ID == null)
                {
                    var firstNhomDonVi = MvcApplication.ListNhomDonVi().Where(o => o.DanhMuc_ID != S4T_HaTinh.Common.DonVi.NhomDonViCapXa).FirstOrDefault();
                    listDonVi = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong && o.NhomDonVi_ID == firstNhomDonVi.DanhMuc_ID);
                }
                else
                {
                    listDonVi = MvcApplication.ListDonVi.Where(o => o.TrangThai == TrangThai.HoatDong && o.NhomDonVi_ID == nhomDonVi_ID);
                }
                
                objDonVi = listDonVi.FirstOrDefault();

                // Danh mục phân hệ nhập liệu
                if (objDonVi != null)
                {
                    // Danh mục phân hệ nhập liệu
                    if (objDonVi.NhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                        ViewBag.ListPhanHeChucNang = S4T_HaTinhBase.GetListBaoCao(DonVi.NhomDonViCapHuyen, TrangThai.HoatDong).OrderBy(o => o.TenChucNang);
                    else
                        ViewBag.ListPhanHeChucNang = S4T_HaTinhBase.GetListBaoCao(DonVi.NhomDonViCapTinh, TrangThai.HoatDong).OrderBy(o => o.TenChucNang);
                }
            }

            ViewBag.SelectListDonVi = new SelectList(listDonVi.OrderBy(o => o.TenDonVi), "DonVi_ID", "TenDonVi");
                        
            // Năm báo cáo
            //ViewBag.ListNamBaoCao = Enumerable.Range(DateTime.Now.Year - 4, 9).ToList();

            // Đợt báo cáo
            ViewBag.ListDotBaoCao = MvcApplication.ListDotBaoCao().Where(o => o.TrangThai == TrangThai.HoatDong);
        }

        // GET: Ht_LichNhapLieu/Create
        public ActionResult Create()
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) 
                return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            ViewBag.TrangThaiNhapLieu = MvcApplication.ListTrangThaiNhapLieu().FirstOrDefault(o => o.DanhMuc_ID == TrangThaiNhapLieu.ThemMoi).DanhMuc_ID; // Lấy mặc định là Thêm mới

            GetViewBag(ViewReport.Create, 0, null);
            var obj = new Ht_LichNhapLieu
            {
                Nam = DateTime.Now.Year
            };

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ht_LichNhapLieu ht_LichNhapLieu, int ListNhomDonVi, string strListDonVi, string strListPhanHe)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return ReturnView(Request.RequestContext.RouteData.GetRequiredString("action"), ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"), ListNhomDonVi);

            //if (ht_LichNhapLieu.TuNgay.Date < DateTime.Now.Date)
            //    ModelState.AddModelError("TuNgay", "Thời gian 'Từ ngày' không được nhỏ hơn thời gian hiện tại");
            if (ht_LichNhapLieu.DenNgay.Date < ht_LichNhapLieu.TuNgay.Date)
                ModelState.AddModelError("DenNgay", "Thời gian 'Đến ngày' không được nhỏ hơn thời gian 'Từ ngày'");

            if (string.IsNullOrEmpty(strListPhanHe)) ModelState.AddModelError("", "Chưa chọn báo cáo");
            if (string.IsNullOrEmpty(strListDonVi)) ModelState.AddModelError("", "Chưa chọn đơn vị");

            if (ModelState.IsValid)
            {
                var listPhanHe = strListPhanHe.Split(';');
                var listDonVi = strListDonVi.Split(';');

                if (listDonVi.Any())
                {
                    try
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            var listLichNhap = db.Ht_LichNhapLieu.Where(o => o.TrangThai == TrangThai.HoatDong);

                            foreach (var phanHe in listPhanHe)
                            {
                                for (int i = 0; i < listDonVi.Count(); i++)
                                {
                                    var donVi_ID = Convert.ToInt32(listDonVi[i]);
                                    var phanHe_ID = Convert.ToInt32(phanHe);

                                    // Kiểm tra nếu chưa có lịch cho đơn vị tạo báo cáo đó thì tạo
                                    var hasCalendar = listLichNhap.FirstOrDefault(o => o.DonVi_ID == donVi_ID
                                                                        && o.PhanHe_ID == phanHe_ID
                                                                        && o.ChucNang_ID != TrangThaiNhapLieu.PheDuyet
                                                                        && o.Nam == ht_LichNhapLieu.Nam
                                                                        && o.DotBaoCao_ID == ht_LichNhapLieu.DotBaoCao_ID);
                                    if (hasCalendar != null)
                                    {
                                        scope.Dispose();
                                        var objDonVi = MvcApplication.ListDonVi.FirstOrDefault(o => o.DonVi_ID == hasCalendar.DonVi_ID);
                                        var objBaoCao = db.Ht_PhanHeChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == phanHe_ID);
                                        ModelState.AddModelError("", "Đang tồn tại lịch nhập báo cáo " + objBaoCao.TenChucNang + " của đơn vị " + objDonVi.TenDonVi + " có trạng thái hoạt động trong hệ thống");
                                        GetViewBag(ViewReport.Create, 0, ListNhomDonVi);

                                        return View(ht_LichNhapLieu);
                                        //return ReturnView(Request.RequestContext.RouteData.GetRequiredString("action"), "Đang tồn tại lịch nhập báo cáo " + objBaoCao.TenChucNang + " của đơn vị " + objDonVi.TenDonVi + " có trạng thái hoạt động trong hệ thống", ListNhomDonVi);
                                    }

                                    ht_LichNhapLieu.DonVi_ID = donVi_ID;
                                    ht_LichNhapLieu.PhanHe_ID = phanHe_ID;
                                    ht_LichNhapLieu.ChucNang_ID = TrangThaiNhapLieu.ThemMoi;
                                    ht_LichNhapLieu.TrangThai = TrangThai.HoatDong;
                                    db.Ht_LichNhapLieu.Add(ht_LichNhapLieu);
                                    db.SaveChanges();
                                }
                            }
                                
                            scope.Complete();

                            return RedirectToAction("Index");
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var exc = new ExceptionViewer();
                        exc.GetError(ex);
                    }
                }
            }

            GetViewBag(ViewReport.Create, 0, ListNhomDonVi);
            return View(ht_LichNhapLieu);
        }

        // GET: Ht_LichNhapLieu/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ht_LichNhapLieu ht_LichNhapLieu = await db.Ht_LichNhapLieu.FindAsync(id);

            //GetViewBag(ht_LichNhapLieu.DonVi_ID);
            ViewBag.TenDonVi = db.Dm_DonVi.FirstOrDefault(o => o.DonVi_ID == ht_LichNhapLieu.DonVi_ID).TenDonVi;
            ViewBag.TenPhanHe = db.Ht_PhanHeChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == ht_LichNhapLieu.PhanHe_ID).TenChucNang;

            if (ht_LichNhapLieu == null)
                return HttpNotFound();
            
            return View(ht_LichNhapLieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Ht_LichNhapLieu ht_LichNhapLieu)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            //if (ht_LichNhapLieu.TuNgay.Date < DateTime.Now.Date)
            //    ModelState.AddModelError("TuNgay", "Thời gian 'Từ ngày' không được nhỏ hơn thời gian hiện tại");
            if (ht_LichNhapLieu.DenNgay < ht_LichNhapLieu.TuNgay)
                ModelState.AddModelError("DenNgay", "Thời gian 'Đến ngày' không được nhỏ hơn thời gian 'Từ ngày'");

            if (ModelState.IsValid)
            {
                var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.LichNhap_ID == ht_LichNhapLieu.LichNhap_ID);
                objLichNhap.DenNgay = ht_LichNhapLieu.DenNgay;
                objLichNhap.TuNgay = ht_LichNhapLieu.TuNgay;
                objLichNhap.DotBaoCao_ID = ht_LichNhapLieu.DotBaoCao_ID;
                objLichNhap.Nam = ht_LichNhapLieu.Nam;

                db.Entry(objLichNhap).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình lưu dữ liệu");
            //GetViewBag(ht_LichNhapLieu.DonVi_ID);
            ViewBag.TenDonVi = db.Dm_DonVi.FirstOrDefault(o => o.DonVi_ID == ht_LichNhapLieu.DonVi_ID).TenDonVi;
            ViewBag.TenPhanHe = db.Ht_PhanHeChucNang.FirstOrDefault(o => o.PhanHeChucNang_ID == ht_LichNhapLieu.PhanHe_ID).TenChucNang;
            return View(ht_LichNhapLieu);
        }

        // GET: Ht_LichNhapLieu/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (S4T_HaTinhBase.GetUserSession() == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
            var per = S4T_HaTinhBase.CheckPermission(Request.RequestContext.RouteData.GetRequiredString("controller"));
            if (per != PermissionType.Write) return Content(ExceptionViewer.GetMessage("UPDATE_NOT_PERMISSION"));

            if (id == null)
                return Json(new { msg = "Không tìm thấy ID dữ liệu" });
            
            Ht_LichNhapLieu ht_LichNhapLieu = await db.Ht_LichNhapLieu.FindAsync(id);
            if (ht_LichNhapLieu == null)
                return Json(new { msg = "Không tìm thấy dữ liệu trong database" });
            
            if(ht_LichNhapLieu.ChucNang_ID != TrangThaiNhapLieu.ThemMoi)
                return Json(new { msg = "Báo cáo đã được tạo, không thể xóa lịch nhập" });

            ht_LichNhapLieu.TrangThai = TrangThai.KhongHoatDong;
            db.Entry(ht_LichNhapLieu).State = EntityState.Modified;
            await db.SaveChangesAsync();
            
            return Json(new { msg = "ok" });
        }

        // Return lại View cũ nếu xảy ra lỗi
        private ActionResult ReturnView(string viewName, string mess, int ListNhomDonVi)
        {
            ModelState.AddModelError("", mess);
            GetViewBag(ViewReport.Create, 0, ListNhomDonVi);
            return View(viewName);
        }

        #region Event
        /// <summary>
        /// Lấy danh sách đơn vị theo Nhóm đơn vị
        /// </summary>
        /// <param name="nhomDonVi_ID">ID của nhóm đơn vị</param>
        public ActionResult ChangeListDonVi(int nhomDonVi_ID)
        {
            try
            {
                var str = new StringBuilder();
                var list = S4T_HaTinhBase.ListDonViByNhomDonVi(nhomDonVi_ID);

                str.Append("<select id='ListDonVi' name='ListDonVi' onchange='ChangeListBaoCao()' style='width: 300px; display: none;' multiple=''>");
                str.Append("<option value=''>Tất cả</option>");

                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        str.AppendFormat("<option value='{0}'>{1}</option>", item.DonVi_ID, item.TenDonVi);
                    }
                }
                str.Append("</select>");

                return Json(new { danhSach = str.ToString() });
            }
            catch (Exception ex)
            {
                var exv = new ExceptionViewer(ex);
                return Json(new { msg = exv.GetErrorMessage(exv.sMessege) });
            }
        }

        public ActionResult ChangeListBaoCao(int nhomDonVi_ID, string view)
        {
            //var objDonVi = MvcApplication.ListDonVi.FirstOrDefault(o => o.DonVi_ID == donVi_ID);
            IEnumerable<Ht_PhanHeChucNang> listPhanHeChucNang = new List<Ht_PhanHeChucNang>();

            // Danh mục phân hệ nhập liệu
            if (nhomDonVi_ID == DonVi.NhomDonViCapHuyen)
                listPhanHeChucNang = S4T_HaTinhBase.GetListBaoCao(DonVi.NhomDonViCapHuyen, TrangThai.HoatDong).OrderBy(o => o.TenChucNang);
            else
                listPhanHeChucNang = S4T_HaTinhBase.GetListBaoCao(DonVi.NhomDonViCapTinh, TrangThai.HoatDong).OrderBy(o => o.TenChucNang);

            var str = new StringBuilder();
            if (view == "Index")
                str.Append("<option value=''>Tất cả</option>");
            else
            {
                str.Append("<select id='ListPhanHe' name='ListPhanHe' style='width: 300px; display: none;' multiple=''>");
                str.Append("<option value=''>Tất cả</option>");
            }

            if (listPhanHeChucNang.Any())
            {
                
                foreach (var item in listPhanHeChucNang)
                {
                    str.AppendFormat("<option value='{0}'>{1}</option>", item.PhanHeChucNang_ID, item.TenChucNang);
                }
                return Json(new { danhSach = str.ToString() });
            }

            return Json(new { msg = "Không tìm thấy danh mục báo cáo" });
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
