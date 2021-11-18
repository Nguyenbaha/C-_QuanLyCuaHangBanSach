using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCuaHangBanSach
{
    public partial class FormMain : Form
    {
        private DataProvider dataProvider = new DataProvider();
        private int maSachLoaiSach;
        private int maSachSach;
        private int maLoaiSachLoaiSach;
        private int maHoaDonHoaDon;
        private int maPhieuNhapPhieuNhap;

        public FormMain()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            initSach();
            initLoaiSach();
            initHoaDon();
            initPhieuNhap();
        }

        //Xu ly sach
        private void initSach()
        {
            loadDgSach();
            loadcbSachLoaiSach();
        }

        private void loadDgSach()
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder("SELECT ma_sach as [Mã Sách]");
            query.Append(",ten_sach as [Tên Sách]");
           // query.Append(",ten_loai_sach as [Loại Sách]");
            query.Append(",tac_gia as [Tác Giả]");
            query.Append(",so_luong as [Số Lượng]");
            query.Append(",gia_ban as [Giá Bán]");
            query.Append(" FROM tbl_sach, tbl_loai_sach");
            query.Append(" WHERE tbl_sach.ma_loai_sach = tbl_loai_sach.ma_loai_sach;");

            dt = dataProvider.execQuery(query.ToString());

            dgSach.DataSource = dt;

            if (dt.Rows.Count > 0) maSachSach = (int)dt.Rows[0][0];
        }

        private void loadcbSachLoaiSach()
        {
            DataTable dt = new DataTable();

            dt = dataProvider.execQuery("SELECT * FROM tbl_loai_sach");

            cbSachLoaiSach.DisplayMember = "ten_loai_sach";
            cbSachLoaiSach.ValueMember = "ma_loai_sach";

            cbSachLoaiSach.DataSource = dt;
        }

        private void dgSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowId = e.RowIndex;
            if (rowId < dgSach.RowCount - 1 && rowId >= 0)
            {
                DataGridViewRow row = dgSach.Rows[rowId];

                maSachSach = (int)row.Cells[0].Value;
                txtSachTenSach.Text = row.Cells[1].Value.ToString();
                cbSachLoaiSach.Text = row.Cells[2].Value.ToString();
                txtSachTacGia.Text = row.Cells[3].Value.ToString();
                numSachSoLuong.Value = (int)row.Cells[4].Value;
                numSachGiaBan.Value = Convert.ToInt32(row.Cells[5].Value);

                maSachLoaiSach = (int)dataProvider.execScaler("SELECT ma_loai_sach FROM tbl_loai_sach WHERE ten_loai_sach = N'" + cbSachLoaiSach.Text + "'");
            }   
        }

        private void btnSachThem_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder("EXEC proc_them_sach");
            query.Append(" @tenSach = N'" + txtSachTenSach.Text +"'");
            query.Append(",@maLoaiSach = " + maSachLoaiSach);
            query.Append(",@tacGia = N'" + txtSachTacGia.Text + "'");
            query.Append(",@soLuong = " + numSachSoLuong.Value);
            query.Append(",@giaBan = " + numSachGiaBan.Value);

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgSach();
                MessageBox.Show("Thêm sách thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Thêm sách không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSachSua_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder("EXEC proc_cap_nhat_sach");
            query.Append(" @maSach = " + maSachSach);
            query.Append(",@tenSach = N'" + txtSachTenSach.Text + "'");
            query.Append(",@maLoaiSach = " + maSachLoaiSach);
            query.Append(",@tacGia = N'" + txtSachTacGia.Text + "'");
            query.Append(",@soLuong = " + numSachSoLuong.Value);
            query.Append(",@giaBan = " + numSachGiaBan.Value);

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgSach();
                MessageBox.Show("Cập nhật sách thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Cập nhật sách không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSachXoa_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Bạn có chắc chắn muốn xóa sách " + txtSachTenSach.Text + " ?", "Cảnh Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (check == DialogResult.Yes)
            {
                string query = "DELETE FROM tbl_sach WHERE ma_sach = " + maSachSach;
                int result = dataProvider.execNonQuery(query.ToString());

                if (result > 0)
                {
                    loadDgSach();
                    MessageBox.Show("Xóa sách thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Xóa sách không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cbSachLoaiSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            maSachLoaiSach = (int)comboBox.SelectedValue;
        }

        //Xu ly loai sach
        private void initLoaiSach()
        {
            loadDgLoaiSach();
        }

        private void loadDgLoaiSach()
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder("SELECT ma_loai_sach as [Mã Loại Sách]");
            query.Append(",ten_loai_sach as [Tên Loại Sách]");
            query.Append(" FROM tbl_loai_sach");

            dt = dataProvider.execQuery(query.ToString());

            dgLoaiSach.DataSource = dt;

            if (dt.Rows.Count > 0) maLoaiSachLoaiSach = (int)dt.Rows[0][0];
        }

        private void dgLoaiSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowId = e.RowIndex;
            if (rowId < dgLoaiSach.RowCount - 1 && rowId >= 0)
            {
                DataGridViewRow row = dgLoaiSach.Rows[rowId];

                maLoaiSachLoaiSach = (int)row.Cells[0].Value;
                txtLoaiSachTenLoaiSach.Text = row.Cells[1].Value.ToString();
            }               
        }

        private void btnLoaiSachThem_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder("EXEC proc_them_loai_sach");
            query.Append(" @tenLoaiSach = N'" + txtLoaiSachTenLoaiSach.Text + "'");

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgLoaiSach();
                loadcbSachLoaiSach();
                MessageBox.Show("Thêm loại sách thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Thêm loại sách không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoaiSachSua_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder("EXEC proc_cap_nhat_loai_sach");
            query.Append(" @tenLoaiSach = N'" + txtLoaiSachTenLoaiSach.Text + "'");
            query.Append(",@maLoaiSach = " + maLoaiSachLoaiSach);

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgLoaiSach();
                loadDgSach();
                loadcbSachLoaiSach();
                MessageBox.Show("Cập nhật loại sách thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Cập nhật loại sách không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnLoaiSachXoa_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Bạn có chắc chắn muốn xóa loại sách " + txtLoaiSachTenLoaiSach.Text + " ?", "Cảnh Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (check == DialogResult.Yes)
            {
                string query = "DELETE FROM tbl_loai_sach WHERE ma_loai_sach = " + maLoaiSachLoaiSach;
                int result = dataProvider.execNonQuery(query.ToString());

                if (result > 0)
                {
                    loadDgLoaiSach();
                    loadcbSachLoaiSach();
                    MessageBox.Show("Xóa loại sách thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Xóa loại sách không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Xu ly hoa don
        private void initHoaDon()
        {
            loadDgHoaDon();
        }

        private void loadDgHoaDon()
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder("SELECT ma_hoa_don as [Mã Hóa Đơn]");
            query.Append(",ngay_lap_hoa_don as [Ngày Lập Hóa Đơn]");
            query.Append(",ten_khach_hang as [Tên Khách Hàng]");
            query.Append(",sdt_khach_hang as [SDT Khách Hàng]");
            query.Append(",( SELECT SUM(tbl_sach.gia_ban * tbl_chi_tiet_hoa_don.so_luong) ");
            query.Append(" FROM tbl_chi_tiet_hoa_don, tbl_sach ");
            query.Append(" WHERE tbl_chi_tiet_hoa_don.ma_sach = tbl_sach.ma_sach");
            query.Append(" AND tbl_chi_tiet_hoa_don.ma_hoa_don = tbl_hoa_don.ma_hoa_don) AS [Thành Tiền]");
            query.Append(" FROM tbl_hoa_don");

            dt = dataProvider.execQuery(query.ToString());

            dgHoaDon.DataSource = dt;

            if (dt.Rows.Count > 0)  maHoaDonHoaDon = (int)dt.Rows[0][0];

            //do du lieu vao phan doanh thu
            double tongTien = 0.0;
            for(int i = 0;i< dt.Rows.Count; i++)
            {
                if (dt.Rows[i][4].ToString() != "")
                    tongTien += (double)dt.Rows[i][4];
            }
            txtTongTienHoaDon.Text = "Tổng Tiền: "+tongTien.ToString();
            txtSoHoaDon.Text = "Số Hóa Đơn: "+dt.Rows.Count.ToString();
        }

        private void dgHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowId = e.RowIndex;
            if (rowId < dgHoaDon.RowCount - 1 && rowId >= 0)
            {
                DataGridViewRow row = dgHoaDon.Rows[rowId];

                maHoaDonHoaDon = (int)row.Cells[0].Value;
                dateNgayLapHoaDon.Value = DateTime.Parse(row.Cells[1].Value.ToString());
                txtHoaDonTenKH.Text = row.Cells[2].Value.ToString();
                txtHoaDonSDTKH.Text = row.Cells[3].Value.ToString();
            }
        }

        private void btnHoaDonThem_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder("EXEC proc_them_hoa_don");
            query.Append(" @ngayLapHoaDon = '" + dateNgayLapHoaDon.Value + "'");
            query.Append(",@tenKhachHang = N'" + txtHoaDonTenKH.Text + "'");
            query.Append(",@sdtKhachHang = '" + txtHoaDonSDTKH.Text + "'");

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgHoaDon();
                MessageBox.Show("Thêm hóa đơn thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Thêm hóa đơn không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnHoaDonSua_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder("EXEC proc_cap_nhat_hoa_don");
            query.Append(" @ngayLapHoaDon = '" + dateNgayLapHoaDon.Value + "'");
            query.Append(",@tenKhachHang = N'" + txtHoaDonTenKH.Text + "'");
            query.Append(",@sdtKhachHang = '" + txtHoaDonSDTKH.Text + "'");
            query.Append(",@maHoaDon = " + maHoaDonHoaDon);

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgHoaDon();
                MessageBox.Show("Cập nhật hóa đơn thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Cập nhật hóa đơn không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnHoaDonXoa_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Bạn có chắc chắn muốn xóa hóa đơn có mã là " + maHoaDonHoaDon + " ?", "Cảnh Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (check == DialogResult.Yes)
            {
                string query = "DELETE FROM tbl_hoa_don WHERE ma_hoa_don = " + maHoaDonHoaDon;
                int result = dataProvider.execNonQuery(query.ToString());

                if (result > 0)
                {
                    loadDgHoaDon();
                    MessageBox.Show("Xóa hóa đơn thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Xóa hóa đơn không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void txtHoaDonSDTKH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        //xu ly phieu nhap
        private void initPhieuNhap()
        {
            loadDgPhieuNhap();
        }

        private void loadDgPhieuNhap()
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder("SELECT ma_phieu_nhap as [Mã Phiếu Nhập]");
            query.Append(", ngay_lap_phieu_nhap as [Ngày Lập Phiếu Nhập]");
            query.Append(",ten_nha_cung_cap as [Tên Nhà Cung Cấp]");
            query.Append(", (SELECT SUM(so_luong * gia_nhap) FROM tbl_chi_tiet_phieu_nhap ");
            query.Append(" WHERE tbl_chi_tiet_phieu_nhap.ma_phieu_nhap = tbl_phieu_nhap.ma_phieu_nhap) as [Thành Tiền]");
            query.Append(" FROM tbl_phieu_nhap");

            dt = dataProvider.execQuery(query.ToString());

            dgPhieuNhap.DataSource = dt;

            if (dt.Rows.Count > 0) maPhieuNhapPhieuNhap = (int)dt.Rows[0][0];
            //do du lieu vao phan phieu nhap
            double tongTien = 0.0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][3].ToString() != "")
                    tongTien += (double)dt.Rows[i][3];
            }
            txtTongTienPhieuNhap.Text = "Tổng Tiền: " + tongTien.ToString();
            txtSoPhieuNhap.Text = "Số Phiếu Nhập: " + dt.Rows.Count.ToString();
        }

        private void dgPhieuNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowId = e.RowIndex;
            if (rowId < dgPhieuNhap.RowCount - 1 && rowId >= 0)
            {
                DataGridViewRow row = dgPhieuNhap.Rows[rowId];
                maPhieuNhapPhieuNhap = (int)row.Cells[0].Value;
                dateNgayLapPhieuNhap.Value = DateTime.Parse(row.Cells[1].Value.ToString());
                txtPhieuNhapNhaCungCap.Text = row.Cells[2].Value.ToString();
            }
        }

        private void btnPhieuNhapThem_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder("EXEC proc_them_phieu_nhap");
            query.Append(" @ngayLapPhieuNhap = '" + dateNgayLapPhieuNhap.Value + "'");
            query.Append(",@tenNhaCungCap = N'" + txtPhieuNhapNhaCungCap.Text + "'");

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgPhieuNhap();
                MessageBox.Show("Thêm phiếu nhập thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Thêm phiếu nhập không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPhieuNhapSua_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder("EXEC proc_cap_nhat_phieu_nhap");
            query.Append(" @ngayLapPhieuNhap = '" + dateNgayLapPhieuNhap.Value + "'");
            query.Append(",@tenNhaCungCap = N'" + txtPhieuNhapNhaCungCap.Text + "'");
            query.Append(",@maPhieuNhap = " + maPhieuNhapPhieuNhap);

            int result = dataProvider.execNonQuery(query.ToString());

            if (result > 0)
            {
                loadDgPhieuNhap();
                MessageBox.Show("Cập nhật phiếu nhập thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Cập nhật phiếu nhập không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPhieuNhapXoa_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu nhập có mã là " + maPhieuNhapPhieuNhap + " ?", "Cảnh Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (check == DialogResult.Yes)
            {
                string query = "DELETE FROM tbl_phieu_nhap WHERE ma_phieu_nhap = " + maPhieuNhapPhieuNhap;
                int result = dataProvider.execNonQuery(query.ToString());

                if (result > 0)
                {
                    loadDgPhieuNhap();
                    MessageBox.Show("Xóa phiếu nhập thành công !", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Xóa phiếu nhập không thành công !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPhieuNhapChiTiet_Click(object sender, EventArgs e)
        {
            FormChiTietPhieuNhap form = new FormChiTietPhieuNhap(maPhieuNhapPhieuNhap);
            form.ShowDialog();
            loadDgSach();
            loadDgPhieuNhap();
        }

        private void btnHoaDonChiTiet_Click(object sender, EventArgs e)
        {
            FormChiTietHoaDon form = new FormChiTietHoaDon(maHoaDonHoaDon);
            form.ShowDialog();
            loadDgSach();
            loadDgHoaDon();
        }

        private void btnTimTheoTenSach_Click(object sender, EventArgs e)
        {
            if (txtTimTenSach.Text == "")
            {
                loadDgSach();
            }else
            {
                DataTable dt = new DataTable();

                StringBuilder query = new StringBuilder("SELECT ma_sach as [Mã Sách]");
                query.Append(",ten_sach as [Tên Sách]");
                query.Append(",ten_loai_sach as [Loại Sách]");
                query.Append(",tac_gia as [Tác Giả]");
                query.Append(",so_luong as [Số Lượng]");
                query.Append(",gia_ban as [Giá Bán]");
                query.Append(" FROM tbl_sach, tbl_loai_sach");
                query.Append(" WHERE tbl_sach.ma_loai_sach = tbl_loai_sach.ma_loai_sach");
                query.Append(" AND ten_sach LiKE N'" + txtTimTenSach.Text + "%';");

                dt = dataProvider.execQuery(query.ToString());

                dgSach.DataSource = dt;
                if (dt.Rows.Count > 0) maSachSach = (int)dt.Rows[0][0];
            }
        }

        private void btnTimTheoTenLoaiSach_Click(object sender, EventArgs e)
        {
            if (txtTimTenLoaiSach.Text == "")
            {
                loadDgLoaiSach();
            }else
            {
                DataTable dt = new DataTable();

                StringBuilder query = new StringBuilder("SELECT ma_loai_sach as [Mã Loại Sách]");
                query.Append(",ten_loai_sach as [Tên Loại Sách]");
                query.Append(" FROM tbl_loai_sach");
                query.Append(" WHERE ten_loai_sach LIKE N'" + txtTimTenLoaiSach.Text + "%'");

                dt = dataProvider.execQuery(query.ToString());

                dgLoaiSach.DataSource = dt;

                if (dt.Rows.Count > 0) maLoaiSachLoaiSach = (int)dt.Rows[0][0];
            }
        }

        private void btnTimMaHoaDon_Click(object sender, EventArgs e)
        {
            if (txtTimMaHD.Text == "")
            {
                loadDgHoaDon();
            }else
            {
                DataTable dt = new DataTable();

                StringBuilder query = new StringBuilder("SELECT ma_hoa_don as [Mã Hóa Đơn]");
                query.Append(",ngay_lap_hoa_don as [Ngày Lập Hóa Đơn]");
                query.Append(",ten_khach_hang as [Tên Khách Hàng]");
                query.Append(",sdt_khach_hang as [SDT Khách Hàng]");
                query.Append(",( SELECT SUM(tbl_sach.gia_ban * tbl_chi_tiet_hoa_don.so_luong) ");
                query.Append(" FROM tbl_chi_tiet_hoa_don, tbl_sach ");
                query.Append(" WHERE tbl_chi_tiet_hoa_don.ma_sach = tbl_sach.ma_sach");
                query.Append(" AND tbl_chi_tiet_hoa_don.ma_hoa_don = tbl_hoa_don.ma_hoa_don) AS [Thành Tiền]");
                query.Append(" FROM tbl_hoa_don");
                query.Append(" WHERE ma_hoa_don = " + txtTimMaHD.Text);

                dt = dataProvider.execQuery(query.ToString());

                dgHoaDon.DataSource = dt;

                if (dt.Rows.Count > 0) maHoaDonHoaDon = (int)dt.Rows[0][0];

                double tongTien = 0.0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][4].ToString() != "")
                        tongTien += (double)dt.Rows[i][4];
                }
                txtTongTienHoaDon.Text = "Tổng Tiền: " + tongTien.ToString();
                txtSoHoaDon.Text = "Số Hóa Đơn: " + dt.Rows.Count.ToString();

            }
        }

        private void txtTimMaHD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void btnThongKeHoaDon_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder("SELECT ma_hoa_don as [Mã Hóa Đơn]");
            query.Append(",ngay_lap_hoa_don as [Ngày Lập Hóa Đơn]");
            query.Append(",ten_khach_hang as [Tên Khách Hàng]");
            query.Append(",sdt_khach_hang as [SDT Khách Hàng]");
            query.Append(",( SELECT SUM(tbl_sach.gia_ban * tbl_chi_tiet_hoa_don.so_luong) ");
            query.Append(" FROM tbl_chi_tiet_hoa_don, tbl_sach ");
            query.Append(" WHERE tbl_chi_tiet_hoa_don.ma_sach = tbl_sach.ma_sach");
            query.Append(" AND tbl_chi_tiet_hoa_don.ma_hoa_don = tbl_hoa_don.ma_hoa_don) AS [Thành Tiền]");
            query.Append(" FROM tbl_hoa_don");
            query.Append(" WHERE ngay_lap_hoa_don BETWEEN '" + dateStartHD.Value.ToString("MM/dd/yyyy") +"' AND '" + dateEndHD.Value.ToString("MM/dd/yyyy") +"'");

            dt = dataProvider.execQuery(query.ToString());

            dgHoaDon.DataSource = dt;
            
            if (dt.Rows.Count > 0) maHoaDonHoaDon = (int)dt.Rows[0][0];

            double tongTien = 0.0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][4].ToString() != "")
                    tongTien += (double)dt.Rows[i][4];
            }
            txtTongTienHoaDon.Text = "Tổng Tiền: " + tongTien.ToString();
            txtSoHoaDon.Text = "Số Hóa Đơn: " + dt.Rows.Count.ToString();

        }

        private void btnTimMaPhieuNhap_Click(object sender, EventArgs e)
        {
            if (txtTimMaPhieuNhap.Text == "")
            {
                loadDgPhieuNhap();
            }else
            {
                DataTable dt = new DataTable();

                StringBuilder query = new StringBuilder("SELECT ma_phieu_nhap as [Mã Phiếu Nhập]");
                query.Append(", ngay_lap_phieu_nhap as [Ngày Lập Phiếu Nhập]");
                query.Append(",ten_nha_cung_cap as [Tên Nhà Cung Cấp]");
                query.Append(", (SELECT SUM(so_luong * gia_nhap) FROM tbl_chi_tiet_phieu_nhap ");
                query.Append(" WHERE tbl_chi_tiet_phieu_nhap.ma_phieu_nhap = tbl_phieu_nhap.ma_phieu_nhap) as [Thành Tiền]");
                query.Append(" FROM tbl_phieu_nhap");
                query.Append(" WHERE ma_phieu_nhap = " + txtTimMaPhieuNhap.Text);

                dt = dataProvider.execQuery(query.ToString());

                dgPhieuNhap.DataSource = dt;

                if (dt.Rows.Count > 0) maPhieuNhapPhieuNhap = (int)dt.Rows[0][0];
                //do du lieu vao phan phieu nhap
                double tongTien = 0.0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][3].ToString() != "")
                        tongTien += (double)dt.Rows[i][3];
                }
                txtTongTienPhieuNhap.Text = "Tổng Tiền: " + tongTien.ToString();
                txtSoPhieuNhap.Text = "Số Phiếu Nhập: " + dt.Rows.Count.ToString();
            }

        }

        private void btnThongKePhieuNhap_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder("SELECT ma_phieu_nhap as [Mã Phiếu Nhập]");
            query.Append(", ngay_lap_phieu_nhap as [Ngày Lập Phiếu Nhập]");
            query.Append(",ten_nha_cung_cap as [Tên Nhà Cung Cấp]");
            query.Append(", (SELECT SUM(so_luong * gia_nhap) FROM tbl_chi_tiet_phieu_nhap ");
            query.Append(" WHERE tbl_chi_tiet_phieu_nhap.ma_phieu_nhap = tbl_phieu_nhap.ma_phieu_nhap) as [Thành Tiền]");
            query.Append(" FROM tbl_phieu_nhap");
            query.Append(" WHERE ngay_lap_phieu_nhap BETWEEN '" + dateStartPN.Value.ToString("MM/dd/yyyy") + "' AND '" + dateEndPN.Value.ToString("MM/dd/yyyy") + "'");


            dt = dataProvider.execQuery(query.ToString());

            dgPhieuNhap.DataSource = dt;

            if (dt.Rows.Count > 0) maPhieuNhapPhieuNhap = (int)dt.Rows[0][0];
            //do du lieu vao phan phieu nhap
            double tongTien = 0.0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][3].ToString() != "")
                    tongTien += (double)dt.Rows[i][3];
            }
            txtTongTienPhieuNhap.Text = "Tổng Tiền: " + tongTien.ToString();
            txtSoPhieuNhap.Text = "Số Phiếu Nhập: " + dt.Rows.Count.ToString();
        }

        private void dgSach_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
