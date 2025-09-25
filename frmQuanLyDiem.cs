using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyDiem : Form
    {
        public frmQuanLyDiem()
        {
            InitializeComponent();
            LoadComboBox();
            LoadDiem();
        }

        private void LoadComboBox()
        {
            // Load học sinh
            string queryHS = "SELECT MaHocSinh, HoTen FROM HocSinh";
            DataTable dtHS = DatabaseHelper.ExecuteQuery(queryHS);
            cbHocSinh.DataSource = dtHS;
            cbHocSinh.DisplayMember = "HoTen";
            cbHocSinh.ValueMember = "MaHocSinh";

            // Load môn học
            string queryMH = "SELECT MaMonHoc, TenMonHoc FROM MonHoc";
            DataTable dtMH = DatabaseHelper.ExecuteQuery(queryMH);
            cbMonHoc.DataSource = dtMH;
            cbMonHoc.DisplayMember = "TenMonHoc";
            cbMonHoc.ValueMember = "MaMonHoc";

            // Load học kỳ
            cbHocKy.Items.Add("1");
            cbHocKy.Items.Add("2");
            cbHocKy.SelectedIndex = 0;
        }

        private void LoadDiem()
        {
            string query = @"SELECT d.MaDiem, hs.HoTen AS TenHocSinh, mh.TenMonHoc, d.HocKy, 
                           d.Diem15Phut, d.Diem1Tiet, d.DiemThi, d.DiemTB
                           FROM Diem d
                           JOIN HocSinh hs ON d.MaHocSinh = hs.MaHocSinh
                           JOIN MonHoc mh ON d.MaMonHoc = mh.MaMonHoc";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvDiem.DataSource = dt;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cbHocSinh.SelectedValue == null || cbMonHoc.SelectedValue == null || cbHocKy.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tính điểm trung bình
            decimal diemTB = (numDiem15Phut.Value * 0.2m + numDiem1Tiet.Value * 0.3m + numDiemThi.Value * 0.5m);

            string query = "INSERT INTO Diem(MaHocSinh, MaMonHoc, HocKy, Diem15Phut, Diem1Tiet, DiemThi, DiemTB) " +
                          "VALUES (@MaHocSinh, @MaMonHoc, @HocKy, @Diem15Phut, @Diem1Tiet, @DiemThi, @DiemTB)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHocSinh", cbHocSinh.SelectedValue),
                new SqlParameter("@MaMonHoc", cbMonHoc.SelectedValue),
                new SqlParameter("@HocKy", cbHocKy.SelectedItem),
                new SqlParameter("@Diem15Phut", numDiem15Phut.Value),
                new SqlParameter("@Diem1Tiet", numDiem1Tiet.Value),
                new SqlParameter("@DiemThi", numDiemThi.Value),
                new SqlParameter("@DiemTB", diemTB)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDiem();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm điểm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvDiem.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn điểm cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tính điểm trung bình
            decimal diemTB = (numDiem15Phut.Value * 0.2m + numDiem1Tiet.Value * 0.3m + numDiemThi.Value * 0.5m);

            string query = "UPDATE Diem SET MaHocSinh=@MaHocSinh, MaMonHoc=@MaMonHoc, HocKy=@HocKy, " +
                          "Diem15Phut=@Diem15Phut, Diem1Tiet=@Diem1Tiet, DiemThi=@DiemThi, DiemTB=@DiemTB " +
                          "WHERE MaDiem=@MaDiem";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHocSinh", cbHocSinh.SelectedValue),
                new SqlParameter("@MaMonHoc", cbMonHoc.SelectedValue),
                new SqlParameter("@HocKy", cbHocKy.SelectedItem),
                new SqlParameter("@Diem15Phut", numDiem15Phut.Value),
                new SqlParameter("@Diem1Tiet", numDiem1Tiet.Value),
                new SqlParameter("@DiemThi", numDiemThi.Value),
                new SqlParameter("@DiemTB", diemTB),
                new SqlParameter("@MaDiem", dgvDiem.SelectedRows[0].Cells["MaDiem"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDiem();
            }
            else
            {
                MessageBox.Show("Cập nhật điểm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDiem.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn điểm cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa điểm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM Diem WHERE MaDiem=@MaDiem";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaDiem", dgvDiem.SelectedRows[0].Cells["MaDiem"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDiem();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa điểm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvDiem_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDiem.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvDiem.SelectedRows[0];
                cbHocSinh.Text = row.Cells["TenHocSinh"].Value.ToString();
                cbMonHoc.Text = row.Cells["TenMonHoc"].Value.ToString();
                cbHocKy.Text = row.Cells["HocKy"].Value.ToString();
                numDiem15Phut.Value = Convert.ToDecimal(row.Cells["Diem15Phut"].Value);
                numDiem1Tiet.Value = Convert.ToDecimal(row.Cells["Diem1Tiet"].Value);
                numDiemThi.Value = Convert.ToDecimal(row.Cells["DiemThi"].Value);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = @"SELECT d.MaDiem, hs.HoTen AS TenHocSinh, mh.TenMonHoc, d.HocKy, 
                           d.Diem15Phut, d.Diem1Tiet, d.DiemThi, d.DiemTB
                           FROM Diem d
                           JOIN HocSinh hs ON d.MaHocSinh = hs.MaHocSinh
                           JOIN MonHoc mh ON d.MaMonHoc = mh.MaMonHoc
                           WHERE hs.HoTen LIKE @Keyword OR mh.TenMonHoc LIKE @Keyword OR d.HocKy LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvDiem.DataSource = dt;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadDiem();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            if (cbHocSinh.Items.Count > 0)
                cbHocSinh.SelectedIndex = 0;
            if (cbMonHoc.Items.Count > 0)
                cbMonHoc.SelectedIndex = 0;
            if (cbHocKy.Items.Count > 0)
                cbHocKy.SelectedIndex = 0;
            numDiem15Phut.Value = 0;
            numDiem1Tiet.Value = 0;
            numDiemThi.Value = 0;
        }

        private void numDiem_ValueChanged(object sender, EventArgs e)
        {
            // Tính điểm TB khi thay đổi các điểm thành phần
            decimal diemTB = (numDiem15Phut.Value * 0.2m + numDiem1Tiet.Value * 0.3m + numDiemThi.Value * 0.5m);
            lblDiemTB.Text = $"Điểm TB: {diemTB.ToString("5.00")}";
        }
    }
}