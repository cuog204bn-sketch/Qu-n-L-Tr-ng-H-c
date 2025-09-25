using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace qlytruonghoc
{
    public partial class frmBaoCaoHocKy : Form
    {
        private DataTable dataBaoCao;

        public frmBaoCaoHocKy()
        {
            InitializeComponent();
            LoadComboBox();
        }

        private void LoadComboBox()
        {
            cbHocKy.Items.Add("1");
            cbHocKy.Items.Add("2");
        }

        private void btnXemTruoc_Click(object sender, EventArgs e)
        {
            if (cbHocKy.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn học kỳ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"SELECT l.MaLop, l.TenLop, 
                          COUNT(h.MaHocSinh) AS SiSo,
                          AVG(d.DiemTB) AS DiemTBLop,
                          SUM(CASE WHEN d.DiemTB >= 5 THEN 1 ELSE 0 END) AS SoDat,
                          SUM(CASE WHEN d.DiemTB < 5 THEN 1 ELSE 0 END) AS SoKhongDat
                          FROM LopHoc l
                          LEFT JOIN HocSinh h ON l.MaLop = h.MaLop
                          LEFT JOIN Diem d ON h.MaHocSinh = d.MaHocSinh AND d.HocKy = @HocKy
                          GROUP BY l.MaLop, l.TenLop";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HocKy", cbHocKy.SelectedItem)
            };

            dataBaoCao = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvBaoCao.DataSource = dataBaoCao;

            CalculateAndDisplaySummary(dataBaoCao);
        }

        private void CalculateAndDisplaySummary(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                int tongSiSo = 0;
                int tongDat = 0;
                int tongKhongDat = 0;
                decimal tongDiemTB = 0;

                foreach (DataRow row in dt.Rows)
                {
                    tongSiSo += Convert.ToInt32(row["SiSo"]);
                    tongDat += Convert.ToInt32(row["SoDat"]);
                    tongKhongDat += Convert.ToInt32(row["SoKhongDat"]);
                    if (row["DiemTBLop"] != DBNull.Value)
                    {
                        tongDiemTB += Convert.ToDecimal(row["DiemTBLop"]);
                    }
                }

                decimal diemTBToanTruong = tongSiSo > 0 ? tongDiemTB / dt.Rows.Count : 0;
                decimal tyLeDat = tongSiSo > 0 ? (decimal)tongDat / tongSiSo * 100 : 0;

                lblTongSiSo.Text = $"Tổng sĩ số: {tongSiSo} HS";
                lblTongDat.Text = $"Tổng đạt: {tongDat} HS";
                lblTongKhongDat.Text = $"Tổng không đạt: {tongKhongDat} HS";
                lblDiemTBToanTruong.Text = $"Điểm TB toàn trường: {diemTBToanTruong.ToString("0.00")}";
                lblTyLeDat.Text = $"Tỷ lệ đạt: {tyLeDat.ToString("0.00")}%";
            }
        }

        private void btnXuatBaoCao_Click(object sender, EventArgs e)
        {
            if (dataBaoCao == null || dataBaoCao.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất báo cáo. Vui lòng xem trước trước khi xuất!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                excelApp.Visible = true;

                // Đặt tiêu đề
                worksheet.Name = "Báo cáo học kỳ " + cbHocKy.SelectedItem;

                // Tiêu đề báo cáo
                Excel.Range header = worksheet.Range["A1", "F1"];
                header.Merge();
                header.Value = "BÁO CÁO TỔNG KẾT HỌC KỲ " + cbHocKy.SelectedItem;
                header.Font.Bold = true;
                header.Font.Size = 16;
                header.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                // Xuất tiêu đề cột
                for (int i = 0; i < dataBaoCao.Columns.Count; i++)
                {
                    worksheet.Cells[3, i + 1] = dataBaoCao.Columns[i].ColumnName;
                }

                // Xuất dữ liệu
                for (int i = 0; i < dataBaoCao.Rows.Count; i++)
                {
                    for (int j = 0; j < dataBaoCao.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 4, j + 1] = dataBaoCao.Rows[i][j].ToString();
                    }
                }

                // Định dạng tiêu đề cột
                Excel.Range columnHeaders = worksheet.Range["A3", "F3"];
                columnHeaders.Font.Bold = true;
                columnHeaders.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                // Xuất thống kê tổng hợp
                int lastRow = dataBaoCao.Rows.Count + 5;
                worksheet.Cells[lastRow, 1] = "TỔNG HỢP TOÀN TRƯỜNG";
                worksheet.Range[worksheet.Cells[lastRow, 1], worksheet.Cells[lastRow, 6]].Merge();
                worksheet.Cells[lastRow, 1].Font.Bold = true;

                worksheet.Cells[lastRow + 1, 1] = lblTongSiSo.Text;
                worksheet.Cells[lastRow + 2, 1] = lblTongDat.Text;
                worksheet.Cells[lastRow + 3, 1] = lblTongKhongDat.Text;
                worksheet.Cells[lastRow + 4, 1] = lblDiemTBToanTruong.Text;
                worksheet.Cells[lastRow + 5, 1] = lblTyLeDat.Text;

                // Tự động điều chỉnh độ rộng cột
                worksheet.Columns.AutoFit();

                // Lưu file Excel
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Lưu báo cáo";
                saveFileDialog.FileName = "BaoCaoHocKy_" + cbHocKy.SelectedItem + ".xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show("Xuất báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Đóng workbook và thoát Excel
                workbook.Close(false);
                excelApp.Quit();

                // Giải phóng COM objects
                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}