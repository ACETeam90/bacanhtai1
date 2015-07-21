using System;
using System.Resources;
using System.Reflection;
using System.Data.Entity.Validation;
using System.Text;

namespace S4T_HaTinh.Common
{  
    public class ExceptionViewer
    {
        public string sMessege;
        private string _sStackTrace;
        private string _sSource;



        /// <summary>
        /// Get các lỗi về Entity Framework
        /// </summary>
        public string GetError(DbEntityValidationException ex)
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

            return "Entity Validation Failed - errors follow:\n" +
                sb.ToString();

            //throw new DbEntityValidationException(
            //    "Entity Validation Failed - errors follow:\n" +
            //    sb.ToString(), ex
            //); // Add the original exception as the innerException
        }


        public ExceptionViewer(){}
        /// <summary>
        /// Hàm tạo sử dụng cho các lỗi xảy ra có exception.
        /// Hàm sẽ lưu lại lỗi vào ht_err_log để bug phần mềm.
        /// </summary>
        /// <param name="ex">Exception đưa vào để lưu log</param>
        public ExceptionViewer(Exception ex)
        {
            SaveErrorLog(ex);
        }

        /// <summary>
        /// Lấy thông báo lỗi tiếng việt từ file Resource, sử dụng cho các exception không có ErrorCode ví dụ như Exception
        /// </summary>
        /// <param name="originalMessage">errIdentifier là từ khóa thông báo lỗi gốc bằng tiếng Anh</param>
        /// <returns>Thông báo lỗi bằng tiếng Việt lấy từ resource</returns>
        public string GetErrorMessage(string originalMessage)
        {
            var errIdentifier = ErrorMsgToIdentifier(originalMessage);
            string viVnErrMsg;
            try
            {
                var rm = new ResourceManager("Resources.ErrorMessage", Assembly.Load("App_GlobalResources"));
                viVnErrMsg = rm.GetString(errIdentifier);
                viVnErrMsg = string.IsNullOrEmpty(viVnErrMsg) ? originalMessage : string.Format(viVnErrMsg, _sSource);
                if(viVnErrMsg.Equals(originalMessage))
                {
                    var errErrorMsgToIndentiferOfDanhMuc  = ErrorMsgToIndentiferOfDanhMuc(originalMessage);
                    viVnErrMsg = rm.GetString(errErrorMsgToIndentiferOfDanhMuc);
                    viVnErrMsg = string.IsNullOrEmpty(viVnErrMsg) ? originalMessage + "\nKey:" + errErrorMsgToIndentiferOfDanhMuc : string.Format(viVnErrMsg, _sSource);
                }
            }
            catch (Exception ex)
            {
                viVnErrMsg = ex.Message;
            }
            return viVnErrMsg.Replace("'","").Replace("`","");
        }

        public static string GetMessage(string mess){
            var rm = new ResourceManager("Resources.ErrorMessage", Assembly.Load("App_GlobalResources"));
            var strMessgage = rm.GetString(mess);
            return String.IsNullOrEmpty(strMessgage) ? mess : strMessgage;
        }

        public static string GetMessage(ObjectLichNhapLieu obj)
        {
            if (obj.TrangThai == TrangThaiLichNhapLieu.ChuaDenThoiDiem)
                return ExceptionViewer.GetMessage("CHUA_DEN_THOI_DIEM") + " Thời gian cho phép nhập báo cáo từ ngày " + obj.LichNhapLieu.TuNgay.ToShortDateString() + " đến ngày " + obj.LichNhapLieu.DenNgay.ToShortDateString();
            else if (obj.TrangThai == TrangThaiLichNhapLieu.KhongHoatDong)
                return ExceptionViewer.GetMessage("LICH_NHAP_KHONG_HOAT_DONG");
            else if (obj.TrangThai == TrangThaiLichNhapLieu.HoatDong)
                return ExceptionViewer.GetMessage("LICH_NHAP_EXITS");
            else
                return "Thời gian nhập báo cáo đến ngày " + obj.LichNhapLieu.DenNgay.ToShortDateString() + " đã hết !";
                //ExceptionViewer.GetMessage("LICH_NHAP_QUA_HAN");
        }

