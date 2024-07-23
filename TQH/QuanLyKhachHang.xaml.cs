using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TQH
{
    /// <summary>
    /// Interaction logic for QuanLyKhachHang.xaml
    /// </summary>
    public partial class QuanLyKhachHang : UserControl
    {
        public QuanLyKhachHang()
        {
            InitializeComponent();
            LoadKH();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;");
        public void LoadKH()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select * from [KhachHang]", conn);
                DataTable dt = new DataTable();
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                conn.Close();
                DataKH.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DataKH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataKH.SelectedItem != null && DataKH.SelectedItem is DataRowView row)
            {
                txtMaKH.Text = row["MaKH"].ToString();
                txtHoTen.Text = row["HoTen"].ToString();
                txtDiaChi.Text = row["DiaChi"].ToString();
                txtSDT.Text = row["SDT"].ToString();
                txtEmail.Text = row["Email"].ToString();
            }
        }

        private void ThemKH_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maKH = txtMaKH.Text.Trim();
                string hoTen = txtHoTen.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();
                string sdt = txtSDT.Text.Trim();
                string email = txtEmail.Text.Trim();

                // Kiểm tra xem MaKH đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM KhachHang WHERE MaKH = @MaKH";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@MaKH", maKH);
                    conn.Open();
                    int existingCount = (int)checkCmd.ExecuteScalar();
                    conn.Close();

                    if (existingCount > 0)
                    {
                        MessageBox.Show($"Mã khách hàng '{maKH}' đã tồn tại trong cơ sở dữ liệu!");
                        return; // Dừng thực hiện tiếp để không thêm khách hàng đã tồn tại
                    }
                }

                // Nếu không tồn tại, tiến hành thêm mới khách hàng
                string insertQuery = "INSERT INTO KhachHang (MaKH, HoTen, DiaChi, SDT, Email) VALUES (@MaKH, @HoTen, @DiaChi, @SDT, @Email)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@MaKH", maKH);
                    insertCmd.Parameters.AddWithValue("@HoTen", hoTen);
                    insertCmd.Parameters.AddWithValue("@DiaChi", diaChi);
                    insertCmd.Parameters.AddWithValue("@SDT", sdt);
                    insertCmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    int rowsAffected = insertCmd.ExecuteNonQuery();
                    conn.Close();

                    LoadKH(); // Cập nhật lại danh sách khách hàng trên DataGrid
                    MessageBox.Show("Thêm khách hàng thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void XoaKH_Click(object sender, RoutedEventArgs e)
        {
            string maKH = txtMaKH.Text.Trim();

            if (string.IsNullOrEmpty(maKH))
            {
                MessageBox.Show("Vui lòng nhập mã khách hàng cần xoá!");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xoá khách hàng có mã " + maKH + "?", "Xác nhận xoá", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM KhachHang WHERE MaKH = @MaKH";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@MaKH", maKH);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xoá khách hàng thành công!");
                            LoadKH(); // Cập nhật lại danh sách khách hàng trên DataGrid
                                      // Xóa các trường thông tin sau khi xoá thành công
                            txtMaKH.Text = "";
                            txtHoTen.Text = "";
                            txtDiaChi.Text = "";
                            txtEmail.Text = "";
                            txtSDT.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy khách hàng có mã " + maKH + " để xoá!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SuaKH_Click(object sender, RoutedEventArgs e)
        {
            string maKH = txtMaKH.Text.Trim();

            if (string.IsNullOrEmpty(maKH))
            {
                MessageBox.Show("Vui lòng nhập mã khách hàng cần sửa!");
                return;
            }

            try
            {
                string query = @"UPDATE KhachHang 
                         SET HoTen = @HoTen, DiaChi = @DiaChi, Email = @Email, SDT = @SDT 
                         WHERE MaKH = @MaKH";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text);
                    cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sửa thông tin khách hàng thành công!");
                        LoadKH(); // Cập nhật lại danh sách khách hàng trên DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khách hàng có mã " + maKH + " để sửa!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TimKH_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maKH = txtMaKH.Text.Trim();
                string sdt = txtSDT.Text.Trim();

                // Kiểm tra xem người dùng đã nhập ít nhất một trong hai trường MaKH hoặc SDT chưa
                if (string.IsNullOrEmpty(maKH) && string.IsNullOrEmpty(sdt))
                {
                    MessageBox.Show("Vui lòng nhập Mã KH hoặc SĐT để tìm kiếm!");
                    return;
                }

                string query = "SELECT * FROM KhachHang WHERE MaKH = @MaKH OR SDT = @SDT";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    cmd.Parameters.AddWithValue("@SDT", sdt);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<KhachHang> listKhachHang = new List<KhachHang>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            KhachHang kh = new KhachHang();
                            kh.MaKH = reader["MaKH"].ToString();
                            kh.HoTen = reader["HoTen"].ToString();
                            kh.DiaChi = reader["DiaChi"].ToString();
                            kh.SDT = reader["SDT"].ToString();
                            kh.Email = reader["Email"].ToString();

                            listKhachHang.Add(kh);
                        }
                        MessageBox.Show("Đã tìm thấy khách hàng");
                        DataKH.ItemsSource = listKhachHang;

                        // Hiện thông tin khách hàng lên các TextBox nếu tìm thấy một khách hàng duy nhất
                        if (listKhachHang.Count == 1)
                        {
                            var kh = listKhachHang[0];
                            txtMaKH.Text = kh.MaKH;
                            txtHoTen.Text = kh.HoTen;
                            txtDiaChi.Text = kh.DiaChi;
                            txtEmail.Text = kh.Email;
                            txtSDT.Text = kh.SDT;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khách hàng có Mã KH = " + maKH + " hoặc SĐT = " + sdt);
                    }

                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HuyThaoTac_Click(object sender, RoutedEventArgs e)
        {
            // Xóa dữ liệu trong các TextBox
            txtMaKH.Text = "";
            txtHoTen.Text = "";
            txtDiaChi.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";

            // Xóa chọn dòng trong DataGrid (nếu có)
            DataKH.SelectedItem = null;
        }
        public class KhachHang
        {
            public string MaKH { get; set; }
            public string HoTen { get; set; }
            public string DiaChi { get; set; }
            public string SDT { get; set; }
            public string Email { get; set; }
        }
    }
}
