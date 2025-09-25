using qlytruonghoc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
            tabControl1.SelectedIndex = 0; 
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string username = txtUsername?.Text; // Kiểm tra null
            string password = txtPassword?.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "SELECT * FROM NguoiDung WHERE Username=@Username AND Password=@Password AND TrangThai=1";
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Username", username),
        new SqlParameter("@Password", password)
            };

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string role = dt.Rows[0]["Role"]?.ToString(); // Kiểm tra null
                    MessageBox.Show($"Đăng nhập thành công với quyền {role}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    frmMain mainForm = new frmMain(role);
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDangKy_Click(object sender, EventArgs e)
        {
            // Validate dữ liệu
            if (string.IsNullOrEmpty(txtDkUsername.Text) ||
                string.IsNullOrEmpty(txtDkPassword.Text) ||
                string.IsNullOrEmpty(txtDkEmail.Text) ||
                string.IsNullOrEmpty(txtDkHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtDkPassword.Text != txtDkXacNhanMatKhau.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra username đã tồn tại chưa
            string checkQuery = "SELECT COUNT(*) FROM NguoiDung WHERE Username=@Username OR Email=@Email";
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@Username", txtDkUsername.Text),
                new SqlParameter("@Email", txtDkEmail.Text)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));
            if (count > 0)
            {
                MessageBox.Show("Tên đăng nhập hoặc email đã được sử dụng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Thêm người dùng mới
            string insertQuery = @"INSERT INTO NguoiDung (Username, Password, Email, HoTen, Role) 
                                VALUES (@Username, @Password, @Email, @HoTen, 'HocSinh')";
            SqlParameter[] insertParams = new SqlParameter[]
            {
                new SqlParameter("@Username", txtDkUsername.Text),
                new SqlParameter("@Password", txtDkPassword.Text),
                new SqlParameter("@Email", txtDkEmail.Text),
                new SqlParameter("@HoTen", txtDkHoTen.Text)
            };

            int result = DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams);
            if (result > 0)
            {
                MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tabControl1.SelectedIndex = 0; // Chuyển về tab đăng nhập
                ClearRegisterFields();
            }
            else
            {
                MessageBox.Show("Đăng ký thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearRegisterFields()
        {
            txtDkUsername.Clear();
            txtDkPassword.Clear();
            txtDkXacNhanMatKhau.Clear();
            txtDkEmail.Clear();
            txtDkHoTen.Clear();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblChuyenSangDangKy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedIndex = 1; // Chuyển sang tab đăng ký
        }

        private void lblChuyenSangDangNhap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedIndex = 0; // Chuyển sang tab đăng nhập
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}