using System;
using System.Windows;
using System.Windows.Input;
using FUS_SMoGPT.Models;
using FUS_SMoGPT.Services;

namespace FUS_SMoGPT.ViewModels
{
    /// <summary>
    /// ViewModel cho màn hình đăng nhập.
    /// Kế thừa từ BaseViewModel (kỹ thuật kế thừa).
    /// Xử lý logic đăng nhập và hiển thị lỗi.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        // ===== FIELDS =====
        private readonly IAuthService _authService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isErrorVisible;

        // ===== PROPERTIES =====

        /// <summary>
        /// Tên đăng nhập (bind với TextBox)
        /// </summary>
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        /// <summary>
        /// Mật khẩu (bind với PasswordBox qua code-behind)
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Thông báo lỗi hiển thị khi đăng nhập thất bại
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
        /// Điều khiển visibility của thông báo lỗi
        /// </summary>
        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set => SetProperty(ref _isErrorVisible, value);
        }

        // ===== COMMANDS =====

        /// <summary>
        /// Command đăng nhập (bind với Button)
        /// </summary>
        public ICommand LoginCommand { get; }

        // ===== EVENTS =====

        /// <summary>
        /// Event kích hoạt khi đăng nhập thành công.
        /// Truyền IUser ra ngoài để MainWindow xử lý.
        /// </summary>
        public event Action<IUser>? LoginSuccessful;

        // ===== CONSTRUCTOR =====

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        // ===== METHODS =====

        /// <summary>
        /// Kiểm tra điều kiện để nút đăng nhập hoạt động
        /// </summary>
        private bool CanExecuteLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Username);
        }

        /// <summary>
        /// Thực hiện đăng nhập.
        /// 
        /// Thuật toán:
        /// 1. Validate input (username không rỗng)
        /// 2. Gọi AuthService.Authenticate()
        /// 3. Nếu thành công → kích hoạt LoginSuccessful event
        /// 4. Nếu thất bại → hiển thị thông báo lỗi
        /// 
        /// Sử dụng Exception handling cho các trường hợp lỗi.
        /// </summary>
        private void ExecuteLogin(object? parameter)
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate
                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Vui lòng nhập tên đăng nhập.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Vui lòng nhập mật khẩu.";
                    return;
                }

                // Xác thực
                IUser? user = _authService.Authenticate(Username, Password);

                if (user != null)
                {
                    // Đăng nhập thành công
                    LoginSuccessful?.Invoke(user);
                }
                else
                {
                    // Đăng nhập thất bại
                    ErrorMessage = "Sai tên đăng nhập hoặc mật khẩu!\nAdmin: username = admin, mật khẩu = 123\nSinh viên: username = MSSV, mật khẩu = 1";
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ bất ngờ
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }
        }
    }
}
