using Microsoft.VisualBasic;
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
    /// Interaction logic for QuanLyHoaDon.xaml
    /// </summary>
    public partial class QuanLyHoaDon : UserControl
    {
        public QuanLyHoaDon()
        {
            InitializeComponent();
            LoadHD();
            LoadComboBoxData();
            txtMaHD.TextChanged += txtMaHD_TextChanged;
        }
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;");
        public void LoadHD()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select * from [HoaDon]", conn);
                DataTable dt = new DataTable();
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                conn.Close();
                DataGridHD.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadChiTietHoaDon(string maHD)
        {
            string query = "SELECT MaHD, MaHangHoa, SL, DonGia, ThanhTien FROM ChiTietHoaDon WHERE MaHD = @MaHD";
            DataTable chiTietHoaDonTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;")) // Sử dụng connection string của bạn
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(chiTietHoaDonTable);
                }
            }

            DataGridCTHD.ItemsSource = chiTietHoaDonTable.DefaultView;
        }
        private void LoadComboBoxData()
        {
            // Load data for cbMaKH (KhachHang)
            string queryKH = "SELECT MaKH, HoTen FROM KhachHang";
            DataTable khachHangTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;")) // Thay thế connectionString với chuỗi kết nối của bạn
            {
                using (SqlCommand cmd = new SqlCommand(queryKH, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(khachHangTable);
                }
            }

            cbMaKH.ItemsSource = khachHangTable.DefaultView;
            cbMaKH.DisplayMemberPath = "HoTen";
            cbMaKH.SelectedValuePath = "MaKH";

            // Load data for cbMaNV (NhanVien)
            string queryNV = "SELECT MaNV, HoTen FROM NhanVien";
            DataTable nhanVienTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;"))
            {
                using (SqlCommand cmd = new SqlCommand(queryNV, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(nhanVienTable);
                }
            }

            cbMaNV.ItemsSource = nhanVienTable.DefaultView;
            cbMaNV.DisplayMemberPath = "HoTen";
            cbMaNV.SelectedValuePath = "MaNV";

            // Load data for cbMaCTHH (HangHoa)
            string queryHH = "SELECT MaHangHoa, TenHangHoa FROM HangHoa";
            DataTable hangHoaTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;"))
            {
                using (SqlCommand cmd = new SqlCommand(queryHH, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(hangHoaTable);
                }
            }

            cbCTMaHH.ItemsSource = hangHoaTable.DefaultView;
            cbCTMaHH.DisplayMemberPath = "TenHangHoa";
            cbCTMaHH.SelectedValuePath = "MaHangHoa";
        }
        private void ThemHD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra và lấy thông tin từ các điều khiển nhập liệu trên giao diện
                string maHD = txtMaHD.Text.Trim();
                string maKH = cbMaKH.SelectedValue as string;
                string maNV = cbMaNV.SelectedValue as string;
                DateTime ngayLap = datePickerNgayLap.SelectedDate ?? DateTime.Now; // Lấy ngày lập, nếu không có thì lấy ngày hiện tại

                // Kiểm tra các thông tin bắt buộc
                if (string.IsNullOrEmpty(maHD) || string.IsNullOrEmpty(maKH) || string.IsNullOrEmpty(maNV))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                    return;
                }

                // Kiểm tra xem Mã hóa đơn có bị trùng không
                if (IsMaHDExists(maHD))
                {
                    MessageBox.Show("Mã hóa đơn đã tồn tại. Vui lòng chọn Mã hóa đơn khác!");
                    return;
                }

                // Thực hiện tính tổng tiền ban đầu (ở đây có thể là 0 đối với hóa đơn mới)
                float tongTien = 0; // Giá trị ban đầu của tổng tiền

                // Thực hiện thêm hóa đơn vào CSDL
                string insertQuery = "INSERT INTO HoaDon (MaHD, MaKH, MaNV, NgayLap, TongTien) VALUES (@MaHD, @MaKH, @MaNV, @NgayLap, @TongTien)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@MaHD", maHD);
                    insertCmd.Parameters.AddWithValue("@MaKH", maKH);
                    insertCmd.Parameters.AddWithValue("@MaNV", maNV);
                    insertCmd.Parameters.AddWithValue("@NgayLap", ngayLap);
                    insertCmd.Parameters.AddWithValue("@TongTien", tongTien); // Thêm tham số tổng tiền vào câu lệnh SQL

                    // Mở kết nối và thực thi câu lệnh SQL
                    conn.Open();
                    int rowsAffected = insertCmd.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thêm hóa đơn thành công!");
                        LoadHD(); // Cập nhật lại danh sách hóa đơn trên DataGridHD
                    }
                    else
                    {
                        MessageBox.Show("Thêm hóa đơn thất bại!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsMaHDExists(string maHD)
        {
            string query = "SELECT COUNT(*) FROM HoaDon WHERE MaHD = @MaHD";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();
                return count > 0;
            }
        }

        private void XoaHD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem người dùng đã chọn hóa đơn để xóa chưa
                if (DataGridHD.SelectedItem != null && DataGridHD.SelectedItem is DataRowView row)
                {
                    string maHD = row["MaHD"].ToString();

                    // Xác nhận với người dùng trước khi xóa
                    MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa hóa đơn này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Thực hiện xóa hóa đơn khỏi CSDL
                        string deleteQuery = "DELETE FROM HoaDon WHERE MaHD = @MaHD";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@MaHD", maHD);

                            conn.Open();
                            int rowsAffected = deleteCmd.ExecuteNonQuery();
                            conn.Close();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Xóa hóa đơn thành công!");
                                LoadHD(); // Cập nhật lại danh sách hóa đơn trên DataGridHD
                            }
                            else
                            {
                                MessageBox.Show("Xóa hóa đơn thất bại!");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn cần xóa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SuaHD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem người dùng đã chọn hóa đơn để sửa chưa
                if (DataGridHD.SelectedItem != null && DataGridHD.SelectedItem is DataRowView row)
                {
                    string maHD = txtMaHD.Text.Trim(); // Lấy Mã hóa đơn từ TextBox (nếu cần)
                    string maKH = cbMaKH.SelectedValue as string;
                    string maNV = cbMaNV.SelectedValue as string;
                    DateTime ngayLap = datePickerNgayLap.SelectedDate ?? DateTime.Now; // Lấy ngày lập, nếu không có thì lấy ngày hiện tại

                    // Kiểm tra các thông tin bắt buộc
                    if (string.IsNullOrEmpty(maHD) || string.IsNullOrEmpty(maKH) || string.IsNullOrEmpty(maNV))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                        return;
                    }

                    // Thực hiện câu lệnh UPDATE để sửa hóa đơn trong CSDL
                    string updateQuery = "UPDATE HoaDon SET MaKH = @MaKH, MaNV = @MaNV, NgayLap = @NgayLap WHERE MaHD = @MaHD";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@MaHD", maHD);
                        updateCmd.Parameters.AddWithValue("@MaKH", maKH);
                        updateCmd.Parameters.AddWithValue("@MaNV", maNV);
                        updateCmd.Parameters.AddWithValue("@NgayLap", ngayLap);

                        conn.Open();
                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sửa thông tin hóa đơn thành công!");
                            LoadHD(); // Cập nhật lại danh sách hóa đơn trên DataGridHD
                        }
                        else
                        {
                            MessageBox.Show("Sửa thông tin hóa đơn thất bại!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn cần sửa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TimHD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maHD = txtMaHD.Text.Trim();

                // Kiểm tra xem người dùng đã nhập mã hóa đơn để tìm kiếm chưa
                if (string.IsNullOrEmpty(maHD))
                {
                    MessageBox.Show("Vui lòng nhập Mã hóa đơn để tìm kiếm!");
                    return;
                }

                string query = "SELECT * FROM HoaDon WHERE MaHD = @MaHD";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaHD", maHD);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<HoaDon> listHoaDon = new List<HoaDon>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            HoaDon hd = new HoaDon();
                            hd.MaHD = reader["MaHD"].ToString();
                            hd.MaKH = reader["MaKH"].ToString();
                            hd.MaNV = reader["MaNV"].ToString();
                            hd.NgayLap = Convert.ToDateTime(reader["NgayLap"]);
                            hd.TongTien = Convert.ToSingle(reader["TongTien"]); // Chuyển đổi sang kiểu dữ liệu phù hợp

                            listHoaDon.Add(hd);
                        }
                        MessageBox.Show("Đã tìm thấy hóa đơn");
                        DataGridHD.ItemsSource = listHoaDon;

                        // Hiện thông tin hóa đơn lên các TextBox nếu tìm thấy một hóa đơn duy nhất
                        if (listHoaDon.Count == 1)
                        {
                            var hd = listHoaDon[0];
                            txtMaHD.Text = hd.MaHD;
                            cbMaKH.SelectedValue = hd.MaKH;
                            cbMaNV.SelectedValue = hd.MaNV;
                            datePickerNgayLap.SelectedDate = hd.NgayLap;
                            txtTongTien.Text = hd.TongTien.ToString(); // Chuyển đổi sang chuỗi để hiển thị
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy hóa đơn có Mã hóa đơn = " + maHD);
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
        public class HoaDon
        {
            public string MaHD { get; set; }
            public string MaKH { get; set; }
            public string MaNV { get; set; }
            public DateTime NgayLap { get; set; }
            public float TongTien { get; set; }
        }
        private void HuyThaoTacHD_Click(object sender, RoutedEventArgs e)
        {
            // Xóa các thông tin đang hiển thị trên giao diện
            txtMaHD.Text = "";
            cbMaKH.SelectedItem = null; // Đặt lại ComboBox MaKH
            cbMaNV.SelectedItem = null; // Đặt lại ComboBox MaNV
            datePickerNgayLap.SelectedDate = DateTime.Now; // Đặt lại DatePicker NgayLap
            txtTongTien.Text = ""; // Xóa tổng tiền

            // Đặt lại DataGridHD nếu cần thiết (nếu bạn muốn cập nhật lại danh sách hóa đơn)
            LoadHD(); // Gọi lại phương thức LoadHD() để cập nhật danh sách hóa đơn trên DataGridHD
        }

        private void ThemCTHD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra và lấy thông tin từ các điều khiển nhập liệu trên giao diện
                string maHD = txtMaHD.Text.Trim(); // Lấy mã hóa đơn từ ComboBox cbCTMaHD
                string maHangHoa = cbCTMaHH.SelectedValue as string;
                int soLuong = int.Parse(txtCTSL.Text.Trim());
                float donGia = float.Parse(txtCTDG.Text.Trim());

                // Kiểm tra các thông tin bắt buộc
                if (string.IsNullOrEmpty(maHD) || string.IsNullOrEmpty(maHangHoa) || soLuong <= 0 || donGia <= 0)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin và kiểm tra định dạng số lượng và đơn giá!");
                    return;
                }

                // Kiểm tra xem chi tiết hóa đơn đã tồn tại chưa
                string checkExistQuery = "SELECT COUNT(*) FROM ChiTietHoaDon WHERE MaHD = @MaHD AND MaHangHoa = @MaHangHoa";
                using (SqlCommand checkExistCmd = new SqlCommand(checkExistQuery, conn))
                {
                    checkExistCmd.Parameters.AddWithValue("@MaHD", maHD);
                    checkExistCmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);

                    conn.Open();
                    int count = Convert.ToInt32(checkExistCmd.ExecuteScalar());
                    conn.Close();

                    if (count > 0)
                    {
                        // Nếu chi tiết hóa đơn đã tồn tại, thực hiện UPDATE
                        string updateQuery = "UPDATE ChiTietHoaDon SET SL = SL + @SL, DonGia = @DonGia WHERE MaHD = @MaHD AND MaHangHoa = @MaHangHoa";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@MaHD", maHD);
                            updateCmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);
                            updateCmd.Parameters.AddWithValue("@SL", soLuong);
                            updateCmd.Parameters.AddWithValue("@DonGia", donGia);

                            conn.Open();
                            int rowsAffected = updateCmd.ExecuteNonQuery();
                            conn.Close();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Cập nhật chi tiết hóa đơn thành công!");
                                LoadChiTietHoaDon(maHD); // Cập nhật lại danh sách chi tiết hóa đơn
                            }
                            else
                            {
                                MessageBox.Show("Cập nhật chi tiết hóa đơn thất bại!");
                            }
                        }
                    }
                    else
                    {
                        // Nếu chi tiết hóa đơn chưa tồn tại, thực hiện INSERT
                        float thanhTien = soLuong * donGia;

                        // Hiển thị thành tiền lên TextBox
                        txtCTThanhTien.Text = thanhTien.ToString();

                        // Thêm chi tiết hóa đơn vào CSDL
                        string insertQuery = "INSERT INTO ChiTietHoaDon (MaHD, MaHangHoa, SL, DonGia) " +
                                             "VALUES (@MaHD, @MaHangHoa, @SL, @DonGia)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@MaHD", maHD);
                            insertCmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);
                            insertCmd.Parameters.AddWithValue("@SL", soLuong);
                            insertCmd.Parameters.AddWithValue("@DonGia", donGia);

                            conn.Open();
                            int rowsAffected = insertCmd.ExecuteNonQuery();
                            conn.Close();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Thêm chi tiết hóa đơn thành công!");
                                LoadChiTietHoaDon(maHD); // Cập nhật lại danh sách chi tiết hóa đơn
                            }
                            else
                            {
                                MessageBox.Show("Thêm chi tiết hóa đơn thất bại!");
                            }
                        }
                    }
                }

                // Cập nhật tổng tiền của hóa đơn
                UpdateTotalAmount(maHD);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void UpdateTotalAmount(string maHD)
        {
            try
            {
                // Truy vấn để tính tổng tiền từ chi tiết hóa đơn
                string query = "SELECT SUM(ThanhTien) AS TongTien FROM ChiTietHoaDon WHERE MaHD = @MaHD";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result != DBNull.Value)
                    {
                        float tongTien = Convert.ToSingle(result);

                        // Cập nhật tổng tiền vào bảng Hóa đơn
                        string updateQuery = "UPDATE HoaDon SET TongTien = @TongTien WHERE MaHD = @MaHD";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@TongTien", tongTien);
                            updateCmd.Parameters.AddWithValue("@MaHD", maHD);

                            conn.Open();
                            int rowsAffected = updateCmd.ExecuteNonQuery();
                            conn.Close();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Cập nhật tổng tiền thành công!");
                                LoadHD();
                            }
                            else
                            {
                                MessageBox.Show("Cập nhật tổng tiền thất bại!");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không có chi tiết hóa đơn nào để tính tổng tiền!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void XoaCTHD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra và lấy thông tin từ các điều khiển nhập liệu trên giao diện
                string maHD = txtMaHD.Text.Trim(); // Lấy mã hóa đơn từ TextBox txtMaHD
                string maHangHoa = cbCTMaHH.SelectedValue as string;

                // Kiểm tra các thông tin bắt buộc
                if (string.IsNullOrEmpty(maHD) || string.IsNullOrEmpty(maHangHoa))
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn và hàng hóa để xóa chi tiết!");
                    return;
                }

                // Xác nhận việc xóa
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa chi tiết hóa đơn này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                // Thực hiện xóa chi tiết hóa đơn từ CSDL
                string deleteQuery = "DELETE FROM ChiTietHoaDon WHERE MaHD = @MaHD AND MaHangHoa = @MaHangHoa";
                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@MaHD", maHD);
                    deleteCmd.Parameters.AddWithValue("@MaHangHoa", maHangHoa);

                    conn.Open();
                    int rowsAffected = deleteCmd.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa chi tiết hóa đơn thành công!");
                        LoadChiTietHoaDon(maHD); // Cập nhật lại danh sách chi tiết hóa đơn

                        // Cập nhật tổng tiền của hóa đơn sau khi xóa chi tiết hóa đơn
                        UpdateTotalAmount(maHD);
                    }
                    else
                    {
                        MessageBox.Show("Xóa chi tiết hóa đơn thất bại!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridCTHD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridCTHD.SelectedItem != null && DataGridCTHD.SelectedItem is DataRowView row)
            {

                // Hiển thị Mã Hàng Hóa trong ComboBox cbCTMaHH
                string maHangHoa = row["MaHangHoa"].ToString();
                foreach (var item in cbCTMaHH.Items)
                {
                    if (item is DataRowView itemView && itemView["MaHangHoa"].ToString() == maHangHoa)
                    {
                        cbCTMaHH.SelectedItem = item;
                        break;
                    }
                }

                txtCTSL.Text = row["SL"].ToString(); // Số Lượng

                if (row["DonGia"] != DBNull.Value)
                {
                    float donGia = Convert.ToSingle(row["DonGia"]); // Convert Đơn Giá to float
                    txtCTDG.Text = donGia.ToString(); // Hiển thị Đơn Giá
                }
                else
                {
                    txtCTDG.Text = ""; // Đặt trống nếu Đơn Giá null trong CSDL
                }
            }
        }
        private void DataGridHD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridHD.SelectedItem != null && DataGridHD.SelectedItem is DataRowView row)
            {
                txtMaHD.Text = row["MaHD"].ToString(); // Mã HĐ

                // Đặt giá trị cho ComboBox mã khách hàng
                cbMaKH.SelectedValue = row["MaKH"].ToString();

                // Đặt giá trị cho ComboBox mã nhân viên
                cbMaNV.SelectedValue = row["MaNV"].ToString();

                datePickerNgayLap.SelectedDate = Convert.ToDateTime(row["NgayLap"]); // Ngày Lập

                if (row["TongTien"] != DBNull.Value)
                {
                    float tongTien = Convert.ToSingle(row["TongTien"]); // Convert Tổng Tiền to float
                    txtTongTien.Text = tongTien.ToString(); // Set TextBox value
                }
                else
                {
                    txtTongTien.Text = ""; // Clear TextBox if TongTien is null in database
                }

                // Load chi tiết hóa đơn liên quan vào DataGridCTHD
                LoadChiTietHoaDon(row["MaHD"].ToString());
            }
        }

        private void txtMaHD_TextChanged(object sender, TextChangedEventArgs e)
        {
            string MaHD  = txtMaHD.Text.Trim();
            LoadChiTietHoaDon(MaHD);
        }
    }
}
