using QuanLySach_DoAn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XAML_KHUNG_QLSach
{
    public partial class DoiMatKhauWindow : Window
    {
        
        public DoiMatKhauWindow()

        {
            
            InitializeComponent();
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnXacNhan_Click(object sender, RoutedEventArgs e)
        {
            string matKhauCu = txtMatKhauCu.Password;
            if (txtMatKhauMoi.Password == txtXacNhanMK.Password && DangNhap.MatKhauCu == matKhauCu)
            {
                MessageBox.Show("Đổi mật khẩu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Đóng form sau khi đổi mật khẩu
            }
            else
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
