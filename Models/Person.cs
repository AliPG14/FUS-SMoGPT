using System;
using System.Text.Json.Serialization;

namespace FUS_SMoGPT.Models
{
    /// <summary>
    /// Lớp trừu tượng (Abstract class) đại diện cho một người trong hệ thống.
    /// Đây là lớp cha cho Student và Admin.
    /// 
    /// Kỹ thuật OOP sử dụng:
    /// - Abstract class: không thể tạo instance trực tiếp
    /// - Fields (private): _name, _id 
    /// - Properties: Name, Id, Username, Password, Role
    /// - Abstract method: GetDisplayInfo() - buộc lớp con phải override
    /// - Virtual method: HasPermission() - lớp con CÓ THỂ override
    /// - Interface implementation: IUser
    /// </summary>
    public abstract class Person : IUser
    {
        // ===== FIELDS (private) =====
        private string _name;
        private string _id;

        // ===== PROPERTIES =====

        /// <summary>
        /// Họ và tên của người dùng
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên không được để trống.");
                _name = value.Trim();
            }
        }

        /// <summary>
        /// Mã định danh (MSSV cho sinh viên, "ADMIN" cho admin)
        /// </summary>
        public string Id
        {
            get => _id;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã định danh không được để trống.");
                _id = value.Trim();
            }
        }

        /// <summary>
        /// Tên đăng nhập - mặc định là Id
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Mật khẩu đăng nhập
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Vai trò người dùng (Admin hoặc Student)
        /// </summary>
        [JsonIgnore]
        public abstract UserRole Role { get; }

        // ===== CONSTRUCTORS =====

        /// <summary>
        /// Constructor mặc định cho deserialization
        /// </summary>
        protected Person() 
        {
            _name = string.Empty;
            _id = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
        }

        /// <summary>
        /// Constructor có tham số
        /// </summary>
        protected Person(string name, string id, string username, string password)
        {
            _name = string.Empty;
            _id = string.Empty;
            Name = name;      // Sử dụng property để validation
            Id = id;           // Sử dụng property để validation
            Username = username;
            Password = password;
        }

        // ===== ABSTRACT METHOD =====

        /// <summary>
        /// Phương thức trừu tượng - lớp con PHẢI override.
        /// Trả về chuỗi thông tin hiển thị của người dùng.
        /// Đây là kỹ thuật đa hình (polymorphism).
        /// </summary>
        public abstract string GetDisplayInfo();

        // ===== VIRTUAL METHOD =====

        /// <summary>
        /// Phương thức ảo - lớp con CÓ THỂ override.
        /// Mặc định không có quyền nào.
        /// </summary>
        public virtual bool HasPermission(string action)
        {
            return false;  // Mặc định: không có quyền
        }

        /// <summary>
        /// Override ToString để hỗ trợ debug
        /// </summary>
        public override string ToString()
        {
            return $"[{Role}] {Name} ({Id})";
        }
    }
}
