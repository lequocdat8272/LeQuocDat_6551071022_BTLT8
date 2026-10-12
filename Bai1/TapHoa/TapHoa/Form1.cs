using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using TapHoa.Models;

namespace TapHoa
{
    public partial class Form1 : Form
    {
        private int _selectedMaHang = 0;
        private bool _isBinding = false;

        public Form1()
        {
            InitializeComponent();
            SetupFormEvents();
        }

        private void SetupFormEvents()
        {
            Load += Form1_Load;

            // Sự kiện lưới
            dgvHangHoa.CellClick += DgvHangHoa_CellClick;
            dgvHangHoa.SelectionChanged += DgvHangHoa_SelectionChanged;

            // Sự kiện nút bấm
            btnThem.Click += async (s, e) => await ThemHangHoaAsync();
            btnSua.Click += async (s, e) => await SuaHangHoaAsync();
            btnXoa.Click += async (s, e) => await XoaHangHoaAsync();
            btnLamMoi.Click += async (s, e) => await LamMoiAsync();
            btnXemBangGia.Click += async (s, e) => await XemBangGiaAsync();
            btnXuatPdf.Click += async (s, e) => await XuatPdfAsync();
            btnXuatExcelLuoi.Click += XuatExcelLuoi;
            btnXuatExcelTonKho.Click += async (s, e) => await XuatExcelTonKhoAsync();

            // Sự kiện tìm kiếm & lọc
            txtTimKiem.TextChanged += (s, e) =>
            {
                timerDebounce.Stop();
                timerDebounce.Start();
            };
            timerDebounce.Tick += async (s, e) =>
            {
                timerDebounce.Stop();
                await LoadDataAsync();
            };
            cboLocLoaiHang.SelectedIndexChanged += async (s, e) =>
            {
                if (!_isBinding)
                {
                    await LoadDataAsync();
                }
            };
            chkLocConKinhDoanh.CheckedChanged += async (s, e) =>
            {
                if (!_isBinding)
                {
                    await LoadDataAsync();
                }
            };
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            _isBinding = true;
            try
            {
                // Khởi tạo ComboBox
                cboLoaiHang.Items.AddRange(new object[] { "Đồ uống", "Thực phẩm khô", "Gia vị", "Đồ gia dụng" });
                cboDonVi.Items.AddRange(new object[] { "Chai", "Gói", "Hộp", "Túi" });

                cboLocLoaiHang.Items.Add("Tất cả loại");
                cboLocLoaiHang.Items.AddRange(new object[] { "Đồ uống", "Thực phẩm khô", "Gia vị", "Đồ gia dụng" });
                cboLocLoaiHang.SelectedIndex = 0;

                // Định dạng hiển thị DataGridView
                colGiaBan.DefaultCellStyle.Format = "N0";
                colGiaBan.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colSoLuongTon.DefaultCellStyle.Format = "N0";
                colSoLuongTon.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colNgayNhap.DefaultCellStyle.Format = "dd/MM/yyyy";
                colNgayNhap.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colMaHang.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colDonVi.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colConKinhDoanh.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                _isBinding = false;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _isBinding = false;
                MessageBox.Show("Lỗi khởi động form: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                using var context = new TiemTapHoaContext();
                var query = context.HangHoas.AsQueryable();

                string keyword = txtTimKiem.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(h => h.TenHang.Contains(keyword));
                }

                if (cboLocLoaiHang.SelectedIndex > 0 && cboLocLoaiHang.SelectedItem != null)
                {
                    string selectedLoai = cboLocLoaiHang.SelectedItem.ToString()!;
                    if (selectedLoai != "Tất cả loại" && selectedLoai != "Tất cả")
                    {
                        query = query.Where(h => h.LoaiHang == selectedLoai);
                    }
                }

                if (chkLocConKinhDoanh.Checked)
                {
                    query = query.Where(h => h.ConKinhDoanh);
                }

                var list = await query.OrderBy(h => h.MaHang).ToListAsync();
                dgvHangHoa.DataSource = list;
                lblTrangThai.Text = $"Tìm thấy {list.Count} mặt hàng";

                if (list.Count > 0 && _selectedMaHang > 0)
                {
                    // Giữ lại dòng được chọn nếu còn trong danh sách
                    foreach (DataGridViewRow row in dgvHangHoa.Rows)
                    {
                        if (row.DataBoundItem is HangHoa hh && hh.MaHang == _selectedMaHang)
                        {
                            row.Selected = true;
                            dgvHangHoa.CurrentCell = row.Cells[0];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy vấn dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindHangHoaToControls(HangHoa hh)
        {
            _selectedMaHang = hh.MaHang;
            txtTenHang.Text = hh.TenHang;
            cboLoaiHang.SelectedItem = hh.LoaiHang;
            cboDonVi.SelectedItem = hh.DonVi;

            if (hh.GiaBan >= numGiaBan.Minimum && hh.GiaBan <= numGiaBan.Maximum)
                numGiaBan.Value = hh.GiaBan;
            else
                numGiaBan.Value = numGiaBan.Minimum;

            if (hh.SoLuongTon >= numSoLuongTon.Minimum && hh.SoLuongTon <= numSoLuongTon.Maximum)
                numSoLuongTon.Value = hh.SoLuongTon;
            else
                numSoLuongTon.Value = numSoLuongTon.Minimum;

            dtpNgayNhap.Value = hh.NgayNhap;
            chkConKinhDoanh.Checked = hh.ConKinhDoanh;
        }

        private void DgvHangHoa_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvHangHoa.Rows[e.RowIndex].DataBoundItem is HangHoa hh)
            {
                BindHangHoaToControls(hh);
            }
        }

        private void DgvHangHoa_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvHangHoa.CurrentRow != null && dgvHangHoa.CurrentRow.DataBoundItem is HangHoa hh)
            {
                BindHangHoaToControls(hh);
            }
        }

        private void ResetInputForm()
        {
            _selectedMaHang = 0;
            txtTenHang.Clear();
            cboLoaiHang.SelectedIndex = -1;
            cboDonVi.SelectedIndex = -1;
            numGiaBan.Value = 0;
            numSoLuongTon.Value = 0;
            dtpNgayNhap.Value = DateTime.Now;
            chkConKinhDoanh.Checked = true;
            dgvHangHoa.ClearSelection();
        }

        private async Task ThemHangHoaAsync()
        {
            try
            {
                // Kiểm tra hợp lệ dữ liệu
                if (string.IsNullOrWhiteSpace(txtTenHang.Text))
                {
                    MessageBox.Show("Tên hàng không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenHang.Focus();
                    return;
                }

                if (numGiaBan.Value <= 0)
                {
                    MessageBox.Show("Giá bán phải lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numGiaBan.Focus();
                    return;
                }

                var hangHoa = new HangHoa
                {
                    TenHang = txtTenHang.Text.Trim(),
                    LoaiHang = cboLoaiHang.SelectedItem?.ToString(),
                    DonVi = cboDonVi.SelectedItem?.ToString(),
                    GiaBan = numGiaBan.Value,
                    SoLuongTon = (int)numSoLuongTon.Value,
                    NgayNhap = dtpNgayNhap.Value,
                    ConKinhDoanh = chkConKinhDoanh.Checked
                };

                using var context = new TiemTapHoaContext();
                await context.HangHoas.AddAsync(hangHoa);
                await context.SaveChangesAsync();

                MessageBox.Show("Thêm hàng hóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetInputForm();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm hàng hóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SuaHangHoaAsync()
        {
            try
            {
                if (_selectedMaHang <= 0)
                {
                    MessageBox.Show("Vui lòng chọn một mặt hàng trên bảng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTenHang.Text))
                {
                    MessageBox.Show("Tên hàng không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenHang.Focus();
                    return;
                }

                if (numGiaBan.Value <= 0)
                {
                    MessageBox.Show("Giá bán phải lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numGiaBan.Focus();
                    return;
                }

                using var context = new TiemTapHoaContext();
                var hangHoa = await context.HangHoas.FindAsync(_selectedMaHang);
                if (hangHoa == null)
                {
                    MessageBox.Show("Không tìm thấy hàng hóa cần sửa trong CSDL!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                hangHoa.TenHang = txtTenHang.Text.Trim();
                hangHoa.LoaiHang = cboLoaiHang.SelectedItem?.ToString();
                hangHoa.DonVi = cboDonVi.SelectedItem?.ToString();
                hangHoa.GiaBan = numGiaBan.Value;
                hangHoa.SoLuongTon = (int)numSoLuongTon.Value;
                hangHoa.NgayNhap = dtpNgayNhap.Value;
                hangHoa.ConKinhDoanh = chkConKinhDoanh.Checked;

                await context.SaveChangesAsync();

                MessageBox.Show("Cập nhật hàng hóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa hàng hóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task XoaHangHoaAsync()
        {
            try
            {
                if (_selectedMaHang <= 0)
                {
                    MessageBox.Show("Vui lòng chọn mặt hàng cần xóa trên bảng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa mặt hàng '{txtTenHang.Text}' không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using var context = new TiemTapHoaContext();
                    var hangHoa = await context.HangHoas.FindAsync(_selectedMaHang);
                    if (hangHoa != null)
                    {
                        context.HangHoas.Remove(hangHoa);
                        await context.SaveChangesAsync();
                        MessageBox.Show("Đã xóa hàng hóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetInputForm();
                        await LoadDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa hàng hóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LamMoiAsync()
        {
            try
            {
                ResetInputForm();
                txtTimKiem.Clear();
                cboLocLoaiHang.SelectedIndex = 0;
                chkLocConKinhDoanh.Checked = false;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi làm mới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region FastReport Bảng Giá & Xuất PDF

        private async Task<FastReport.Report?> PrepareBangGiaReportAsync()
        {
            using var context = new TiemTapHoaContext();
            var list = await context.HangHoas
                .Where(h => h.ConKinhDoanh)
                .OrderBy(h => h.MaHang)
                .ToListAsync();

            var dt = new DataTable("HangHoa");
            dt.Columns.Add("MaHang", typeof(int));
            dt.Columns.Add("TenHang", typeof(string));
            dt.Columns.Add("LoaiHang", typeof(string));
            dt.Columns.Add("DonVi", typeof(string));
            dt.Columns.Add("GiaBan", typeof(decimal));
            dt.Columns.Add("SoLuongTon", typeof(int));
            dt.Columns.Add("NgayNhap", typeof(DateTime));
            dt.Columns.Add("ConKinhDoanh", typeof(bool));

            foreach (var item in list)
            {
                dt.Rows.Add(item.MaHang, item.TenHang, item.LoaiHang, item.DonVi, item.GiaBan, item.SoLuongTon, item.NgayNhap, item.ConKinhDoanh);
            }

            string reportPath = Path.Combine(Application.StartupPath, "Reports", "BangGia.frx");
            if (!File.Exists(reportPath))
            {
                reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "BangGia.frx");
            }
            if (!File.Exists(reportPath))
            {
                // Thử tìm tại thư mục gốc solution nếu đang chạy debug
                string altPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Reports", "BangGia.frx");
                if (File.Exists(altPath))
                {
                    reportPath = Path.GetFullPath(altPath);
                }
            }

            if (!File.Exists(reportPath))
            {
                MessageBox.Show($"Không tìm thấy file mẫu báo cáo tại: {reportPath}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var report = new FastReport.Report();
            report.Load(reportPath);
            report.RegisterData(dt, "HangHoa");
            var ds = report.GetDataSource("HangHoa");
            if (ds != null)
            {
                ds.Enabled = true;
            }

            report.SetParameterValue("NgayIn", DateTime.Now.ToString("dd/MM/yyyy"));
            return report;
        }

        private async Task XemBangGiaAsync()
        {
            try
            {
                using var report = await PrepareBangGiaReportAsync();
                if (report == null) return;
                report.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở báo cáo bảng giá: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task XuatPdfAsync()
        {
            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = "BangGia_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf",
                    Title = "Chọn nơi lưu file PDF bảng giá"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using var report = await PrepareBangGiaReportAsync();
                    if (report == null) return;
                    report.Prepare();

                    // Xuất PDF bằng PDFExport
                    using var pdfExport = new FastReport.Export.Pdf.PDFExport();
                    report.Export(pdfExport, sfd.FileName);

                    var ask = MessageBox.Show("Xuất file PDF thành công! Bạn có muốn mở file ngay không?",
                        "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ask == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("File PDF đang được mở bởi ứng dụng khác. Vui lòng đóng file rồi thử lại!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất file PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Xuất Excel (ClosedXML)

        private void XuatExcelLuoi(object? sender, EventArgs e)
        {
            try
            {
                if (dgvHangHoa.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu trên bảng để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var sfd = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = "DanhSachHangHoa_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx",
                    Title = "Chọn nơi lưu file Excel"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using var workbook = new XLWorkbook();
                    var ws = workbook.Worksheets.Add("HangHoa");

                    // Header
                    string[] headers = { "Mã", "Tên hàng", "Loại hàng", "Đơn vị", "Giá bán", "Tồn", "Ngày nhập", "Kinh doanh" };
                    for (int c = 0; c < headers.Length; c++)
                    {
                        var cell = ws.Cell(1, c + 1);
                        cell.Value = headers[c];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E0E0E0");
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    int rowIdx = 2;
                    foreach (DataGridViewRow dgvRow in dgvHangHoa.Rows)
                    {
                        if (dgvRow.IsNewRow) continue;

                        int ma = Convert.ToInt32(dgvRow.Cells["colMaHang"].Value);
                        string ten = dgvRow.Cells["colTenHang"].Value?.ToString() ?? "";
                        string loai = dgvRow.Cells["colLoaiHang"].Value?.ToString() ?? "";
                        string donVi = dgvRow.Cells["colDonVi"].Value?.ToString() ?? "";
                        decimal giaBan = Convert.ToDecimal(dgvRow.Cells["colGiaBan"].Value ?? 0);
                        int soLuong = Convert.ToInt32(dgvRow.Cells["colSoLuongTon"].Value ?? 0);
                        DateTime ngayNhap = Convert.ToDateTime(dgvRow.Cells["colNgayNhap"].Value);
                        bool conKinhDoanh = Convert.ToBoolean(dgvRow.Cells["colConKinhDoanh"].Value ?? false);

                        ws.Cell(rowIdx, 1).SetValue(ma);
                        ws.Cell(rowIdx, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell(rowIdx, 2).SetValue(ten);
                        ws.Cell(rowIdx, 3).SetValue(loai);
                        ws.Cell(rowIdx, 4).SetValue(donVi);
                        ws.Cell(rowIdx, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        var cGiaBan = ws.Cell(rowIdx, 5);
                        cGiaBan.SetValue((double)giaBan);
                        cGiaBan.Style.NumberFormat.Format = "#,##0";
                        cGiaBan.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        var cTon = ws.Cell(rowIdx, 6);
                        cTon.SetValue(soLuong);
                        cTon.Style.NumberFormat.Format = "#,##0";
                        cTon.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        var cNgay = ws.Cell(rowIdx, 7);
                        cNgay.SetValue(ngayNhap.ToString("dd/MM/yyyy"));
                        cNgay.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        var cKd = ws.Cell(rowIdx, 8);
                        cKd.SetValue(conKinhDoanh ? "Còn bán" : "Ngừng bán");
                        cKd.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        rowIdx++;
                    }

                    int lastRow = rowIdx - 1;
                    var tableRange = ws.Range(1, 1, lastRow, headers.Length);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(sfd.FileName);

                    var ask = MessageBox.Show("Xuất file Excel từ lưới thành công! Bạn có muốn mở file ngay không?",
                        "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ask == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("File Excel đang được mở bởi ứng dụng khác. Vui lòng đóng file rồi thử lại!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel từ lưới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task XuatExcelTonKhoAsync()
        {
            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = "BaoCaoTonKho_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx",
                    Title = "Chọn nơi lưu file Excel tồn kho"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using var context = new TiemTapHoaContext();
                    var list = await context.HangHoas
                        .Where(h => h.ConKinhDoanh)
                        .OrderBy(h => h.MaHang)
                        .ToListAsync();

                    using var workbook = new XLWorkbook();
                    var ws = workbook.Worksheets.Add("TonKho");

                    // Tiêu đề cột
                    // 1: STT, 2: Mã hàng, 3: Tên hàng, 4: Loại hàng, 5: Đơn vị, 6: Giá bán (F), 7: Số lượng tồn (G), 8: Giá trị tồn (H)
                    string[] headers = { "STT", "Mã hàng", "Tên hàng", "Loại hàng", "Đơn vị", "Giá bán", "Số lượng tồn", "Giá trị tồn" };
                    for (int c = 0; c < headers.Length; c++)
                    {
                        var cell = ws.Cell(1, c + 1);
                        cell.Value = headers[c];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E1F2");
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    int rowIdx = 2;
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        ws.Cell(rowIdx, 1).SetValue(i + 1);
                        ws.Cell(rowIdx, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell(rowIdx, 2).SetValue(item.MaHang);
                        ws.Cell(rowIdx, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell(rowIdx, 3).SetValue(item.TenHang);
                        ws.Cell(rowIdx, 4).SetValue(item.LoaiHang ?? "");
                        ws.Cell(rowIdx, 5).SetValue(item.DonVi ?? "");
                        ws.Cell(rowIdx, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Giá bán (F)
                        var cGiaBan = ws.Cell(rowIdx, 6);
                        cGiaBan.SetValue((double)item.GiaBan);
                        cGiaBan.Style.NumberFormat.Format = "#,##0";
                        cGiaBan.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        // Số lượng tồn (G)
                        var cTon = ws.Cell(rowIdx, 7);
                        cTon.SetValue(item.SoLuongTon);
                        cTon.Style.NumberFormat.Format = "#,##0";
                        cTon.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        // Giá trị tồn (H) = F * G (công thức)
                        var cGiaTriTon = ws.Cell(rowIdx, 8);
                        cGiaTriTon.FormulaA1 = $"=F{rowIdx}*G{rowIdx}";
                        cGiaTriTon.Style.NumberFormat.Format = "#,##0";
                        cGiaTriTon.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        rowIdx++;
                    }

                    int lastDataRow = rowIdx - 1;

                    // Dòng "Tổng"
                    ws.Cell(rowIdx, 1).SetValue("Tổng");
                    ws.Range(rowIdx, 1, rowIdx, 7).Merge();
                    ws.Cell(rowIdx, 1).Style.Font.Bold = true;
                    ws.Cell(rowIdx, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var cTongGiaTri = ws.Cell(rowIdx, 8);
                    cTongGiaTri.FormulaA1 = $"=SUM(H2:H{lastDataRow})";
                    cTongGiaTri.Style.Font.Bold = true;
                    cTongGiaTri.Style.NumberFormat.Format = "#,##0";
                    cTongGiaTri.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    // Viền bảng
                    var dataRange = ws.Range(1, 1, rowIdx, 8);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // AutoFilter cho các dòng dữ liệu (từ dòng 1 đến lastDataRow)
                    ws.Range(1, 1, lastDataRow, 8).SetAutoFilter();

                    // Freeze dòng tiêu đề
                    ws.SheetView.FreezeRows(1);

                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(sfd.FileName);

                    var ask = MessageBox.Show("Xuất file Excel tồn kho thành công! Bạn có muốn mở file ngay không?",
                        "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ask == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("File Excel đang được mở bởi ứng dụng khác. Vui lòng đóng file rồi thử lại!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel tồn kho: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
