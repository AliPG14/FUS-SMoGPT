namespace FUS_SMoGPT.Models
{
    /// <summary>
    /// Vai trò người dùng trong hệ thống
    /// </summary>
    public enum UserRole
    {
        Admin,
        Student
    }

    /// <summary>
    /// Xếp loại học lực dựa trên GPA hệ 4
    /// 0 → <1: Kém | 1 → <2: Yếu | 2 → <2.5: Trung Bình
    /// 2.5 → <3.5: Khá | 3.5 → <4: Giỏi | 4: Xuất Sắc
    /// </summary>
    public enum HocLuc
    {
        Kem,        // 0 - <1
        Yeu,        // 1 - <2
        TrungBinh,  // 2 - <2.5
        Kha,        // 2.5 - <3.5
        Gioi,       // 3.5 - <4
        XuatSac     // 4
    }
}
