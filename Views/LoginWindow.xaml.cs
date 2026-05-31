using System.Windows;
using System.Windows.Input;
using FUS_SMoGPT.Models;
using FUS_SMoGPT.ViewModels;

namespace FUS_SMoGPT.Views
{
    /// <summary>
    /// Code-behind cho LoginWindow.
    /// Chỉ chứa logic liên quan đến UI (không chứa business logic).
    /// Business logic nằm trong LoginViewModel.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Lắng nghe event đăng nhập thành công
            _viewModel.LoginSuccessful += OnLoginSuccessful;

            // Focus vào ô username khi mở
            Loaded += (s, e) => UsernameTextBox.Focus();
        }

        /// <summary>
        /// Xử lý khi đăng nhập thành công.
        /// Mở MainWindow và đóng LoginWindow.
        /// </summary>
        private void OnLoginSuccessful(IUser user)
        {
            var mainWindow = new MainWindow(user);
            mainWindow.Show();
            
            _viewModel.LoginSuccessful -= OnLoginSuccessful;
            this.Close();
        }

        /// <summary>
        /// Cho phép kéo window (vì WindowStyle = None)
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        /// <summary>
        /// Đóng ứng dụng
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Đồng bộ PasswordBox vào ViewModel (PasswordBox không hỗ trợ binding trực tiếp)
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        /// <summary>
        /// Nhấn Enter trong TextBox username → focus sang Password
        /// </summary>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox.Focus();
            }
        }

        /// <summary>
        /// Nhấn Enter trong PasswordBox → thực hiện đăng nhập
        /// </summary>
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.Password = PasswordBox.Password;
                if (_viewModel.LoginCommand.CanExecute(null))
                    _viewModel.LoginCommand.Execute(null);
            }
        }
    }
}
