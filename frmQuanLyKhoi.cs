using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyKhoi : Form
    {
        public frmQuanLyKhoi()
        {
            InitializeComponent();
            dgvKhoiHoc.SelectionChanged += dgvKhoiHoc_SelectionChanged; // Kết nối sự kiện
            LoadKhoiData();
        }

        private void LoadKhoiData()
        {
            try
            {
                string query = "SELECT MaKhoi, TenKhoi FROM Khoi";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvKhoiHoc.DataSource = dt; // Thay đổi từ dgvKhoi sang dgvKhoiHoc

                // Ẩn cột mã khối nếu cần
                if (dgvKhoiHoc.Columns.Contains("MaKhoi"))
                    dgvKhoiHoc.Columns["MaKhoi"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu khối: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKhoi.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khối!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenKhoi.Focus();
                return;
            }

            try
            {
                string query = "INSERT INTO Khoi (TenKhoi) VALUES (@TenKhoi)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TenKhoi", txtTenKhoi.Text.Trim())
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Thêm khối thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadKhoiData();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Thêm khối thất bại!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khối: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvKhoiHoc.SelectedRows.Count == 0) // Đã sửa thành dgvKhoiHoc
            {
                MessageBox.Show("Vui lòng chọn khối cần sửa!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenKhoi.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khối!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenKhoi.Focus();
                return;
            }

            try
            {
                int maKhoi = Convert.ToInt32(dgvKhoiHoc.SelectedRows[0].Cells["MaKhoi"].Value); // Đã sửa

                string query = "UPDATE Khoi SET TenKhoi = @TenKhoi WHERE MaKhoi = @MaKhoi";
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@TenKhoi", txtTenKhoi.Text.Trim()),
            new SqlParameter("@MaKhoi", maKhoi)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Cập nhật khối thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadKhoiData();
                }
                else
                {
                    MessageBox.Show("Cập nhật khối thất bại!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật khối: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvKhoiHoc.SelectedRows.Count == 0) // Đã sửa thành dgvKhoiHoc
            {
                MessageBox.Show("Vui lòng chọn khối cần xóa!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            int maKhoi = Convert.ToInt32(dgvKhoiHoc.SelectedRows[0].Cells["MaKhoi"].Value); // Đã sửa
            string tenKhoi = dgvKhoiHoc.SelectedRows[0].Cells["TenKhoi"].Value.ToString(); // Đã sửa


            // Kiểm tra ràng buộc trước khi xóa
            if (KiemTraRangBuoc(maKhoi))
            {
                MessageBox.Show($"Không thể xóa khối {tenKhoi} vì có lớp học thuộc khối này!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa khối {tenKhoi}?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Khoi WHERE MaKhoi = @MaKhoi";
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@MaKhoi", maKhoi)
                    };

                    int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                    if (result > 0)
                    {
                        MessageBox.Show("Xóa khối thành công!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadKhoiData();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Xóa khối thất bại!",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa khối: " + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool KiemTraRangBuoc(int maKhoi)
        {
            string query = "SELECT COUNT(*) FROM LopHoc WHERE MaKhoi = @MaKhoi";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaKhoi", maKhoi)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }

        private void dgvKhoiHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKhoiHoc.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvKhoiHoc.SelectedRows[0];
                txtMaKhoi.Text = row.Cells["MaKhoi"].Value?.ToString() ?? "";
                txtTenKhoi.Text = row.Cells["TenKhoi"].Value?.ToString() ?? "";
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = "SELECT MaKhoi, TenKhoi FROM Khoi WHERE TenKhoi LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvKhoiHoc.DataSource = dt; // Thay đổi từ dgvKhoi sang dgvKhoiHoc
        }

       
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadKhoiData();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            txtMaKhoi.Clear(); // Thêm dòng này
            txtTenKhoi.Clear();
            dgvKhoiHoc.ClearSelection(); // Thay đổi từ dgvKhoi sang dgvKhoiHoc
        }
     
        private void dgvKhoiHoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frmQuanLyKhoi_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }


}