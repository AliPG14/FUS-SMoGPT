using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using FUS_SMoGPT.Exceptions;
using FUS_SMoGPT.Models;

namespace FUS_SMoGPT.Services
{
    /// <summary>
    /// Implementation của IDataService sử dụng file JSON để lưu trữ dữ liệu.
    /// 
    /// Kỹ thuật sử dụng:
    /// - Interface implementation (IDataService)
    /// - Exception handling (try/catch, custom exceptions)
    /// - File I/O với JSON serialization
    /// - LINQ cho tìm kiếm và lọc dữ liệu
    /// </summary>
    public class JsonDataService : IDataService
    {
        // ===== FIELDS =====
        private readonly string _filePath;
        private List<Student> _students;
        private readonly JsonSerializerOptions _jsonOptions;

        // ===== CONSTRUCTOR =====
        public JsonDataService()
        {
            // Đường dẫn file JSON trong thư mục Data
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "students.json");
            
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,                    // Format JSON đẹp
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _students = new List<Student>();
            LoadData();
        }

        // ===== PRIVATE METHODS =====

        /// <summary>
        /// Đọc dữ liệu từ file JSON.
        /// Sử dụng exception handling cho FileNotFoundException và JsonException.
        /// </summary>
        private void LoadData()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    // Tạo thư mục nếu chưa có
                    string? directory = Path.GetDirectoryName(_filePath);
                    if (directory != null && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Tạo dữ liệu mẫu
                    _students = CreateSampleData();
                    SaveChanges();
                    return;
                }

