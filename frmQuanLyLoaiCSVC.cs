using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyLoaiCSVC : Form
    {
        public frmQuanLyLoaiCSVC()
        {
            InitializeComponent();
            LoadLoaiCSVCData();
        }

        private void LoadLoaiCSVCData()
        {
            try
            {
                string query = "SELECT MaLoaiCSVC, TenLoaiCSVC, MoTa FROM LoaiCoSoVatChat";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvLoaiCSVC.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu loại CSVC: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenLoaiCSVC.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại CSVC!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenLoaiCSVC.Focus();
                return;
            }

            try
            {
                string query = "INSERT INTO LoaiCoSoVatChat(TenLoaiCSVC, MoTa) VALUES (@TenLoaiCSVC, @MoTa)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TenLoaiCSVC", txtTenLoaiCSVC.Text.Trim()),
                    new SqlParameter("@MoTa", txtMoTa.Text.Trim())
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Thêm loại CSVC thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadLoaiCSVCData();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Thêm loại CSVC thất bại!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm loại CSVC: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvLoaiCSVC.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn loại CSVC cần sửa!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenLoaiCSVC.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại CSVC!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenLoaiCSVC.Focus();
                return;
            }

            try
            {
                int maLoaiCSVC = Convert.ToInt32(dgvLoaiCSVC.SelectedRows[0].Cells["MaLoaiCSVC"].Value);

                string query = "UPDATE LoaiCoSoVatChat SET TenLoaiCSVC=@TenLoaiCSVC, MoTa=@MoTa WHERE MaLoaiCSVC=@MaLoaiCSVC";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TenLoaiCSVC", txtTenLoaiCSVC.Text.Trim()),
                    new SqlParameter("@MoTa", txtMoTa.Text.Trim()),
                    new SqlParameter("@MaLoaiCSVC", maLoaiCSVC)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Cập nhật loại CSVC thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadLoaiCSVCData();
                }
                else
                {
                    MessageBox.Show("Cập nhật loại CSVC thất bại!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật loại CSVC: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvLoaiCSVC.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn loại CSVC cần xóa!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int maLoaiCSVC = Convert.ToInt32(dgvLoaiCSVC.SelectedRows[0].Cells["MaLoaiCSVC"].Value);
            string tenLoaiCSVC = dgvLoaiCSVC.SelectedRows[0].Cells["TenLoaiCSVC"].Value.ToString();

            // Kiểm tra ràng buộc trước khi xóa
            if (KiemTraRangBuoc(maLoaiCSVC))
            {
                MessageBox.Show($"Không thể xóa loại CSVC {tenLoaiCSVC} vì có CSVC thuộc loại này!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa loại CSVC {tenLoaiCSVC}?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM LoaiCoSoVatChat WHERE MaLoaiCSVC=@MaLoaiCSVC";
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@MaLoaiCSVC", maLoaiCSVC)
                    };

                    int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                    if (result > 0)
                    {
                        MessageBox.Show("Xóa loại CSVC thành công!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadLoaiCSVCData();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Xóa loại CSVC thất bại!",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa loại CSVC: " + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool KiemTraRangBuoc(int maLoaiCSVC)
        {
            string query = "SELECT COUNT(*) FROM CoSoVatChatLop WHERE MaLoaiCSVC = @MaLoaiCSVC";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaLoaiCSVC", maLoaiCSVC)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }

        private void dgvLoaiCSVC_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLoaiCSVC.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvLoaiCSVC.SelectedRows[0];
                txtTenLoaiCSVC.Text = row.Cells["TenLoaiCSVC"].Value.ToString();
                txtMoTa.Text = row.Cells["MoTa"].Value?.ToString();
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = "SELECT MaLoaiCSVC, TenLoaiCSVC, MoTa FROM LoaiCoSoVatChat WHERE TenLoaiCSVC LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvLoaiCSVC.DataSource = dt;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadLoaiCSVCData();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            txtTenLoaiCSVC.Clear();
            txtMoTa.Clear();
            dgvLoaiCSVC.ClearSelection();
        }
    }
}