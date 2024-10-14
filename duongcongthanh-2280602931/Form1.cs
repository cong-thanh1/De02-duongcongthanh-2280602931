using duongcongthanh_2280602931.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace duongcongthanh_2280602931
{
    public partial class frmSampham : Form
    {
        Model1 context = new Model1();

        public frmSampham()
        {
            InitializeComponent();
        }

        private void frmSampham_Load(object sender, EventArgs e)
        {
            LoadSanPham();
            LoadLoaiSP();
            txtID.Clear();
            txtName.Clear();
            dateNgayNhap.Value = DateTime.Now;
            cmbLoai.SelectedIndex = -1;
            txtTim.Clear();
        }
        private void LoadSanPham()
        {
            dgvSanPham.Rows.Clear();
            var sanphams = context.Sanphams.ToList();
            foreach (var sp in sanphams)
            {
                var loaiSP = context.LoaiSPs.FirstOrDefault(l => l.MaLoai == sp.MaLoai);
                dgvSanPham.Rows.Add(sp.MaSP, sp.TenSP, sp.NgayNhap, loaiSP?.TenLoai);
            }
        }
        private void LoadLoaiSP()
        {
            var loaiSPs = context.LoaiSPs.ToList();
            cmbLoai.DataSource = loaiSPs;
            cmbLoai.DisplayMember = "TenLoai";
            cmbLoai.ValueMember = "MaLoai";
            var defaultItem = loaiSPs.FirstOrDefault(l => l.TenLoai == "Giải khát");
        }
        private void ResetControls()
        {
            txtID.Clear();
            txtName.Clear();
            dateNgayNhap.Value = DateTime.Now;
            cmbLoai.SelectedIndex = -1;
        }
        private void dgvSanPham_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dgvSanPham.Rows[e.RowIndex];
                txtID.Text = selectedRow.Cells[0].Value.ToString();
                txtName.Text = selectedRow.Cells[1].Value.ToString();
                dateNgayNhap.Value = Convert.ToDateTime(selectedRow.Cells[2].Value);
                var tenLoai = selectedRow.Cells[3].Value.ToString();
                var loaiSP = context.LoaiSPs.FirstOrDefault(l => l.TenLoai == tenLoai);
                if (loaiSP != null)
                {
                    cmbLoai.SelectedValue = loaiSP.MaLoai;
                }
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtID.Text))
                {
                    MessageBox.Show("Vui lòng nhập Mã sản phẩm.");
                    return;
                }

                if (cmbLoai.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn loại sản phẩm.");
                    return;
                }

                var selectedLoai = (LoaiSP)cmbLoai.SelectedItem;
                var existingProduct = context.Sanphams.FirstOrDefault(sp => sp.MaSP == txtID.Text.Trim());
                if (existingProduct != null)
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại. Vui lòng nhập mã sản phẩm khác.");
                    return;
                }

                var sanpham = new Sanpham
                {
                    MaSP = txtID.Text.Trim(),
                    TenSP = txtName.Text.Trim(),
                    NgayNhap = dateNgayNhap.Value,
                    MaLoai = selectedLoai.MaLoai
                };

                context.Sanphams.Add(sanpham);
                context.SaveChanges();

                LoadSanPham();
                ResetControls();

                MessageBox.Show("Thêm sản phẩm thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


        private void btnTim_Click(object sender, EventArgs e)
        {

            try
            {
                var keyword = txtTim.Text.ToLower();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    LoadSanPham();
                    return;
                }

                var sanphams = context.Sanphams
                    .Where(sp => sp.TenSP.ToLower().Contains(keyword))
                    .ToList();

                dgvSanPham.Rows.Clear();
                foreach (var sp in sanphams)
                {
                    var loaiSP = context.LoaiSPs.FirstOrDefault(l => l.MaLoai == sp.MaLoai);
                    dgvSanPham.Rows.Add(sp.MaSP, sp.TenSP, sp.NgayNhap, loaiSP?.TenLoai);
                }

                if (sanphams.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedMaSP = txtID.Text.Trim();
                var sanpham = context.Sanphams.FirstOrDefault(sp => sp.MaSP == selectedMaSP);

                if (sanpham != null)
                {
                    var oldMaSP = sanpham.MaSP;

                    sanpham.MaSP = txtID.Text;
                    sanpham.TenSP = txtName.Text;
                    sanpham.NgayNhap = dateNgayNhap.Value;
                    sanpham.MaLoai = ((LoaiSP)cmbLoai.SelectedItem).MaLoai;

                    context.SaveChanges();
                    foreach (DataGridViewRow row in dgvSanPham.Rows)
                    {
                        if (row.Cells[0].Value.ToString() == oldMaSP)
                        {
                            row.Cells[0].Value = sanpham.MaSP;
                            row.Cells[1].Value = sanpham.TenSP;
                            row.Cells[2].Value = sanpham.NgayNhap;
                            row.Cells[3].Value = ((LoaiSP)cmbLoai.SelectedItem).TenLoai;
                            break;
                        }
                    }

                    MessageBox.Show("Sửa sản phẩm thành công!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm để sửa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedMaSP = txtID.Text.Trim();
                if (string.IsNullOrEmpty(selectedMaSP))
                {
                    MessageBox.Show("Vui lòng nhập mã sản phẩm để xóa.");
                    return;
                }

                var sanpham = context.Sanphams.FirstOrDefault(sp => sp.MaSP == selectedMaSP);

                if (sanpham != null)
                {
                    var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?",
                                                        "Xác nhận xóa",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Warning);
                    if (confirmResult == DialogResult.Yes)
                    {
                        context.Sanphams.Remove(sanpham);
                        context.SaveChanges();
                        MessageBox.Show("Xóa sản phẩm thành công!");
                        LoadSanPham();
                        ResetControls(); 
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm để xóa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát không?",
                                         "Xác nhận thoát",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
           try
            {
                var selectedLoai = (LoaiSP)cmbLoai.SelectedItem;
                var sanpham = new Sanpham
                {
                    MaSP = txtID.Text,
                    TenSP = txtName.Text,
                    NgayNhap = dateNgayNhap.Value,
                    MaLoai = selectedLoai.MaLoai
                };
                context.Sanphams.Add(sanpham);
                context.SaveChanges();
                dgvSanPham.Rows.Add(sanpham.MaSP, sanpham.TenSP, sanpham.NgayNhap, selectedLoai.TenLoai);

                MessageBox.Show("Thêm sản phẩm thành công!");
                btnLuu.Enabled = false;
                btnNotLuu.Enabled = false;
            }catch (Exception ex)
    {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnNotLuu_Click(object sender, EventArgs e)
        {
            ResetControls();
            btnLuu.Enabled = false;
            btnNotLuu.Enabled = false;

            MessageBox.Show("Thêm sản phẩm đã bị hủy.");
        }
    }
}
