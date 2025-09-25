using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc  
{
    public partial class frmQuanLyKhenThuong : Form
    {
        public frmQuanLyKhenThuong()
        {
            InitializeComponent();
            LoadKhenThuong();
            LoadComboBox();
        }

        private void LoadComboBox()
        {
            // Load học sinh
            string queryHS = "SELECT MaHocSinh, HoTen FROM HocSinh";
            DataTable dtHS = DatabaseHelper.ExecuteQuery(queryHS);
            cbHocSinh.DataSource = dtHS;
            cbHocSinh.DisplayMember = "HoTen";
            cbHocSinh.ValueMember = "MaHocSinh";

            // Load học kỳ
            cbHocKy.Items.Add("1");
            cbHocKy.Items.Add("2");
        }

        private void LoadKhenThuong()
        {
            string query = "SELECT kt.*, h.HoTen AS TenHocSinh FROM KhenThuong kt " +
                          "JOIN HocSinh h ON kt.MaHocSinh = h.MaHocSinh";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvKhenThuong.DataSource = dt;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtHinhThuc.Text) || string.IsNullOrEmpty(txtLyDo.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO KhenThuong(MaHocSinh, HocKy, HinhThuc, LyDo, NgayKhenThuong) " +
                          "VALUES (@MaHocSinh, @HocKy, @HinhThuc, @LyDo, @NgayKhenThuong)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHocSinh", cbHocSinh.SelectedValue),
                new SqlParameter("@HocKy", cbHocKy.SelectedItem),
                new SqlParameter("@HinhThuc", txtHinhThuc.Text),
                new SqlParameter("@LyDo", txtLyDo.Text),
                new SqlParameter("@NgayKhenThuong", dtpNgayKhenThuong.Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm khen thưởng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadKhenThuong();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm khen thưởng thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvKhenThuong.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn khen thưởng cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "UPDATE KhenThuong SET MaHocSinh=@MaHocSinh, HocKy=@HocKy, " +
                          "HinhThuc=@HinhThuc, LyDo=@LyDo, NgayKhenThuong=@NgayKhenThuong " +
                          "WHERE MaKhenThuong=@MaKhenThuong";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHocSinh", cbHocSinh.SelectedValue),
                new SqlParameter("@HocKy", cbHocKy.SelectedItem),
                new SqlParameter("@HinhThuc", txtHinhThuc.Text),
                new SqlParameter("@LyDo", txtLyDo.Text),
                new SqlParameter("@NgayKhenThuong", dtpNgayKhenThuong.Value),
                new SqlParameter("@MaKhenThuong", dgvKhenThuong.SelectedRows[0].Cells["MaKhenThuong"].Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật khen thưởng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadKhenThuong();
            }
            else
            {
                MessageBox.Show("Cập nhật khen thưởng thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvKhenThuong.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn khen thưởng cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa khen thưởng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM KhenThuong WHERE MaKhenThuong=@MaKhenThuong";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaKhenThuong", dgvKhenThuong.SelectedRows[0].Cells["MaKhenThuong"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa khen thưởng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadKhenThuong();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa khen thưởng thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvKhenThuong_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKhenThuong.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvKhenThuong.SelectedRows[0];
                cbHocSinh.SelectedValue = row.Cells["MaHocSinh"].Value;
                cbHocKy.SelectedItem = row.Cells["HocKy"].Value.ToString();
                txtHinhThuc.Text = row.Cells["HinhThuc"].Value.ToString();
                txtLyDo.Text = row.Cells["LyDo"].Value.ToString();
                dtpNgayKhenThuong.Value = Convert.ToDateTime(row.Cells["NgayKhenThuong"].Value);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)   
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = @"SELECT kt.*, h.HoTen  AS TenHocSinh 
                          FROM KhenThuong kt
                          JOIN HocSinh h ON kt.MaHocSinh = h.MaHocSinh
                          WHERE h.HoTen LIKE @Keyword OR kt.HinhThuc LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvKhenThuong.DataSource = dt;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadKhenThuong();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            if (cbHocSinh.Items.Count > 0)
                cbHocSinh.SelectedIndex = 0;
            if (cbHocKy.Items.Count > 0)
                cbHocKy.SelectedIndex = 0;
            txtHinhThuc.Clear();
            txtLyDo.Clear();
            dtpNgayKhenThuong.Value = DateTime.Now;
        }
    }
}