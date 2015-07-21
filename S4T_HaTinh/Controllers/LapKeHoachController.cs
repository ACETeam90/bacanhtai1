using S4T_HaTinh.Common;
using S4T_HaTinh.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace S4T_HaTinh.Controllers
{
    public class LapKeHoachController : Controller
    {
        private S4T_HaTinhEntities db = new S4T_HaTinhEntities();

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index(int? ddlLoaiCV, string TuNgay, string DenNgay, string NguoiThucHien, string ac = "")
        {

            var user = (ApplicationUser)S4T_HaTinhBase.GetUserSession();
            if (user == null)   return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            string action = "";
            if (Session["ac"] != null)
                action = Session["ac"].ToString();

            if (action == "" || (ac != "" && ac != action))
            {
                Session["ac"] = ac;
                action = ac;
            }

            GetViewBag();

            if (string.IsNullOrEmpty(TuNgay))
            {
                TuNgay = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            }
            if (string.IsNullOrEmpty(DenNgay))
            {
                DenNgay = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
            }

            ViewBag.TuNgay = TuNgay;
            ViewBag.DenNgay = DenNgay;

            if (action == "TraLoiYKien")
            {
                var lstXYK = db.TN_Chuyen.Where(o => o.Active == 1 && o.NguoiNhan_ID.Contains(user.Id) && o.LoaiChuyen == LoaiChuyen.XinYKien);
                var listData = db.TN_DauViec.Where(o =>
                        lstXYK.Any(u => u.Viec_ID == o.ID)
                    );
                return View("TraLoi", listData);
            }

            if (ddlLoaiCV != null && !string.IsNullOrEmpty(TuNgay)) // && !string.IsNullOrEmpty(DenNgay))
            {
                var tn = new DateTime();
                var dn = new DateTime();
                try
                {
                    tn = DateTime.ParseExact(TuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dn = DateTime.ParseExact(DenNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    ViewBag.Err = "Sai định dạng ngày tháng";
                    return View();
                }

                var listData = db.TN_DauViec.Where(o =>
                       o.TuNgay >= tn &&
                       o.TuNgay <= dn &&
                       o.LoaiViec == ddlLoaiCV &&
                       o.Parent_ID == null &&
                       (o.NguoiTao_ID == user.Id || o.NguoiThucHien_ID.Contains(user.Id) || o.Display.Contains(user.Id) 
                       //|| o.NguoiDuyet_ID == user.Id 
                       )
                       );
                if (action == "DangThucHien")
                {
                    listData = listData.Where(o => o.TrangThai != TrangThaiDauViec.KetThuc);
                }
                if (action == "SapQuaHan")
                {
                    listData = listData.Where(o => o.DenNgay == DateTime.Now.AddDays(-2));
                }
                if (action == "QuaHan")
                {
                    listData = listData.Where(o => o.DenNgay > DateTime.Now);
                }
                if (action == "HoanThanh")
                {
                    listData = listData.Where(o => o.TrangThai == TrangThaiDauViec.KetThuc);
                }
                //if (!string.IsNullOrEmpty(NguoiThucHien))
                //    listData = listData.Where(o => o.NguoiThucHien_ID.Contains(NguoiThucHien));
                if (!string.IsNullOrEmpty(NguoiThucHien))
                    listData = listData.Where(o => o.Display.Contains(NguoiThucHien));

                return View(listData);
            }
            return View();
        }

        public ActionResult Create(int? ParentID, string returnUrl)
        {
            var objCauHinh = db.Ht_ThamSoHeThong.First(o => o.MaThamSo == "SepID");
            GetViewBag();
            var user = (ApplicationUser)S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            var obj = new TN_DauViec();
            ViewBag.returnUrl = returnUrl;
            if (ParentID != null)
            {
                //ViewBag.returnUrl = "/LapKeHoach/Detail/" + ParentID;
                obj.PhamVi = PhamVi.CaNhan;
                obj.Parent_ID = ParentID;
                return View(obj);
            }

            if (objCauHinh != null && !string.IsNullOrEmpty(objCauHinh.GiaTriThamSo) && objCauHinh.GiaTriThamSo.Contains(user.Id))
            {
                return View();
            }
            obj.PhamVi = PhamVi.CaNhan;
            return View(obj);
        }

        public ActionResult Edit(int ID, string returnUrl)
        {
            var obj = db.TN_DauViec.First(a => a.ID == ID);
            ViewBag.returnUrl = returnUrl;
            var objCauHinh = db.Ht_ThamSoHeThong.First(o => o.MaThamSo == "SepID");
            GetViewBag();
            return View("Create", obj);
        }

        public ActionResult TienDo(int tiendo, int id)
        {
            var obj = db.TN_DauViec.FirstOrDefault(o => o.ID == id);
            obj.TienDo = tiendo;

            if (tiendo == 100 && DateTime.Now.Date <= obj.DenNgay)
                obj.OnTime = Han.HoanThanhDungHan;
            else
                if (tiendo == 100 && DateTime.Now.Date > obj.DenNgay)
                    obj.OnTime = Han.HoanThanhMuon;
                else
                    obj.OnTime = null;

            if (obj.Parent_ID != null)
            {
                var lstParent = db.TN_DauViec.Where(o => o.Parent_ID == obj.Parent_ID);
                var percent = 0;
                if (lstParent != null && lstParent.Any())
                {
                    foreach (var item in lstParent)
                    {
                        if (item.TienDo != null)
                            percent += item.TienDo.Value;
                    }

                    percent = percent / (lstParent.Count());
                }
                if (percent != 0)
                {
                    var objCha = db.TN_DauViec.First(o => o.ID == obj.Parent_ID);
                    objCha.TienDo = percent;
                    if (percent == 100 && DateTime.Now.Date <= objCha.DenNgay)
                        objCha.OnTime = Han.HoanThanhDungHan;
                    else
                        if (percent == 100 && DateTime.Now.Date > objCha.DenNgay)
                            objCha.OnTime = Han.HoanThanhMuon;
                        else
                            objCha.OnTime = null;
                    db.Entry(objCha).State = EntityState.Modified;
                }
            }

            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { });
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Detail(int ID, string returnUrl)
        {
            var user = (ApplicationUser)S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            var lst = db.TN_DauViec.Where(o =>
                (o.Parent_ID == ID || o.ID == ID) &&
                (o.NguoiTao_ID == user.Id || o.NguoiThucHien_ID.Contains(user.Id) || o.Display.Contains(user.Id) || o.NguoiDuyet_ID == user.Id ) 
                //&& (o.TrangThai != TrangThaiDauViec.KetThuc)
                ).ToList();

            ViewBag.Parent_ID = ID;
            var lstChuyen = db.TN_Chuyen.Where(o => o.Viec_ID == ID).ToList();
            ViewBag.lstChat = lstChuyen;

            var activeChuyen = lstChuyen.Where(o => o.Active == 1 && o.NguoiNhan_ID.Substring(0, 36) == user.Id);
            if (activeChuyen != null && activeChuyen.ToList().Any()) ViewBag.ActiveChuyen = activeChuyen;

            GetViewBag();
            ViewBag.returnUrl = returnUrl;
            ViewBag.lstLoaiChuyen = GetLoaiChuyen(user, lst.First(o => o.ID == ID));
            return View(lst);
        }

        private dynamic GetLoaiChuyen(ApplicationUser user, TN_DauViec tN_DauViec)
        {
            var list = new List<SelectListItem>();
            if (tN_DauViec == null) return list;
            var objChuyenActiveUser = db.TN_Chuyen.FirstOrDefault(o => o.Active == 1 && o.NguoiNhan_ID == user.Id && o.LoaiChuyen == LoaiChuyen.BaoCao && o.Viec_ID == tN_DauViec.ID);

            //if (tN_DauViec.NguoiDuyet_ID.Contains(user.Id))
            if (objChuyenActiveUser != null && objChuyenActiveUser.Chuyen_ID > 0)
                list.Add(new SelectListItem() { Text = "Giao việc", Value = "0", Selected = false });

            var lstChuyen = db.TN_Chuyen.Where(o => o.Viec_ID == tN_DauViec.ID && o.Active == 1 && (o.NguoiNhan_ID.Substring(0, 36).Contains(user.Id) || o.LoaiChuyen == LoaiChuyen.XinYKien));

            if (lstChuyen != null && lstChuyen.Any())
            {
                if (lstChuyen.Where(o => o.LoaiChuyen == LoaiChuyen.BaoCao || o.LoaiChuyen == LoaiChuyen.GiaoViec) != null &&
                    lstChuyen.Where(o => o.LoaiChuyen == LoaiChuyen.BaoCao || o.LoaiChuyen == LoaiChuyen.GiaoViec).Any() &&
                    !tN_DauViec.NguoiDuyet_ID.Contains(user.Id))
                {
                    list.Add(new SelectListItem() { Text = "Giao việc", Value = "0", Selected = false });
                    list.Add(new SelectListItem() { Text = "Báo cáo", Value = "1", Selected = false });
                }

                var xyk = lstChuyen.Where(o => o.LoaiChuyen == LoaiChuyen.XinYKien && o.NguoiNhan_ID == user.Id);
                if (xyk != null && xyk.Any())
                    list.Add(new SelectListItem() { Text = "Trả lời ý kiến", Value = "3", Selected = false });
            }

            if (tN_DauViec.NguoiThucHien_ID.Contains(user.Id))
                list.Add(new SelectListItem() { Text = "Lấy ý kiến", Value = "2", Selected = false });

            return list;
        }

        private void GetViewBag()
        {
            var lstCauHinh = db.Ht_ThamSoHeThong.ToList();
            var objSep = lstCauHinh.FirstOrDefault(o => o.MaThamSo == "SepID");
            var objGiamDoc = lstCauHinh.FirstOrDefault(o => o.MaThamSo == "GiamDoc_ID");

            var lstNhanVien = new List<AspNetUsers>();
            lstNhanVien.Add(new AspNetUsers { Id = "" });
            var user = (ApplicationUser)S4T_HaTinhBase.GetUserSession();

            if (user != null && objSep != null && !string.IsNullOrEmpty(objSep.GiaTriThamSo) && !objSep.GiaTriThamSo.Contains(user.Id))
                ViewBag.lstSepID = objSep.GiaTriThamSo;
            else
                ViewBag.lstSepID = "";

            //if (objGiamDoc != null && !string.IsNullOrEmpty(objGiamDoc.GiaTriThamSo) && objGiamDoc.GiaTriThamSo.Contains(user.Id))
            //    ViewBag.lstSepID = user.Id;

            if (objGiamDoc != null && !string.IsNullOrEmpty(objGiamDoc.GiaTriThamSo))
                ViewBag.GiamDoc = objGiamDoc.GiaTriThamSo;
            else
                ViewBag.GiamDoc = "";

            lstNhanVien.AddRange(db.AspNetUsers.Where(o => o.DonVi_ID == DonVi.SoThongTinTruyenThong && o.TrangThai == TrangThai.HoatDong));
            ViewBag.lstNhanVien = lstNhanVien;

            var lstLoaiViec = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Selected = true, Text = "Bình thường", Value = "0"},
                        new SelectListItem { Selected = false, Text = "Ưu tiên", Value = "1"},
                        new SelectListItem { Selected = false, Text = "Khẩn", Value = "2"},
                    }, "Value", "Text", 1);
            ViewBag.lstLoaiViec = lstLoaiViec;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TN_DauViec obj, List<HttpPostedFileBase> fileUpload, string FileXoa, string returnUrl, string Ykien)
        {
            var user = (ApplicationUser)S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            obj.PhamVi = PhamVi.PhongBan;// make to default

            //var objCauHinh = db.Ht_ThamSoHeThong.First(o => o.MaThamSo == "SepID");
            GetViewBag();
            if (!Validate(obj))
            {
                ViewBag.returnUrl = returnUrl;
                return View(obj);
            }
            try
            {
                var path = Server.MapPath("~") + "\\Upload\\TacNghiep";
                var idDelete = string.Empty;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var lstName = string.Empty;
                if (fileUpload != null && fileUpload.Any())
                    foreach (var item in fileUpload)
                    {
                        if (item != null)
                        {
                            var fileName = item.FileName;
                            if (item.ContentLength > 0 && !string.IsNullOrEmpty(fileName))
                            {
                                string newPath = Path.Combine(path, fileName);
                                fileName = Path.GetFileNameWithoutExtension(newPath) + "_" + DateTime.Now.ToBinary() + "_" + Path.GetExtension(newPath);
                                newPath = Path.Combine(path, fileName);
                                item.SaveAs(newPath);
                                lstName += fileName + ";";
                            }
                        }
                    }
                obj.Display = obj.NguoiThucHien_ID;
                obj.TenNguoiThucHien = GetTenNguoiThucHien(obj.NguoiThucHien_ID);
                //obj.TenNguoiDuyet = db.AspNetUsers.FirstOrDefault(o => o.Id == obj.NguoiDuyet_ID).HoVaTen;
                obj.TenNguoiDuyet = user.HoVaTen;
                obj.NguoiTao_ID = user.Id;
                obj.NgayTao = DateTime.Now;
                if (obj.ID <= 0)
                {
                    obj.NguoiDuyet_ID = user.Id;// make to default
                    if (lstName.Length > 2)
                        obj.FileDinhKem = lstName;
                    db.TN_DauViec.Add(obj);
                }
                else
                {
                    var objOld = db.TN_DauViec.FirstOrDefault(o => o.ID == obj.ID);
                    idDelete = GetchangeAssign(objOld.NguoiThucHien_ID, obj.NguoiThucHien_ID);
                    if (!string.IsNullOrEmpty(FileXoa))
                    {
                        foreach (var item in FileXoa.Split(';'))
                        {
                            if (string.IsNullOrEmpty(item)) break;
                            objOld.FileDinhKem = objOld.FileDinhKem.Replace(item, "");
                            objOld.FileDinhKem = objOld.FileDinhKem.Replace(";;", ";");
                        }

                    }
                    objOld.FileDinhKem += lstName;

                    objOld.DenNgay = obj.DenNgay;
                    objOld.LoaiViec = obj.LoaiViec;
                    objOld.NgayTao = obj.NgayTao;
                    //objOld.NguoiDuyet_ID = obj.NguoiDuyet_ID;
                    objOld.NguoiTao_ID = obj.NguoiTao_ID;
                    objOld.NguoiThucHien_ID = obj.NguoiThucHien_ID;
                    objOld.NoiDung = obj.NoiDung;
                    objOld.Parent_ID = obj.Parent_ID;
                    //objOld.PhamVi = obj.PhamVi;
                    objOld.TenDauViec = obj.TenDauViec;
                    objOld.TenNguoiDuyet = obj.TenNguoiDuyet;
                    objOld.TienDo = obj.TienDo;
                    objOld.TenNguoiThucHien = obj.TenNguoiThucHien;
                    objOld.TuNgay = obj.TuNgay;
                    objOld.Display = obj.Display;

                    db.Entry(objOld).State = EntityState.Modified;
                }

                if (obj.Parent_ID != null)
                {
                    var parentObj = db.TN_DauViec.FirstOrDefault(o => o.ID == obj.Parent_ID);
                    var lstNguoiThucHien = db.AspNetUsers.Where(o => obj.NguoiThucHien_ID.Contains(o.Id));
                    var lstDauViecCon = db.TN_DauViec.Where(o => o.Parent_ID == obj.Parent_ID && o.ID != obj.ID);// another child

                    foreach (var item in idDelete.Split(','))//Delete Display ID in parent which delete in child.
                    {
                        if (string.IsNullOrEmpty(item) || lstDauViecCon.Where(o => o.Display.Contains(item)).Any())// null or exist work item which contain idDelete
                            continue;
                        parentObj.Display = parentObj.Display.Replace("," + item, "");
                        //parentObj.Display.Replace(item, "");
                    }

                    foreach (var item in lstNguoiThucHien)
                    {
                        if (!parentObj.NguoiThucHien_ID.Contains(item.Id))
                        {
                            if (!parentObj.Display.Contains(item.Id))
                                parentObj.Display += "," + item.Id;
                        }
                    }
                    db.Entry(parentObj).State = EntityState.Modified;
                }
                db.SaveChanges();

                var objChuyen = new TN_Chuyen();
                objChuyen.LoaiChuyen = LoaiChuyen.GiaoViec;
                objChuyen.NguoiChuyen_ID = user.Id;
                objChuyen.NguoiNhan_ID = obj.NguoiThucHien_ID;
                objChuyen.TenNguoiChuyen = user.HoVaTen;
                objChuyen.TenNguoiNhan = obj.TenNguoiThucHien;
                objChuyen.Viec_ID = obj.ID;
                objChuyen.Ykien = Ykien;
                objChuyen.Active = 1;
                objChuyen.LuongBaoCao = user.Id + ";";
                db.TN_Chuyen.Add(objChuyen);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("FileDinhKem", "Lỗi upload");
                ViewBag.returnUrl = returnUrl;
                return View(obj);
            }
            return RedirectToLocal(returnUrl);
        }

        private string GetchangeAssign(string old, string ne)
        {
            var lst = old.Split(',').ToList();
            var idDelete = string.Empty;
            foreach (var item in lst)
            {
                if (!ne.Contains(item))
                    idDelete += item + ",";
            }
            return idDelete;
        }

        private string GetTenNguoiThucHien(string NguoiThucHien_ID)
        {
            var Ten = new StringBuilder();
            var lstUser = db.AspNetUsers.Where(o => NguoiThucHien_ID.Contains(o.Id));
            foreach (var item in lstUser)
            {
                Ten.Append(item.HoVaTen);
                Ten.Append("; ");
            }
            return Ten.ToString();
        }

        public ActionResult ChangeLoaiChuyen(int? value, int id)
        {
            var str = new StringBuilder();
            var user = (ApplicationUser)S4T_HaTinhBase.GetUserSession();
            var objViec = db.TN_DauViec.First(o => o.ID == id);
            var objCauHinh = db.Ht_ThamSoHeThong.FirstOrDefault(o => o.MaThamSo == "SepID");

            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            if (value != null)
            {
                str.Append("<option value=''></option>");
                if (value == LoaiChuyen.BaoCao)
                {
                    var objChuyen = db.TN_Chuyen.FirstOrDefault(o => o.Viec_ID == id && o.Active == 1
                        && (o.LoaiChuyen == LoaiChuyen.BaoCao || o.LoaiChuyen == LoaiChuyen.GiaoViec) && o.NguoiNhan_ID.Contains(user.Id));
                    if (objChuyen != null)
                    {
                        var IDBaoCao = objChuyen.LuongBaoCao.Substring(0, objChuyen.LuongBaoCao.Count() - 1).Split(';').Last();
                        if (!string.IsNullOrEmpty(IDBaoCao))
                        {
                            var nv = db.AspNetUsers.First(o => o.Id == IDBaoCao);
                            str.AppendFormat("<option value='{0}'>{1}</option>", nv.Id, nv.HoVaTen);
                        }
                    }
                }

                if (value == LoaiChuyen.GiaoViec)
                {
                    var objChuyen = db.TN_Chuyen.FirstOrDefault(o => o.Viec_ID == id && o.Active == 1
                        && (o.LoaiChuyen == LoaiChuyen.BaoCao || o.LoaiChuyen == LoaiChuyen.GiaoViec) && o.NguoiNhan_ID.Contains(user.Id));

                    if (objChuyen != null)
                    {
                        var lstNhanVien = db.AspNetUsers.Where(o => o.TrangThai == TrangThai.HoatDong && !objChuyen.LuongBaoCao.Contains(o.Id) &&
                            o.DonVi_ID == DonVi.SoThongTinTruyenThong && o.Id != user.Id);
                        if (objCauHinh != null && !string.IsNullOrEmpty(objCauHinh.GiaTriThamSo) && !objCauHinh.GiaTriThamSo.Contains(user.Id))
                            lstNhanVien = lstNhanVien.Where(o => !objCauHinh.GiaTriThamSo.Contains(o.Id));

                        if (lstNhanVien != null && lstNhanVien.Any())
                            foreach (var item in lstNhanVien)
                            {
                                str.AppendFormat("<option value='{0}'>{1}</option>", item.Id, item.HoVaTen);
                            }
                    }
                }

                if (value == LoaiChuyen.XinYKien)
                {
                    var lstChuyen = db.TN_Chuyen.Where(o => o.Viec_ID == id && o.Active == 1 && o.LoaiChuyen == LoaiChuyen.XinYKien && o.NguoiChuyen_ID == user.Id);
                    dynamic lstNhanVien;
                    if (lstChuyen != null && lstChuyen.Any())
                        lstNhanVien = db.AspNetUsers.Where(o => o.TrangThai == TrangThai.HoatDong && !lstChuyen.Select(a => a.NguoiNhan_ID).Contains(o.Id) &&
                                                            o.DonVi_ID == DonVi.SoThongTinTruyenThong &&
                                                            o.Id != user.Id
                                                            );
                    else
                        lstNhanVien = db.AspNetUsers.Where(o => o.TrangThai == TrangThai.HoatDong && o.DonVi_ID == DonVi.SoThongTinTruyenThong &&
                                                            o.Id != user.Id
                                                            );

                    if (lstNhanVien != null)
                        foreach (var item in lstNhanVien)
                        {
                            str.AppendFormat("<option value='{0}'>{1}</option>", item.Id, item.HoVaTen);
                        }
                }

                if (value == LoaiChuyen.TraLoi)
                {
                    var lstChuyen = db.TN_Chuyen.Where(o => o.Viec_ID == id && o.Active == 1 && o.LoaiChuyen == LoaiChuyen.XinYKien && o.NguoiNhan_ID == user.Id);
                    dynamic lstNhanVien;
                    if (lstChuyen != null && lstChuyen.Any())
                        lstNhanVien = db.AspNetUsers.Where(o => o.TrangThai == TrangThai.HoatDong && lstChuyen.Select(a => a.NguoiChuyen_ID).Contains(o.Id) && o.DonVi_ID == DonVi.SoThongTinTruyenThong);
                    else
                        lstNhanVien = null;

                    if (lstNhanVien != null)
                        foreach (var item in lstNhanVien)
                        {
                            str.AppendFormat("<option value='{0}'>{1}</option>", item.Id, item.HoVaTen);
                        }
                }
            }
            return Json(new { value = str.ToString() });
        }

        [ValidateInput(false)]
        public ActionResult Fuck(int? LoaiChuyen, string NguoiChuyen, int viec_ID, string comment, string returnUrl)
        {
            //if (LoaiChuyen == null || string.IsNullOrEmpty(NguoiChuyen)) return JavaScript("Chưa chọn loại chuyển hoặc người nhận");
            if (LoaiChuyen == null || string.IsNullOrEmpty(NguoiChuyen)) return Content("Chưa chọn loại chuyển hoặc người nhận");
            var user = (ApplicationUser)S4T_HaTinhBase.GetUserSession();
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });

            if (LoaiChuyen != null)
            {
                var objViec = db.TN_DauViec.FirstOrDefault(o => o.ID == viec_ID);
                if (LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.BaoCao)
                {
                    var objChuyen = db.TN_Chuyen.FirstOrDefault(o => o.Viec_ID == viec_ID && o.Active == 1 &&
                        (o.LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.BaoCao || o.LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.GiaoViec) && o.NguoiNhan_ID.Contains(user.Id));

                    if (objChuyen != null)
                    {
                        var objNew = new TN_Chuyen();
                        objNew.LoaiChuyen = S4T_HaTinh.Common.LoaiChuyen.BaoCao;
                        objNew.NguoiChuyen_ID = user.Id;
                        objNew.NguoiNhan_ID = NguoiChuyen;
                        objNew.TenNguoiChuyen = user.HoVaTen;
                        objNew.TenNguoiNhan = db.AspNetUsers.First(o => o.Id == NguoiChuyen).HoVaTen;
                        objNew.Viec_ID = viec_ID;
                        objNew.Ykien = comment;
                        objNew.Active = 1;
                        objNew.LuongBaoCao = objChuyen.LuongBaoCao.Replace(NguoiChuyen + ";", "");
                        db.TN_Chuyen.Add(objNew);

                        objChuyen.Active = 0;
                        db.Entry(objChuyen).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }

                if (LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.GiaoViec)
                {
                    var objChuyen = db.TN_Chuyen.FirstOrDefault(o => o.Viec_ID == viec_ID && o.Active == 1 &&
                        (o.LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.BaoCao || o.LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.GiaoViec) && o.NguoiNhan_ID.Contains(user.Id));
                    if (objChuyen != null)
                    {
                        var objNew = new TN_Chuyen();
                        objNew.LoaiChuyen = S4T_HaTinh.Common.LoaiChuyen.GiaoViec;
                        objNew.NguoiChuyen_ID = user.Id;
                        objNew.NguoiNhan_ID = NguoiChuyen;
                        objNew.TenNguoiChuyen = user.HoVaTen;
                        objNew.TenNguoiNhan = db.AspNetUsers.First(o => o.Id == NguoiChuyen).HoVaTen;
                        objNew.Viec_ID = viec_ID;
                        objNew.Ykien = comment;
                        objNew.Active = 1;
                        objNew.LuongBaoCao = objChuyen.LuongBaoCao + user.Id + ";";
                        db.TN_Chuyen.Add(objNew);

                        objChuyen.Active = 0;
                        db.Entry(objChuyen).State = EntityState.Modified;

                        objViec.Display = objViec.Display + "," + NguoiChuyen;
                        db.Entry(objViec).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }

                if (LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.XinYKien)
                {
                    var objNew = new TN_Chuyen();
                    objNew.LoaiChuyen = S4T_HaTinh.Common.LoaiChuyen.XinYKien;
                    objNew.NguoiChuyen_ID = user.Id;
                    objNew.NguoiNhan_ID = NguoiChuyen;
                    objNew.TenNguoiChuyen = user.HoVaTen;
                    objNew.TenNguoiNhan = db.AspNetUsers.First(o => o.Id == NguoiChuyen).HoVaTen;
                    objNew.Viec_ID = viec_ID;
                    objNew.Ykien = comment;
                    objNew.Active = 1;
                    objNew.LuongBaoCao = "";
                    db.TN_Chuyen.Add(objNew);

                    objViec.Display = objViec.Display + "," + NguoiChuyen;
                    db.Entry(objViec).State = EntityState.Modified;

                    db.SaveChanges();
                }

                if (LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.TraLoi)
                {
                    var objChuyen = db.TN_Chuyen.FirstOrDefault(o => o.Viec_ID == viec_ID && o.Active == 1 && o.LoaiChuyen == S4T_HaTinh.Common.LoaiChuyen.XinYKien && o.NguoiNhan_ID == user.Id);
                    var objNew = new TN_Chuyen();
                    objNew.LoaiChuyen = S4T_HaTinh.Common.LoaiChuyen.TraLoi;
                    objNew.NguoiChuyen_ID = user.Id;
                    objNew.NguoiNhan_ID = NguoiChuyen;
                    objNew.TenNguoiChuyen = user.HoVaTen;
                    objNew.TenNguoiNhan = db.AspNetUsers.First(o => o.Id == NguoiChuyen).HoVaTen;
                    objNew.Viec_ID = viec_ID;
                    objNew.Ykien = comment;
                    objNew.Active = 1;
                    objNew.LuongBaoCao = "";
                    objNew.TraLoi_ID = objChuyen.Chuyen_ID;
                    db.TN_Chuyen.Add(objNew);

                    objChuyen.Active = 0;
                    db.Entry(objChuyen).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }
            return RedirectToLocal(returnUrl);
        }

        public ActionResult KetThuc(int id, string returnUrl)
        {
            DeQuyKetThuc(id);
            db.SaveChanges();
            return Redirect(returnUrl);
        }

        private void DeQuyKetThuc(int id)
        {
            var obj = db.TN_DauViec.First(o => o.ID == id);
            obj.TrangThai = TrangThaiDauViec.KetThuc;
            db.Entry(obj).State = EntityState.Modified;

            var lst = db.TN_DauViec.Where(o => o.Parent_ID == id);
            if (lst != null && lst.ToList().Any())
            {
                foreach (var item in lst)
                {
                    DeQuyKetThuc(item.ID);
                }
            }
            return;
        }

        public ActionResult Download(string name)
        {
            if (string.IsNullOrEmpty(name)) return JavaScript("alert('Không tìm thấy file trên server !')");
            var path = Server.MapPath("..") + "\\Upload\\TacNghiep";
            string newPath = Path.Combine(path, name);

            if (System.IO.File.Exists(newPath))
            {
                var contentType = GetContentType(newPath);
                return File(new FileStream(newPath, FileMode.Open), contentType, name);
            }
            return JavaScript("alert('Không tìm thấy file trên server!')");
        }

        private string GetContentType(string fileName)
        {
            string contentType = "application/octetstream";
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                string ext = extension.ToLower();
                var registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (registryKey != null && registryKey.GetValue("Content Type") != null)
                    contentType = registryKey.GetValue("Content Type").ToString();
            }
            return contentType;
        }
        private bool Validate(TN_DauViec obj)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(obj.TenDauViec) || string.IsNullOrEmpty(obj.TenDauViec.Trim()))
            {
                ModelState.AddModelError("TenDauViec", "Tên đầu việc không được để trống");
            }
            if (string.IsNullOrEmpty(obj.NguoiThucHien_ID))
            {
                ModelState.AddModelError("NguoiThucHien_ID", "Người thực hiện không được để trống");
            }
            if (obj.TuNgay == null || obj.TuNgay == new DateTime())
            {
                ModelState.AddModelError("TuNgay", "Ngày bắt đầu không hợp lệ");
            }
            if (obj.DenNgay == null || obj.DenNgay == new DateTime())
            {
                ModelState.AddModelError("DenNgay", "Ngày kết thúc không hợp lệ");
            }
            //if (obj.PhamVi == null)
            //{
            //    ModelState.AddModelError("PhamVi", "Chưa chọn phạm vi công việc");
            //}
            if (string.IsNullOrEmpty(obj.NoiDung) || string.IsNullOrEmpty(obj.NoiDung.Trim()))
            {
                ModelState.AddModelError("NoiDung", "Nội dung không được để trống");
            }
            if (ModelState.IsValid)
                return true;
            else
                return false;
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "LapKeHoach");
        }
    }
}