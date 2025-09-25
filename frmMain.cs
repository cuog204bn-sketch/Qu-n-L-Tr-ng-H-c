using qlytruonghoc;
using System;
using System.Data.SqlTypes;
using System.Windows.Forms;


namespace qlytruonghoc
{
    public partial class frmMain : Form
    {
        private string userRole;

        public frmMain(string role)
        {
            InitializeComponent();
            userRole = role;
            SetupMenuByRole();
            lblWelcome.Text = $"Xin chào, {role}";

            // Khởi tạo timer
            timer1.Interval = 1000; // 1 giây
            timer1.Start();
        }

        private void SetupMenuByRole()
        {
            if (userRole == "GiaoVien")
            {
                quảnLýGiáoViênToolStripMenuItem.Enabled = false;
                quảnLýNgườiDùngToolStripMenuItem.Enabled = false;
                quảnLýKhoiToolStripMenuItem.Enabled = false; // Thêm dòng này
                quảnLýLoạiCSVCToolStripMenuItem.Enabled = false;
                quảnLýCSVCToolStripMenuItem.Enabled = false;
                thốngKêBáoCáoToolStripMenuItem.Enabled = false;
                quảnLýThờiKhóaBiểuToolStripMenuItem.Enabled = false;
            }
            else if (userRole == "HocSinh")
            {
                quảnLýHọcSinhToolStripMenuItem.Enabled = false;
                quảnLýGiáoViênToolStripMenuItem.Enabled = false;
                quảnLýLớpToolStripMenuItem.Enabled = false;
                quảnLýMônHọcToolStripMenuItem.Enabled = false;
                quảnLýPhânCôngToolStripMenuItem.Enabled = false;
                quảnLýKhoiToolStripMenuItem.Enabled = false;
                quảnLýLoạiCSVCToolStripMenuItem.Enabled = false;
                quảnLýCSVCToolStripMenuItem.Enabled = false;
                quảnLýNgườiDùngToolStripMenuItem.Enabled = false;
                quảnLýThờiKhóaBiểuToolStripMenuItem.Enabled = false;
                thốngKêBáoCáoToolStripMenuItem.Enabled = false;
            }
        }
       

        private void quảnLýHọcSinhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyHocSinh());
        }

        private void quảnLýGiáoViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyGiaoVien());
        }

        private void quảnLýLớpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyLop());
        }

        private void quảnLýMônHọcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyMonHoc());
        }

        private void quảnLýĐiểmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyDiem());
        }

        private void quảnLýPhânCôngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmPhanCongGiangDay());
        }
        private void quảnLýKhoiToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyKhoi());
        }

        private void thốngKêĐiểmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmThongKeDiemTheoLop());
        }

        private void báoCáoHọcKỳToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmBaoCaoHocKy());
        }

        private void quảnLýKhenThưởngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyKhenThuong());
        }

        private void quảnLýNgườiDùngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyNguoiDung());
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmDangNhap frm = new frmDangNhap();
            frm.Show();
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OpenChildForm(Form childForm)
        {
            foreach (Form form in this.MdiChildren)
            {
                form.Close();
            }
            childForm.MdiParent = this;
            childForm.Dock = DockStyle.Fill;
            childForm.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void quảnLýToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void quảnLýKhoiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyKhoi());
        }

        private void quảnLýCSVCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyCoSoVatChat());
        }
        private void quảnLýLoạiCSVCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyLoaiCSVC());
        }
        private void quảnLýCSVCToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyKhoi());
        }

        private void panelSidebar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void quảnLýThờiKhóaBiểuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyThoiKhoaBieu());
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnQuanLyThoiKhoaBieu_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQuanLyThoiKhoaBieu());
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}