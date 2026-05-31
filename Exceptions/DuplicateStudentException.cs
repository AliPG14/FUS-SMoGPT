using System;

namespace FUS_SMoGPT.Exceptions
{
    /// <summary>
    /// Ngoại lệ tùy chỉnh được ném khi MSSV đã tồn tại trong hệ thống.
    /// Kế thừa từ Exception - thể hiện kỹ thuật kế thừa.
    /// </summary>
    public class DuplicateStudentException : Exception
    {
        /// <summary>
        /// Mã số sinh viên bị trùng
        /// </summary>
        public string Mssv { get; }

        public DuplicateStudentException(string mssv)
            : base($"Sinh viên với MSSV '{mssv}' đã tồn tại trong hệ thống.")
        {
            Mssv = mssv;
        }

        public DuplicateStudentException(string mssv, Exception innerException)
            : base($"Sinh viên với MSSV '{mssv}' đã tồn tại trong hệ thống.", innerException)
        {
            Mssv = mssv;
        }
    }
}
