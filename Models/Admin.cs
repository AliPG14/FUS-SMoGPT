using System.Text.Json.Serialization;

namespace FUS_SMoGPT.Models
{
    /// <summary>
    /// Lớp Admin kế thừa từ Person (Abstract class).
    /// Đại diện cho quản trị viên hệ thống.
    /// 
    /// Kỹ thuật OOP sử dụng:
    /// - Kế thừa (Inheritance): Admin : Person
    /// - Đa hình (Polymorphism): Override GetDisplayInfo(), HasPermission()
    /// </summary>
    public class Admin : Person
    {
        /// <summary>
        /// Vai trò - luôn là Admin (override từ abstract property)
        /// </summary>
        [JsonIgnore]
        public override UserRole Role => UserRole.Admin;

        // ===== CONSTRUCTORS =====

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public Admin() : base()
        {
        }

        /// <summary>
        /// Constructor có tham số
        /// </summary>
        public Admin(string name, string username, string password)
            : base(name, "ADMIN", username, password)
        {
        }

        // ===== OVERRIDE METHODS (Đa hình - Polymorphism) =====

        /// <summary>
        /// Override phương thức trừu tượng GetDisplayInfo().
        /// Admin hiển thị thông tin đơn giản.
        /// </summary>
        public override string GetDisplayInfo()
        {
            return $"Quản trị viên: {Name}\nVai trò: Administrator\nQuyền hạn: Toàn quyền quản lý sinh viên";
        }

        /// <summary>
        /// Override phương thức ảo HasPermission().
        /// Admin có TẤT CẢ quyền hạn - thể hiện đa hình so với Student.
        /// </summary>
        public override bool HasPermission(string action)
        {
            return true; // Admin có toàn quyền
        }
    }
}
