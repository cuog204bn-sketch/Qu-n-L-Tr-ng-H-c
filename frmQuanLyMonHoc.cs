using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyMonHoc : Form
    {
        public frmQuanLyMonHoc()
        {
            InitializeComponent();
            LoadMonHoc();
        }

        private void LoadMonHoc()
        {
            string query = "SELECT * FROM MonHoc";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvMonHoc.DataSource = dt;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenMonHoc.Text))
            {
                MessageBox.Show("Vui lòng nhập tên môn học!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO MonHoc(TenMonHoc, SoTiet, HeSo) VALUES (@TenMonHoc, @SoTiet, @HeSo)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TenMonHoc", txtTenMonHoc.Text),
                new SqlParameter("@SoTiet", numSoTiet.Value),
                new SqlParameter("@HeSo", numHeSo.Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMonHoc();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm môn học thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvMonHoc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn môn học cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "UPDATE MonHoc SET TenMonHoc=@TenMonHoc, SoTiet=@SoTiet, HeSo=@HeSo WHERE MaMonHoc=@MaMonHoc";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TenMonHoc", txtTenMonHoc.Text),
                new SqlParameter("@SoTiet", numSoTiet.Value),
                new SqlParameter("@HeSo", numHeSo.Value),
                new SqlParameter("@MaMonHoc", dgvMonHoc.SelectedRows[0].Cells["MaMonHoc"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMonHoc();
            }
            else
            {
                MessageBox.Show("Cập nhật môn học thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvMonHoc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn môn học cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa môn học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM MonHoc WHERE MaMonHoc=@MaMonHoc";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaMonHoc", dgvMonHoc.SelectedRows[0].Cells["MaMonHoc"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMonHoc();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa môn học thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvMonHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMonHoc.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvMonHoc.SelectedRows[0];
                txtTenMonHoc.Text = row.Cells["TenMonHoc"].Value.ToString();
                numSoTiet.Value = Convert.ToInt32(row.Cells["SoTiet"].Value);
                numHeSo.Value = Convert.ToDecimal(row.Cells["HeSo"].Value);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = "SELECT * FROM MonHoc WHERE TenMonHoc LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvMonHoc.DataSource = dt;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadMonHoc();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            txtTenMonHoc.Clear();
            numSoTiet.Value = 1;
            numHeSo.Value = 1;
        }

        private void numHeSo_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}