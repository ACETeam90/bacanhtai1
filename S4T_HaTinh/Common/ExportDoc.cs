using System;
using System.Collections.Generic;
using System.Data;
using Novacode;
using System.IO;
using S4T_HaTinh.Models;

namespace S4T_HaTinh.Common
{
    public static class ExportDoc
    {
        public static string GetTemplateByChucNang(string pathServer, Type type)
        {
            string path = string.Empty;
            if (type == typeof(HaTangKyThuatCNTT))
            {
                path = Path.Combine(pathServer, "HaTangKyThuat.docx");
            }
            else if (type == typeof(HaTangNhanLucCNTT))
            {
                path = Path.Combine(pathServer, "HaTangNhanLuc.docx");
            }
            else if (type == typeof(UngDungCNTT))
            {
                path = Path.Combine(pathServer, "UngDungCNTT.docx");
            }
            else if (type == typeof(CongThongTinDienTu))
            {
                path = Path.Combine(pathServer, "CongThongTinDienTu.docx");
            }
            else if (type == typeof(HaTangKyThuatCNTT_Huyen))
            {
                path = Path.Combine(pathServer, "HaTangKyThuat_CapHuyen.docx");
            }
            else if (type == typeof(HaTangNhanLucCNTT_Huyen))
            {
                path = Path.Combine(pathServer, "HaTangNhanLuc_CapHuyen.docx");
            }
            else if (type == typeof(UngDungCNTT_Huyen))
            {
                path = Path.Combine(pathServer, "UngDungCNTT_CapHuyen.docx");
            }
            else if (type == typeof(CongThongTinDienTu_Huyen))
            {
                path = Path.Combine(pathServer, "CongThongTinDienTu_CapHuyen.docx");
            }
            //switch (type)
            //{
            //    case typeof(HaTangKyThuatCNTT): // Hạ tầng kỹ thuật
                    
            //        type = typeof(HaTangKyThuatCNTT);
            //        break;
                //case 11: // Hạ tầng nhân lực CNTT
                //    break;
                //case 12: // Ứng dụng CNTT
                //    break;
                //case 14: // Cổng thông tin điện tử
                //    break;
                //case 15: // Hạ tầng kỹ thuật CNTT cấp Huyện
                //    break;
                //case 16: // Hạ tầng nhân lực CNTT cấp Huyện
                //    break;
                //case 20: // Cổng Thông Tin Điện Tử Cấp Huyện
                //    break;
                //case 21: // Ứng Dụng CNTT Cấp Huyện
                //    break;
            //}
            return path;
        }

        public static DocX ExportWord(string pathServer, Type type, object obj)
        {
            string wordTemplate = GetTemplateByChucNang(pathServer, type);
            DocX doc = DocX.Load(wordTemplate);
            if (doc == null)    return null;

            FillContent(doc, type, obj);

            var storeStream = new MemoryStream();
            doc.SaveAs(storeStream);

            return doc;
        }

        private static void FillContent(DocX doc, Type type, object obj){
            foreach (var prop in type.GetProperties())
            {
                doc.AddCustomProperty(new CustomProperty("@" + prop.Name, prop.GetValue(obj,null).ToString()));
            }
        }
    }
}