using System;

namespace FUS_SMoGPT.Exceptions
{
    /// <summary>
    /// Ngoại lệ tùy chỉnh được ném khi giá trị GPA không hợp lệ (ngoài khoảng 0-4).
    /// Kế thừa từ Exception - thể hiện kỹ thuật kế thừa.
    /// </summary>
    public class InvalidGpaException : Exception
    {
        /// <summary>
        /// Giá trị GPA không hợp lệ
        /// </summary>
        public double InvalidValue { get; }

        public InvalidGpaException(double value)
            : base($"Giá trị GPA '{value}' không hợp lệ. GPA phải nằm trong khoảng từ 0.0 đến 4.0.")
        {
            InvalidValue = value;
        }

        public InvalidGpaException(double value, Exception innerException)
            : base($"Giá trị GPA '{value}' không hợp lệ. GPA phải nằm trong khoảng từ 0.0 đến 4.0.", innerException)
        {
            InvalidValue = value;
        }
    }
}
