using System.Windows;
using System.Windows.Input;
using FUS_SMoGPT.ViewModels;

namespace FUS_SMoGPT.Views
{
    /// <summary>
    /// Code-behind cho StudentFormDialog.
    /// Dialog dùng chung cho thêm mới và chỉnh sửa sinh viên.
    /// </summary>
    public partial class StudentFormDialog : Window
    {
        public StudentFormDialog(StudentFormViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        /// <summary>
        /// Cho phép kéo dialog
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
