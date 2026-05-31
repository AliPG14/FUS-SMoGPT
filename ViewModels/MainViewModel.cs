using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FUS_SMoGPT.Exceptions;
using FUS_SMoGPT.Models;
using FUS_SMoGPT.Services;

namespace FUS_SMoGPT.ViewModels
{
    /// <summary>
    /// ViewModel chính của ứng dụng sau khi đăng nhập.
    /// Quản lý hiển thị danh sách sinh viên và các thao tác CRUD.
    /// Kế thừa từ BaseViewModel (kỹ thuật kế thừa).
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        // ===== FIELDS =====
        private readonly IDataService _dataService;
        private readonly IAuthService _authService;
        private IUser _currentUser = null!;
        private Student? _selectedStudent;
        private string _searchText = string.Empty;
        private string _statusMessage = string.Empty;
        private bool _isAdmin;

        // ===== PROPERTIES =====

        /// <summary>
        /// Người dùng hiện tại đang đăng nhập
        /// </summary>
        public IUser CurrentUser
        {
            get => _currentUser;
            set
            {
                SetProperty(ref _currentUser, value);
                IsAdmin = value?.Role == UserRole.Admin;
                OnPropertyChanged(nameof(CurrentUserDisplay));
                OnPropertyChanged(nameof(WelcomeMessage));
                LoadStudents();
            }
        }

        /// <summary>
        /// Tên hiển thị của người dùng hiện tại
        /// </summary>
        public string CurrentUserDisplay => _currentUser?.GetDisplayInfo() ?? "";

        /// <summary>
        /// Thông báo chào mừng
        /// </summary>
        public string WelcomeMessage => _currentUser != null
            ? $"Xin chào, {(_currentUser is Student s ? s.Name : "Quản trị viên")}!"
            : "";

        /// <summary>
        /// Người dùng có phải Admin không (điều khiển visibility các nút CRUD)
        /// </summary>
        public bool IsAdmin
        {
            get => _isAdmin;
            set => SetProperty(ref _isAdmin, value);
        }

        /// <summary>
        /// Danh sách sinh viên (ObservableCollection tự động update UI)
        /// </summary>
        public ObservableCollection<Student> Students { get; } = new ObservableCollection<Student>();

