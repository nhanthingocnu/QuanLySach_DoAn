using QuanLySach_DoAn;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions; // thêm Regex
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XAML_KHUNG_QLSach
{
    public partial class MainWindow : Window
    {
        private readonly SQL_SACHEntities db;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QLSach;Integrated Security=True";

        public MainWindow()
        {
            InitializeComponent();
            db = new SQL_SACHEntities();
            LoadCombo();
            LoadGrid();
        }

        // ✅ Chỉ cho phép nhập số vào Giá Bán
        private void txt_GiaBan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // chỉ cho nhập số
            e.Handled = regex.IsMatch(e.Text);
        }

        // ✅ Tự động format khi nhập Giá Bán
        private void txt_GiaBan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                string text = textBox.Text.Replace(".", "").Replace(",", "").Replace("₫", "").Trim();

                if (decimal.TryParse(text, out decimal value))
                {
                    textBox.TextChanged -= txt_GiaBan_TextChanged; // tránh vòng lặp
                    textBox.Text = string.Format(CultureInfo.InvariantCulture, "{0:N0}", value);
                    textBox.CaretIndex = textBox.Text.Length;
                    textBox.TextChanged += txt_GiaBan_TextChanged;
                }
            }
        }

        // Load DataGrid
        public void LoadGrid()
        {
            dg_Sach.ItemsSource = db.SACHes
                .Include(s => s.THELOAI) // join để hiển thị luôn tên thể loại
                .ToList();
        }

        // Load combobox thể loại
        public void LoadCombo()
        {
            var dsTheLoai = db.THELOAIs.ToList();
            cb_TheLoai.ItemsSource = dsTheLoai;

            if (dsTheLoai.Count > 0)
                cb_TheLoai.SelectedIndex = 0;
        }

        // Thêm sách
        private void Them_Click(object sender, RoutedEventArgs e)
        {
            SACH sachMoi = null;
            try
            {
                sachMoi = new SACH
                {
                    MaSach = txt_MaSach.Text,
                    TenSach = txt_TenSach.Text,
                    TacGia = txt_TacGia.Text,
                    NamXB = dp_NamXB.SelectedDate,
                    MaTheLoai = (int?)cb_TheLoai.SelectedValue,
                    SoLuong = int.Parse(txt_SoLuong.Text),
                    GiaBan = decimal.Parse(txt_GiaBan.Text, NumberStyles.AllowThousands, CultureInfo.InvariantCulture),
                    TrangThai = chk_TrangThai.IsChecked == true
                };

                db.SACHes.Add(sachMoi);
                db.SaveChanges();
                MessageBox.Show("Thêm thành công");
                LoadGrid();
            }
            catch (Exception)
            {
                if (sachMoi != null)
                    db.Entry(sachMoi).State = EntityState.Detached;

                MessageBox.Show("Trùng Mã Sách hoặc dữ liệu không hợp lệ!");
            }
        }


        // Xoá sách
        private void Xoa_Click(object sender, RoutedEventArgs e)
        {
            var sachXoa = db.SACHes.FirstOrDefault(s => s.MaSach == txt_MaSach.Text);
            if (sachXoa == null)
            {
                MessageBox.Show("Không tìm thấy để xoá");
                return;
            }

            db.SACHes.Remove(sachXoa);
            db.SaveChanges();
            MessageBox.Show("Xoá thành công");
            LoadGrid();
        }

        // Sửa thông tin sách
        private void Sua_Click(object sender, RoutedEventArgs e)
        {
            var sachSua = db.SACHes.FirstOrDefault(s => s.MaSach == txt_MaSach.Text);
            if (sachSua == null)
            {
                MessageBox.Show("Không có để sửa");
                return;
            }

            sachSua.TenSach = txt_TenSach.Text;
            sachSua.TacGia = txt_TacGia.Text;
            sachSua.NamXB = dp_NamXB.SelectedDate;
            sachSua.MaTheLoai = (int?)cb_TheLoai.SelectedValue;

            sachSua.TrangThai = chk_TrangThai.IsChecked == true ? true : false;
            sachSua.SoLuong = int.Parse(txt_SoLuong.Text);
            sachSua.GiaBan = decimal.Parse(txt_GiaBan.Text);
            int thayDoi = db.SaveChanges();
            if (thayDoi > 0)
                MessageBox.Show("Sửa thành công");
            else
                MessageBox.Show("Không có thay đổi");

            LoadGrid();
        }

      
        private void dg_Sach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_Sach.SelectedItem is SACH sach)
            {
               
                txt_MaSach.Text = sach.MaSach ?? string.Empty;
                txt_TenSach.Text = sach.TenSach ?? string.Empty;
                txt_TacGia.Text = sach.TacGia ?? string.Empty;

              
                txt_SoLuong.Text = sach.SoLuong.ToString();

                
                txt_GiaBan.Text = sach.GiaBan.ToString("#,0", new CultureInfo("vi-VN"));



                
                if (sach.NamXB.HasValue)
                    dp_NamXB.SelectedDate = sach.NamXB.Value;
                else
                    dp_NamXB.SelectedDate = null;
            
               
                cb_TheLoai.SelectedValue = sach.MaTheLoai;

            
                chk_TrangThai.IsChecked = sach.TrangThai;
            }
            else
            {
                
                txt_MaSach.Clear();
                txt_TenSach.Clear();
                txt_TacGia.Clear();
                txt_SoLuong.Clear();
                txt_GiaBan.Clear();
                dp_NamXB.SelectedDate = null;
                if (cb_TheLoai.Items.Count > 0)
                    cb_TheLoai.SelectedIndex = 0;
                chk_TrangThai.IsChecked = false;
            }
        }


        // Nút load lại toàn bộ
        private void LoadLai_Click(object sender, RoutedEventArgs e)
        {
            LoadGrid();
        }

        // Nút tìm kiếm
        private void Tim_Click(object sender, RoutedEventArgs e)
        {
            string tim = txt_Tim.Text;
            dg_Sach.ItemsSource = db.SACHes
                .Where(s => s.MaSach.Contains(tim) || s.TenSach.Contains(tim))
                .Include(s => s.THELOAI)
                .ToList();
        }

        // Nút làm mới 
        private void LamMoi_Click(object sender, RoutedEventArgs e)
        {
            txt_MaSach.Clear();
            txt_TenSach.Clear();
            txt_TacGia.Clear();
            txt_SoLuong.Clear();
            txt_GiaBan.Clear();

            dp_NamXB.SelectedDate = null;

            if (cb_TheLoai.Items.Count > 0)
                cb_TheLoai.SelectedIndex = 0;

            chk_TrangThai.IsChecked = false;
        }


        // Nút thoát  
        private void Thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDoiMatKhau_Click(object sender, RoutedEventArgs e)
        {
            DoiMatKhauWindow doiMK = new DoiMatKhauWindow();
            doiMK.ShowDialog();
        }

        private void btnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            
            this.Hide();

            
            DangNhap login = new DangNhap();
            login.Show();

           
            this.Close();
        }


        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        
        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string backupFolder = @"C:\Backup";
                string backupPath = System.IO.Path.Combine(backupFolder, "QLSach.bak");

                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                string query = $@"BACKUP DATABASE [QLSach] 
                                  TO DISK = N'{backupPath}' 
                                  WITH INIT, STATS = 10";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Backup thành công!\nFile lưu tại: " + backupPath, "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi Backup: " + ex.Message, "Lỗi");
            }
        }

      
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string backupPath = @"C:\Backup\QLSach.bak";

                if (!File.Exists(backupPath))
                {
                    MessageBox.Show("❌ Không tìm thấy file backup tại: " + backupPath, "Lỗi");
                    return;
                }

                string query = $@"
                    USE master;
                    ALTER DATABASE QLSach SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    RESTORE DATABASE QLSach FROM DISK = N'{backupPath}' WITH REPLACE;
                    ALTER DATABASE QLSach SET MULTI_USER;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Phục hồi dữ liệu thành công!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi Restore: " + ex.Message, "Lỗi");
            }
        }

    }

}


