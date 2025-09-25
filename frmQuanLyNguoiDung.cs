using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyNguoiDung : Form
    {
        public frmQuanLyNguoiDung()
        {
            InitializeComponent();
            LoadNguoiDung();
        }

        private void LoadNguoiDung()
        {
            string query = "SELECT * FROM NguoiDung";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvNguoiDung.DataSource = dt;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO NguoiDung(Username, Password, Role) VALUES (@Username, @Password, @Role)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", txtUsername.Text),
                new SqlParameter("@Password", txtPassword.Text),
                new SqlParameter("@Role", cbRole.SelectedItem.ToString())
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadNguoiDung();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm người dùng thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvNguoiDung.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn người dùng cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "UPDATE NguoiDung SET Username=@Username, Password=@Password, Role=@Role WHERE MaNguoiDung=@MaNguoiDung";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", txtUsername.Text),
                new SqlParameter("@Password", txtPassword.Text),
                new SqlParameter("@Role", cbRole.SelectedItem.ToString()),
                new SqlParameter("@MaNguoiDung", dgvNguoiDung.SelectedRows[0].Cells["MaNguoiDung"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật người dùng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadNguoiDung();
            }
            else
            {
                MessageBox.Show("Cập nhật người dùng thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvNguoiDung.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn người dùng cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa người dùng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM NguoiDung WHERE MaNguoiDung=@MaNguoiDung";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaNguoiDung", dgvNguoiDung.SelectedRows[0].Cells["MaNguoiDung"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa người dùng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadNguoiDung();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa người dùng thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvNguoiDung_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvNguoiDung.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvNguoiDung.SelectedRows[0];
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                txtPassword.Text = row.Cells["Password"].Value.ToString();
                cbRole.SelectedItem = row.Cells["Role"].Value.ToString();
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = "SELECT * FROM NguoiDung WHERE Username LIKE @Keyword OR Role LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvNguoiDung.DataSource = dt;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadNguoiDung();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            if (cbRole.Items.Count > 0)
                cbRole.SelectedIndex = 0;
        }
    }
}