using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FUS_SMoGPT.ViewModels
{
    /// <summary>
    /// Lớp cơ sở cho tất cả ViewModel trong ứng dụng.
    /// Implements INotifyPropertyChanged để hỗ trợ data binding trong WPF.
    /// 
    /// Kỹ thuật sử dụng:
    /// - Interface implementation (INotifyPropertyChanged)
    /// - Kế thừa: các ViewModel khác kế thừa từ BaseViewModel
    /// - CallerMemberName attribute để tự động lấy tên property
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event được kích hoạt khi một property thay đổi giá trị.
        /// WPF sử dụng event này để cập nhật giao diện tự động.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Thông báo cho UI rằng một property đã thay đổi.
        /// [CallerMemberName] tự động lấy tên property gọi method này.
        /// </summary>
        /// <param name="propertyName">Tên property đã thay đổi</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Helper method để set giá trị property và tự động notify UI.
        /// Chỉ notify khi giá trị thực sự thay đổi (tránh update không cần thiết).
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của property</typeparam>
        /// <param name="field">Reference đến backing field</param>
        /// <param name="value">Giá trị mới</param>
        /// <param name="propertyName">Tên property (tự động từ CallerMemberName)</param>
        /// <returns>true nếu giá trị đã thay đổi</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
