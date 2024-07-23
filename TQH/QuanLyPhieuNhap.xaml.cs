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
    /// Interaction logic for QuanLyPhieuNhap.xaml
    /// </summary>
    public partial class QuanLyPhieuNhap : UserControl
    {
        public QuanLyPhieuNhap()
        {
            InitializeComponent();
            LoadMaHangHoa();
            LoadPN();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;");
        public void LoadPN()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select * from [PhieuNhap]", conn);
                DataTable dt = new DataTable();
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                conn.Close();
                DataPhieuNhap.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadMaHangHoa()
        {
            // Load MaHangHoa into ComboBox cbMaHangHoa
            string query = "SELECT MaHangHoa FROM HangHoa";
            List<string> maHangHoaList = new List<string>();

            using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;"))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string maHangHoa = reader["MaHangHoa"].ToString();
                        maHangHoaList.Add(maHangHoa);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            cbMaHangHoa.ItemsSource = maHangHoaList;
        }
        private void DataPhieuNhap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ThemPhieuNhap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maHangHoa = cbMaHangHoa.SelectedValue.ToString();
                int soLuongNhap = int.Parse(txtSoLuongNhap.Text.Trim());
                DateTime ngayNhap = dpNgayNhap.SelectedDate ?? DateTime.Now; // Lấy giá trị ngày nhập từ DatePicker, nếu không có thì lấy ngày hiện tại
                float giaNhap = string.IsNullOrEmpty(txtGiaNhap.Text.Trim()) ? 0 : float.Parse(txtGiaNhap.Text.Trim());

                // Kiểm tra xem MaHangHoa đã tồn tại trong bảng HangHoa chưa
                string checkQuery = "SELECT COUNT(*) FROM HangHoa WHERE MaHangHoa = @MaHangHoa";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);
                    conn.Open();
                    int existingCount = (int)checkCmd.ExecuteScalar();
                    conn.Close();

                    if (existingCount == 0)
                    {
                        MessageBox.Show($"Mã hàng hóa '{maHangHoa}' không tồn tại trong cơ sở dữ liệu!");
                        return; // Dừng thực hiện tiếp để không thêm phiếu nhập cho mã hàng hóa không tồn tại
                    }
                }

                // Nếu mã hàng hóa tồn tại, tiến hành thêm mới phiếu nhập
                string insertQuery = "INSERT INTO PhieuNhap (MaHangHoa, SoLuongNhap, NgayNhap, GiaNhap) VALUES (@MaHangHoa, @SoLuongNhap, @NgayNhap, @GiaNhap)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);
                    insertCmd.Parameters.AddWithValue("@SoLuongNhap", soLuongNhap);
                    insertCmd.Parameters.AddWithValue("@NgayNhap", ngayNhap);
                    insertCmd.Parameters.AddWithValue("@GiaNhap", giaNhap);

                    conn.Open();
                    int rowsAffected = insertCmd.ExecuteNonQuery();
                    conn.Close();

                    // Thông báo thành công và cập nhật lại danh sách phiếu nhập trên DataGrid
                    MessageBox.Show("Thêm phiếu nhập thành công!");
                    LoadPN();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SuaPhieuNhap_Click(object sender, RoutedEventArgs e)
        {
            if (DataPhieuNhap.SelectedItem != null && DataPhieuNhap.SelectedItem is DataRowView row)
            {
                try
                {
                    string maHangHoa = cbMaHangHoa.SelectedValue.ToString(); // Lấy giá trị MaHangHoa từ ComboBox
                    int soLuongNhap = int.Parse(txtSoLuongNhap.Text.Trim());
                    DateTime ngayNhap = dpNgayNhap.SelectedDate ?? DateTime.Now; // Lấy giá trị NgayNhap từ DatePicker, nếu không có thì lấy ngày hiện tại
                    float giaNhap = string.IsNullOrEmpty(txtGiaNhap.Text.Trim()) ? 0 : float.Parse(txtGiaNhap.Text.Trim());

                    // Thực hiện câu lệnh SQL UPDATE để cập nhật phiếu nhập
                    string updateQuery = @"UPDATE PhieuNhap 
                                   SET SoLuongNhap = @SoLuongNhap, NgayNhap = @NgayNhap, GiaNhap = @GiaNhap 
                                   WHERE MaHangHoa = @MaHangHoa";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);
                        updateCmd.Parameters.AddWithValue("@SoLuongNhap", soLuongNhap);
                        updateCmd.Parameters.AddWithValue("@NgayNhap", ngayNhap);
                        updateCmd.Parameters.AddWithValue("@GiaNhap", giaNhap);

                        conn.Open();
                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật phiếu nhập thành công!");
                            LoadPN(); // Cập nhật lại danh sách phiếu nhập trên DataGrid
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy phiếu nhập có mã hàng hóa " + maHangHoa + " để sửa!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập cần sửa!");
            }
        }

        private void XoaPhieuNhap_Click(object sender, RoutedEventArgs e)
        {
            if (DataPhieuNhap.SelectedItem != null && DataPhieuNhap.SelectedItem is DataRowView row)
            {
                string maHangHoa = row["MaHangHoa"].ToString();

                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xoá phiếu nhập có mã hàng hóa " + maHangHoa + "?", "Xác nhận xoá", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = "DELETE FROM PhieuNhap WHERE MaHangHoa = @MaHangHoa";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);

                            conn.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();
                            conn.Close();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Xoá phiếu nhập thành công!");
                                LoadPN(); // Cập nhật lại danh sách phiếu nhập trên DataGrid

                                // Xóa các trường thông tin sau khi xoá thành công
                                cbMaHangHoa.SelectedIndex = -1;
                                txtSoLuongNhap.Text = "";
                                dpNgayNhap.SelectedDate = null;
                                txtGiaNhap.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy phiếu nhập có mã hàng hóa " + maHangHoa + " để xoá!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập cần xoá!");
            }
        }

        private void TimKiem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maHangHoa = cbMaHangHoa.SelectedValue?.ToString();

                // Kiểm tra xem người dùng đã chọn mã hàng hóa để tìm kiếm chưa
                if (string.IsNullOrEmpty(maHangHoa))
                {
                    MessageBox.Show("Vui lòng chọn Mã hàng hóa từ danh sách để tìm kiếm!");
                    return;
                }

                string query = "SELECT * FROM PhieuNhap WHERE MaHangHoa = @MaHangHoa";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<PhieuNhap> listPhieuNhap = new List<PhieuNhap>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            PhieuNhap pn = new PhieuNhap();
                            pn.MaHangHoa = reader["MaHangHoa"].ToString();
                            pn.SoLuongNhap = int.Parse(reader["SoLuongNhap"].ToString());
                            pn.NgayNhap = DateTime.Parse(reader["NgayNhap"].ToString());

                            if (reader["GiaNhap"] != DBNull.Value)
                            {
                                pn.GiaNhap = float.Parse(reader["GiaNhap"].ToString());
                            }
                            else
                            {
                                pn.GiaNhap = null; // Gán null cho GiaNhap nếu giá trị trong database là NULL
                            }

                            listPhieuNhap.Add(pn);
                        }

                        MessageBox.Show($"Đã tìm thấy {listPhieuNhap.Count} phiếu nhập cho Mã hàng hóa = {maHangHoa}");
                        DataPhieuNhap.ItemsSource = listPhieuNhap;

                        // Hiển thị thông tin phiếu nhập lên các TextBox nếu tìm thấy một phiếu nhập duy nhất
                        if (listPhieuNhap.Count == 1)
                        {
                            var pn = listPhieuNhap[0];
                            cbMaHangHoa.SelectedValue = pn.MaHangHoa;
                            txtSoLuongNhap.Text = pn.SoLuongNhap.ToString();
                            dpNgayNhap.SelectedDate = pn.NgayNhap;

                            if (pn.GiaNhap != null)
                            {
                                txtGiaNhap.Text = pn.GiaNhap.ToString(); // Chuyển đổi sang chuỗi để hiển thị
                            }
                            else
                            {
                                txtGiaNhap.Text = ""; // Đặt giá trị rỗng nếu GiaNhap là null
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Không tìm thấy phiếu nhập có Mã hàng hóa = {maHangHoa}");
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
        public class PhieuNhap
        {
            public string MaHangHoa { get; set; }
            public int SoLuongNhap { get; set; }
            public DateTime NgayNhap { get; set; }
            public float? GiaNhap { get; set; } // Sử dụng nullable float để xử lý trường hợp giá nhập có thể null
        }
        private void HuyThaoTac_Click(object sender, RoutedEventArgs e)
        {
            // Đặt lại giá trị của ComboBox cbMaHangHoa
            cbMaHangHoa.SelectedIndex = -1; // Chọn lại giá trị mặc định (không chọn)

            // Xóa dữ liệu trong các TextBox và DatePicker
            txtSoLuongNhap.Text = "";
            dpNgayNhap.SelectedDate = null;
            txtGiaNhap.Text = "";

            // Xóa chọn dòng trong DataPhieuNhap (nếu có)
            DataPhieuNhap.SelectedItem = null;
        }

        private void CbMaHangHoa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
