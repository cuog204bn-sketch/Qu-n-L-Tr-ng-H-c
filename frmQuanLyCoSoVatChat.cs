using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmQuanLyCoSoVatChat : Form
    {
        public frmQuanLyCoSoVatChat()
        {
            InitializeComponent();
            LoadComboBox();
            LoadCSVCData();
        }

        private void LoadComboBox()
        {
            try
            {
                // Load loại cơ sở vật chất
                string queryLoaiCSVC = "SELECT MaLoaiCSVC, TenLoaiCSVC FROM LoaiCoSoVatChat";
                DataTable dtLoaiCSVC = DatabaseHelper.ExecuteQuery(queryLoaiCSVC);
                cbLoaiCSVC.DataSource = dtLoaiCSVC;
                cbLoaiCSVC.DisplayMember = "TenLoaiCSVC";
                cbLoaiCSVC.ValueMember = "MaLoaiCSVC";

                // Load lớp học
                string queryLop = "SELECT MaLop, TenLop FROM LopHoc";
                DataTable dtLop = DatabaseHelper.ExecuteQuery(queryLop);
                cbLop.DataSource = dtLop;
                cbLop.DisplayMember = "TenLop";
                cbLop.ValueMember = "MaLop";

                // Thiết lập giá trị mặc định cho ComboBox tình trạng
                cbTinhTrang.Items.AddRange(new string[] { "Tốt", "Hỏng nhẹ", "Hỏng nặng", "Đã thanh lý" });
                cbTinhTrang.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu combobox: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCSVCData()
        {
            string query = @"SELECT c.MaCSVC, c.MaLoaiCSVC, l.TenLoaiCSVC, c.MaLop, lp.TenLop, 
            c.SoLuong, c.TinhTrang, c.GhiChu, c.NgayKiemTra
            FROM CoSoVatChatLop c
            JOIN LoaiCoSoVatChat l ON c.MaLoaiCSVC = l.MaLoaiCSVC
            JOIN LopHoc lp ON c.MaLop = lp.MaLop";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvCSVC.DataSource = dt;

            // Kiểm tra xem cột có tồn tại trước khi ẩn
            if (dgvCSVC.Columns.Contains("MaLoaiCSVC"))
                dgvCSVC.Columns["MaLoaiCSVC"].Visible = false;

            if (dgvCSVC.Columns.Contains("MaLop"))
                dgvCSVC.Columns["MaLop"].Visible = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLoaiCSVC.Text) || string.IsNullOrEmpty(cbLop.Text))
            {
                MessageBox.Show("Vui lòng chọn loại CSVC và lớp học!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"INSERT INTO CoSoVatChatLop(MaLoaiCSVC, MaLop, SoLuong, TinhTrang, GhiChu, NgayKiemTra)
                           VALUES (@MaLoaiCSVC, @MaLop, @SoLuong, @TinhTrang, @GhiChu, @NgayKiemTra)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaLoaiCSVC", cbLoaiCSVC.SelectedValue),
                new SqlParameter("@MaLop", cbLop.SelectedValue),
                new SqlParameter("@SoLuong", numSoLuong.Value),
                new SqlParameter("@TinhTrang", cbTinhTrang.SelectedItem.ToString()),
                new SqlParameter("@GhiChu", txtGhiChu.Text),
                new SqlParameter("@NgayKiemTra", dtpNgayKiemTra.Value)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm CSVC thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCSVCData();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Thêm CSVC thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvCSVC.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn CSVC cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Kiểm tra giá trị SelectedValue có null không
                if (cbLoaiCSVC.SelectedValue == null || cbLop.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = @"UPDATE CoSoVatChatLop SET MaLoaiCSVC=@MaLoaiCSVC, MaLop=@MaLop, SoLuong=@SoLuong, 
                       TinhTrang=@TinhTrang, GhiChu=@GhiChu, NgayKiemTra=@NgayKiemTra WHERE MaCSVC=@MaCSVC";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@MaLoaiCSVC", cbLoaiCSVC.SelectedValue),
            new SqlParameter("@MaLop", cbLop.SelectedValue),
            new SqlParameter("@SoLuong", numSoLuong.Value),
            new SqlParameter("@TinhTrang", cbTinhTrang.SelectedItem?.ToString() ?? ""),
            new SqlParameter("@GhiChu", txtGhiChu.Text),
            new SqlParameter("@NgayKiemTra", dtpNgayKiemTra.Value),
            new SqlParameter("@MaCSVC", dgvCSVC.SelectedRows[0].Cells["MaCSVC"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Cập nhật CSVC thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCSVCData();
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu nào được cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật CSVC: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvCSVC.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn CSVC cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa CSVC này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM CoSoVatChatLop WHERE MaCSVC=@MaCSVC";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaCSVC", dgvCSVC.SelectedRows[0].Cells["MaCSVC"].Value)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa CSVC thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCSVCData();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa CSVC thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void dgvCSVC_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCSVC.SelectedRows.Count > 0 && dgvCSVC.SelectedRows[0] != null)
            {
                try
                {
                    DataGridViewRow row = dgvCSVC.SelectedRows[0];

                    // Kiểm tra null và tồn tại của các cột
                    if (row.Cells["MaLoaiCSVC"].Value != null)
                        cbLoaiCSVC.SelectedValue = row.Cells["MaLoaiCSVC"].Value;

                    if (row.Cells["MaLop"].Value != null)
                        cbLop.SelectedValue = row.Cells["MaLop"].Value;

                    if (row.Cells["SoLuong"].Value != null)
                        numSoLuong.Value = Convert.ToInt32(row.Cells["SoLuong"].Value);

                    if (row.Cells["TinhTrang"].Value != null)
                        cbTinhTrang.SelectedItem = row.Cells["TinhTrang"].Value.ToString();

                    txtGhiChu.Text = row.Cells["GhiChu"].Value?.ToString();

                    if (row.Cells["NgayKiemTra"].Value != null)
                        dtpNgayKiemTra.Value = Convert.ToDateTime(row.Cells["NgayKiemTra"].Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            string query = @"SELECT c.MaCSVC, l.TenLoaiCSVC, lp.TenLop, c.SoLuong, c.TinhTrang, c.GhiChu, c.NgayKiemTra
                           FROM CoSoVatChatLop c
                           JOIN LoaiCoSoVatChat l ON c.MaLoaiCSVC = l.MaLoaiCSVC
                           JOIN LopHoc lp ON c.MaLop = lp.MaLop
                           WHERE l.TenLoaiCSVC LIKE @Keyword OR lp.TenLop LIKE @Keyword OR c.TinhTrang LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvCSVC.DataSource = dt;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadCSVCData();
            ClearFields();
            txtTimKiem.Clear();
        }

        private void ClearFields()
        {
            if (cbLoaiCSVC.Items.Count > 0)
                cbLoaiCSVC.SelectedIndex = 0;
            if (cbLop.Items.Count > 0)
                cbLop.SelectedIndex = 0;
            numSoLuong.Value = 1;
            cbTinhTrang.SelectedIndex = 0;
            txtGhiChu.Clear();
            dtpNgayKiemTra.Value = DateTime.Now;
        }

        private void dgvCSVC_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}