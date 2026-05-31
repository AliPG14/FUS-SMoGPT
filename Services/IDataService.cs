using System.Collections.Generic;
using FUS_SMoGPT.Models;

namespace FUS_SMoGPT.Services
{
    /// <summary>
    /// Interface định nghĩa hợp đồng cho dịch vụ quản lý dữ liệu sinh viên.
    /// Sử dụng Interface để đảm bảo loose coupling - có thể dễ dàng 
    /// thay đổi implementation (JSON, SQLite, SQL Server...) mà không ảnh hưởng code khác.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Lấy danh sách tất cả sinh viên
        /// </summary>
        List<Student> GetAllStudents();

        /// <summary>
        /// Tìm sinh viên theo MSSV
        /// </summary>
        /// <param name="mssv">Mã số sinh viên cần tìm</param>
        /// <returns>Đối tượng Student nếu tìm thấy</returns>
        /// <exception cref="Exceptions.StudentNotFoundException">Khi không tìm thấy MSSV</exception>
        Student GetStudentByMssv(string mssv);

        /// <summary>
        /// Thêm sinh viên mới vào hệ thống
        /// </summary>
        /// <param name="student">Sinh viên cần thêm</param>
        /// <exception cref="Exceptions.DuplicateStudentException">Khi MSSV đã tồn tại</exception>
        void AddStudent(Student student);

        /// <summary>
        /// Cập nhật thông tin sinh viên
        /// </summary>
        /// <param name="student">Sinh viên với thông tin đã cập nhật</param>
        /// <exception cref="Exceptions.StudentNotFoundException">Khi không tìm thấy MSSV</exception>
        void UpdateStudent(Student student);

        /// <summary>
        /// Xóa sinh viên khỏi hệ thống
        /// </summary>
        /// <param name="mssv">MSSV của sinh viên cần xóa</param>
        /// <exception cref="Exceptions.StudentNotFoundException">Khi không tìm thấy MSSV</exception>
        void DeleteStudent(string mssv);

        /// <summary>
        /// Tìm kiếm sinh viên theo từ khóa (tên hoặc MSSV)
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách sinh viên phù hợp</returns>
        List<Student> SearchStudents(string keyword);

        /// <summary>
        /// Lưu thay đổi xuống file
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Tải lại dữ liệu từ file
        /// </summary>
        void ReloadData();
    }
}
