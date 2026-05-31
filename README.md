# FUS-SMoGPT - Ứng Dụng Quản Lý Sinh Viên

🎓 **Student Management System** built with WPF C# (.NET)

## Mô tả

Ứng dụng desktop quản lý sinh viên với giao diện hiện đại, sử dụng nền tảng WPF và kiến trúc MVVM. Hỗ trợ hệ thống phân quyền đăng nhập (Admin / Sinh viên).

## Chức năng

- **Đăng nhập** phân quyền (Admin / Sinh viên)
- **CRUD** sinh viên (chỉ Admin)
- **Xem thông tin** cá nhân (Sinh viên)
- **Tìm kiếm** theo tên, MSSV, ngành, khóa
- **Xếp loại học lực** tự động từ GPA

## Công nghệ

- C# / .NET 10
- WPF (Windows Presentation Foundation)
- MVVM Architecture
- JSON Data Storage

## Cách chạy

```bash
# Clone repository
git clone <url>
cd FUS-SMoGPT

# Build và chạy
dotnet run
```

## Tài khoản đăng nhập

| Vai trò | Username | Password |
|---------|----------|----------|
| Admin | admin | 123 |
| Sinh viên | MSSV (vd: SE180001) | 1 |

## Thang xếp loại GPA

| GPA | Học lực |
|-----|---------|
| 0 → <1 | Kém |
| 1 → <2 | Yếu |
| 2 → <2.5 | Trung Bình |
| 2.5 → <3.5 | Khá |
| 3.5 → <4 | Giỏi |
| = 4 | Xuất Sắc |

## Kỹ thuật OOP sử dụng

- ✅ Lớp, phương thức, field, properties
- ✅ Kế thừa (Student : Person, Admin : Person)
- ✅ Đa hình (Override GetDisplayInfo, HasPermission)
- ✅ Interface (IUser, IDataService, IAuthService)
- ✅ Abstract class (Person)
- ✅ Exception handling (Custom exceptions)

## Cấu trúc thư mục

```
FUS-SMoGPT/
├── Models/          # Lớp dữ liệu (OOP)
├── ViewModels/      # Logic giao diện (MVVM)
├── Views/           # Giao diện XAML
├── Services/        # Dịch vụ đọc/ghi dữ liệu
├── Exceptions/      # Ngoại lệ tùy chỉnh
├── Themes/          # Styles và theme
├── Data/            # Dữ liệu JSON
└── BaoCao_FUS-SMoGPT.txt  # Báo cáo
```

## Tác giả

Đồ án môn học - FPT University
