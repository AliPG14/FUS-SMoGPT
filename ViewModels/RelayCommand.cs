using System;
using System.Windows.Input;

namespace FUS_SMoGPT.ViewModels
{
    /// <summary>
    /// Implementation của ICommand cho WPF command binding.
    /// Cho phép bind commands từ ViewModel vào các Button, MenuItem trong XAML.
    /// 
    /// Kỹ thuật sử dụng:
    /// - Interface implementation (ICommand)
    /// - Delegate (Action, Func)
    /// - Event (CanExecuteChanged)
    /// </summary>
    public class RelayCommand : ICommand
    {
        // ===== FIELDS =====
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        // ===== CONSTRUCTOR =====

        /// <summary>
        /// Tạo command với action thực thi
        /// </summary>
        /// <param name="execute">Action được thực thi khi command được gọi</param>
        /// <param name="canExecute">Điều kiện để command có thể thực thi (optional)</param>
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // ===== ICOMMAND IMPLEMENTATION =====

        /// <summary>
        /// Event kích hoạt khi điều kiện CanExecute thay đổi.
        /// Kết nối với CommandManager để tự động re-evaluate.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Kiểm tra command có thể thực thi không
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Thực thi command
        /// </summary>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Buộc UI re-evaluate CanExecute
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