        /// <summary>
        /// Sinh viên đang được chọn trong DataGrid
        /// </summary>
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                SetProperty(ref _selectedStudent, value);
                OnPropertyChanged(nameof(SelectedStudentInfo));
                OnPropertyChanged(nameof(HasSelectedStudent));
            }
        }

        /// <summary>
        /// Thông tin chi tiết sinh viên đang chọn
        /// </summary>
        public string SelectedStudentInfo => _selectedStudent?.GetDisplayInfo() ?? "Chưa chọn sinh viên nào.";

        /// <summary>
        /// Có sinh viên nào đang được chọn không
        /// </summary>
        public bool HasSelectedStudent => _selectedStudent != null;

        /// <summary>
        /// Text tìm kiếm
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                PerformSearch();
            }
        }

        /// <summary>
        /// Thông báo trạng thái ở thanh status bar
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // ===== COMMANDS =====
        public ICommand AddStudentCommand { get; }
        public ICommand EditStudentCommand { get; }
        public ICommand DeleteStudentCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LogoutCommand { get; }

        // ===== EVENTS =====

        /// <summary>
        /// Event yêu cầu mở dialog thêm/sửa sinh viên
        /// </summary>
        public event Action<Student?>? RequestStudentForm;

        /// <summary>
        /// Event yêu cầu đăng xuất
        /// </summary>
        public event Action? RequestLogout;

        // ===== CONSTRUCTOR =====
        public MainViewModel(IDataService dataService, IAuthService authService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            // Khởi tạo Commands
            AddStudentCommand = new RelayCommand(ExecuteAddStudent, _ => IsAdmin);
            EditStudentCommand = new RelayCommand(ExecuteEditStudent, _ => IsAdmin && HasSelectedStudent);
            DeleteStudentCommand = new RelayCommand(ExecuteDeleteStudent, _ => IsAdmin && HasSelectedStudent);
            RefreshCommand = new RelayCommand(_ => LoadStudents());
            LogoutCommand = new RelayCommand(_ => RequestLogout?.Invoke());
        }

        // ===== METHODS =====

        /// <summary>
        /// Tải danh sách sinh viên.
        /// Admin: xem tất cả | Sinh viên: chỉ xem bản thân.
        /// </summary>
        public void LoadStudents()
        {
            try
            {
                Students.Clear();

                if (_currentUser == null) return;

                if (IsAdmin)
                {
                    // Admin xem tất cả sinh viên
                    var allStudents = _dataService.GetAllStudents();
                    foreach (var student in allStudents)
                    {
                        Students.Add(student);
                    }
                    StatusMessage = $"Tổng cộng {Students.Count} sinh viên.";
                }
                else if (_currentUser is Student currentStudent)
                {
                    // Sinh viên chỉ xem bản thân
                    try
                    {
                        var self = _dataService.GetStudentByMssv(currentStudent.Mssv);
                        Students.Add(self);
                        SelectedStudent = self;
                        StatusMessage = "Đang xem thông tin cá nhân.";
                    }
                    catch (StudentNotFoundException)
                    {
                        StatusMessage = "Không tìm thấy thông tin sinh viên.";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi tải dữ liệu: {ex.Message}";
            }
        }

        /// <summary>
        /// Tìm kiếm sinh viên theo từ khóa (chỉ Admin)
        /// </summary>
        private void PerformSearch()
        {
            try
            {
                if (!IsAdmin) return;

                Students.Clear();
                var results = _dataService.SearchStudents(SearchText);
                foreach (var student in results)
                {
                    Students.Add(student);
                }
                StatusMessage = string.IsNullOrWhiteSpace(SearchText)
                    ? $"Tổng cộng {Students.Count} sinh viên."
                    : $"Tìm thấy {Students.Count} sinh viên phù hợp.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi tìm kiếm: {ex.Message}";
            }
        }

        /// <summary>
        /// Mở form thêm sinh viên mới
        /// </summary>
        private void ExecuteAddStudent(object? parameter)
        {
            RequestStudentForm?.Invoke(null); // null = thêm mới
        }

        /// <summary>
        /// Mở form sửa sinh viên đang chọn
        /// </summary>
        private void ExecuteEditStudent(object? parameter)
        {
            if (SelectedStudent != null)
            {
                RequestStudentForm?.Invoke(SelectedStudent);
            }
        }

        /// <summary>
        /// Xóa sinh viên đang chọn.
        /// Hiển thị dialog xác nhận trước khi xóa.
        /// </summary>
        private void ExecuteDeleteStudent(object? parameter)
        {
            if (SelectedStudent == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa sinh viên?\n\n" +
                    $"Họ tên: {SelectedStudent.Name}\n" +
                    $"MSSV: {SelectedStudent.Mssv}",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    string deletedName = SelectedStudent.Name;
                    _dataService.DeleteStudent(SelectedStudent.Mssv);
                    SelectedStudent = null;
                    LoadStudents();
                    StatusMessage = $"Đã xóa sinh viên {deletedName}.";
                }
            }
            catch (StudentNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = $"Lỗi: {ex.Message}";
            }
        }

        /// <summary>
        /// Thêm sinh viên mới vào hệ thống (được gọi từ form dialog)
        /// </summary>
        public void AddNewStudent(Student student)
        {
            try
            {
                _dataService.AddStudent(student);
                LoadStudents();
                StatusMessage = $"Đã thêm sinh viên {student.Name} thành công.";
            }
            catch (DuplicateStudentException)
            {
                throw; // Re-throw để form dialog xử lý
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi thêm sinh viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin sinh viên (được gọi từ form dialog)
        /// </summary>
        public void UpdateExistingStudent(Student student)
        {
            try
            {
                _dataService.UpdateStudent(student);
                LoadStudents();
                StatusMessage = $"Đã cập nhật thông tin sinh viên {student.Name}.";
            }
            catch (StudentNotFoundException)
            {
                throw; // Re-throw để form dialog xử lý
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật: {ex.Message}", ex);
            }
        }
    }
}
