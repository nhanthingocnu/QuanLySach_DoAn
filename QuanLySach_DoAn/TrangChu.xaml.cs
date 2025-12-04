using System.Windows;
using XAML_KHUNG_QLSach;

namespace QuanLySach_DoAn
{
    /// <summary>
    /// Interaction logic for TrangChu.xaml
    /// </summary>
    public partial class TrangChu : Window
    {
        public TrangChu()
        {
            InitializeComponent(); 
        }
        
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            DangNhap login = new DangNhap();
            login.Show();
            this.Close(); 
        }

        
        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
