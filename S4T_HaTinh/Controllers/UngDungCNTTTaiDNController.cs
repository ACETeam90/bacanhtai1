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

namespace S4T_HaTinh.Controllers
{
    public class UngDungCNTTTaiDNController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        // GET: /UngDungCNTTTaiDN/
        public ActionResult Index()
        {
            //Dữ liệu test là Phòng CNTT


            int donVi_ID = 3;  // Đơn vị test
            GetViewBag();

            var phanHe_ID = Convert.ToInt32(ConfigurationManager.AppSettings["UngDungCNTTTaiDN"]);
            var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.DonVi_ID == donVi_ID && o.PhanHe_ID == phanHe_ID);
            if (objLichNhap != null)
            {
                // Lấy ~ bản ghi đang có hiệu lực trong db, Success != Thêm mới và Thành công
                var objUngDungCNTTTaiDN = db.UngDungCNTTTaiDN.FirstOrDefault(o => o.DonVi_ID == donVi_ID && o.Success != TrangThaiNhapLieu.PheDuyet);

                // Đã có bản ghi nhập liệu
                if (objUngDungCNTTTaiDN != null)
                {
                    if (objUngDungCNTTTaiDN.Success == (byte)TrangThaiNhapLieu.Sua)
                    {
                        // Kiểm tra thời gian
                        if (objLichNhap.TuNgay <= DateTime.Now && objLichNhap.DenNgay >= DateTime.Now)
                            return RedirectToAction("Edit?id=" + objUngDungCNTTTaiDN.UngDungCNTTTaiDN_ID);
                        else
                            return View(db.UngDungCNTTTaiDN.Where(o => o.UngDungCNTTTaiDN_ID == objUngDungCNTTTaiDN.UngDungCNTTTaiDN_ID));
                    }
                }
                else // Chưa có bản ghi thì chuyển sang View Create (Thêm mới)
                {
                    return RedirectToAction("Create", new { donVi_ID = donVi_ID, tuNgay = objLichNhap.TuNgay, denNgay = objLichNhap.DenNgay });
                }
                return View(db.UngDungCNTTTaiDN.Where(o => o.DonVi_ID == donVi_ID && o.Success != TrangThaiNhapLieu.PheDuyet));
            }
            else
                return View();
        }

        /// <summary>
        /// Lấy các ViewBag
        /// </summary>
        private void GetViewBag()
        {
            int donVi_ID = 3;  // Đơn vị test

            // Đơn vị
            if (db.Dm_DonVi.Count() > 0)
                ViewBag.ListDonVi = db.Dm_DonVi;
            var objDonVi = db.Dm_DonVi.FirstOrDefault(o => o.DonVi_ID == donVi_ID);
            ViewBag.TenDonVi = objDonVi.TenDonVi ?? "";
            ViewBag.DiaChi = objDonVi.DiaChi ?? "";
            ViewBag.NganhNghe = db.Dm_DanhMucChung.FirstOrDefault(o => o.DanhMuc_ID == objDonVi.NganhNghe_ID).TenDanhMuc ?? "";
            ViewBag.QuyMo = objDonVi.QuyMo.ToString() ?? "";
        }

        // GET: /UngDungCNTTTaiDN/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UngDungCNTTTaiDN ungdungcntttaidn = await db.UngDungCNTTTaiDN.FindAsync(id);
            if (ungdungcntttaidn == null)
            {
                return HttpNotFound();
            }
            return View(ungdungcntttaidn);
        }

        // GET: /UngDungCNTTTaiDN/Create
        public ActionResult Create(int donVi_ID)
        {
            GetViewBag();
            UngDungCNTTTaiDN obj = new UngDungCNTTTaiDN();
            obj.Success = (byte)TrangThaiNhapLieu.ThemMoi;
            obj.DonVi_ID = donVi_ID;            
            return View(obj);
        }

        // POST: /UngDungCNTTTaiDN/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="UngDungCNTTTaiDN_ID,DonVi_ID,FileUpload,DiaChi,NguoiDaiDien,SoDienThoai,NganhNgheKinhDoanh,LAN,LoaiHinhDoanhNghiep,DoanhThu,SoMayTinh,PhanMemDangUngDung,SoWebsite,KienNghi,Success,TruongNhapLai")] UngDungCNTTTaiDN ungdungcntttaidn)
        {
            if (ModelState.IsValid)
            {
                // Đổi trạng thái nhập liệu
                ungdungcntttaidn.Success = (byte)TrangThaiNhapLieu.DaGui;
                db.UngDungCNTTTaiDN.Add(ungdungcntttaidn);
                await db.SaveChangesAsync();

                // Đổi trạng thái nhập liệu trong bảng lịch nhập liệu
                var phanHe_ID = Convert.ToInt32(ConfigurationManager.AppSettings["UngDungCNTTTaiDNs"]);
                var objLichNhap = db.Ht_LichNhapLieu.FirstOrDefault(o => o.DonVi_ID == ungdungcntttaidn.DonVi_ID && o.PhanHe_ID == phanHe_ID);
                objLichNhap.ChucNang_ID = TrangThaiNhapLieu.DaGui;
                db.Entry(objLichNhap).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(ungdungcntttaidn);
        }

        // GET: /UngDungCNTTTaiDN/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UngDungCNTTTaiDN ungdungcntttaidn = await db.UngDungCNTTTaiDN.FindAsync(id);
            if (ungdungcntttaidn == null)
            {
                return HttpNotFound();
            }
            return View(ungdungcntttaidn);
        }

        // POST: /UngDungCNTTTaiDN/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="UngDungCNTTTaiDN_ID,DonVi_ID,FileUpload,DiaChi,NguoiDaiDien,SoDienThoai,NganhNgheKinhDoanh,LAN,LoaiHinhDoanhNghiep,DoanhThu,SoMayTinh,PhanMemDangUngDung,SoWebsite,KienNghi,Success,TruongNhapLai")] UngDungCNTTTaiDN ungdungcntttaidn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ungdungcntttaidn).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ungdungcntttaidn);
        }

        // GET: /UngDungCNTTTaiDN/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UngDungCNTTTaiDN ungdungcntttaidn = await db.UngDungCNTTTaiDN.FindAsync(id);
            if (ungdungcntttaidn == null)
            {
                return HttpNotFound();
            }
            return View(ungdungcntttaidn);
        }

        // POST: /UngDungCNTTTaiDN/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UngDungCNTTTaiDN ungdungcntttaidn = await db.UngDungCNTTTaiDN.FindAsync(id);
            db.UngDungCNTTTaiDN.Remove(ungdungcntttaidn);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
