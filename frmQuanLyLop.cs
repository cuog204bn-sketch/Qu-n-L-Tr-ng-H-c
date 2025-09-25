using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyLop : Form
    {
        public frmQuanLyLop()
        {
            InitializeComponent();
            LoadLopHoc();
            LoadComboBox();
        }

        private void LoadComboBox()
        {
            // Load khối
            string queryKhoi = "SELECT MaKhoi, TenKhoi FROM Khoi";
            DataTable dtKhoi = DatabaseHelper.ExecuteQuery(queryKhoi);
            cbKhoi.DataSource = dtKhoi;
            cbKhoi.DisplayMember = "TenKhoi";
            cbKhoi.ValueMember = "MaKhoi";

            // Load giáo viên chủ nhiệm
            string queryGV = "SELECT MaGiaoVien, HoTen FROM GiaoVien";
            DataTable dtGV = DatabaseHelper.ExecuteQuery(queryGV);
            cbGiaoVienCN.DataSource = dtGV;
            cbGiaoVienCN.DisplayMember = "HoTen";
            cbGiaoVienCN.ValueMember = "MaGiaoVien";
        }

        private void LoadLopHoc()
        {
            string query = @"SELECT l.MaLop, l.TenLop, l.SiSo, k.TenKhoi, gv.HoTen AS TenGiaoVienCN, 
                           l.MaKhoi, l.MaGiaoVienCN
                           FROM LopHoc l
                           JOIN Khoi k ON l.MaKhoi = k.MaKhoi
                           LEFT JOIN GiaoVien gv ON l.MaGiaoVienCN = gv.MaGiaoVien";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvLopHoc.DataSource = dt;
            dgvLopHoc.Columns["MaKhoi"].Visible = false;
            dgvLopHoc.Columns["MaGiaoVienCN"].Visible = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenLop.Text))
            {
                MessageBox.Show("Vui lòng nhập tên lớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO LopHoc(TenLop, SiSo, MaKhoi, MaGiaoVienCN) VALUES (@TenLop, @SiSo, @MaKhoi, @MaGiaoVienCN)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TenLop", txtTenLop.Text),
                new SqlParameter("@SiSo", numSiSo.Value),
                new SqlParameter("@MaKhoi", cbKhoi.SelectedValue),
                new SqlParameter("@MaGiaoVienCN", cbGiaoVienCN.SelectedValue)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadLopHoc();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm lớp học thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lớp cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "UPDATE LopHoc SET TenLop=@TenLop, SiSo=@SiSo, MaKhoi=@MaKhoi, MaGiaoVienCN=@MaGiaoVienCN WHERE MaLop=@MaLop";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TenLop", txtTenLop.Text),
                new SqlParameter("@SiSo", numSiSo.Value),
                new SqlParameter("@MaKhoi", cbKhoi.SelectedValue),
                new SqlParameter("@MaGiaoVienCN", cbGiaoVienCN.SelectedValue),
                new SqlParameter("@MaLop", dgvLopHoc.SelectedRows[0].Cells["MaLop"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadLopHoc();
            }
            else
            {
                MessageBox.Show("Cập nhật lớp học thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lớp cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa lớp học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM LopHoc WHERE MaLop=@MaLop";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaLop", dgvLopHoc.SelectedRows[0].Cells["MaLop"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadLopHoc();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa lớp học thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvLopHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLopHoc.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow row = dgvLopHoc.SelectedRows[0];

                    // Xử lý tên lớp
                    txtTenLop.Text = row.Cells["TenLop"].Value?.ToString() ?? "";

                    // Xử lý sĩ số - FIXED
                    if (row.Cells["SiSo"].Value != null)
                    {
                        decimal siSoValue;
                        if (decimal.TryParse(row.Cells["SiSo"].Value.ToString(), out siSoValue))
                        {
                            // Chuyển decimal sang int an toàn
                            int siSo = (int)Math.Round(siSoValue);
                            // Đảm bảo giá trị nằm trong khoảng Min-Max
                            siSo = Math.Max((int)numSiSo.Minimum, Math.Min((int)numSiSo.Maximum, siSo));
                            numSiSo.Value = siSo;
                        }
                        else
                        {
                            numSiSo.Value = numSiSo.Minimum;
                        }
                    }
                    else
                    {
                        numSiSo.Value = numSiSo.Minimum;
                    }

                    // Xử lý khối
                    if (row.Cells["MaKhoi"].Value != null)
                        cbKhoi.SelectedValue = row.Cells["MaKhoi"].Value;

                    // Xử lý giáo viên CN
                    if (row.Cells["MaGiaoVienCN"].Value != null)
                        cbGiaoVienCN.SelectedValue = row.Cells["MaGiaoVienCN"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = @"SELECT l.MaLop, l.TenLop, l.SiSo, k.TenKhoi, gv.HoTen AS TenGiaoVienCN, 
                           l.MaKhoi, l.MaGiaoVienCN
                           FROM LopHoc l
                           JOIN Khoi k ON l.MaKhoi = k.MaKhoi
                           LEFT JOIN GiaoVien gv ON l.MaGiaoVienCN = gv.MaGiaoVien
                           WHERE l.TenLop LIKE @Keyword OR k.TenKhoi LIKE @Keyword OR gv.HoTen LIKE @Keyword";
            //string query = @"SELECT l.MaLop, l.TenLop, l.SiSo, k.TenKhoi, gv.HoTen AS TenGiaoVienCN, 
            //   l.MaKhoi, l.MaGiaoVienCN
            //   FROM LopHoc l
            //   JOIN Khoi k ON l.MaKhoi = k.MaKhoi
            //   LEFT JOIN GiaoVien gv ON l.MaGiaoVienCN = gv.MaGiaoVien
            //   WHERE l.SiSo > 25";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvLopHoc.DataSource = dt;
            dgvLopHoc.Columns["MaKhoi"].Visible = false;
            dgvLopHoc.Columns["MaGiaoVienCN"].Visible = false;
        }
      
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadLopHoc();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            txtTenLop.Clear();
            numSiSo.Minimum = 0; // Thêm dòng này
            numSiSo.Value = 0;
            if (cbKhoi.Items.Count > 0)
                cbKhoi.SelectedIndex = 0;
            if (cbGiaoVienCN.Items.Count > 0)
                cbGiaoVienCN.SelectedIndex = 0;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmQuanLyLop_Load(object sender, EventArgs e)
        {

        }
    }
}