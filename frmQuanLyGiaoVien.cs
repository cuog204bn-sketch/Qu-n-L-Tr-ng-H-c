using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyGiaoVien : Form
    {
        public frmQuanLyGiaoVien()
        {
            InitializeComponent();
            LoadGiaoVien();
            LoadComboBox();
        }

        private void LoadComboBox()
        {
            // Load môn học
            string queryMH = "SELECT MaMonHoc, TenMonHoc FROM MonHoc";
            DataTable dtMH = DatabaseHelper.ExecuteQuery(queryMH);
            cbMonHoc.DataSource = dtMH;
            cbMonHoc.DisplayMember = "TenMonHoc";
            cbMonHoc.ValueMember = "MaMonHoc";
        }

        private void LoadGiaoVien()
        {
            string query = @"SELECT gv.MaGiaoVien, gv.HoTen, gv.NgaySinh, gv.GioiTinh, gv.DiaChi, gv.SoDT, 
                           mh.TenMonHoc, mh.MaMonHoc
                           FROM GiaoVien gv
                           JOIN MonHoc mh ON gv.MaMonHoc = mh.MaMonHoc";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvGiaoVien.DataSource = dt;
            dgvGiaoVien.Columns["MaMonHoc"].Visible = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên giáo viên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO GiaoVien(HoTen, NgaySinh, GioiTinh, DiaChi, SoDT, MaMonHoc) VALUES (@HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SoDT, @MaMonHoc)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HoTen", txtHoTen.Text),
                new SqlParameter("@NgaySinh", dtpNgaySinh.Value),
                new SqlParameter("@GioiTinh", rbNam.Checked ? "Nam" : "Nữ"),
                new SqlParameter("@DiaChi", txtDiaChi.Text),
                new SqlParameter("@SoDT", txtSoDT.Text),
                new SqlParameter("@MaMonHoc", cbMonHoc.SelectedValue)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm giáo viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadGiaoVien();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm giáo viên thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvGiaoVien.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn giáo viên cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "UPDATE GiaoVien SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, DiaChi=@DiaChi, SoDT=@SoDT, MaMonHoc=@MaMonHoc WHERE MaGiaoVien=@MaGiaoVien";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HoTen", txtHoTen.Text),
                new SqlParameter("@NgaySinh", dtpNgaySinh.Value),
                new SqlParameter("@GioiTinh", rbNam.Checked ? "Nam" : "Nữ"),
                new SqlParameter("@DiaChi", txtDiaChi.Text),
                new SqlParameter("@SoDT", txtSoDT.Text),
                new SqlParameter("@MaMonHoc", cbMonHoc.SelectedValue),
                new SqlParameter("@MaGiaoVien", dgvGiaoVien.SelectedRows[0].Cells["MaGiaoVien"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật giáo viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadGiaoVien();
            }
            else
            {
                MessageBox.Show("Cập nhật giáo viên thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvGiaoVien.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn giáo viên cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa giáo viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM GiaoVien WHERE HoTen=@HoTen";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@HoTen", dgvGiaoVien.SelectedRows[0].Cells["HoTen"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa giáo viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGiaoVien();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa giáo viên thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvGiaoVien_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGiaoVien.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvGiaoVien.SelectedRows[0];
                txtHoTen.Text = row.Cells["HoTen"].Value.ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                string gioiTinh = row.Cells["GioiTinh"].Value.ToString();
                rbNam.Checked = (gioiTinh == "Nam");
                rbNu.Checked = !rbNam.Checked;
                txtDiaChi.Text = row.Cells["DiaChi"].Value.ToString();
                txtSoDT.Text = row.Cells["SoDT"].Value.ToString();
                cbMonHoc.SelectedValue = row.Cells["MaMonHoc"].Value;
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = @"SELECT gv.MaGiaoVien, gv.HoTen, gv.NgaySinh, gv.GioiTinh, gv.DiaChi, gv.SoDT, 
                           mh.TenMonHoc, mh.MaMonHoc
                           FROM GiaoVien gv
                           JOIN MonHoc mh ON gv.MaMonHoc = mh.MaMonHoc
                           WHERE gv.HoTen LIKE @Keyword OR mh.TenMonHoc LIKE @Keyword";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvGiaoVien.DataSource = dt;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadGiaoVien();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            rbNam.Checked = true;
            txtDiaChi.Clear();
            txtSoDT.Clear();
            if (cbMonHoc.Items.Count > 0)
                cbMonHoc.SelectedIndex = 0;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}