        private static string ErrorMsgToIndentiferOfDanhMuc(string sInput)
        {
            if (sInput.Contains("Incorrect string value:"))
            {
                var iStartPost = sInput.IndexOf("for column");
                if (iStartPost > 0)
                {
                    iStartPost = sInput.IndexOf("'", iStartPost);
                    if (iStartPost > 0)
                    {
                        var iEndPost = sInput.IndexOf("'", iStartPost + 1);
                        if (iEndPost > iStartPost)
                        {
                            var DM_ID = sInput.Substring(iStartPost + 1, iEndPost - iStartPost - 1);
                            return "INCORRECT_STRING_VALUE_" + DM_ID;
                        }
                    }
                }
            }
            return sInput.Replace("@", "").Replace("'", "").Replace(":", "").Replace(".", "")
                .Replace("(", "").Replace(")", "").Replace("!", "")
                .Replace(",", "").Replace(" ", "_").Replace("___", "_").Replace("__", "_").ToUpper().Trim();
        }

        private static string ErrorMsgToIdentifier(string sInput)
        {
            if (sInput.Contains("Cannot delete or update a parent row: a foreign key constraint fails"))
                return "CANNOT_DELETE_OR_UPDATE_A_PARENT_ROW_A_FOREIGN_KEY_CONSTRAINT_FAILS_CONSTRAINT_FOREIGN_KEY_REFERENCES_ON_DELETE_NO_ACTION_ON_UPDATE_NO_ACTION";

            var sOutPut = sInput;
            var iStart = sOutPut.IndexOf("`", 0, StringComparison.Ordinal);
            int iEnd;
            while (iStart >= 0)
            {
                iEnd = sOutPut.IndexOf("`", iStart + 1, StringComparison.Ordinal);
                sOutPut = iEnd >= 0 ? sOutPut.Remove(iStart, iEnd - iStart + 1) : sOutPut.Remove(iStart, 1);
                //if (iEnd >= 0) sOutPut = sOutPut.Remove(iStart, iEnd - iStart + 1);
                iStart = sOutPut.IndexOf("`", 0, StringComparison.Ordinal);
            }

            iStart = sOutPut.IndexOf("'", 0, StringComparison.Ordinal);
            while (iStart >= 0)
            {
                iEnd = sOutPut.IndexOf("'", iStart + 1, StringComparison.Ordinal);
                sOutPut = iEnd >= 0 ? sOutPut.Remove(iStart, iEnd - iStart + 1) : sOutPut.Remove(iStart, 1);
                iStart = sOutPut.IndexOf("'", 0, StringComparison.Ordinal);
            }

            sOutPut = sOutPut.Replace("@", "").Replace("'", "").Replace(":", "").Replace(".", "")
                .Replace("(", "").Replace(")", "").Replace("!", "")
                .Replace(",", "").Replace(" ", "_").Replace("___", "_").Replace("__", "_").ToUpper().Trim();

            return sOutPut;
        }

        /// <summary>
        /// Lưu nhật lý lỗi
        /// </summary>
        /// <param name="ex">Exception</param>
        private void SaveErrorLog(Exception ex)
        {
            if (ex == null)
            {
                sMessege = "";
                return;
            }

            //Tìm exception lõi
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            
            sMessege = ex.Message;
            _sSource = ex.Source;
            _sStackTrace = ex.StackTrace;

            //Người dùng không có quyền truy cập thì không cần ghi log lỗi
            if (sMessege.Equals("VIEW_NOT_PERMISSION") || sMessege.Equals("UPDATE_NOT_PERMISSION") || sMessege.Equals("DELETE_NOT_PERMISSION"))
                return;
            try
            {
                //Lưu thông tin lỗi vào nhật ký lỗi
                
            }
            catch
            { }
        }
    }
}