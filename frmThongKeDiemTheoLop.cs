using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmThongKeDiemTheoLop : Form
    {
        public frmThongKeDiemTheoLop()
        {
            InitializeComponent();
            LoadComboBox();
        }

        private void LoadComboBox()
        {
            // Load lớp học
            string queryLH = "SELECT MaLop, TenLop FROM LopHoc";
            DataTable dtLH = DatabaseHelper.ExecuteQuery(queryLH);
            cbLopHoc.DataSource = dtLH;
            cbLopHoc.DisplayMember = "TenLop";
            cbLopHoc.ValueMember = "MaLop";

            // Load học kỳ
            cbHocKy.Items.Add("1");
            cbHocKy.Items.Add("2");
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            if (cbLopHoc.SelectedValue == null || cbHocKy.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn lớp và học kỳ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"SELECT hs.MaHocSinh, hs.HoTen, mh.TenMonHoc, d.DiemTB 
                           FROM Diem d
                           JOIN HocSinh hs ON d.MaHocSinh = hs.MaHocSinh
                           JOIN MonHoc mh ON d.MaMonHoc = mh.MaMonHoc
                           WHERE hs.MaLop = @MaLop AND d.HocKy = @HocKy";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaLop", cbLopHoc.SelectedValue),
                new SqlParameter("@HocKy", cbHocKy.SelectedItem)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvThongKe.DataSource = dt;

            // Tính điểm trung bình lớp
            if (dt.Rows.Count > 0)
            {
                decimal tongDiem = 0;
                foreach (DataRow row in dt.Rows)
                {
                    tongDiem += Convert.ToDecimal(row["DiemTB"]);
                }
                decimal diemTBLop = tongDiem / dt.Rows.Count;
                lblDiemTBLop.Text = $"Điểm TB lớp: {diemTBLop.ToString("0.00")}";
            }
            else
            {
                lblDiemTBLop.Text = "Điểm TB lớp: N/A";
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}