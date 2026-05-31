using System;
using System.Text.Json.Serialization;
using FUS_SMoGPT.Exceptions;

namespace FUS_SMoGPT.Models
{
    /// <summary>
    /// Lớp Student kế thừa từ Person (Abstract class).
    /// Đại diện cho một sinh viên trong hệ thống.
    /// 
    /// Kỹ thuật OOP sử dụng:
    /// - Kế thừa (Inheritance): Student : Person
    /// - Đa hình (Polymorphism): Override GetDisplayInfo(), HasPermission()
    /// - Properties với validation
    /// - Computed property: HocLuc (tự tính từ GPA)
    /// </summary>
    public class Student : Person
    {
        // ===== PRIVATE FIELDS =====
        private double _gpa;
        private string _mssv;
        private string _nganhHoc;
        private string _khoaHoc;

        // ===== PROPERTIES =====

        /// <summary>
        /// Mã số sinh viên (unique identifier)
        /// </summary>
        public string Mssv
        {
            get => _mssv;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã số sinh viên không được để trống.");
                _mssv = value.Trim().ToUpper();
            }
        }

        /// <summary>
        /// Ngành học của sinh viên
        /// </summary>
        public string NganhHoc
        {
            get => _nganhHoc;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ngành học không được để trống.");
                _nganhHoc = value.Trim();
            }
        }

        /// <summary>
        /// Khóa học (vd: K17, K18, K19...)
        /// </summary>
        public string KhoaHoc
        {
            get => _khoaHoc;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Khóa học không được để trống.");
                _khoaHoc = value.Trim();
            }
        }

        /// <summary>
        /// Điểm GPA hệ 4 (0.0 - 4.0)
        /// Sử dụng InvalidGpaException khi giá trị không hợp lệ
        /// </summary>
        public double Gpa
        {
            get => _gpa;
            set
            {
                if (value < 0.0 || value > 4.0)
                    throw new InvalidGpaException(value);
                _gpa = Math.Round(value, 2); // Làm tròn 2 chữ số thập phân
            }
        }

        /// <summary>
        /// Xếp loại học lực - Computed Property (chỉ đọc).
        /// Thuật toán xếp loại:
        /// 0 → dưới 1: Kém
        /// 1 → dưới 2: Yếu
        /// 2 → dưới 2.5: Trung Bình
        /// 2.5 → dưới 3.5: Khá
        /// 3.5 → dưới 4: Giỏi
        /// Bằng 4: Xuất Sắc
        /// </summary>
        [JsonIgnore]
        public HocLuc XepLoaiHocLuc
        {
            get
            {
                return _gpa switch
                {
                    4.0 => HocLuc.XuatSac,
                    >= 3.5 => HocLuc.Gioi,
                    >= 2.5 => HocLuc.Kha,
                    >= 2.0 => HocLuc.TrungBinh,
                    >= 1.0 => HocLuc.Yeu,
                    _ => HocLuc.Kem
                };
            }
        }

        /// <summary>
        /// Chuỗi hiển thị học lực bằng tiếng Việt
        /// </summary>
        [JsonIgnore]
        public string HocLucDisplay
        {
            get
            {
                return XepLoaiHocLuc switch
                {
                    HocLuc.XuatSac => "Xuất Sắc",
                    HocLuc.Gioi => "Giỏi",
                    HocLuc.Kha => "Khá",
                    HocLuc.TrungBinh => "Trung Bình",
                    HocLuc.Yeu => "Yếu",
                    HocLuc.Kem => "Kém",
                    _ => "Không xác định"
                };
            }
        }

        /// <summary>
        /// Vai trò - luôn là Student (override từ abstract property)
        /// </summary>
        [JsonIgnore]
        public override UserRole Role => UserRole.Student;

        // ===== CONSTRUCTORS =====

        /// <summary>
        /// Constructor mặc định cho deserialization
        /// </summary>
        public Student() : base()
        {
            _mssv = string.Empty;
            _nganhHoc = string.Empty;
            _khoaHoc = string.Empty;
            _gpa = 0.0;
        }

        /// <summary>
        /// Constructor đầy đủ tham số
        /// </summary>
        /// <param name="name">Họ và tên sinh viên</param>
        /// <param name="mssv">Mã số sinh viên</param>
        /// <param name="nganhHoc">Ngành học</param>
        /// <param name="khoaHoc">Khóa học</param>
        /// <param name="gpa">Điểm GPA hệ 4</param>
        public Student(string name, string mssv, string nganhHoc, string khoaHoc, double gpa)
            : base(name, mssv, mssv, "1") // Username = MSSV, Password mặc định = "1"
        {
            _mssv = string.Empty;
            _nganhHoc = string.Empty;
            _khoaHoc = string.Empty;
            Mssv = mssv;
            NganhHoc = nganhHoc;
            KhoaHoc = khoaHoc;
            Gpa = gpa;
        }

        // ===== OVERRIDE METHODS (Đa hình - Polymorphism) =====

        /// <summary>
        /// Override phương thức trừu tượng GetDisplayInfo().
        /// Trả về thông tin chi tiết của sinh viên.
        /// </summary>
        public override string GetDisplayInfo()
        {
            return $"Sinh viên: {Name}\n" +
                   $"MSSV: {Mssv}\n" +
                   $"Ngành: {NganhHoc}\n" +
                   $"Khóa: {KhoaHoc}\n" +
                   $"GPA: {Gpa:F2}\n" +
                   $"Học lực: {HocLucDisplay}";
        }

        /// <summary>
        /// Override phương thức ảo HasPermission().
        /// Sinh viên chỉ có quyền xem thông tin bản thân ("viewSelf").
        /// </summary>
        public override bool HasPermission(string action)
        {
            // Sinh viên chỉ được xem thông tin bản thân
            return action?.ToLower() == "viewself";
        }
    }
}
