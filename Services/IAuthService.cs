using FUS_SMoGPT.Models;

namespace FUS_SMoGPT.Services
{
    /// <summary>
    /// Interface định nghĩa hợp đồng cho dịch vụ xác thực người dùng.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Xác thực người dùng bằng username và password
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Đối tượng IUser nếu xác thực thành công, null nếu thất bại</returns>
        IUser? Authenticate(string username, string password);

        /// <summary>
        /// Kiểm tra người dùng có phải Admin không
        /// </summary>
        bool IsAdmin(IUser user);
    }
}
