using System;
using FUS_SMoGPT.Models;

namespace FUS_SMoGPT.Services
{
    /// <summary>
    /// Implementation của IAuthService.
    /// Xử lý xác thực đăng nhập cho Admin và Sinh viên.
    /// 
    /// Kỹ thuật sử dụng:
    /// - Interface implementation (IAuthService)
    /// - Đa hình (Polymorphism): trả về IUser có thể là Admin hoặc Student
    /// - Exception handling
    /// </summary>
    public class AuthService : IAuthService
    {
        // ===== FIELDS =====
        private readonly IDataService _dataService;

        // Thông tin tài khoản Admin (hardcoded theo yêu cầu)
        private const string ADMIN_USERNAME = "admin";
        private const string ADMIN_PASSWORD = "123";

        // ===== CONSTRUCTOR =====
        public AuthService(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        // ===== INTERFACE IMPLEMENTATION =====

        /// <summary>
        /// Xác thực người dùng.
        /// 
        /// Thuật toán xác thực:
        /// 1. Kiểm tra nếu username = "admin" và password = "123" → đăng nhập Admin
        /// 2. Nếu không, tìm sinh viên có MSSV = username và password = "1" → đăng nhập Student
        /// 3. Nếu không tìm thấy → trả về null (đăng nhập thất bại)
        /// </summary>
        /// <param name="username">Tên đăng nhập (MSSV cho sinh viên, "admin" cho admin)</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>IUser nếu thành công, null nếu thất bại</returns>
        public IUser? Authenticate(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return null;

                string trimmedUsername = username.Trim();
                string trimmedPassword = password.Trim();

                // Bước 1: Kiểm tra Admin
                if (trimmedUsername.Equals(ADMIN_USERNAME, StringComparison.OrdinalIgnoreCase) 
                    && trimmedPassword == ADMIN_PASSWORD)
                {
                    return new Admin("Quản trị viên", ADMIN_USERNAME, ADMIN_PASSWORD);
                }

                // Bước 2: Kiểm tra Sinh viên
                var students = _dataService.GetAllStudents();
                foreach (var student in students)
                {
                    if (student.Mssv.Equals(trimmedUsername, StringComparison.OrdinalIgnoreCase)
                        && student.Password == trimmedPassword)
                    {
                        return student;
                    }
                }

                // Bước 3: Không tìm thấy → đăng nhập thất bại
                return null;
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về null
                System.Diagnostics.Debug.WriteLine($"Lỗi xác thực: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra người dùng có phải Admin không
        /// </summary>
        public bool IsAdmin(IUser user)
        {
            return user != null && user.Role == UserRole.Admin;
        }
    }
}
