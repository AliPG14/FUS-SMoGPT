namespace FUS_SMoGPT.Models
{
    /// <summary>
    /// Interface định nghĩa hợp đồng cho tất cả người dùng trong hệ thống.
    /// Được sử dụng để đảm bảo tính đa hình và loose coupling.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Tên đăng nhập của người dùng
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Mật khẩu của người dùng
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Vai trò của người dùng (Admin hoặc Student)
        /// </summary>
        UserRole Role { get; }

        /// <summary>
        /// Kiểm tra quyền hạn của người dùng cho một hành động cụ thể
        /// </summary>
        /// <param name="action">Tên hành động cần kiểm tra (vd: "add", "edit", "delete", "viewAll")</param>
        /// <returns>true nếu có quyền, false nếu không</returns>
        bool HasPermission(string action);

        /// <summary>
        /// Lấy thông tin hiển thị của người dùng
        /// </summary>
        /// <returns>Chuỗi thông tin hiển thị</returns>
        string GetDisplayInfo();
    }
}
