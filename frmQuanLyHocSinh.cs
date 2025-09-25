using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyHocSinh : Form
    {
        public frmQuanLyHocSinh()
        {
            InitializeComponent();
            LoadHocSinh();
            LoadComboBox();
        }

        private void LoadComboBox()
        {
          
            string queryLH = "SELECT MaLop, TenLop FROM LopHoc";
            DataTable dtLH = DatabaseHelper.ExecuteQuery(queryLH);
            cbLop.DataSource = dtLH;
            cbLop.DisplayMember = "TenLop";
            cbLop.ValueMember = "MaLop";
        }

        private void LoadHocSinh()
        {
            string query = @"SELECT hs.MaHocSinh, hs.HoTen, hs.NgaySinh, hs.GioiTinh, hs.DiaChi, 
                           lh.TenLop, lh.MaLop
                           FROM HocSinh hs
                           JOIN LopHoc lh ON hs.MaLop = lh.MaLop";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvHocSinh.DataSource = dt;
            dgvHocSinh.Columns["MaLop"].Visible = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên học sinh!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO HocSinh(HoTen, NgaySinh, GioiTinh, DiaChi, MaLop) VALUES (@HoTen, @NgaySinh, @GioiTinh, @DiaChi, @MaLop)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HoTen", txtHoTen.Text),
                new SqlParameter("@NgaySinh", dtpNgaySinh.Value),
                new SqlParameter("@GioiTinh", cbbgtinh.SelectedItem.ToString()),
                new SqlParameter("@DiaChi", txtDiaChi.Text),
                new SqlParameter("@MaLop", cbLop.SelectedValue)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm học sinh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHocSinh();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm học sinh thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvHocSinh.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn học sinh cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "UPDATE HocSinh SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, DiaChi=@DiaChi, MaLop=@MaLop WHERE MaHocSinh=@MaHocSinh";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HoTen", txtHoTen.Text),
                new SqlParameter("@NgaySinh", dtpNgaySinh.Value),
                new SqlParameter("@GioiTinh", rbNam.Checked ? "Nam" : "Nữ"),
                new SqlParameter("@DiaChi", txtDiaChi.Text),
                new SqlParameter("@MaLop", cbLop.SelectedValue),
                new SqlParameter("@MaHocSinh", dgvHocSinh.SelectedRows[0].Cells["MaHocSinh"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật học sinh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHocSinh();
            }
            else
            {
                MessageBox.Show("Cập nhật học sinh thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvHocSinh.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn học sinh cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa học sinh này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM HocSinh WHERE MaHocSinh=@MaHocSinh";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaHocSinh", dgvHocSinh.SelectedRows[0].Cells["MaHocSinh"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa học sinh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHocSinh();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa học sinh thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvHocSinh_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHocSinh.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvHocSinh.SelectedRows[0];
                txtHoTen.Text = row.Cells["HoTen"].Value.ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                string gioiTinh = row.Cells["GioiTinh"].Value.ToString();
                rbNam.Checked = (gioiTinh == "Nam");
                rbNu.Checked = !rbNam.Checked;
                txtDiaChi.Text = row.Cells["DiaChi"].Value.ToString();
                cbLop.SelectedValue = row.Cells["MaLop"].Value;
            }
        }
        //string keyword = txtTimKiem.Text.Trim();
        //string query = @"SELECT hs.MaHocSinh, hs.HoTen, hs.NgaySinh, hs.GioiTinh, hs.DiaChi, 
        //                    lh.TenLop, lh.MaLop
        //                    FROM HocSinh hs
        //                    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
        //                    WHERE hs.HoTen LIKE @Keyword OR lh.TenLop LIKE @Keyword";
        //SqlParameter[] parameters = new SqlParameter[]

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = @"SELECT hs.MaHocSinh, hs.HoTen, hs.NgaySinh, hs.GioiTinh, hs.DiaChi, 
                            lh.TenLop, lh.MaLop
                            FROM HocSinh hs
                            JOIN LopHoc lh ON hs.MaLop = lh.MaLop
                            WHERE hs.HoTen LIKE @Keyword OR lh.TenLop LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvHocSinh.DataSource = dt;
            dgvHocSinh.Columns["MaLop"].Visible = false;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadHocSinh();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            rbNam.Checked = true;
            txtDiaChi.Clear();
            if (cbLop.Items.Count > 0)
                cbLop.SelectedIndex = 0;
        }

        private void cbLop_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rbNam_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}