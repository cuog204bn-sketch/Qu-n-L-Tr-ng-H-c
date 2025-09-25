using System;
using System.Data;
using System.Windows.Forms;

namespace qlytruonghoc
{
    public partial class frmXemThoiKhoaBieu : Form
    {
        public frmXemThoiKhoaBieu(DataTable data, string tenLop)
        {
            InitializeComponent();
            Text = "Thời khóa biểu lớp " + tenLop;
            HienThiThoiKhoaBieu(data);
        }

        private void HienThiThoiKhoaBieu(DataTable data)
        {
            // Tạo bảng 10 tiết x 6 thứ (Thứ 2 - Thứ 7)
            DataTable tkbTable = new DataTable();

            // Thêm cột tiết
            tkbTable.Columns.Add("Tiết");

            // Thêm cột các thứ
            for (int thu = 2; thu <= 7; thu++)
            {
                tkbTable.Columns.Add("Thứ " + thu);
            }

            // Thêm dữ liệu các tiết
            for (int tiet = 1; tiet <= 10; tiet++)
            {
                DataRow row = tkbTable.NewRow();
                row["Tiết"] = "Tiết " + tiet;

                for (int thu = 2; thu <= 7; thu++)
                {
                    DataRow[] rows = data.Select($"Thu = 'Thứ {thu}' AND Tiet = {tiet}");
                    if (rows.Length > 0)
                    {
                        row["Thứ " + thu] = $"{rows[0]["TenMonHoc"]}\nGV: {rows[0]["TenGiaoVien"]}\nPhòng: {rows[0]["PhongHoc"]}";
                    }
                    else
                    {
                        row["Thứ " + thu] = "Trống";
                    }
                }

                tkbTable.Rows.Add(row);
            }

            dgvTKB.DataSource = tkbTable;

            // Định dạng DataGridView
            dgvTKB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTKB.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTKB.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTKB.RowHeadersVisible = false;

            // Đặt chiều cao hàng tự động
            dgvTKB.RowTemplate.Height = 60;
        }

        private void dgvTKB_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}