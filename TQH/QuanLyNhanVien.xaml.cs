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
    /// Interaction logic for QuanLyNhanVien.xaml
    /// </summary>
    public partial class QuanLyNhanVien : UserControl
    {
        public QuanLyNhanVien()
        {
            InitializeComponent();
            LoadNV();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;");
        public void LoadNV()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select * from [NhanVien]", conn);
                DataTable dt = new DataTable();
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                conn.Close();
                DataNV.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ThemNV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem MaNV đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM Nhanvien WHERE MaNV = @MaNV";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.CommandType = CommandType.Text;
                    checkCmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text.Trim());

                    conn.Open();
                    int existingCount = (int)checkCmd.ExecuteScalar();
                    conn.Close();

                    // Nếu đã tồn tại MaNV trong cơ sở dữ liệu
                    if (existingCount > 0)
                    {
                        MessageBox.Show("Mã nhân viên đã tồn tại trong hệ thống. Vui lòng nhập mã nhân viên khác!");
                        return;
                    }
                }

                // Thêm nhân viên mới
                string insertQuery = "INSERT INTO Nhanvien (MaNV, HoTen, GioiTinh, DiaChi, Email, SDT) VALUES (@MaNV, @HoTen, @GioiTinh, @DiaChi, @Email, @SDT)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text.Trim());
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());

                    // Kiểm tra và thêm giới tính
                    if (cbGioiTinh.SelectedIndex == 0)
                        cmd.Parameters.AddWithValue("@GioiTinh", "Nam");
                    else if (cbGioiTinh.SelectedIndex == 1)
                        cmd.Parameters.AddWithValue("@GioiTinh", "Nữ");
                    else
                    {
                        MessageBox.Show("Vui lòng chọn giới tính!");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", Convert.ToInt32(txtSDT.Text.Trim()));

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    LoadNV(); // Cập nhật lại danh sách nhân viên trên DataGrid
                    MessageBox.Show("Thêm nhân viên thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void XoaNV_Click(object sender, RoutedEventArgs e)
        {
            string maNV = txtMaNV.Text.Trim();

            if (string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên cần xoá!");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xoá nhân viên có mã " + maNV + "?", "Xác nhận xoá", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Nhanvien WHERE MaNV = @MaNV";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@MaNV", maNV);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xoá nhân viên thành công!");
                            LoadNV(); // Cập nhật lại danh sách nhân viên trên DataGrid
                                      // Xóa các trường thông tin sau khi xoá thành công
                            txtMaNV.Text = "";
                            txtHoTen.Text = "";
                            cbGioiTinh.SelectedIndex = -1;
                            txtDiaChi.Text = "";
                            txtEmail.Text = "";
                            txtSDT.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhân viên có mã " + maNV + " để xoá!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SuaNV_Click(object sender, RoutedEventArgs e)
        {
            string maNV = txtMaNV.Text.Trim();

            if (string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên cần sửa!");
                return;
            }

            try
            {
                string query = @"UPDATE Nhanvien 
                 SET HoTen = @HoTen, GioiTinh = @GioiTinh, DiaChi = @DiaChi, Email = @Email, SDT = @SDT 
                 WHERE MaNV = @MaNV";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text);

                    // Kiểm tra và thêm giới tính
                    if (cbGioiTinh.SelectedIndex == 0)
                        cmd.Parameters.AddWithValue("@GioiTinh", "Nam");
                    else if (cbGioiTinh.SelectedIndex == 1)
                        cmd.Parameters.AddWithValue("@GioiTinh", "Nữ");
                    else
                    {
                        MessageBox.Show("Vui lòng chọn giới tính!");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sửa thông tin nhân viên thành công!");
                        LoadNV(); // Cập nhật lại danh sách nhân viên trên DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy nhân viên có mã " + maNV + " để sửa!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TimNV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maNV = txtMaNV.Text.Trim();
                string sdt = txtSDT.Text.Trim();

                // Kiểm tra xem người dùng đã nhập ít nhất một trong hai trường MaNV hoặc SDT chưa
                if (string.IsNullOrEmpty(maNV) && string.IsNullOrEmpty(sdt))
                {
                    MessageBox.Show("Vui lòng nhập Mã NV hoặc SĐT để tìm kiếm!");
                    return;
                }

                string query = "SELECT * FROM Nhanvien WHERE MaNV = @MaNV OR SDT = @SDT";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@SDT", sdt);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Nhanvien> listNhanvien = new List<Nhanvien>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Nhanvien nv = new Nhanvien();
                            nv.MaNV = reader["MaNV"].ToString();
                            nv.HoTen = reader["HoTen"].ToString();
                            // Đọc giới tính là chuỗi và kiểm tra giá trị để gán lại cho GioiTinh
                            if (reader["GioiTinh"] != DBNull.Value)
                            {
                                string gioiTinhValue = reader["GioiTinh"].ToString();
                                if (gioiTinhValue == "Nam")
                                    nv.GioiTinh = "Nam";
                                else if (gioiTinhValue == "Nữ")
                                    nv.GioiTinh = "Nữ";
                            }
                            nv.DiaChi = reader["DiaChi"].ToString();
                            nv.Email = reader["Email"].ToString();
                            nv.SDT = reader["SDT"].ToString();

                            listNhanvien.Add(nv);
                        }
                        MessageBox.Show("Đã tìm thấy nhân viên");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy nhân viên có Mã NV = " + maNV + " hoặc SĐT = " + sdt);
                    }

                    reader.Close();
                    conn.Close();

                    // Gán danh sách nhân viên vào DataGrid
                    DataNV.ItemsSource = listNhanvien;

                    // Hiện thông tin nhân viên lên các TextBox nếu tìm thấy một nhân viên duy nhất
                    if (listNhanvien.Count == 1)
                    {
                        var nv = listNhanvien[0];
                        txtMaNV.Text = nv.MaNV;
                        txtHoTen.Text = nv.HoTen;
                        if (nv.GioiTinh == "Nam")
                            cbGioiTinh.SelectedIndex = 0;
                        else if (nv.GioiTinh == "Nữ")
                            cbGioiTinh.SelectedIndex = 1;
                        else
                            cbGioiTinh.SelectedIndex = -1;
                        txtDiaChi.Text = nv.DiaChi;
                        txtEmail.Text = nv.Email;
                        txtSDT.Text = nv.SDT;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HuyThaoTac_Click(object sender, RoutedEventArgs e)
        {
            txtMaNV.Text = "";
            txtHoTen.Text = "";
            cbGioiTinh.SelectedIndex = -1;
            txtDiaChi.Text = "";
            txtEmail.Text = "";
            txtSDT.Text = "";
        }

        private void DataUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataNV.SelectedItem != null && DataNV.SelectedItem is DataRowView row)
            {
                txtMaNV.Text = row["MaNV"].ToString();
                txtHoTen.Text = row["HoTen"].ToString();

                // Giới tính
                if (row["GioiTinh"] != DBNull.Value)
                {
                    string gioiTinh = row["GioiTinh"].ToString();
                    if (gioiTinh == "Nam")
                        cbGioiTinh.SelectedIndex = 0; // Nam
                    else if (gioiTinh == "Nữ")
                        cbGioiTinh.SelectedIndex = 1; // Nữ
                    else
                        cbGioiTinh.SelectedIndex = -1; // Chưa chọn
                }
                else
                {
                    cbGioiTinh.SelectedIndex = -1; // Chưa chọn
                }

                txtDiaChi.Text = row["DiaChi"].ToString();
                txtEmail.Text = row["Email"].ToString();
                txtSDT.Text = row["SDT"].ToString();
            }
        }
        public class Nhanvien
        {
            public string MaNV { get; set; }
            public string HoTen { get; set; }
            public string GioiTinh { get; set; }
            public string DiaChi { get; set; }
            public string Email { get; set; }
            public string SDT { get; set; }
        }

    }

}
