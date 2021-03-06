﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace S4T_HaTinh.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using System.Data.SqlClient;
    
    public partial class S4T_HaTinhEntities : DbContext
    {
        public S4T_HaTinhEntities()
            : base("name=S4T_HaTinhEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<CongNghiepCNTT> CongNghiepCNTT { get; set; }
        public virtual DbSet<CongThongTinDienTu> CongThongTinDienTu { get; set; }
        public virtual DbSet<CongThongTinDienTu_Huyen> CongThongTinDienTu_Huyen { get; set; }
        public virtual DbSet<Dm_CapXuLy> Dm_CapXuLy { get; set; }
        public virtual DbSet<Dm_DanhMucChung> Dm_DanhMucChung { get; set; }
        public virtual DbSet<Dm_DonVi> Dm_DonVi { get; set; }
        public virtual DbSet<Dm_LoaiDanhMuc> Dm_LoaiDanhMuc { get; set; }
        public virtual DbSet<Dm_LoaiDuAn> Dm_LoaiDuAn { get; set; }
        public virtual DbSet<Dm_LoaiKeHoach> Dm_LoaiKeHoach { get; set; }
        public virtual DbSet<Dm_LoaiVanBan> Dm_LoaiVanBan { get; set; }
        public virtual DbSet<EditUserViewModels> EditUserViewModels { get; set; }
        public virtual DbSet<HaTangKyThuatCNTT> HaTangKyThuatCNTT { get; set; }
        public virtual DbSet<HaTangKyThuatCNTT_Huyen> HaTangKyThuatCNTT_Huyen { get; set; }
        public virtual DbSet<HaTangNhanLucCNTT> HaTangNhanLucCNTT { get; set; }
        public virtual DbSet<HaTangNhanLucCNTT_Huyen> HaTangNhanLucCNTT_Huyen { get; set; }
        public virtual DbSet<Ht_CauHinhUpload> Ht_CauHinhUpload { get; set; }
        public virtual DbSet<Ht_FileDinhKem> Ht_FileDinhKem { get; set; }
        public virtual DbSet<Ht_FileHoSo_LoaiDuAn> Ht_FileHoSo_LoaiDuAn { get; set; }
        public virtual DbSet<Ht_LichNhapLieu> Ht_LichNhapLieu { get; set; }
        public virtual DbSet<HT_Log> HT_Log { get; set; }
        public virtual DbSet<Ht_PhanHe> Ht_PhanHe { get; set; }
        public virtual DbSet<Ht_PhanHeChucNang> Ht_PhanHeChucNang { get; set; }
        public virtual DbSet<Ht_Role_PhanHe_ChucNang> Ht_Role_PhanHe_ChucNang { get; set; }
        public virtual DbSet<Ht_ThamSoHeThong> Ht_ThamSoHeThong { get; set; }
        public virtual DbSet<Ht_User> Ht_User { get; set; }
        public virtual DbSet<KeHoach> KeHoach { get; set; }
        public virtual DbSet<PhanMemQLCN> PhanMemQLCN { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TD_CauHinh> TD_CauHinh { get; set; }
        public virtual DbSet<TD_FileHoSoDinhKem> TD_FileHoSoDinhKem { get; set; }
        public virtual DbSet<TD_HoSoDuAn> TD_HoSoDuAn { get; set; }
        public virtual DbSet<TD_LuongCongViec> TD_LuongCongViec { get; set; }
        public virtual DbSet<TD_LuongCongViec_FileDinhKem> TD_LuongCongViec_FileDinhKem { get; set; }
        public virtual DbSet<TienDo> TienDo { get; set; }
        public virtual DbSet<TinhHinhSXDN> TinhHinhSXDN { get; set; }
        public virtual DbSet<TN_Chuyen> TN_Chuyen { get; set; }
        public virtual DbSet<TN_DauViec> TN_DauViec { get; set; }
        public virtual DbSet<ToChucChinhSachCNTT> ToChucChinhSachCNTT { get; set; }
        public virtual DbSet<UngDungCNTT> UngDungCNTT { get; set; }
        public virtual DbSet<UngDungCNTT_PMQLCN> UngDungCNTT_PMQLCN { get; set; }
        public virtual DbSet<UngDungCNTTTaiDN> UngDungCNTTTaiDN { get; set; }
        public virtual DbSet<VanBan> VanBan { get; set; }
        public virtual DbSet<Ht_MaxID> Ht_MaxID { get; set; }
        public virtual DbSet<ViewLichNhap_CongThongTinDienTu> ViewLichNhap_CongThongTinDienTu { get; set; }
        public virtual DbSet<ViewLichNhap_HaTangKyThuat> ViewLichNhap_HaTangKyThuat { get; set; }
        public virtual DbSet<ViewLichNhap_HaTangNhanLuc> ViewLichNhap_HaTangNhanLuc { get; set; }
        public virtual DbSet<ViewLichNhap_UngDungCNTT> ViewLichNhap_UngDungCNTT { get; set; }
        public virtual DbSet<UngDungCNTT_Huyen> UngDungCNTT_Huyen { get; set; }
        public virtual DbSet<LoaiVanBan> LoaiVanBan { get; set; }
        public virtual DbSet<NhomVanBan> NhomVanBan { get; set; }
        public virtual DbSet<View_Loai_Nhom_VanBan> View_Loai_Nhom_VanBan { get; set; }
        public virtual DbSet<ViewLichNhap_CongThongTinDienTuCapHuyen> ViewLichNhap_CongThongTinDienTuCapHuyen { get; set; }
        public virtual DbSet<ViewLichNhap_HaTangKyThuatCapHuyen> ViewLichNhap_HaTangKyThuatCapHuyen { get; set; }
        public virtual DbSet<ViewLichNhap_HaTangNhanLucCapHuyen> ViewLichNhap_HaTangNhanLucCapHuyen { get; set; }
        public virtual DbSet<ViewLichNhap_UngDungCNTTCapHuyen> ViewLichNhap_UngDungCNTTCapHuyen { get; set; }
    
        public virtual ObjectResult<sp_GetChucNangByRole_Result> sp_GetChucNangByRole(string roleId)
        {
            var roleIdParameter = roleId != null ?
                new ObjectParameter("RoleId", roleId) :
                new ObjectParameter("RoleId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetChucNangByRole_Result>("sp_GetChucNangByRole", roleIdParameter);
        }
    
        public virtual ObjectResult<sp_GetMaxLuongCongViecByMaHoSo_Result> sp_GetMaxLuongCongViecByMaHoSo(string maHoSo)
        {
            var maHoSoParameter = maHoSo != null ?
                new ObjectParameter("maHoSo", maHoSo) :
                new ObjectParameter("maHoSo", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetMaxLuongCongViecByMaHoSo_Result>("sp_GetMaxLuongCongViecByMaHoSo", maHoSoParameter);
        }
    
        public virtual ObjectResult<sp_GetUserAndRole_Result> sp_GetUserAndRole(string id)
        {
            var idParameter = id != null ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetUserAndRole_Result>("sp_GetUserAndRole", idParameter);
        }

        public virtual ObjectResult<sp_GetUserByDonVi_Role_Result> sp_GetUserByDonVi_Role(Nullable<int> nhomDonVi_ID, Nullable<int> donViCap1_ID, Nullable<int> donVi_ID, string role, Nullable<int> trangThai)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("nhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("nhomDonVi_ID", DBNull.Value);

            var donViCap1_IDParameter = donViCap1_ID.HasValue ?
                new SqlParameter("donViCap1_ID", donViCap1_ID) :
                new SqlParameter("donViCap1_ID", DBNull.Value);

            var donVi_IDParameter = donVi_ID.HasValue ?
                new SqlParameter("donVi_ID", donVi_ID) :
                new SqlParameter("donVi_ID", DBNull.Value);

            var roleParameter = role != null ?
                new SqlParameter("role", role) :
                new SqlParameter("role", DBNull.Value);

            var trangThaiParameter = trangThai.HasValue ?
                new SqlParameter("trangThai", trangThai) :
                new SqlParameter("trangThai", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetUserByDonVi_Role_Result>("sp_GetUserByDonVi_Role @nhomDonVi_ID, @donViCap1_ID, @donVi_ID, @role, @trangThai", nhomDonVi_IDParameter, donViCap1_IDParameter, donVi_IDParameter, roleParameter, trangThaiParameter);
        }

        public virtual List<sp_HoSoDuAnWithLuongXuLy_Result> sp_HoSoDuAnWithLuongXuLy(string maHoSo, string tenDuAn, string tongMucDauTu, Nullable<int> tinhChatDuAn, Nullable<int> cap_ID, string nguoiNhan, Nullable<int> trangThai)
        {
            var maHoSoParameter = maHoSo != null ?
                new SqlParameter("maHoSo", maHoSo) :
                new SqlParameter("maHoSo", DBNull.Value);

            var tenDuAnParameter = tenDuAn != null ?
                new SqlParameter("tenDuAn", tenDuAn) :
                new SqlParameter("tenDuAn", DBNull.Value);

            if (String.IsNullOrEmpty(tongMucDauTu))
            {
                tongMucDauTu = "-1";
            }
            double dTongMucDauTu = 0;
            try
            {
                dTongMucDauTu = Double.Parse(tongMucDauTu);
            }
            catch (Exception)
            {
                dTongMucDauTu = -2;
            }

            var tongMucDauTuParameter = new SqlParameter("tongMucDauTu", dTongMucDauTu);

            var tinhChatDuAnParameter = tinhChatDuAn.HasValue ?
                new SqlParameter("tinhChatDuAn", tinhChatDuAn) :
                new SqlParameter("tinhChatDuAn", DBNull.Value);

            var cap_IDParameter = cap_ID.HasValue ?
                new SqlParameter("cap_ID", cap_ID) :
                new SqlParameter("cap_ID", DBNull.Value);


            var nguoiNhanParameter = nguoiNhan != null ?
                new SqlParameter("nguoiNhan", nguoiNhan) :
                new SqlParameter("nguoiNhan", DBNull.Value);

            var trangThaiParameter = trangThai.HasValue ?
                new SqlParameter("trangThai", trangThai) :
                new SqlParameter("trangThai", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_HoSoDuAnWithLuongXuLy_Result>("sp_HoSoDuAnWithLuongXuLy @maHoSo, @tenDuAn,@tongMucDauTu, @tinhChatDuAn, @cap_ID, @nguoiNhan, @trangThai", maHoSoParameter, tenDuAnParameter, tongMucDauTuParameter, tinhChatDuAnParameter, cap_IDParameter, nguoiNhanParameter, trangThaiParameter).ToList();
        }

        public virtual ObjectResult<sp_LichNhapLieuWithOption_Result> sp_LichNhapLieuWithOption(Nullable<int> donVi, Nullable<int> phanHe, Nullable<int> trangThai)
        {
            var donViParameter = donVi.HasValue ?
                new SqlParameter("donVi", donVi) :
                new SqlParameter("donVi", DBNull.Value);

            var phanHeParameter = phanHe.HasValue ?
                new SqlParameter("PhanHe", phanHe) :
                new SqlParameter("PhanHe", DBNull.Value);

            var trangThaiParameter = trangThai.HasValue ?
                new SqlParameter("trangThai", trangThai) :
                new SqlParameter("trangThai", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_LichNhapLieuWithOption_Result>("sp_LichNhapLieuWithOption @donVi, @PhanHe, @trangThai", donViParameter, phanHeParameter, trangThaiParameter);
        }

        public virtual List<sp_GetCongThongTinDienTu_Result> sp_GetCongThongTinDienTu(Nullable<int> nhomDonVi_ID, Nullable<int> dotBaoCao_ID, Nullable<int> nam, Nullable<int> chucNang_ID)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("NhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("NhomDonVi_ID", DBNull.Value);
    
            var dotBaoCao_IDParameter = dotBaoCao_ID.HasValue ?
                new SqlParameter("DotBaoCao_ID", dotBaoCao_ID) :
                new SqlParameter("DotBaoCao_ID", DBNull.Value);
    
            var namParameter = nam.HasValue ?
                new SqlParameter("Nam", nam) :
                new SqlParameter("Nam", DBNull.Value);
    
            var chucNang_IDParameter = chucNang_ID.HasValue ?
                new SqlParameter("ChucNang_ID", chucNang_ID) :
                new SqlParameter("ChucNang_ID", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetCongThongTinDienTu_Result>("sp_GetCongThongTinDienTu @NhomDonVi_ID, @DotBaoCao_ID, @Nam, @ChucNang_ID", nhomDonVi_IDParameter, dotBaoCao_IDParameter, namParameter, chucNang_IDParameter).ToList();
        }

        public virtual List<sp_GetHaTangKyThuat_Result> sp_GetHaTangKyThuat(Nullable<int> nhomDonVi_ID, Nullable<int> dotBaoCao_ID, Nullable<int> nam, Nullable<int> chucNang_ID)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("NhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("NhomDonVi_ID", DBNull.Value);
    
            var dotBaoCao_IDParameter = dotBaoCao_ID.HasValue ?
                new SqlParameter("DotBaoCao_ID", dotBaoCao_ID) :
                new SqlParameter("DotBaoCao_ID", DBNull.Value);
    
            var namParameter = nam.HasValue ?
                new SqlParameter("Nam", nam) :
                new SqlParameter("Nam", DBNull.Value);
    
            var chucNang_IDParameter = chucNang_ID.HasValue ?
                new SqlParameter("ChucNang_ID", chucNang_ID) :
                new SqlParameter("ChucNang_ID", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetHaTangKyThuat_Result>("sp_GetHaTangKyThuat @NhomDonVi_ID, @DotBaoCao_ID, @Nam, @ChucNang_ID", nhomDonVi_IDParameter, dotBaoCao_IDParameter, namParameter, chucNang_IDParameter).ToList();
        }

        public virtual List<sp_GetHaTangNhanLuc_Result> sp_GetHaTangNhanLuc(Nullable<int> nhomDonVi_ID, Nullable<int> dotBaoCao_ID, Nullable<int> nam, Nullable<int> chucNang_ID)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("NhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("NhomDonVi_ID", DBNull.Value);
    
            var dotBaoCao_IDParameter = dotBaoCao_ID.HasValue ?
                new SqlParameter("DotBaoCao_ID", dotBaoCao_ID) :
                new SqlParameter("DotBaoCao_ID", DBNull.Value);
    
            var namParameter = nam.HasValue ?
                new SqlParameter("Nam", nam) :
                new SqlParameter("Nam", DBNull.Value);
    
            var chucNang_IDParameter = chucNang_ID.HasValue ?
                new SqlParameter("ChucNang_ID", chucNang_ID) :
                new SqlParameter("ChucNang_ID", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetHaTangNhanLuc_Result>("sp_GetHaTangNhanLuc @NhomDonVi_ID, @DotBaoCao_ID, @Nam, @ChucNang_ID", nhomDonVi_IDParameter, dotBaoCao_IDParameter, namParameter, chucNang_IDParameter).ToList();
        }

        public virtual List<sp_GetUngDungCNTT_Result> sp_GetUngDungCNTT(Nullable<int> nhomDonVi_ID, Nullable<int> dotBaoCao_ID, Nullable<int> nam, Nullable<int> chucNang_ID)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("NhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("NhomDonVi_ID", DBNull.Value);
    
            var dotBaoCao_IDParameter = dotBaoCao_ID.HasValue ?
                new SqlParameter("DotBaoCao_ID", dotBaoCao_ID) :
                new SqlParameter("DotBaoCao_ID", DBNull.Value);
    
            var namParameter = nam.HasValue ?
                new SqlParameter("Nam", nam) :
                new SqlParameter("Nam", DBNull.Value);
    
            var chucNang_IDParameter = chucNang_ID.HasValue ?
                new SqlParameter("ChucNang_ID", chucNang_ID) :
                new SqlParameter("ChucNang_ID", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetUngDungCNTT_Result>("sp_GetUngDungCNTT  @NhomDonVi_ID, @DotBaoCao_ID, @Nam, @ChucNang_ID", nhomDonVi_IDParameter, dotBaoCao_IDParameter, namParameter, chucNang_IDParameter).ToList();
        }

        public virtual List<sp_GetCongThongTinDienTuCapHuyen_Result> sp_GetCongThongTinDienTuCapHuyen(Nullable<int> nhomDonVi_ID, Nullable<int> dotBaoCao_ID, Nullable<int> nam, Nullable<int> chucNang_ID)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("NhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("NhomDonVi_ID", DBNull.Value);
    
            var dotBaoCao_IDParameter = dotBaoCao_ID.HasValue ?
                new SqlParameter("DotBaoCao_ID", dotBaoCao_ID) :
                new SqlParameter("DotBaoCao_ID", DBNull.Value);
    
            var namParameter = nam.HasValue ?
                new SqlParameter("Nam", nam) :
                new SqlParameter("Nam", DBNull.Value);
    
            var chucNang_IDParameter = chucNang_ID.HasValue ?
                new SqlParameter("ChucNang_ID", chucNang_ID) :
                new SqlParameter("ChucNang_ID", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetCongThongTinDienTuCapHuyen_Result>("sp_GetCongThongTinDienTuCapHuyen  @NhomDonVi_ID, @DotBaoCao_ID, @Nam, @ChucNang_ID", nhomDonVi_IDParameter, dotBaoCao_IDParameter, namParameter, chucNang_IDParameter).ToList();
        }

        public virtual List<sp_GetHaTangKyThuatCapHuyen_Result> sp_GetHaTangKyThuatCapHuyen(Nullable<int> nhomDonVi_ID, Nullable<int> dotBaoCao_ID, Nullable<int> nam, Nullable<int> chucNang_ID)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("NhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("NhomDonVi_ID", DBNull.Value);
    
            var dotBaoCao_IDParameter = dotBaoCao_ID.HasValue ?
                new SqlParameter("DotBaoCao_ID", dotBaoCao_ID) :
                new SqlParameter("DotBaoCao_ID", DBNull.Value);
    
            var namParameter = nam.HasValue ?
                new SqlParameter("Nam", nam) :
                new SqlParameter("Nam", DBNull.Value);
    
            var chucNang_IDParameter = chucNang_ID.HasValue ?
                new SqlParameter("ChucNang_ID", chucNang_ID) :
                new SqlParameter("ChucNang_ID", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetHaTangKyThuatCapHuyen_Result>("sp_GetHaTangKyThuatCapHuyen  @NhomDonVi_ID, @DotBaoCao_ID, @Nam, @ChucNang_ID", nhomDonVi_IDParameter, dotBaoCao_IDParameter, namParameter, chucNang_IDParameter).ToList();
        }

        public virtual List<sp_GetHaTangNhanLucCapHuyen_Result> sp_GetHaTangNhanLucCapHuyen(Nullable<int> nhomDonVi_ID, Nullable<int> dotBaoCao_ID, Nullable<int> nam, Nullable<int> chucNang_ID)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("NhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("NhomDonVi_ID", DBNull.Value);
    
            var dotBaoCao_IDParameter = dotBaoCao_ID.HasValue ?
                new SqlParameter("DotBaoCao_ID", dotBaoCao_ID) :
                new SqlParameter("DotBaoCao_ID", DBNull.Value);
    
            var namParameter = nam.HasValue ?
                new SqlParameter("Nam", nam) :
                new SqlParameter("Nam", DBNull.Value);
    
            var chucNang_IDParameter = chucNang_ID.HasValue ?
                new SqlParameter("ChucNang_ID", chucNang_ID) :
                new SqlParameter("ChucNang_ID", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetHaTangNhanLucCapHuyen_Result>("sp_GetHaTangNhanLucCapHuyen  @NhomDonVi_ID, @DotBaoCao_ID, @Nam, @ChucNang_ID", nhomDonVi_IDParameter, dotBaoCao_IDParameter, namParameter, chucNang_IDParameter).ToList();
        }

        public virtual List<sp_GetUngDungCNTTCapHuyen_Result> sp_GetUngDungCNTTCapHuyen(Nullable<int> nhomDonVi_ID, Nullable<int> dotBaoCao_ID, Nullable<int> nam, Nullable<int> chucNang_ID)
        {
            var nhomDonVi_IDParameter = nhomDonVi_ID.HasValue ?
                new SqlParameter("NhomDonVi_ID", nhomDonVi_ID) :
                new SqlParameter("NhomDonVi_ID", DBNull.Value);
    
            var dotBaoCao_IDParameter = dotBaoCao_ID.HasValue ?
                new SqlParameter("DotBaoCao_ID", dotBaoCao_ID) :
                new SqlParameter("DotBaoCao_ID", DBNull.Value);
    
            var namParameter = nam.HasValue ?
                new SqlParameter("Nam", nam) :
                new SqlParameter("Nam", DBNull.Value);
    
            var chucNang_IDParameter = chucNang_ID.HasValue ?
                new SqlParameter("ChucNang_ID", chucNang_ID) :
                new SqlParameter("ChucNang_ID", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<sp_GetUngDungCNTTCapHuyen_Result>("sp_GetUngDungCNTTCapHuyen  @NhomDonVi_ID, @DotBaoCao_ID, @Nam, @ChucNang_ID", nhomDonVi_IDParameter, dotBaoCao_IDParameter, namParameter, chucNang_IDParameter).ToList();
        }
    }
}
