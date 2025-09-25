using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmPhanCongGiangDay : Form
    {
        public frmPhanCongGiangDay()
        {
            InitializeComponent();
            LoadComboBox();
            LoadPhanCong();
        }

        private void LoadComboBox()
        {
            // Load giáo viên
            string queryGV = "SELECT MaGiaoVien, HoTen FROM GiaoVien";
            DataTable dtGV = DatabaseHelper.ExecuteQuery(queryGV);
            cbGiaoVien.DataSource = dtGV;
            cbGiaoVien.DisplayMember = "HoTen";
            cbGiaoVien.ValueMember = "MaGiaoVien";

            // Load lớp học
            string queryLH = "SELECT MaLop, TenLop FROM LopHoc";
            DataTable dtLH = DatabaseHelper.ExecuteQuery(queryLH);
            cbLopHoc.DataSource = dtLH;
            cbLopHoc.DisplayMember = "TenLop";
            cbLopHoc.ValueMember = "MaLop";

            // Load môn học
            string queryMH = "SELECT MaMonHoc, TenMonHoc FROM MonHoc";
            DataTable dtMH = DatabaseHelper.ExecuteQuery(queryMH);
            cbMonHoc.DataSource = dtMH;
            cbMonHoc.DisplayMember = "TenMonHoc";
            cbMonHoc.ValueMember = "MaMonHoc";
        }

        private void LoadPhanCong()
        {
            string query = @"SELECT pc.MaPhanCong, gv.HoTen AS TenGiaoVien, lh.TenLop, mh.TenMonHoc, pc.NgayPhanCong,
                           pc.MaGiaoVien, pc.MaLop, pc.MaMonHoc
                           FROM PhanCongGiangDay pc
                           JOIN GiaoVien gv ON pc.MaGiaoVien = gv.MaGiaoVien
                           JOIN LopHoc lh ON pc.MaLop = lh.MaLop
                           JOIN MonHoc mh ON pc.MaMonHoc = mh.MaMonHoc";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvPhanCong.DataSource = dt;
            dgvPhanCong.Columns["MaGiaoVien"].Visible = false;
            dgvPhanCong.Columns["MaLop"].Visible = false;
            dgvPhanCong.Columns["MaMonHoc"].Visible = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cbGiaoVien.SelectedValue == null || cbLopHoc.SelectedValue == null || cbMonHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO PhanCongGiangDay(MaGiaoVien, MaLop, MaMonHoc, NgayPhanCong) " +
                          "VALUES (@MaGiaoVien, @MaLop, @MaMonHoc, @NgayPhanCong)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaGiaoVien", cbGiaoVien.SelectedValue),
                new SqlParameter("@MaLop", cbLopHoc.SelectedValue),
                new SqlParameter("@MaMonHoc", cbMonHoc.SelectedValue),
                new SqlParameter("@NgayPhanCong", dtpNgayPhanCong.Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm phân công thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPhanCong();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm phân công thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvPhanCong.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phân công cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "UPDATE PhanCongGiangDay SET MaGiaoVien=@MaGiaoVien, MaLop=@MaLop, " +
                          "MaMonHoc=@MaMonHoc, NgayPhanCong=@NgayPhanCong WHERE MaPhanCong=@MaPhanCong";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaGiaoVien", cbGiaoVien.SelectedValue),
                new SqlParameter("@MaLop", cbLopHoc.SelectedValue),
                new SqlParameter("@MaMonHoc", cbMonHoc.SelectedValue),
                new SqlParameter("@NgayPhanCong", dtpNgayPhanCong.Value),
                new SqlParameter("@MaPhanCong", dgvPhanCong.SelectedRows[0].Cells["MaPhanCong"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật phân công thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPhanCong();
            }
            else
            {
                MessageBox.Show("Cập nhật phân công thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvPhanCong.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phân công cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa phân công này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM PhanCongGiangDay WHERE MaPhanCong=@MaPhanCong";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaPhanCong", dgvPhanCong.SelectedRows[0].Cells["MaPhanCong"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa phân công thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPhanCong();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa phân công thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvPhanCong_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPhanCong.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvPhanCong.SelectedRows[0];
                cbGiaoVien.Text = row.Cells["TenGiaoVien"].Value.ToString();
                cbLopHoc.Text = row.Cells["TenLop"].Value.ToString();
                cbMonHoc.Text = row.Cells["TenMonHoc"].Value.ToString();
                dtpNgayPhanCong.Value = Convert.ToDateTime(row.Cells["NgayPhanCong"].Value);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = @"SELECT pc.MaPhanCong, gv.HoTen AS TenGiaoVien, lh.TenLop, mh.TenMonHoc, pc.NgayPhanCong,
                           pc.MaGiaoVien, pc.MaLop, pc.MaMonHoc
                           FROM PhanCongGiangDay pc
                           JOIN GiaoVien gv ON pc.MaGiaoVien = gv.MaGiaoVien
                           JOIN LopHoc lh ON pc.MaLop = lh.MaLop
                           JOIN MonHoc mh ON pc.MaMonHoc = mh.MaMonHoc
                           WHERE gv.HoTen LIKE @Keyword OR lh.TenLop LIKE @Keyword OR mh.TenMonHoc LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvPhanCong.DataSource = dt;
            dgvPhanCong.Columns["MaGiaoVien"].Visible = false;
            dgvPhanCong.Columns["MaLop"].Visible = false;
            dgvPhanCong.Columns["MaMonHoc"].Visible = false;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadPhanCong();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            if (cbGiaoVien.Items.Count > 0)
                cbGiaoVien.SelectedIndex = 0;
            if (cbLopHoc.Items.Count > 0)
                cbLopHoc.SelectedIndex = 0;
            if (cbMonHoc.Items.Count > 0)
                cbMonHoc.SelectedIndex = 0;
            dtpNgayPhanCong.Value = DateTime.Now;
        }
    }
}