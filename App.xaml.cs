using System.Windows;
using FUS_SMoGPT.Services;
using FUS_SMoGPT.ViewModels;
using FUS_SMoGPT.Views;

namespace FUS_SMoGPT
{
    /// <summary>
    /// Application entry point.
    /// Khởi tạo các services và hiển thị LoginWindow.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Được gọi khi ứng dụng khởi động.
        /// Tạo các dependency và mở cửa sổ đăng nhập.
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Khởi tạo Services (Dependency setup)
            IDataService dataService = new JsonDataService();
            IAuthService authService = new AuthService(dataService);

            // Khởi tạo LoginViewModel
            var loginViewModel = new LoginViewModel(authService);

            // Mở LoginWindow
            var loginWindow = new LoginWindow(loginViewModel);
            loginWindow.Show();
        }
    }
}
