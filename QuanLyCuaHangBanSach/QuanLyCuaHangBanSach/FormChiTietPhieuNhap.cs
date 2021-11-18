using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCuaHangBanSach
{
    public partial class FormChiTietPhieuNhap : Form
    {
        DataProvider dataProvider = new DataProvider();
        private int maSach;
        private int maPhieuNhap;
        private string tenSach;

        //so luong sach co trong phieu nhap
        private int soLuongSachHienTai;

        public FormChiTietPhieuNhap(int maPhieuNhap)
        {
            InitializeComponent();
            this.maPhieuNhap = maPhieuNhap;
            init();
        }

        private void init()
        {
            title.Text = "Chi Tiết Phiếu Nhập " + maPhieuNhap;
            loadDgPhieuNhap();
            loadCbSach();
            loadTongTien();
        }

        private void loadDgPhieuNhap()
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder("SELECT tbl_sach.ten_sach as [Tên Sách]");
            query.Append(",tbl_chi_tiet_phieu_nhap.so_luong as [Số Lượng]");
            query.Append(",tbl_chi_tiet_phieu_nhap.gia_nhap as [Giá Nhập]");
            query.Append(",tbl_chi_tiet_phieu_nhap.gia_nhap * tbl_chi_tiet_phieu_nhap.so_luong as [Thành Tiền]");

            query.Append(" FROM tbl_sach, tbl_chi_tiet_phieu_nhap");
            query.Append(" WHERE tbl_sach.ma_sach = tbl_chi_tiet_phieu_nhap.ma_sach");
            query.Append(" AND ma_phieu_nhap = " + maPhieuNhap);

            dt = dataProvider.execQuery(query.ToString());

            dgPhieuNhap.DataSource = dt;
        }

        private void loadTongTien()
        {
            if ((int)dataProvider.execScaler("SELECT COUNT(*) FROM tbl_chi_tiet_phieu_nhap WHERE ma_phieu_nhap = " + maPhieuNhap) > 0)
            {
                double tongTien = (double)dataProvider.execScaler("SELECT SUM(so_luong * gia_nhap) from tbl_chi_tiet_phieu_nhap WHERE ma_phieu_nhap = " + maPhieuNhap);
                txtTongTien.Text = "Tổng Tiền: " + tongTien;
            }
        }

        private void loadCbSach()
        {

            DataTable dt = new DataTable();

            dt = dataProvider.execQuery("SELECT * FROM tbl_sach");

            cbSach.DisplayMember = "ten_sach";
            cbSach.ValueMember = "ma_sach";

            cbSach.DataSource = dt;
        }

        private void cbSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            maSach = (int)comboBox.SelectedValue;
            tenSach = comboBox.Text;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            int dem = (int)dataProvider.execScaler("Select COUNT(*) FROM tbl_chi_tiet_phieu_nhap WHERE ma_phieu_nhap = " + maPhieuNhap + "AND ma_sach = " + maSach);
            if (dem == 0)
            {
                StringBuilder query = new StringBuilder("EXEC proc_them_chi_tiet_phieu_nhap");
                query.Append(" @maPhieuNhap = " + maPhieuNhap);
                query.Append(",@maSach = " + maSach);
                query.Append(",@soLuong = " + numSoLuongSach.Value);
                query.Append(",@giaNhap = " + numGiaNhapSach.Value);

                int result = dataProvider.execNonQuery(query.ToString());

                if (result > 0)
                {
                    loadDgPhieuNhap();
                    loadTongTien();

                    query = new StringBuilder("UPDATE tbl_sach SET so_luong = so_luong + " + numSoLuongSach.Value);
                    query.Append(" WHERE ma_sach = " + maSach);
                    dataProvider.execNonQuery(query.ToString());

                    MessageBox.Show("Thêm sách vào phiếu nhập thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Thêm sách vào phiếu nhập không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }else
            {
                dem = (int)dataProvider.execScaler("Select SUM(so_luong) FROM tbl_chi_tiet_phieu_nhap WHERE ma_phieu_nhap = " + maPhieuNhap + "AND ma_sach = " + maSach);
                update(dem);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            update(0);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Bạn có chắc chắn muốn xóa sách " + tenSach +" ?", "Cảnh Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (check == DialogResult.Yes)
            {
                StringBuilder query = new StringBuilder("DELETE FROM tbl_chi_tiet_phieu_nhap WHERE ma_phieu_nhap = " + maPhieuNhap + "AND ma_sach = " + maSach);
                int result = dataProvider.execNonQuery(query.ToString());

                if (result > 0)
                {
                    loadDgPhieuNhap();
                    loadTongTien();

                    query = new StringBuilder("UPDATE tbl_sach SET so_luong = so_luong - " + soLuongSachHienTai);
                    query.Append(" WHERE ma_sach = " + maSach);
                    dataProvider.execNonQuery(query.ToString());

                    MessageBox.Show("Xóa sách khỏi phiếu nhập thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Xóa sách khỏi phiều nhập không thành công thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void update(int soLuong)
        {
            StringBuilder query = new StringBuilder("EXEC proc_cap_nhat_chi_tiet_phieu_nhap");
            query.Append(" @maPhieuNhap = " + maPhieuNhap);
            query.Append(",@maSach = " + maSach);
            query.Append(",@soLuong = " + (numSoLuongSach.Value + soLuong));
            query.Append(",@giaNhap = " + numGiaNhapSach.Value);

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgPhieuNhap();
                loadTongTien();

                if (soLuong > 0)
                {
                    query = new StringBuilder("UPDATE tbl_sach SET so_luong = so_luong + " + numSoLuongSach.Value);
                    query.Append(" WHERE ma_sach = " + maSach);
                    dataProvider.execNonQuery(query.ToString());
                }
                else
                {
                    query = new StringBuilder("UPDATE tbl_sach SET so_luong = so_luong + " + (numSoLuongSach.Value - soLuongSachHienTai));
                    query.Append(" WHERE ma_sach = " + maSach);
                    dataProvider.execNonQuery(query.ToString());
                }

                MessageBox.Show("Cập nhật sách trong phiếu nhập thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Cập nhật sách trong  phiếu nhập không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dgPhieuNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowId = e.RowIndex;

            if (rowId < dgPhieuNhap.RowCount - 1 && rowId >= 0)
            {
                DataGridViewRow row = dgPhieuNhap.Rows[rowId];

                tenSach = row.Cells[0].Value.ToString();
                cbSach.Text = tenSach;

                soLuongSachHienTai = (int)row.Cells[1].Value;
                numSoLuongSach.Value = soLuongSachHienTai;
                numGiaNhapSach.Value = Convert.ToInt32(row.Cells[2].Value);

                maSach = (int)dataProvider.execScaler("SELECT ma_sach FROM tbl_sach WHERE ten_sach = N'" + tenSach + "'");
            }
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = dataProvider.execQuery("EXEC proc_in_chi_tiet_phieu_nhap @maPhieuNhap = " + maPhieuNhap);

            ReportPhieuNhap report = new ReportPhieuNhap();
            report.SetDataSource(dt);

            FormInReports form = new FormInReports(report);
            form.ShowDialog();
        }
    }
}
