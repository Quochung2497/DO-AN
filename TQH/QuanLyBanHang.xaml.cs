using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace TQH
{
    /// <summary>
    /// Interaction logic for QuanLyBanHang.xaml
    /// </summary>
    public partial class QuanLyBanHang : Window
    {
        public QuanLyBanHang()
        {
            InitializeComponent();
        }
       
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ReleaseCapture();
                SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }


        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Hide();
        }

        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void QLHD_Click(object sender, RoutedEventArgs e)
        {
            QuanLyHoaDon QLHD = new QuanLyHoaDon();
            MainContentControl.Content = QLHD;
        }

        private void QLHH_Click(object sender, RoutedEventArgs e)
        {
            QuanLyHangHoa qlhh = new QuanLyHangHoa();
            MainContentControl.Content = qlhh;
        }

        private void QLNV_Click(object sender, RoutedEventArgs e)
        {
            QuanLyNhanVien QLNV = new QuanLyNhanVien();
            MainContentControl.Content = QLNV;
        }

        private void QLTK_Click(object sender, RoutedEventArgs e)
        {
            QuanLyTaiKhoan QLTK = new QuanLyTaiKhoan();
            MainContentControl.Content = QLTK;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Login = new MainWindow();
            Login.Show();
            this.Hide();
            
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = null;
        }

        private void QLPN_Click(object sender, RoutedEventArgs e)
        {
            QuanLyPhieuNhap pn = new QuanLyPhieuNhap();
            MainContentControl.Content = pn;
        }

        private void QLKH_Click(object sender, RoutedEventArgs e)
        {
            QuanLyKhachHang QLKH = new QuanLyKhachHang();
            MainContentControl.Content = QLKH;
        }

        private void QLNCC_Click(object sender, RoutedEventArgs e)
        {
            QuanLyNCC quanLyNCC = new QuanLyNCC();
            MainContentControl.Content = quanLyNCC;
        }
    }
}
