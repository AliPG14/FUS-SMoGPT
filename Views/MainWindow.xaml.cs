using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FUS_SMoGPT.Exceptions;
using FUS_SMoGPT.Models;
using FUS_SMoGPT.Services;
using FUS_SMoGPT.ViewModels;

namespace FUS_SMoGPT.Views
{
    /// <summary>
    /// Code-behind cho MainWindow.
    /// Xử lý logic UI: window controls, detail panel update, dialog mở.
    /// Business logic nằm trong MainViewModel.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly IDataService _dataService;

        public MainWindow(IUser currentUser)
        {
            InitializeComponent();

            // Khởi tạo Services
            _dataService = new JsonDataService();
            var authService = new AuthService(_dataService);

            // Khởi tạo ViewModel
            _viewModel = new MainViewModel(_dataService, authService);
            DataContext = _viewModel;

            // Đăng ký events
            _viewModel.RequestStudentForm += OnRequestStudentForm;
            _viewModel.RequestLogout += OnRequestLogout;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Set current user (triggers LoadStudents)
            _viewModel.CurrentUser = currentUser;
        }

        /// <summary>
        /// Cập nhật detail panel khi SelectedStudent thay đổi
        /// </summary>
        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedStudent))
            {
                UpdateDetailPanel();
            }
        }

        /// <summary>
        /// Cập nhật panel chi tiết sinh viên
        /// </summary>
        private void UpdateDetailPanel()
        {
            var student = _viewModel.SelectedStudent;

            if (student == null)
            {
                NoSelectionPanel.Visibility = Visibility.Visible;
                DetailPanel.Visibility = Visibility.Collapsed;
                return;
            }

            NoSelectionPanel.Visibility = Visibility.Collapsed;
            DetailPanel.Visibility = Visibility.Visible;

            // Update detail fields
            AvatarText.Text = student.Name.Length > 0 ? student.Name[0].ToString().ToUpper() : "?";
            DetailName.Text = student.Name;
            DetailMssv.Text = student.Mssv;
            DetailNganh.Text = student.NganhHoc;
            DetailKhoa.Text = student.KhoaHoc;
            DetailGpa.Text = student.Gpa.ToString("F2");
            DetailHocLuc.Text = student.HocLucDisplay;

            // Set badge color based on HocLuc
            HocLucBadge.Background = student.XepLoaiHocLuc switch
            {
                HocLuc.XuatSac => new LinearGradientBrush(Color.FromRgb(255, 215, 0), Color.FromRgb(255, 165, 0), 0),
                HocLuc.Gioi => new LinearGradientBrush(Color.FromRgb(0, 200, 81), Color.FromRgb(0, 230, 118), 0),
                HocLuc.Kha => new LinearGradientBrush(Color.FromRgb(78, 84, 200), Color.FromRgb(143, 148, 251), 0),
                HocLuc.TrungBinh => new LinearGradientBrush(Color.FromRgb(255, 187, 51), Color.FromRgb(255, 214, 102), 0),
                HocLuc.Yeu => new LinearGradientBrush(Color.FromRgb(255, 152, 0), Color.FromRgb(255, 183, 77), 0),
                HocLuc.Kem => new LinearGradientBrush(Color.FromRgb(255, 68, 68), Color.FromRgb(255, 107, 107), 0),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        /// <summary>
        /// Mở form thêm/sửa sinh viên
        /// </summary>
        private void OnRequestStudentForm(Student? existingStudent)
        {
            try
            {
                var formViewModel = new StudentFormViewModel(existingStudent);
                var dialog = new StudentFormDialog(formViewModel);
                dialog.Owner = this;

                formViewModel.SaveCompleted += (student) =>
                {
                    try
                    {
                        if (existingStudent == null)
                        {
                            // Thêm mới
                            _viewModel.AddNewStudent(student);
                        }
                        else
                        {
                            // Cập nhật
                            _viewModel.UpdateExistingStudent(student);
                        }
                        dialog.DialogResult = true;
                        dialog.Close();
                    }
                    catch (DuplicateStudentException ex)
                    {
                        MessageBox.Show(ex.Message, "Lỗi trùng MSSV", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                formViewModel.CancelRequested += () =>
                {
                    dialog.DialogResult = false;
                    dialog.Close();
                };

                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xử lý đăng xuất - quay về LoginWindow
        /// </summary>
        private void OnRequestLogout()
        {
            var dataService = new JsonDataService();
            var authService = new AuthService(dataService);
            var loginViewModel = new LoginViewModel(authService);
            var loginWindow = new LoginWindow(loginViewModel);
            loginWindow.Show();

            _viewModel.RequestStudentForm -= OnRequestStudentForm;
            _viewModel.RequestLogout -= OnRequestLogout;
            this.Close();
        }

        // ===== WINDOW CONTROLS =====

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
