using System;
using System.Collections.Generic;
using System.Windows.Input;
using FUS_SMoGPT.Exceptions;
using FUS_SMoGPT.Models;

namespace FUS_SMoGPT.ViewModels
{
    /// <summary>
    /// ViewModel cho form thêm/sửa sinh viên.
    /// Kế thừa từ BaseViewModel (kỹ thuật kế thừa).
    /// Dùng chung cho cả 2 chức năng: Thêm mới và Chỉnh sửa.
    /// </summary>
    public class StudentFormViewModel : BaseViewModel
    {
        // ===== FIELDS =====
        private string _name = string.Empty;
        private string _mssv = string.Empty;
        private string _selectedNganhHoc = string.Empty;
        private string _selectedKhoaHoc = string.Empty;
        private string _gpaText = string.Empty;
        private string _hocLucPreview = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isEditMode;
        private bool _isErrorVisible;

        // ===== PROPERTIES =====

        /// <summary>
        /// Họ và tên sinh viên
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                ValidateForm();
            }
        }

        /// <summary>
        /// Mã số sinh viên (readonly khi edit)
        /// </summary>
        public string Mssv
        {
            get => _mssv;
            set
            {
                SetProperty(ref _mssv, value);
                ValidateForm();
            }
        }

        /// <summary>
        /// Ngành học đã chọn
        /// </summary>
        public string SelectedNganhHoc
        {
            get => _selectedNganhHoc;
            set
            {
                SetProperty(ref _selectedNganhHoc, value);
                ValidateForm();
            }
        }

        /// <summary>
        /// Khóa học đã chọn
        /// </summary>
        public string SelectedKhoaHoc
        {
            get => _selectedKhoaHoc;
            set
            {
                SetProperty(ref _selectedKhoaHoc, value);
                ValidateForm();
            }
        }

        /// <summary>
        /// Text GPA (dùng string để xử lý validation dễ hơn)
        /// </summary>
        public string GpaText
        {
            get => _gpaText;
            set
            {
                SetProperty(ref _gpaText, value);
                UpdateHocLucPreview();
                ValidateForm();
            }
        }

        /// <summary>
        /// Preview xếp loại học lực khi nhập GPA
        /// </summary>
        public string HocLucPreview
        {
            get => _hocLucPreview;
            set => SetProperty(ref _hocLucPreview, value);
        }

        /// <summary>
        /// Thông báo lỗi
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                IsErrorVisible = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Hiển thị lỗi hay không
        /// </summary>
        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set => SetProperty(ref _isErrorVisible, value);
        }

        /// <summary>
        /// Đang ở chế độ sửa (true) hay thêm mới (false)
        /// </summary>
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                SetProperty(ref _isEditMode, value);
                OnPropertyChanged(nameof(FormTitle));
                OnPropertyChanged(nameof(IsMssvEditable));
            }
        }

        /// <summary>
        /// MSSV có thể chỉnh sửa không (chỉ khi thêm mới)
        /// </summary>
        public bool IsMssvEditable => !_isEditMode;

        /// <summary>
        /// Tiêu đề form
        /// </summary>
        public string FormTitle => _isEditMode ? "✏️ Chỉnh Sửa Sinh Viên" : "➕ Thêm Sinh Viên Mới";

        /// <summary>
        /// Danh sách ngành học để chọn
        /// </summary>
        public List<string> DanhSachNganhHoc { get; } = new List<string>
        {
            "Công nghệ thông tin",
            "Kỹ thuật phần mềm",
            "An toàn thông tin",
            "Trí tuệ nhân tạo",
            "Thiết kế đồ họa",
            "Quản trị kinh doanh",
            "Marketing",
            "Ngôn ngữ Anh",
            "Ngôn ngữ Nhật",
            "Ngôn ngữ Hàn"
        };

        /// <summary>
        /// Danh sách khóa học
        /// </summary>
        public List<string> DanhSachKhoaHoc { get; } = new List<string>
        {
            "K16", "K17", "K18", "K19", "K20", "K21", "K22", "K23", "K24"
        };

        // ===== COMMANDS =====
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // ===== EVENTS =====

        /// <summary>
        /// Kích hoạt khi lưu thành công, truyền Student ra ngoài
        /// </summary>
        public event Action<Student>? SaveCompleted;

        /// <summary>
        /// Kích hoạt khi hủy form
        /// </summary>
        public event Action? CancelRequested;

        // ===== CONSTRUCTOR =====

        /// <summary>
        /// Constructor.
        /// Nếu existingStudent != null → chế độ sửa.
        /// Nếu existingStudent == null → chế độ thêm mới.
        /// </summary>
        public StudentFormViewModel(Student? existingStudent = null)
        {
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(_ => CancelRequested?.Invoke());

            if (existingStudent != null)
            {
                // Chế độ sửa - điền thông tin hiện tại
                IsEditMode = true;
                Name = existingStudent.Name;
                Mssv = existingStudent.Mssv;
                SelectedNganhHoc = existingStudent.NganhHoc;
                SelectedKhoaHoc = existingStudent.KhoaHoc;
                GpaText = existingStudent.Gpa.ToString("F2");
            }
            else
            {
                // Chế độ thêm mới
                IsEditMode = false;
            }
        }

        // ===== METHODS =====

        /// <summary>
        /// Cập nhật preview xếp loại học lực khi nhập GPA.
        /// Thuật toán: Parse GPA → tạo Student tạm → lấy HocLucDisplay.
        /// </summary>
        private void UpdateHocLucPreview()
        {
            try
            {
                if (double.TryParse(GpaText, out double gpa) && gpa >= 0 && gpa <= 4.0)
                {
                    // Tạo Student tạm để tính HocLuc (sử dụng đa hình)
                    var tempStudent = new Student { Gpa = gpa };
                    HocLucPreview = $"📊 Học lực: {tempStudent.HocLucDisplay}";
                }
                else if (!string.IsNullOrEmpty(GpaText))
                {
                    HocLucPreview = "⚠️ GPA phải từ 0.0 đến 4.0";
                }
                else
                {
                    HocLucPreview = string.Empty;
                }
            }
            catch
            {
                HocLucPreview = "⚠️ Giá trị GPA không hợp lệ";
            }
        }

        /// <summary>
        /// Kiểm tra form có hợp lệ để lưu không
        /// </summary>
        private bool CanExecuteSave(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Name)
                && !string.IsNullOrWhiteSpace(Mssv)
                && !string.IsNullOrWhiteSpace(SelectedNganhHoc)
                && !string.IsNullOrWhiteSpace(SelectedKhoaHoc)
                && double.TryParse(GpaText, out double gpa)
                && gpa >= 0 && gpa <= 4.0;
        }

        /// <summary>
        /// Validate form và cập nhật ErrorMessage
        /// </summary>
        private void ValidateForm()
        {
            ErrorMessage = string.Empty;
        }

        /// <summary>
        /// Thực hiện lưu sinh viên.
        /// 
        /// Thuật toán:
        /// 1. Validate tất cả fields
        /// 2. Parse GPA và kiểm tra range (0-4)
        /// 3. Tạo đối tượng Student mới
        /// 4. Kích hoạt SaveCompleted event
        /// 
        /// Sử dụng Exception handling:
        /// - InvalidGpaException khi GPA ngoài range
        /// - ArgumentException khi field rỗng
        /// </summary>
        private void ExecuteSave(object? parameter)
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate từng field
                if (string.IsNullOrWhiteSpace(Name))
                {
                    ErrorMessage = "Vui lòng nhập họ và tên.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Mssv))
                {
                    ErrorMessage = "Vui lòng nhập mã số sinh viên.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedNganhHoc))
                {
                    ErrorMessage = "Vui lòng chọn ngành học.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedKhoaHoc))
                {
                    ErrorMessage = "Vui lòng chọn khóa học.";
                    return;
                }

                // Parse và validate GPA
                if (!double.TryParse(GpaText, out double gpa))
                {
                    ErrorMessage = "GPA phải là một số hợp lệ (vd: 3.5).";
                    return;
                }

                if (gpa < 0 || gpa > 4.0)
                {
                    throw new InvalidGpaException(gpa);
                }

                // Tạo đối tượng Student
                var student = new Student(Name.Trim(), Mssv.Trim(), SelectedNganhHoc, SelectedKhoaHoc, gpa);

                // Kích hoạt event
                SaveCompleted?.Invoke(student);
            }
            catch (InvalidGpaException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }
        }
    }
}
