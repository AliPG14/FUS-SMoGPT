using System;

namespace FUS_SMoGPT.Exceptions
{
    /// <summary>
    /// Ngoại lệ tùy chỉnh được ném khi không tìm thấy sinh viên theo MSSV.
    /// Kế thừa từ Exception - thể hiện kỹ thuật kế thừa.
    /// </summary>
    public class StudentNotFoundException : Exception
    {
        /// <summary>
        /// Mã số sinh viên không tìm thấy
        /// </summary>
        public string Mssv { get; }

        public StudentNotFoundException(string mssv)
            : base($"Không tìm thấy sinh viên với MSSV: {mssv}")
        {
            Mssv = mssv;
        }

        public StudentNotFoundException(string mssv, Exception innerException)
            : base($"Không tìm thấy sinh viên với MSSV: {mssv}", innerException)
        {
            Mssv = mssv;
        }
    }
}
