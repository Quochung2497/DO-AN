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
using static TQH.QuanLyNCC;

namespace TQH
{
    /// <summary>
    /// Interaction logic for QuanLyHangHoa.xaml
    /// </summary>
    public partial class QuanLyHangHoa : UserControl
    {
        public QuanLyHangHoa()
        {
            InitializeComponent();
            LoadMaNCC();
            LoadHH();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;");
        private void LoadMaNCC()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;"))
                {
                    string query = "SELECT MaNCC FROM NCC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<string> maNCCList = new List<string>();
                    while (reader.Read())
                    {
                        string maNCC = reader["MaNCC"].ToString();
                        maNCCList.Add(maNCC);
                    }

                    reader.Close();
                    conn.Close();

                    cbNCC.ItemsSource = maNCCList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading MaNCC: " + ex.Message);
            }
        }
        private void LoadHH()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select * from [HangHoa]", conn);
                DataTable dt = new DataTable();
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                conn.Close();
                DataHangHoa.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DataHangHoa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataHangHoa.SelectedItem != null && DataHangHoa.SelectedItem is DataRowView row)
            {
                txtMaHangHoa.Text = row["MaHangHoa"].ToString();
                txtTenHangHoa.Text = row["TenHangHoa"].ToString();
                txtGia.Text = row["Gia"].ToString();
                txtSoLuong.Text = row["SoLuong"].ToString();
                cbNCC.SelectedItem = row["NCC"].ToString(); // Assuming NCC is the column name for Nhà Cung Cấp
            }
        }

        private void ThemHangHoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maHangHoa = txtMaHangHoa.Text.Trim();
                string tenHangHoa = txtTenHangHoa.Text.Trim();
                float gia = float.Parse(txtGia.Text.Trim());
                int soLuong = int.Parse(txtSoLuong.Text.Trim());
                string maNCC = cbNCC.SelectedValue as string;

                // Kiểm tra các thông tin bắt buộc
                if (string.IsNullOrEmpty(maHangHoa) || string.IsNullOrEmpty(tenHangHoa) || string.IsNullOrEmpty(maNCC))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                    return;
                }

                // Thực hiện thêm hàng hóa vào CSDL
                string insertQuery = "INSERT INTO HangHoa (MaHangHoa, TenHangHoa, Gia, SoLuong, NCC) " +
                                     "VALUES (@MaHangHoa, @TenHangHoa, @Gia, @SoLuong, @NCC)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);
                    insertCmd.Parameters.AddWithValue("@TenHangHoa", tenHangHoa);
                    insertCmd.Parameters.AddWithValue("@Gia", gia);
                    insertCmd.Parameters.AddWithValue("@SoLuong", soLuong);
                    insertCmd.Parameters.AddWithValue("@NCC", maNCC);

                    conn.Open();
                    int rowsAffected = insertCmd.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thêm hàng hóa thành công!");
                        LoadHH(); // Cập nhật lại danh sách hàng hóa trên DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Thêm hàng hóa thất bại!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void XoaHangHoa_Click(object sender, RoutedEventArgs e)
        {
            string maHangHoa = txtMaHangHoa.Text.Trim();

            if (string.IsNullOrEmpty(maHangHoa))
            {
                MessageBox.Show("Vui lòng nhập mã hàng hóa cần xoá!");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xoá hàng hóa có mã " + maHangHoa + "?", "Xác nhận xoá", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM HangHoa WHERE MaHangHoa = @MaHangHoa";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xoá hàng hóa thành công!");
                            LoadHH(); // Cập nhật lại danh sách hàng hóa trên DataGrid
                                      // Xóa các trường thông tin sau khi xoá thành công
                            txtMaHangHoa.Text = "";
                            txtTenHangHoa.Text = "";
                            txtGia.Text = "";
                            txtSoLuong.Text = "";
                            cbNCC.SelectedIndex = -1; // Chọn lại giá trị mặc định cho ComboBox
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy hàng hóa có mã " + maHangHoa + " để xoá!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void SuaHangHoa_Click(object sender, RoutedEventArgs e)
        {
            string maHangHoa = txtMaHangHoa.Text.Trim();
            string maNCC = cbNCC.SelectedValue as string;
            if (string.IsNullOrEmpty(maHangHoa))
            {
                MessageBox.Show("Vui lòng nhập mã hàng hóa cần sửa!");
                return;
            }

            try
            {
                string query = @"UPDATE HangHoa 
                         SET TenHangHoa = @TenHangHoa, Gia = @Gia, SoLuong = @SoLuong, NCC = @NCC
                         WHERE MaHangHoa = @MaHangHoa";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);
                    cmd.Parameters.AddWithValue("@TenHangHoa", txtTenHangHoa.Text);
                    cmd.Parameters.AddWithValue("@Gia", float.Parse(txtGia.Text)); // Chuyển đổi sang kiểu dữ liệu phù hợp (float, double, decimal)
                    cmd.Parameters.AddWithValue("@SoLuong", int.Parse(txtSoLuong.Text)); // Chuyển đổi sang kiểu dữ liệu phù hợp (int)

                    // Xử lý lựa chọn ComboBox cbNCC
                    if (cbNCC.SelectedItem != null)
                    {
                        cmd.Parameters.AddWithValue("@NCC", maNCC); // Lấy MaNCC từ ComboBox
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NCC", DBNull.Value); // Hoặc xử lý giá trị mặc định
                    }

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sửa thông tin hàng hóa thành công!");
                        LoadHH(); // Cập nhật lại danh sách hàng hóa trên DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy hàng hóa có mã " + maHangHoa + " để sửa!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TimHangHoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maHangHoa = txtMaHangHoa.Text.Trim();

                // Kiểm tra xem người dùng đã nhập mã hàng hóa để tìm kiếm chưa
                if (string.IsNullOrEmpty(maHangHoa))
                {
                    MessageBox.Show("Vui lòng nhập Mã hàng hóa để tìm kiếm!");
                    return;
                }

                string query = "SELECT * FROM HangHoa WHERE MaHangHoa = @MaHangHoa";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<HangHoa> listHangHoa = new List<HangHoa>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            HangHoa hh = new HangHoa();
                            hh.MaHangHoa = reader["MaHangHoa"].ToString();
                            hh.TenHangHoa = reader["TenHangHoa"].ToString();
                            hh.Gia = float.Parse(reader["Gia"].ToString()); // Chuyển đổi sang kiểu dữ liệu phù hợp
                            hh.SoLuong = int.Parse(reader["SoLuong"].ToString()); // Chuyển đổi sang kiểu dữ liệu phù hợp
                            hh.NCC = reader["NCC"].ToString();

                            listHangHoa.Add(hh);
                        }
                        MessageBox.Show("Đã tìm thấy hàng hóa");
                        DataHangHoa.ItemsSource = listHangHoa;

                        // Hiện thông tin hàng hóa lên các TextBox nếu tìm thấy một hàng hóa duy nhất
                        if (listHangHoa.Count == 1)
                        {
                            var hh = listHangHoa[0];
                            txtMaHangHoa.Text = hh.MaHangHoa;
                            txtTenHangHoa.Text = hh.TenHangHoa;
                            txtGia.Text = hh.Gia.ToString(); // Chuyển đổi sang chuỗi để hiển thị
                            txtSoLuong.Text = hh.SoLuong.ToString(); // Chuyển đổi sang chuỗi để hiển thị

                            // Đặt lại giá trị của ComboBox cbNCC nếu NCC có trong danh sách NCC
                            foreach (object item in cbNCC.Items)
                            {
                                if (item is NCC ncc && ncc.MaNCC == hh.NCC)
                                {
                                    cbNCC.SelectedItem = item;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy hàng hóa có Mã hàng hóa = " + maHangHoa);
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
        public class HangHoa
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public float Gia { get; set; }
            public int SoLuong { get; set; }
            public string NCC { get; set; }
        }
        private void HuyThaoTac_Click(object sender, RoutedEventArgs e)
        {
            // Xóa dữ liệu trong các TextBox
            txtMaHangHoa.Text = "";
            txtTenHangHoa.Text = "";
            txtGia.Text = "";
            txtSoLuong.Text = "";
            cbNCC.SelectedItem = -1;

            // Xóa chọn dòng trong DataNV (nếu có)
            DataHangHoa.SelectedItem = null;
        }
    }
}
