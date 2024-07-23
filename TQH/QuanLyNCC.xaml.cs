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
    /// Interaction logic for QuanLyNCC.xaml
    /// </summary>
    public partial class QuanLyNCC : UserControl
    {
        public QuanLyNCC()
        {
            InitializeComponent();
            LoadNCC();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;");
        public void LoadNCC()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select * from [NCC]", conn);
                DataTable dt = new DataTable();
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                conn.Close();
                DataGridNCC.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DataGridNCC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridNCC.SelectedItem != null && DataGridNCC.SelectedItem is DataRowView row)
            {
                txtMaNCC.Text = row["MaNCC"].ToString();
                txtTenNCC.Text = row["TenNCC"].ToString();
                txtDiaChiNCC.Text = row["DiaChi"].ToString();
                txtSDTNCC.Text = row["SDT"].ToString();
                txtEmailNCC.Text = row["Email"].ToString();
            }
        }

        private void ThemNCC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem MaNCC đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM NCC WHERE MaNCC = @MaNCC";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.CommandType = CommandType.Text;
                    checkCmd.Parameters.AddWithValue("@MaNCC", txtMaNCC.Text.Trim());

                    conn.Open();
                    int existingCount = (int)checkCmd.ExecuteScalar();
                    conn.Close();

                    // Nếu đã tồn tại MaNCC trong cơ sở dữ liệu
                    if (existingCount > 0)
                    {
                        MessageBox.Show("Mã nhà cung cấp đã tồn tại trong hệ thống. Vui lòng nhập mã khác!");
                        return;
                    }
                }

                // Thêm nhà cung cấp mới
                string insertQuery = "INSERT INTO NCC (MaNCC, TenNCC, DiaChi, SDT, Email) VALUES (@MaNCC, @TenNCC, @DiaChi, @SDT, @Email)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNCC", txtMaNCC.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenNCC", txtTenNCC.Text.Trim());
                    cmd.Parameters.AddWithValue("@DiaChi", txtDiaChiNCC.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", txtSDTNCC.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmailNCC.Text.Trim());

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    LoadNCC(); // Cập nhật lại danh sách nhà cung cấp trên DataGrid
                    MessageBox.Show("Thêm nhà cung cấp thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            }

        private void XoaNCC_Click(object sender, RoutedEventArgs e)
        {
            string maNCC = txtMaNCC.Text.Trim();

            if (string.IsNullOrEmpty(maNCC))
            {
                MessageBox.Show("Vui lòng nhập mã nhà cung cấp cần xoá!");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xoá nhà cung cấp có mã " + maNCC + "?", "Xác nhận xoá", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM NCC WHERE MaNCC = @MaNCC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@MaNCC", maNCC);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xoá nhà cung cấp thành công!");
                            LoadNCC(); // Cập nhật lại danh sách nhà cung cấp trên DataGrid
                                        // Xóa các trường thông tin sau khi xoá thành công
                            txtMaNCC.Text = "";
                            txtTenNCC.Text = "";
                            txtDiaChiNCC.Text = "";
                            txtEmailNCC.Text = "";
                            txtSDTNCC.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhà cung cấp có mã " + maNCC + " để xoá!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SuaNCC_Click(object sender, RoutedEventArgs e)
        {
            string maNCC = txtMaNCC.Text.Trim();

            if (string.IsNullOrEmpty(maNCC))
            {
                MessageBox.Show("Vui lòng nhập mã nhà cung cấp cần sửa!");
                return;
            }

            try
            {
                string query = @"UPDATE NCC 
                         SET TenNCC = @TenNCC, DiaChi = @DiaChi, SDT = @SDT, Email = @Email 
                         WHERE MaNCC = @MaNCC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNCC", maNCC);
                    cmd.Parameters.AddWithValue("@TenNCC", txtTenNCC.Text);
                    cmd.Parameters.AddWithValue("@DiaChi", txtDiaChiNCC.Text);
                    cmd.Parameters.AddWithValue("@SDT", txtSDTNCC.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmailNCC.Text);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sửa thông tin nhà cung cấp thành công!");
                        LoadNCC(); // Cập nhật lại danh sách nhà cung cấp trên DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy nhà cung cấp có mã " + maNCC + " để sửa!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TimNCC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maNCC = txtMaNCC.Text.Trim();
                string sdt = txtSDTNCC.Text.Trim();

                // Kiểm tra xem người dùng đã nhập ít nhất một trong hai trường MaNCC hoặc SDT chưa
                if (string.IsNullOrEmpty(maNCC) && string.IsNullOrEmpty(sdt))
                {
                    MessageBox.Show("Vui lòng nhập Mã NCC hoặc SĐT để tìm kiếm!");
                    return;
                }

                string query = "SELECT * FROM NCC WHERE MaNCC = @MaNCC OR SDT = @SDT";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNCC", maNCC);
                    cmd.Parameters.AddWithValue("@SDT", sdt);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<NCC> listNCC = new List<NCC>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            NCC ncc = new NCC();
                            ncc.MaNCC = reader["MaNCC"].ToString();
                            ncc.TenNCC = reader["TenNCC"].ToString();
                            ncc.DiaChi = reader["DiaChi"].ToString();
                            ncc.Email = reader["Email"].ToString();
                            ncc.SDT = reader["SDT"].ToString();

                            listNCC.Add(ncc);
                        }
                        MessageBox.Show("Đã tìm thấy nhà cung cấp");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy nhà cung cấp có Mã NCC = " + maNCC + " hoặc SĐT = " + sdt);
                    }

                    reader.Close();
                    conn.Close();

                    // Gán danh sách nhà cung cấp vào DataGrid
                    DataGridNCC.ItemsSource = listNCC;

                    // Hiện thông tin nhà cung cấp lên các TextBox nếu tìm thấy một nhà cung cấp duy nhất
                    if (listNCC.Count == 1)
                    {
                        var ncc = listNCC[0];
                        txtMaNCC.Text = ncc.MaNCC;
                        txtTenNCC.Text = ncc.TenNCC;
                        txtDiaChiNCC.Text = ncc.DiaChi;
                        txtEmailNCC.Text = ncc.Email;
                        txtSDTNCC.Text = ncc.SDT;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HuyThaoTacNCC_Click(object sender, RoutedEventArgs e)
        {
            txtMaNCC.Text = "";
            txtTenNCC.Text = "";
            txtDiaChiNCC.Text = "";
            txtSDTNCC.Text = "";
            txtEmailNCC.Text = "";
        }
        public class NCC
        {
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public string DiaChi { get; set; }
            public string Email { get; set; }
            public string SDT { get; set; }
        }
    }
}
