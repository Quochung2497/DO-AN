using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using static TQH.QuanLyNhanVien;

namespace TQH
{
    /// <summary>
    /// Interaction logic for QuanLyTaiKhoan.xaml
    /// </summary>
    public partial class QuanLyTaiKhoan : UserControl
    {
        public QuanLyTaiKhoan()
        {
            InitializeComponent();
            LoadUser();
            LoadMaNVData();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-TUHC54O;Initial Catalog=QLBH_DoAn;Integrated Security=True;");
        public void LoadUser()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select * from [User]", conn);
                DataTable dt = new DataTable();
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                conn.Close();
                DataUsers.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadMaNVData()
        {
            try
            {
                string query = "SELECT MaNV FROM NhanVien"; // Adjust the query based on your table
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);

                List<string> maNVList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    maNVList.Add(row["MaNV"].ToString());
                }

                cmbMaNV.ItemsSource = maNVList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateAccount_Btn(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem MaNV đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM [User] WHERE MaNV = @MaNV";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.CommandType = CommandType.Text;
                    checkCmd.Parameters.AddWithValue("@MaNV", cmbMaNV.SelectedItem.ToString());

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

                // Thêm tài khoản mới
                string insertQuery = "INSERT INTO [User] (MaNV, username, password, email, SDT) VALUES (@MaNV, @username, @password, @email, @SDT)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNV", cmbMaNV.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@username", txtUserName.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", txtPassword.Password.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", Convert.ToInt32(txtSDT.Text.Trim()));

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    LoadUser(); // Cập nhật lại danh sách người dùng trên DataGrid
                    MessageBox.Show("Thêm tài khoản thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteAccount_Btn(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "Delete from [user] where MaNV = @MaNV ";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@MaNV", SqlDbType.Char).Value = cmbMaNV.SelectedItem.ToString();
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();
                    LoadUser();
                    MessageBox.Show("Xoá tài khoản thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditAccount_Btn(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE [User] SET username = @username, password = @password, email = @email, SDT = @SDT WHERE MaNV = @MaNV";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNV", cmbMaNV.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@username", txtUserName.Text);
                    cmd.Parameters.AddWithValue("@password", txtPassword.Password);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@SDT", Convert.ToInt32(txtSDT.Text));

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();
                    LoadUser();
                    MessageBox.Show("Sửa tài khoản thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FindAccount_Btn(object sender, RoutedEventArgs e)
        {
            try
            {
                string maNV = cmbMaNV.SelectedItem?.ToString();
                string sdt = txtSDT.Text.Trim();

                // Kiểm tra xem người dùng đã nhập ít nhất một trong hai trường MaNV hoặc SDT chưa
                if (string.IsNullOrEmpty(maNV) && string.IsNullOrEmpty(sdt))
                {
                    MessageBox.Show("Vui lòng nhập Mã NV hoặc SĐT để tìm kiếm!");
                    return;
                }

                string query = "SELECT * FROM [User] WHERE MaNV = @MaNV OR SDT = @SDT";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@SDT", sdt);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<User> listUsers = new List<User>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            User user = new User();
                            user.MaNV = reader["MaNV"].ToString();
                            user.username = reader["username"].ToString();
                            user.password = reader["password"].ToString();
                            user.email = reader["email"].ToString();
                            user.SDT = reader["SDT"].ToString();

                            listUsers.Add(user);
                        }
                        MessageBox.Show("Đã tìm thấy tài khoản");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy tài khoản có MaNV = " + maNV + " hoặc SDT = " + sdt);
                    }

                    reader.Close();
                    conn.Close();

                    // Gán danh sách người dùng vào DataGrid
                    DataUsers.ItemsSource = listUsers;

                    // Hiện thông tin người dùng lên các TextBox nếu tìm thấy một tài khoản duy nhất
                    if (listUsers.Count == 1)
                    {
                        var user = listUsers[0];
                        cmbMaNV.SelectedItem = user.MaNV;
                        txtUserName.Text = user.username;
                        txtPassword.Password = user.password;
                        txtEmail.Text = user.email;
                        txtSDT.Text = user.SDT;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Cancle_Btn(object sender, RoutedEventArgs e)
        {
            cmbMaNV.SelectedIndex = -1;
            txtPassword.Password = "";
            txtSDT.Text = "";
            txtEmail.Text = "";
            txtUserName.Text = "";
        }

        private void DataUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataUsers.SelectedItem != null)
            {
                User selectedUser = (User)DataUsers.SelectedItem;
                cmbMaNV.SelectedItem = selectedUser.MaNV;
                txtUserName.Text = selectedUser.username;
                txtPassword.Password = selectedUser.password;
                txtEmail.Text = selectedUser.email;
                txtSDT.Text = selectedUser.SDT;
            }
        }
        public class User
        {
            public string MaNV { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public string SDT { get; set; }
        }

        private void cmbMaNV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
        }
    }
}