                string jsonContent = File.ReadAllText(_filePath);
                
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _students = CreateSampleData();
                    SaveChanges();
                    return;
                }

                _students = JsonSerializer.Deserialize<List<Student>>(jsonContent, _jsonOptions) 
                            ?? new List<Student>();

                // Đảm bảo mỗi sinh viên có Username và Password
                foreach (var student in _students)
                {
                    if (string.IsNullOrEmpty(student.Username))
                        student.Username = student.Mssv;
                    if (string.IsNullOrEmpty(student.Password))
                        student.Password = "1";
                }
            }
            catch (JsonException ex)
            {
                // Xử lý ngoại lệ khi file JSON bị lỗi format
                System.Diagnostics.Debug.WriteLine($"Lỗi đọc file JSON: {ex.Message}");
                _students = CreateSampleData();
                SaveChanges();
            }
            catch (IOException ex)
            {
                // Xử lý ngoại lệ khi không thể đọc file
                System.Diagnostics.Debug.WriteLine($"Lỗi I/O: {ex.Message}");
                _students = new List<Student>();
            }
        }

        /// <summary>
        /// Tạo dữ liệu sinh viên mẫu
        /// </summary>
        private List<Student> CreateSampleData()
        {
            return new List<Student>
            {
                new Student("Nguyễn Văn An",    "SE180001", "Công nghệ thông tin",  "K18", 3.6),
                new Student("Trần Thị Bích",    "SE180002", "Công nghệ thông tin",  "K18", 3.2),
                new Student("Lê Hoàng Cường",   "SE180003", "Kỹ thuật phần mềm",   "K18", 2.8),
                new Student("Phạm Minh Đức",    "SE180004", "Kỹ thuật phần mềm",   "K18", 3.9),
                new Student("Hoàng Thị Em",     "SE190005", "Thiết kế đồ họa",     "K19", 2.3),
                new Student("Võ Quốc Phong",    "SE190006", "Quản trị kinh doanh", "K19", 1.8),
                new Student("Đặng Thùy Giang",  "SE190007", "Công nghệ thông tin",  "K19", 4.0),
                new Student("Bùi Thanh Hải",    "SE200008", "Kỹ thuật phần mềm",   "K20", 0.8),
                new Student("Ngô Thị Inh",      "SE200009", "An toàn thông tin",    "K20", 2.5),
                new Student("Lý Minh Khoa",     "SE200010", "Trí tuệ nhân tạo",    "K20", 3.5),
            };
        }

        // ===== INTERFACE IMPLEMENTATION =====

        /// <summary>
        /// Lấy danh sách tất cả sinh viên
        /// </summary>
        public List<Student> GetAllStudents()
        {
            return new List<Student>(_students); // Trả về bản copy để bảo vệ dữ liệu
        }

        /// <summary>
        /// Tìm sinh viên theo MSSV.
        /// Thuật toán: Duyệt tuyến tính (Linear Search) theo MSSV.
        /// </summary>
        /// <exception cref="StudentNotFoundException">Khi MSSV không tồn tại</exception>
        public Student GetStudentByMssv(string mssv)
        {
            if (string.IsNullOrWhiteSpace(mssv))
                throw new ArgumentException("MSSV không được để trống.");

            var student = _students.FirstOrDefault(s => 
                s.Mssv.Equals(mssv.Trim(), StringComparison.OrdinalIgnoreCase));

            if (student == null)
                throw new StudentNotFoundException(mssv);

            return student;
        }

        /// <summary>
        /// Thêm sinh viên mới.
        /// Kiểm tra trùng MSSV trước khi thêm.
        /// </summary>
        /// <exception cref="DuplicateStudentException">Khi MSSV đã tồn tại</exception>
        public void AddStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student), "Sinh viên không được null.");

            // Kiểm tra trùng MSSV
            bool exists = _students.Any(s => 
                s.Mssv.Equals(student.Mssv, StringComparison.OrdinalIgnoreCase));

            if (exists)
                throw new DuplicateStudentException(student.Mssv);

            // Gán username và password mặc định
            student.Username = student.Mssv;
            student.Password = "1";
            student.Id = student.Mssv;

            _students.Add(student);
            SaveChanges();
        }

        /// <summary>
        /// Cập nhật thông tin sinh viên.
        /// Tìm sinh viên cũ bằng MSSV rồi thay thế.
        /// </summary>
        /// <exception cref="StudentNotFoundException">Khi không tìm thấy MSSV</exception>
        public void UpdateStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student), "Sinh viên không được null.");

            int index = _students.FindIndex(s => 
                s.Mssv.Equals(student.Mssv, StringComparison.OrdinalIgnoreCase));

            if (index == -1)
                throw new StudentNotFoundException(student.Mssv);

            // Giữ username và password
            student.Username = student.Mssv;
            if (string.IsNullOrEmpty(student.Password))
                student.Password = "1";
            student.Id = student.Mssv;

            _students[index] = student;
            SaveChanges();
        }

        /// <summary>
        /// Xóa sinh viên theo MSSV.
        /// </summary>
        /// <exception cref="StudentNotFoundException">Khi không tìm thấy MSSV</exception>
        public void DeleteStudent(string mssv)
        {
            if (string.IsNullOrWhiteSpace(mssv))
                throw new ArgumentException("MSSV không được để trống.");

            var student = _students.FirstOrDefault(s => 
                s.Mssv.Equals(mssv.Trim(), StringComparison.OrdinalIgnoreCase));

            if (student == null)
                throw new StudentNotFoundException(mssv);

            _students.Remove(student);
            SaveChanges();
        }

        /// <summary>
        /// Tìm kiếm sinh viên theo từ khóa.
        /// Thuật toán: Tìm kiếm mờ (Fuzzy Search) trên nhiều trường.
        /// So khớp không phân biệt hoa/thường trên: Tên, MSSV, Ngành học, Khóa học.
        /// </summary>
        public List<Student> SearchStudents(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAllStudents();

            string searchTerm = keyword.Trim().ToLower();

            return _students.Where(s =>
                s.Name.ToLower().Contains(searchTerm) ||
                s.Mssv.ToLower().Contains(searchTerm) ||
                s.NganhHoc.ToLower().Contains(searchTerm) ||
                s.KhoaHoc.ToLower().Contains(searchTerm) ||
                s.HocLucDisplay.ToLower().Contains(searchTerm)
            ).ToList();
        }

        /// <summary>
        /// Lưu dữ liệu xuống file JSON.
        /// Xử lý ngoại lệ IOException.
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                string? directory = Path.GetDirectoryName(_filePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string jsonContent = JsonSerializer.Serialize(_students, _jsonOptions);
                File.WriteAllText(_filePath, jsonContent);
            }
            catch (IOException ex)
            {
                throw new IOException($"Không thể lưu dữ liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tải lại dữ liệu từ file
        /// </summary>
        public void ReloadData()
        {
            LoadData();
        }
    }
}
