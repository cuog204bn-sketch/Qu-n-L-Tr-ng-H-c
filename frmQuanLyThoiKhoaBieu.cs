using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyThoiKhoaBieu : Form
    {
        public frmQuanLyThoiKhoaBieu()
        {
            InitializeComponent();
            LoadComboBox();
            LoadThoiKhoaBieu();
        }

        private void LoadComboBox()
        {
            // Load lớp học
            string queryLop = "SELECT MaLop, TenLop FROM LopHoc";
            DataTable dtLop = DatabaseHelper.ExecuteQuery(queryLop);
            cbLopHoc.DataSource = dtLop;
            cbLopHoc.DisplayMember = "TenLop";
            cbLopHoc.ValueMember = "MaLop";

            // Load môn học
            string queryMonHoc = "SELECT MaMonHoc, TenMonHoc FROM MonHoc";
            DataTable dtMonHoc = DatabaseHelper.ExecuteQuery(queryMonHoc);
            cbMonHoc.DataSource = dtMonHoc;
            cbMonHoc.DisplayMember = "TenMonHoc";
            cbMonHoc.ValueMember = "MaMonHoc";

            // Load giáo viên
            string queryGV = "SELECT MaGiaoVien, HoTen FROM GiaoVien";
            DataTable dtGV = DatabaseHelper.ExecuteQuery(queryGV);
            cbGiaoVien.DataSource = dtGV;
            cbGiaoVien.DisplayMember = "HoTen";
            cbGiaoVien.ValueMember = "MaGiaoVien";

            // Load thứ
            cbThu.Items.AddRange(new object[] { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" });
            cbThu.SelectedIndex = 0;

            // Load tiết
            for (int i = 1; i <= 10; i++)
            {
                cbTiet.Items.Add(i);
            }
            cbTiet.SelectedIndex = 0;
        }

        private void LoadThoiKhoaBieu()
        {
            string query = @"SELECT t.MaTKB, l.TenLop, 
                           CASE t.Thu 
                               WHEN 2 THEN N'Thứ 2' 
                               WHEN 3 THEN N'Thứ 3' 
                               WHEN 4 THEN N'Thứ 4' 
                               WHEN 5 THEN N'Thứ 5' 
                               WHEN 6 THEN N'Thứ 6' 
                               WHEN 7 THEN N'Thứ 7' 
                               ELSE N'Chủ nhật' 
                           END AS Thu,
                           t.Tiet, m.TenMonHoc, g.HoTen AS TenGiaoVien, t.PhongHoc,
                           t.MaLop, t.MaMonHoc, t.MaGiaoVien, t.Thu AS ThuSo
                           FROM ThoiKhoaBieu t
                           JOIN LopHoc l ON t.MaLop = l.MaLop
                           JOIN MonHoc m ON t.MaMonHoc = m.MaMonHoc
                           JOIN GiaoVien g ON t.MaGiaoVien = g.MaGiaoVien
                           ORDER BY l.TenLop, t.Thu, t.Tiet";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvThoiKhoaBieu.DataSource = dt;

            // Ẩn các cột ID
            dgvThoiKhoaBieu.Columns["MaLop"].Visible = false;
            dgvThoiKhoaBieu.Columns["MaMonHoc"].Visible = false;
            dgvThoiKhoaBieu.Columns["MaGiaoVien"].Visible = false;
            dgvThoiKhoaBieu.Columns["ThuSo"].Visible = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cbLopHoc.SelectedValue == null || cbMonHoc.SelectedValue == null || cbGiaoVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int thu = cbThu.SelectedIndex + 2; // Vì index bắt đầu từ 0

            // Kiểm tra trùng lịch
            string checkQuery = @"SELECT COUNT(*) FROM ThoiKhoaBieu 
                                WHERE MaLop = @MaLop AND Thu = @Thu AND Tiet = @Tiet";
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@MaLop", cbLopHoc.SelectedValue),
                new SqlParameter("@Thu", thu),
                new SqlParameter("@Tiet", cbTiet.SelectedItem)
            };

            int count = (int)DatabaseHelper.ExecuteScalar(checkQuery, checkParams);
            if (count > 0)
            {
                MessageBox.Show("Tiết học này đã có trong thời khóa biểu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string insertQuery = @"INSERT INTO ThoiKhoaBieu(MaLop, Thu, Tiet, MaMonHoc, MaGiaoVien, PhongHoc)
                                VALUES (@MaLop, @Thu, @Tiet, @MaMonHoc, @MaGiaoVien, @PhongHoc)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaLop", cbLopHoc.SelectedValue),
                new SqlParameter("@Thu", thu),
                new SqlParameter("@Tiet", cbTiet.SelectedItem),
                new SqlParameter("@MaMonHoc", cbMonHoc.SelectedValue),
                new SqlParameter("@MaGiaoVien", cbGiaoVien.SelectedValue),
                new SqlParameter("@PhongHoc", txtPhongHoc.Text)
            };

            int result = DatabaseHelper.ExecuteNonQuery(insertQuery, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm thời khóa biểu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadThoiKhoaBieu();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm thời khóa biểu thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvThoiKhoaBieu.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn thời khóa biểu cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int thu = cbThu.SelectedIndex + 2; // Vì index bắt đầu từ 0

            string updateQuery = @"UPDATE ThoiKhoaBieu 
                                 SET MaLop = @MaLop, 
                                     Thu = @Thu, 
                                     Tiet = @Tiet, 
                                     MaMonHoc = @MaMonHoc, 
                                     MaGiaoVien = @MaGiaoVien, 
                                     PhongHoc = @PhongHoc
                                 WHERE MaTKB = @MaTKB";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaLop", cbLopHoc.SelectedValue),
                new SqlParameter("@Thu", thu),
                new SqlParameter("@Tiet", cbTiet.SelectedItem),
                new SqlParameter("@MaMonHoc", cbMonHoc.SelectedValue),
                new SqlParameter("@MaGiaoVien", cbGiaoVien.SelectedValue),
                new SqlParameter("@PhongHoc", txtPhongHoc.Text),
                new SqlParameter("@MaTKB", dgvThoiKhoaBieu.SelectedRows[0].Cells["MaTKB"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(updateQuery, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật thời khóa biểu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadThoiKhoaBieu();
            }
            else
            {
                MessageBox.Show("Cập nhật thời khóa biểu thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvThoiKhoaBieu.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn thời khóa biểu cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa thời khóa biểu này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string deleteQuery = "DELETE FROM ThoiKhoaBieu WHERE MaTKB = @MaTKB";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaTKB", dgvThoiKhoaBieu.SelectedRows[0].Cells["MaTKB"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(deleteQuery, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa thời khóa biểu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadThoiKhoaBieu();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa thời khóa biểu thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvThoiKhoaBieu_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvThoiKhoaBieu.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvThoiKhoaBieu.SelectedRows[0];

                // Lớp học
                if (row.Cells["MaLop"].Value != null)
                    cbLopHoc.SelectedValue = row.Cells["MaLop"].Value;

                // Thứ
                if (row.Cells["ThuSo"].Value != null)
                {
                    int thu = Convert.ToInt32(row.Cells["ThuSo"].Value);
                    cbThu.SelectedIndex = thu - 2;
                }

                // Tiết
                if (row.Cells["Tiet"].Value != null)
                    cbTiet.SelectedItem = row.Cells["Tiet"].Value;

                // Môn học
                if (row.Cells["MaMonHoc"].Value != null)
                    cbMonHoc.SelectedValue = row.Cells["MaMonHoc"].Value;

                // Giáo viên
                if (row.Cells["MaGiaoVien"].Value != null)
                    cbGiaoVien.SelectedValue = row.Cells["MaGiaoVien"].Value;

                // Phòng học
                txtPhongHoc.Text = row.Cells["PhongHoc"].Value?.ToString() ?? "";
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = @"SELECT t.MaTKB, l.TenLop, 
                           CASE t.Thu 
                               WHEN 2 THEN 'Thứ 2' 
                               WHEN 3 THEN 'Thứ 3' 
                               WHEN 4 THEN 'Thứ 4' 
                               WHEN 5 THEN 'Thứ 5' 
                               WHEN 6 THEN 'Thứ 6' 
                               WHEN 7 THEN 'Thứ 7' 
                               ELSE 'Chủ nhật' 
                           END AS Thu,
                           t.Tiet, m.TenMonHoc, g.HoTen AS TenGiaoVien, t.PhongHoc,
                           t.MaLop, t.MaMonHoc, t.MaGiaoVien, t.Thu AS ThuSo
                           FROM ThoiKhoaBieu t
                           JOIN LopHoc l ON t.MaLop = l.MaLop
                           JOIN MonHoc m ON t.MaMonHoc = m.MaMonHoc
                           JOIN GiaoVien g ON t.MaGiaoVien = g.MaGiaoVien
                           WHERE l.TenLop LIKE @Keyword OR m.TenMonHoc LIKE @Keyword 
                           OR g.HoTen LIKE @Keyword OR t.PhongHoc LIKE @Keyword
                           ORDER BY l.TenLop, t.Thu, t.Tiet";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvThoiKhoaBieu.DataSource = dt;

            // Ẩn các cột ID
            dgvThoiKhoaBieu.Columns["MaLop"].Visible = false;
            dgvThoiKhoaBieu.Columns["MaMonHoc"].Visible = false;
            dgvThoiKhoaBieu.Columns["MaGiaoVien"].Visible = false;
            dgvThoiKhoaBieu.Columns["ThuSo"].Visible = false;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadThoiKhoaBieu();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            if (cbLopHoc.Items.Count > 0)
                cbLopHoc.SelectedIndex = 0;
            if (cbThu.Items.Count > 0)
                cbThu.SelectedIndex = 0;
            if (cbTiet.Items.Count > 0)
                cbTiet.SelectedIndex = 0;
            if (cbMonHoc.Items.Count > 0)
                cbMonHoc.SelectedIndex = 0;
            if (cbGiaoVien.Items.Count > 0)
                cbGiaoVien.SelectedIndex = 0;
            txtPhongHoc.Clear();
        }

        private void btnXemTKB_Click(object sender, EventArgs e)
        {
            if (cbLopHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int maLop = (int)cbLopHoc.SelectedValue;
            XemThoiKhoaBieu(maLop);
        }

        private void XemThoiKhoaBieu(int maLop)
        {
            string query = @"SELECT 
                           CASE t.Thu 
                               WHEN 2 THEN 'Thứ 2' 
                               WHEN 3 THEN 'Thứ 3' 
                               WHEN 4 THEN 'Thứ 4' 
                               WHEN 5 THEN 'Thứ 5' 
                               WHEN 6 THEN 'Thứ 6' 
                               WHEN 7 THEN 'Thứ 7' 
                               ELSE 'Chủ nhật' 
                           END AS Thu,
                           t.Tiet, m.TenMonHoc, g.HoTen AS TenGiaoVien, t.PhongHoc
                           FROM ThoiKhoaBieu t
                           JOIN MonHoc m ON t.MaMonHoc = m.MaMonHoc
                           JOIN GiaoVien g ON t.MaGiaoVien = g.MaGiaoVien
                           WHERE t.MaLop = @MaLop
                           ORDER BY t.Thu, t.Tiet";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaLop", maLop)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            // Tạo form hiển thị thời khóa biểu dạng bảng
            frmXemThoiKhoaBieu frm = new frmXemThoiKhoaBieu(dt, cbLopHoc.Text);
            frm.ShowDialog();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}