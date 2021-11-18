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
    public partial class FormChiTietHoaDon : Form
    {
        private int maHoaDon;
        DataProvider dataProvider = new DataProvider();

        private int maSach;
        private string tenSach;
        private int soLuongSachHienTai;

        public FormChiTietHoaDon(int maHoaDon)
        {
            InitializeComponent();
            this.maHoaDon = maHoaDon;  
            init();
        }

        private void init()
        {
            title.Text = "Chi Tiết Hóa Đơn " + maHoaDon;
            loadDgHoaDon();
            loadTongTien();
            loadCbSach();
        }

        private void loadDgHoaDon()
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder("SELECT tbl_sach.ten_sach as [Tên Sách]");
            query.Append(",tbl_chi_tiet_hoa_don.so_luong as [Số Lượng]");
            query.Append(",tbl_sach.gia_ban as [Giá Bán]");
            query.Append(",tbl_chi_tiet_hoa_don.so_luong * tbl_sach.gia_ban as [Thành Tiền]");

            query.Append(" FROM tbl_sach, tbl_chi_tiet_hoa_don");
            query.Append(" WHERE tbl_sach.ma_sach = tbl_chi_tiet_hoa_don.ma_sach");
            query.Append(" AND ma_hoa_don = " + maHoaDon);

            dt = dataProvider.execQuery(query.ToString());

            dgHoaDon.DataSource = dt;
            dgHoaDon.ClearSelection();
        }

        private void loadTongTien()
        {
            if ((int)dataProvider.execScaler("SELECT COUNT(*) FROM tbl_chi_tiet_hoa_don WHERE ma_hoa_don = " + maHoaDon) > 0)
            {
                StringBuilder query = new StringBuilder("SELECT SUM(tbl_chi_tiet_hoa_don.so_luong * tbl_sach.gia_ban)");
                query.Append(" FROM tbl_sach, tbl_chi_tiet_hoa_don");
                query.Append(" WHERE tbl_sach.ma_sach = tbl_chi_tiet_hoa_don.ma_sach");
                query.Append(" AND tbl_chi_tiet_hoa_don.ma_hoa_don = " + maHoaDon);
                double tongTien = (double)dataProvider.execScaler(query.ToString());
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

        private void dgHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowId = e.RowIndex;

            if (rowId < dgHoaDon.RowCount - 1 && rowId >= 0)
            {
                DataGridViewRow row = dgHoaDon.Rows[rowId];

                tenSach = row.Cells[0].Value.ToString();
                cbSach.Text = tenSach;
                soLuongSachHienTai = (int)row.Cells[1].Value;
                numSoLuongSach.Value = soLuongSachHienTai;

                maSach = (int)dataProvider.execScaler("SELECT ma_sach FROM tbl_sach WHERE ten_sach = N'" + tenSach + "'");
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            int dem = (int)dataProvider.execScaler("SELECT so_luong FROM tbl_sach WHERE ma_sach = " + maSach);
            if (dem < numSoLuongSach.Value)
            {
                MessageBox.Show("Số lượng sách trong cửa hàng không đủ để mua ( Còn lại: "+dem+") !" , "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                dem = (int)dataProvider.execScaler("Select COUNT(*) FROM tbl_chi_tiet_hoa_don WHERE ma_hoa_don = " + maHoaDon + "AND ma_sach = " + maSach);
                if (dem == 0)
                {
                    StringBuilder query = new StringBuilder("EXEC proc_them_chi_tiet_hoa_don");
                    query.Append(" @maHoaDon = " + maHoaDon);
                    query.Append(",@maSach = " + maSach);
                    query.Append(",@soLuong = " + numSoLuongSach.Value);

                    int result = dataProvider.execNonQuery(query.ToString());

                    if (result > 0)
                    {
                        loadDgHoaDon();
                        loadTongTien();

                        //update sach trong tabsach
                        query = new StringBuilder("UPDATE tbl_sach SET so_luong = so_luong - " + numSoLuongSach.Value);
                        query.Append(" WHERE ma_sach = " + maSach);
                        dataProvider.execNonQuery(query.ToString());

                        MessageBox.Show("Thêm sách vào hóa đơn thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                    {
                        MessageBox.Show("Thêm sách vào hóa đơn không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    dem = (int)dataProvider.execScaler("Select SUM(so_luong) FROM tbl_chi_tiet_hoa_don WHERE ma_hoa_don = " + maHoaDon + "AND ma_sach = " + maSach);
                    update(dem);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            int dem = (int)dataProvider.execScaler("SELECT so_luong FROM tbl_sach WHERE ma_sach = " + maSach);
            if (dem < numSoLuongSach.Value - soLuongSachHienTai)
            {
                MessageBox.Show("Số lượng sách trong cửa hàng không đủ để mua ( Còn lại: " + dem + ") !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else update(0);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Bạn có chắc chắn muốn xóa sách " + tenSach + " ?", "Cảnh Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (check == DialogResult.Yes)
            {
                StringBuilder query = new StringBuilder("DELETE FROM tbl_chi_tiet_hoa_don WHERE ma_hoa_don = " + maHoaDon + "AND ma_sach = " + maSach);
                int result = dataProvider.execNonQuery(query.ToString());

                if (result > 0)
                {
                    loadDgHoaDon();
                    loadTongTien();

                    query = new StringBuilder("UPDATE tbl_sach SET so_luong = so_luong + " + soLuongSachHienTai);
                    query.Append(" WHERE ma_sach = " + maSach);
                    dataProvider.execNonQuery(query.ToString());

                    MessageBox.Show("Xóa sách khỏi hóa đơn thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Xóa sách khỏi hóa đơn không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void update(int soLuong)
        {
            StringBuilder query = new StringBuilder("EXEC proc_cap_nhat_chi_tiet_hoa_don");
            query.Append(" @maHoaDon = " + maHoaDon);
            query.Append(",@maSach = " + maSach);
            query.Append(",@soLuong = " + (numSoLuongSach.Value + soLuong));

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgHoaDon();
                loadTongTien();

                if (soLuong > 0)
                {
                    query = new StringBuilder("UPDATE tbl_sach SET so_luong = so_luong - " + numSoLuongSach.Value);
                    query.Append(" WHERE ma_sach = " + maSach);
                    dataProvider.execNonQuery(query.ToString());
                }else
                {
                    query = new StringBuilder("UPDATE tbl_sach SET so_luong = so_luong - " + (numSoLuongSach.Value - soLuongSachHienTai));
                    query.Append(" WHERE ma_sach = " + maSach);
                    dataProvider.execNonQuery(query.ToString());
                }
                

                MessageBox.Show("Cập nhật sách vào hóa đơn thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Cập nhật sách vào hóa đơn không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = dataProvider.execQuery("EXEC proc_in_chi_tiet_hoa_don @maHoaDon = " + maHoaDon);

            ReportHoaDon report = new ReportHoaDon();
            report.SetDataSource(dt);

            FormInReports form = new FormInReports(report);
            form.ShowDialog();
        }


    }
}
