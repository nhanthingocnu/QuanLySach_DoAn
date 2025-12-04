using System;
using System.Linq;
using System.Windows;

namespace QuanLySach_DoAn
{
    /// <summary>
    /// Interaction logic for DangNhap.xaml
    /// </summary>
    public partial class DangNhap : Window
    {
        public static string MatKhauCu;
        private readonly SQL_SACHEntities db;
        int dem = 0;

        public DangNhap()
        {
            InitializeComponent();
            db = new SQL_SACHEntities();
        }

        private void DangNhap_Click(object sender, RoutedEventArgs e)
        {
            // Tìm tài khoản trong DB
            var taikhoan = db.TAIKHOANs
                .FirstOrDefault(tk => tk.TenDangNhap == txt_TenDangNhap.Text
                                   && tk.MatKhau == txt_MatKhau.Password);

            if (taikhoan != null)
            {
                MatKhauCu = taikhoan.MatKhau;
                // Đăng nhập thành công -> mở MainWindow
                var mw = new XAML_KHUNG_QLSach.MainWindow();
                mw.Show();
                this.Close();
                dem = 0;
            }
            else
            {
                dem++;
                MessageBox.Show("Sai Tài khoản/Mật khẩu lần " + dem);

                if (dem >= 3)
                {
                    MessageBox.Show("Nhập sai quá 3 lần, thoát chương trình!");
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
