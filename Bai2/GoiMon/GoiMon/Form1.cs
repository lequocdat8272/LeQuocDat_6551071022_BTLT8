using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastReport;
using FastReport.Export.Pdf;
using GoiMon.Models;
using Microsoft.EntityFrameworkCore;

namespace GoiMon;

public partial class Form1 : Form
{
    // Culture định dạng tiền tệ Việt Nam (142.000 đ)
    private static readonly CultureInfo ViCulture = new("vi-VN");

    // Controls vùng Tìm kiếm (Top)
    private GroupBox grpTimKiem = null!;
    private DateTimePicker dtpTuNgay = null!;
    private DateTimePicker dtpDenNgay = null!;
    private ComboBox cboTimSoBan = null!;
    private ComboBox cboTimHinhThuc = null!;
    private TextBox txtTimTenMon = null!;
    private Button btnTim = null!;

    // Controls vùng Danh sách hóa đơn (Left)
    private Label lblDanhSachHeader = null!;
    private DataGridView dgvHoaDon = null!;
    private Label lblTongDoanhThu = null!;

    // Controls vùng Chi tiết hóa đơn (Right)
    private Label lblChiTietHeader = null!;
    private DateTimePicker dtpNgayLap = null!;
    private ComboBox cboSoBan = null!;
    private RadioButton rdoTienMat = null!;
    private RadioButton rdoChuyenKhoan = null!;
    private TextBox txtGhiChu = null!;
    private ComboBox cboMon = null!;
    private NumericUpDown numSoLuong = null!;
    private Button btnThemMon = null!;
    private DataGridView dgvChiTiet = null!;
    private Label lblTongCong = null!;

    // Controls thanh nút bấm dưới cùng (Bottom)
    private Button btnHoaDonMoi = null!;
    private Button btnSuaHoaDon = null!;
    private Button btnXoaHoaDon = null!;
    private Button btnSuaSL = null!;
    private Button btnXoaMon = null!;
    private Button btnXemHoaDon = null!;
    private Button btnXuatPdf = null!;
    private Button btnIn = null!;

    // Trạng thái hóa đơn đang chọn
    private int? selectedInvoiceId = null;
    private bool isUpdatingSelection = false;

    public Form1()
    {
        InitializeComponent();
        InitCustomControls();
        Load += Form1_Load;
    }

    private void InitCustomControls()
    {
        Text = "Quán cà phê Góc Phố - Quản lý hóa đơn";
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Size = new Size(1180, 680);
        MinimumSize = new Size(1100, 640);
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleMode = AutoScaleMode.Dpi;

        // ==========================================
        // 1. GroupBox Tìm kiếm hóa đơn (Top)
        // ==========================================
        grpTimKiem = new GroupBox
        {
            Text = "Tìm kiếm hóa đơn",
            Location = new Point(12, 10),
            Size = new Size(1140, 70),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblTuNgay = new Label { Text = "Từ ngày", Location = new Point(15, 20), AutoSize = true };
        dtpTuNgay = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = new DateTime(2026, 10, 1),
            Location = new Point(15, 38),
            Size = new Size(120, 24)
        };

        var lblGachNgang = new Label { Text = "-", Location = new Point(140, 41), AutoSize = true };

        var lblDenNgay = new Label { Text = "Đến ngày", Location = new Point(155, 20), AutoSize = true };
        dtpDenNgay = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = new DateTime(2026, 10, 11),
            Location = new Point(155, 38),
            Size = new Size(120, 24)
        };

        var lblSoBanSearch = new Label { Text = "Số bàn", Location = new Point(290, 20), AutoSize = true };
        cboTimSoBan = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(290, 38),
            Size = new Size(100, 24)
        };
        cboTimSoBan.Items.Add("Tất cả");
        for (int i = 1; i <= 10; i++) cboTimSoBan.Items.Add(i.ToString());
        cboTimSoBan.SelectedIndex = 0;

        var lblHinhThucSearch = new Label { Text = "Hình thức", Location = new Point(405, 20), AutoSize = true };
        cboTimHinhThuc = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(405, 38),
            Size = new Size(125, 24)
        };
        cboTimHinhThuc.Items.AddRange(new object[] { "Tất cả", "Tiền mặt", "Chuyển khoản" });
        cboTimHinhThuc.SelectedIndex = 0;

        var lblTenMonSearch = new Label { Text = "Tên món", Location = new Point(545, 20), AutoSize = true };
        txtTimTenMon = new TextBox
        {
            Location = new Point(545, 38),
            Size = new Size(180, 24)
        };

        btnTim = new Button
        {
            Text = "Tìm",
            Location = new Point(740, 36),
            Size = new Size(85, 28)
        };
        btnTim.Click += async (_, _) => await LoadInvoicesAsync();

        grpTimKiem.Controls.AddRange(new Control[]
        {
            lblTuNgay, dtpTuNgay, lblGachNgang, lblDenNgay, dtpDenNgay,
            lblSoBanSearch, cboTimSoBan, lblHinhThucSearch, cboTimHinhThuc,
            lblTenMonSearch, txtTimTenMon, btnTim
        });

        // ==========================================
        // 2. Vùng Trái: Danh sách hóa đơn
        // ==========================================
        lblDanhSachHeader = new Label
        {
            Text = "Danh sách hóa đơn",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            Location = new Point(12, 88),
            AutoSize = true
        };

        dgvHoaDon = new DataGridView
        {
            Location = new Point(12, 110),
            Size = new Size(520, 385),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersWidth = 26,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.Fixed3D,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã HD", FillWeight = 18, MinimumWidth = 85 });
        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày lập", FillWeight = 28, MinimumWidth = 135 });
        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số bàn", FillWeight = 14, MinimumWidth = 55 });
        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hình thức TT", FillWeight = 20, MinimumWidth = 95 });
        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Tổng tiền",
            FillWeight = 20,
            MinimumWidth = 90,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        dgvHoaDon.SelectionChanged += DgvHoaDon_SelectionChanged;

        lblTongDoanhThu = new Label
        {
            Text = "Tổng doanh thu đang hiển thị: 0 đ",
            Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
            ForeColor = Color.Black,
            Location = new Point(12, 508),
            Size = new Size(520, 35),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };

        // ==========================================
        // 3. Vùng Phải: Chi tiết hóa đơn
        // ==========================================
        lblChiTietHeader = new Label
        {
            Text = "Chi tiết hóa đơn",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            Location = new Point(545, 88),
            AutoSize = true
        };

        // Row 1: Ngày lập, Số bàn, Tiền mặt/Chuyển khoản
        dtpNgayLap = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy HH:mm",
            Location = new Point(545, 110),
            Size = new Size(155, 24),
            Anchor = AnchorStyles.Top | AnchorStyles.Left
        };

        var lblSoBanDetail = new Label { Text = "Số bàn:", Location = new Point(712, 113), AutoSize = true };
        cboSoBan = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(765, 110),
            Size = new Size(55, 24)
        };
        for (int i = 1; i <= 10; i++) cboSoBan.Items.Add(i);
        cboSoBan.SelectedIndex = 0;

        rdoTienMat = new RadioButton { Text = "Tiền mặt", Location = new Point(830, 112), AutoSize = true, Checked = true };
        rdoChuyenKhoan = new RadioButton { Text = "Chuyển khoản", Location = new Point(918, 112), AutoSize = true };

        // Row 2: Ghi chú
        var lblGhiChu = new Label { Text = "Ghi chú:", Location = new Point(545, 145), AutoSize = true };
        txtGhiChu = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Location = new Point(610, 143),
            Size = new Size(542, 45),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Row 3: Chọn món, SL, Thêm món
        var lblChonMon = new Label { Text = "Chọn món:", Location = new Point(545, 200), AutoSize = true };
        cboMon = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(625, 197),
            Size = new Size(210, 24)
        };

        var lblSL = new Label { Text = "SL:", Location = new Point(845, 200), AutoSize = true };
        numSoLuong = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 50,
            Value = 1,
            Location = new Point(875, 197),
            Size = new Size(55, 24)
        };

        btnThemMon = new Button
        {
            Text = "Thêm món",
            Location = new Point(940, 195),
            Size = new Size(95, 28),
            AutoSize = true,
            Padding = new Padding(6, 2, 6, 2)
        };
        btnThemMon.Click += async (_, _) => await ThemMonAsync();

        // Row 4: DataGridView Chi tiết
        dgvChiTiet = new DataGridView
        {
            Location = new Point(545, 235),
            Size = new Size(607, 260),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersWidth = 26,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.Fixed3D,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "STT", FillWeight = 10, MinimumWidth = 38, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên món", FillWeight = 40, MinimumWidth = 140 });
        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SL", FillWeight = 12, MinimumWidth = 42, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá", FillWeight = 19, MinimumWidth = 75, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành tiền", FillWeight = 19, MinimumWidth = 80, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvChiTiet.SelectionChanged += DgvChiTiet_SelectionChanged;

        // Row 5: Label Tổng cộng
        lblTongCong = new Label
        {
            Text = "Tổng cộng: 0 đ",
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            ForeColor = Color.Black,
            Location = new Point(680, 508),
            Size = new Size(472, 35),
            TextAlign = ContentAlignment.MiddleRight,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };

        // ==========================================
        // 4. Thanh nút bấm dưới cùng (Bottom Flow Panel)
        // ==========================================
        var pnlBottomButtons = new FlowLayoutPanel
        {
            Location = new Point(12, 560),
            Size = new Size(1140, 45),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            WrapContents = false,
            AutoScroll = false,
            Padding = new Padding(0)
        };

        btnHoaDonMoi = CreateActionButton("Hóa đơn mới", async (_, _) => await TaoHoaDonMoiAsync(), 105);
        btnSuaHoaDon = CreateActionButton("Sửa hóa đơn", async (_, _) => await SuaHoaDonAsync(), 105);
        btnXoaHoaDon = CreateActionButton("Xóa hóa đơn", async (_, _) => await XoaHoaDonAsync(), 105);
        btnSuaSL = CreateActionButton("Sửa SL", async (_, _) => await SuaSoLuongMonAsync(), 85);
        btnXoaMon = CreateActionButton("Xóa món", async (_, _) => await XoaMonAsync(), 85);
        btnXemHoaDon = CreateActionButton("Xem hóa đơn", async (_, _) => await XemHoaDonReportAsync(), 110);
        btnXuatPdf = CreateActionButton("Xuất PDF", async (_, _) => await XuatPdfReportAsync(), 95);
        btnIn = CreateActionButton("In", async (_, _) => await InHoaDonReportAsync(), 75);

        pnlBottomButtons.Controls.AddRange(new Control[]
        {
            btnHoaDonMoi, btnSuaHoaDon, btnXoaHoaDon, btnSuaSL, btnXoaMon,
            btnXemHoaDon, btnXuatPdf, btnIn
        });

        // Thêm controls vào Form
        Controls.AddRange(new Control[]
        {
            grpTimKiem,
            lblDanhSachHeader, dgvHoaDon, lblTongDoanhThu,
            lblChiTietHeader, dtpNgayLap, lblSoBanDetail, cboSoBan, rdoTienMat, rdoChuyenKhoan,
            lblGhiChu, txtGhiChu,
            lblChonMon, cboMon, lblSL, numSoLuong, btnThemMon,
            dgvChiTiet, lblTongCong,
            pnlBottomButtons
        });
    }

    private static Button CreateActionButton(string text, EventHandler clickHandler, int minWidth)
    {
        var b = new Button
        {
            Text = text,
            AutoSize = true,
            MinimumSize = new Size(minWidth, 32),
            Height = 32,
            Padding = new Padding(8, 2, 8, 2),
            Margin = new Padding(0, 0, 8, 0),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular)
        };
        b.Click += clickHandler;
        return b;
    }

    private async void Form1_Load(object? sender, EventArgs e)
    {
        await LoadMonComboBoxAsync();
        await LoadInvoicesAsync();
    }

    // ==========================================
    // CÁC HÀM XỬ LÝ DỮ LIỆU BẤT ĐỒNG BỘ
    // ==========================================

    private static string FormatTien(decimal value) => value.ToString("N0", ViCulture);

    private async Task LoadMonComboBoxAsync()
    {
        try
        {
            using var db = new QuanCaPheContext();
            var danhSachMon = await db.Mons.AsNoTracking().OrderBy(m => m.NhomMon).ThenBy(m => m.TenMon).ToListAsync();
            var items = danhSachMon.Select(m => new MonItemView
            {
                MaMon = m.MaMon,
                TenMon = m.TenMon,
                DonGia = m.DonGia,
                HienThi = $"{m.TenMon} - {FormatTien(m.DonGia)} đ"
            }).ToList();

            cboMon.DataSource = items;
            cboMon.DisplayMember = nameof(MonItemView.HienThi);
            cboMon.ValueMember = nameof(MonItemView.MaMon);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tải danh sách món: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadInvoicesAsync(int? selectInvoiceId = null)
    {
        try
        {
            using var db = new QuanCaPheContext();
            var tuNgay = dtpTuNgay.Value.Date;
            var denNgay = dtpDenNgay.Value.Date.AddDays(1).AddTicks(-1);

            var query = db.HoaDons.AsNoTracking().Where(h => h.NgayLap >= tuNgay && h.NgayLap <= denNgay);

            if (cboTimSoBan.SelectedIndex > 0 && int.TryParse(cboTimSoBan.SelectedItem?.ToString(), out int ban))
            {
                query = query.Where(h => h.SoBan == ban);
            }

            if (cboTimHinhThuc.SelectedIndex > 0)
            {
                string hinhThuc = cboTimHinhThuc.SelectedItem!.ToString()!;
                query = query.Where(h => h.HinhThucTt == hinhThuc);
            }

            string tenMonTim = txtTimTenMon.Text.Trim();
            if (!string.IsNullOrEmpty(tenMonTim))
            {
                query = query.Where(h => h.ChiTietHoaDons.Any(ct => ct.MaMonNavigation.TenMon.Contains(tenMonTim)));
            }

            var rawList = await query
                .OrderBy(h => h.MaHd)
                .Select(h => new
                {
                    h.MaHd,
                    h.NgayLap,
                    h.SoBan,
                    h.HinhThucTt,
                    TongTien = h.ChiTietHoaDons.Sum(ct => (decimal?)(ct.SoLuong * ct.DonGia)) ?? 0m
                })
                .ToListAsync();

            isUpdatingSelection = true;
            dgvHoaDon.Rows.Clear();
            decimal tongDoanhThu = 0;
            int selectedIndex = -1;
            int targetId = selectInvoiceId ?? selectedInvoiceId ?? (rawList.Count > 0 ? rawList[0].MaHd : 0);

            for (int i = 0; i < rawList.Count; i++)
            {
                var h = rawList[i];
                tongDoanhThu += h.TongTien;
                int rowIdx = dgvHoaDon.Rows.Add(
                    $"HD{h.MaHd:D6}",
                    h.NgayLap.ToString("dd/MM/yyyy HH:mm"),
                    h.SoBan,
                    h.HinhThucTt,
                    FormatTien(h.TongTien)
                );
                dgvHoaDon.Rows[rowIdx].Tag = h.MaHd;

                if (h.MaHd == targetId)
                {
                    selectedIndex = rowIdx;
                }
            }

            lblTongDoanhThu.Text = $"Tổng doanh thu đang hiển thị: {FormatTien(tongDoanhThu)} đ";
            isUpdatingSelection = false;

            if (dgvHoaDon.Rows.Count > 0)
            {
                if (selectedIndex < 0) selectedIndex = 0;
                dgvHoaDon.Rows[selectedIndex].Selected = true;
                dgvHoaDon.CurrentCell = dgvHoaDon.Rows[selectedIndex].Cells[0];
                int id = (int)dgvHoaDon.Rows[selectedIndex].Tag!;
                await HienThiChiTietHoaDonAsync(id);
            }
            else
            {
                selectedInvoiceId = null;
                lblChiTietHeader.Text = "Chi tiết hóa đơn";
                dgvChiTiet.Rows.Clear();
                lblTongCong.Text = "Tổng cộng: 0 đ";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tải danh sách hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void DgvHoaDon_SelectionChanged(object? sender, EventArgs e)
    {
        if (isUpdatingSelection) return;
        if (dgvHoaDon.CurrentRow?.Tag is int id)
        {
            await HienThiChiTietHoaDonAsync(id);
        }
    }

    private async Task HienThiChiTietHoaDonAsync(int maHd)
    {
        try
        {
            using var db = new QuanCaPheContext();
            var hd = await db.HoaDons.AsNoTracking().FirstOrDefaultAsync(h => h.MaHd == maHd);
            if (hd == null) return;

            selectedInvoiceId = maHd;
            lblChiTietHeader.Text = $"Chi tiết hóa đơn HD{maHd:D6}";
            dtpNgayLap.Value = hd.NgayLap;
            cboSoBan.SelectedItem = hd.SoBan;
            if (hd.HinhThucTt == "Chuyển khoản") rdoChuyenKhoan.Checked = true;
            else rdoTienMat.Checked = true;
            txtGhiChu.Text = hd.GhiChu ?? "";

            var rawDetails = await db.ChiTietHoaDons
                .AsNoTracking()
                .Where(ct => ct.MaHd == maHd)
                .OrderBy(ct => ct.MaCt)
                .Select(ct => new
                {
                    ct.MaCt,
                    ct.MaMon,
                    ct.MaMonNavigation.TenMon,
                    ct.SoLuong,
                    ct.DonGia,
                    ThanhTien = ct.SoLuong * ct.DonGia
                })
                .ToListAsync();

            dgvChiTiet.Rows.Clear();
            decimal tongTien = 0;
            for (int i = 0; i < rawDetails.Count; i++)
            {
                var ct = rawDetails[i];
                tongTien += ct.ThanhTien;
                int rowIdx = dgvChiTiet.Rows.Add(
                    i + 1,
                    ct.TenMon,
                    ct.SoLuong,
                    FormatTien(ct.DonGia),
                    FormatTien(ct.ThanhTien)
                );
                dgvChiTiet.Rows[rowIdx].Tag = ct.MaCt;
            }

            lblTongCong.Text = $"Tổng cộng: {FormatTien(tongTien)} đ";

            // Cập nhật lại tổng tiền ở dòng tương ứng bên lưới trái
            UpdateHoaDonRowTongTien(maHd, tongTien);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tải chi tiết hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateHoaDonRowTongTien(int maHd, decimal tongTien)
    {
        foreach (DataGridViewRow row in dgvHoaDon.Rows)
        {
            if (row.Tag is int id && id == maHd)
            {
                row.Cells[4].Value = FormatTien(tongTien);
                break;
            }
        }

        // Cập nhật lại tổng doanh thu trên lưới
        decimal tongDoanhThu = 0;
        foreach (DataGridViewRow row in dgvHoaDon.Rows)
        {
            string strVal = row.Cells[4].Value?.ToString() ?? "0";
            if (decimal.TryParse(strVal.Replace(".", "").Replace(",", ""), out decimal val))
            {
                tongDoanhThu += val;
            }
        }
        lblTongDoanhThu.Text = $"Tổng doanh thu đang hiển thị: {FormatTien(tongDoanhThu)} đ";
    }

    private void DgvChiTiet_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvChiTiet.CurrentRow != null && int.TryParse(dgvChiTiet.CurrentRow.Cells[2].Value?.ToString(), out int sl))
        {
            numSoLuong.Value = Math.Clamp(sl, (int)numSoLuong.Minimum, (int)numSoLuong.Maximum);
        }
    }

    // ==========================================
    // THÊM - SỬA - XÓA HÓA ĐƠN
    // ==========================================

    private async Task TaoHoaDonMoiAsync()
    {
        try
        {
            if (cboSoBan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn số bàn.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new QuanCaPheContext();
            var hd = new HoaDon
            {
                NgayLap = dtpNgayLap.Value,
                SoBan = (int)cboSoBan.SelectedItem,
                HinhThucTt = rdoTienMat.Checked ? "Tiền mặt" : "Chuyển khoản",
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            db.HoaDons.Add(hd);
            await db.SaveChangesAsync();

            MessageBox.Show($"Đã tạo thành công hóa đơn HD{hd.MaHd:D6}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadInvoicesAsync(hd.MaHd);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tạo hóa đơn mới: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SuaHoaDonAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboSoBan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn số bàn.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new QuanCaPheContext();
            var hd = await db.HoaDons.FindAsync(maHd);
            if (hd == null)
            {
                MessageBox.Show("Không tìm thấy hóa đơn cần cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            hd.NgayLap = dtpNgayLap.Value;
            hd.SoBan = (int)cboSoBan.SelectedItem;
            hd.HinhThucTt = rdoTienMat.Checked ? "Tiền mặt" : "Chuyển khoản";
            hd.GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim();

            await db.SaveChangesAsync();
            MessageBox.Show($"Đã cập nhật thông tin hóa đơn HD{maHd:D6}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadInvoicesAsync(maHd);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi sửa hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task XoaHoaDonAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialog = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa hóa đơn HD{maHd:D6} không?\nToàn bộ các dòng món của hóa đơn này cũng sẽ bị xóa.",
                "Xác nhận xóa hóa đơn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dialog != DialogResult.Yes) return;

            using var db = new QuanCaPheContext();
            var chiTiets = await db.ChiTietHoaDons.Where(ct => ct.MaHd == maHd).ToListAsync();
            db.ChiTietHoaDons.RemoveRange(chiTiets);

            var hd = await db.HoaDons.FindAsync(maHd);
            if (hd != null)
            {
                db.HoaDons.Remove(hd);
            }

            await db.SaveChangesAsync();
            MessageBox.Show($"Đã xóa hóa đơn HD{maHd:D6} thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            selectedInvoiceId = null;
            await LoadInvoicesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi xóa hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ==========================================
    // THÊM - SỬA - XÓA CHI TIẾT MÓN
    // ==========================================

    private async Task ThemMonAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn trước khi thêm món.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboMon.SelectedValue is not int maMon)
            {
                MessageBox.Show("Vui lòng chọn món cần thêm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soLuong = (int)numSoLuong.Value;
            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new QuanCaPheContext();
            var mon = await db.Mons.FindAsync(maMon);
            if (mon == null)
            {
                MessageBox.Show("Món không tồn tại trong CSDL.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var chiTietMoi = new ChiTietHoaDon
            {
                MaHd = maHd,
                MaMon = mon.MaMon,
                SoLuong = soLuong,
                DonGia = mon.DonGia // Giá tại thời điểm bán
            };

            db.ChiTietHoaDons.Add(chiTietMoi);
            await db.SaveChangesAsync();

            await HienThiChiTietHoaDonAsync(maHd);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi thêm món: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SuaSoLuongMonAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvChiTiet.CurrentRow?.Tag is not int maCt)
            {
                MessageBox.Show("Vui lòng chọn một dòng món trong lưới chi tiết cần sửa số lượng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soLuong = (int)numSoLuong.Value;
            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new QuanCaPheContext();
            var ct = await db.ChiTietHoaDons.FindAsync(maCt);
            if (ct == null)
            {
                MessageBox.Show("Không tìm thấy dòng món cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ct.SoLuong = soLuong;
            await db.SaveChangesAsync();

            await HienThiChiTietHoaDonAsync(maHd);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi sửa số lượng món: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task XoaMonAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvChiTiet.CurrentRow?.Tag is not int maCt)
            {
                MessageBox.Show("Vui lòng chọn dòng món trong lưới chi tiết cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Bạn có muốn xóa món đang chọn khỏi hóa đơn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            using var db = new QuanCaPheContext();
            var ct = await db.ChiTietHoaDons.FindAsync(maCt);
            if (ct != null)
            {
                db.ChiTietHoaDons.Remove(ct);
                await db.SaveChangesAsync();
            }

            await HienThiChiTietHoaDonAsync(maHd);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi xóa món: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ==========================================
    // FASTREPORT: XEM, XUẤT PDF, IN
    // ==========================================

    private async Task<Report?> ChuanBiReportAsync(int maHd)
    {
        using var db = new QuanCaPheContext();
        var hd = await db.HoaDons.AsNoTracking().FirstOrDefaultAsync(h => h.MaHd == maHd);
        if (hd == null)
        {
            MessageBox.Show("Không tìm thấy hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        var chiTiets = await db.ChiTietHoaDons
            .AsNoTracking()
            .Where(c => c.MaHd == maHd)
            .OrderBy(c => c.MaCt)
            .Select(c => new
            {
                c.MaMonNavigation.TenMon,
                c.SoLuong,
                c.DonGia,
                ThanhTien = c.SoLuong * c.DonGia
            })
            .ToListAsync();

        if (chiTiets.Count == 0)
        {
            MessageBox.Show("Hóa đơn chưa có món nào, không thể in hoặc xuất PDF.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        var danhSachIn = chiTiets.Select((x, idx) => new ChiTietIn
        {
            STT = idx + 1,
            TenMon = x.TenMon,
            SoLuong = x.SoLuong,
            DonGia = x.DonGia,
            ThanhTien = x.ThanhTien
        }).ToList();

        var report = new Report();
        string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "HoaDon.frx");
        if (!File.Exists(reportPath))
        {
            reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "HoaDon.frx");
        }
        if (!File.Exists(reportPath))
        {
            MessageBox.Show($"Không tìm thấy file mẫu report: {reportPath}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }

        report.Load(reportPath);
        report.RegisterData(danhSachIn, "ChiTiet");
        report.GetDataSource("ChiTiet").Enabled = true;

        report.SetParameterValue("TenQuan", "QUÁN CÀ PHÊ GÓC PHỐ");
        report.SetParameterValue("SoHD", $"HD{hd.MaHd:D6}");
        report.SetParameterValue("NgayLap", hd.NgayLap.ToString("dd/MM/yyyy HH:mm"));
        report.SetParameterValue("SoBan", hd.SoBan.ToString());
        report.SetParameterValue("HinhThucTT", hd.HinhThucTt);

        report.Prepare();
        return report;
    }

    private async Task XemHoaDonReportAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để xem.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var report = await ChuanBiReportAsync(maHd);
            if (report == null) return;

            report.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi mở xem trước hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task XuatPdfReportAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để xuất PDF.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var report = await ChuanBiReportAsync(maHd);
            if (report == null) return;

            using var sfd = new SaveFileDialog
            {
                Filter = "Tài liệu PDF (*.pdf)|*.pdf",
                FileName = $"HoaDon_HD{maHd:D6}.pdf",
                Title = "Chọn nơi lưu file hóa đơn PDF"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                using var pdfExport = new PDFExport();
                report.Export(pdfExport, sfd.FileName);
                MessageBox.Show($"Xuất file PDF thành công:\n{sfd.FileName}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task InHoaDonReportAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để in.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var report = await ChuanBiReportAsync(maHd);
            if (report == null) return;

            report.Print();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi in hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Xuất Excel bằng ClosedXML theo yêu cầu chung
    public async Task XuatExcelClosedXmlAsync()
    {
        try
        {
            if (selectedInvoiceId is not int maHd)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để xuất Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new QuanCaPheContext();
            var hd = await db.HoaDons.AsNoTracking().FirstOrDefaultAsync(h => h.MaHd == maHd);
            if (hd == null) return;

            var chiTiets = await db.ChiTietHoaDons
                .AsNoTracking()
                .Where(c => c.MaHd == maHd)
                .OrderBy(c => c.MaCt)
                .Select(c => new
                {
                    c.MaMonNavigation.TenMon,
                    c.SoLuong,
                    c.DonGia,
                    ThanhTien = c.SoLuong * c.DonGia
                })
                .ToListAsync();

            if (chiTiets.Count == 0)
            {
                MessageBox.Show("Hóa đơn chưa có món, không thể xuất Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"HoaDon_HD{maHd:D6}.xlsx",
                Title = "Chọn nơi lưu file Excel"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                using var wb = new ClosedXML.Excel.XLWorkbook();
                var ws = wb.Worksheets.Add("HoaDon");
                ws.Cell(1, 1).Value = "QUÁN CÀ PHÊ GÓC PHỐ";
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(2, 1).Value = "HÓA ĐƠN THANH TOÁN";
                ws.Cell(2, 1).Style.Font.Bold = true;
                ws.Cell(3, 1).Value = $"Số HĐ: HD{hd.MaHd:D6}";
                ws.Cell(3, 3).Value = $"Ngày: {hd.NgayLap:dd/MM/yyyy HH:mm}";
                ws.Cell(4, 1).Value = $"Bàn: {hd.SoBan}";
                ws.Cell(4, 3).Value = $"Hình thức: {hd.HinhThucTt}";

                ws.Cell(6, 1).Value = "STT";
                ws.Cell(6, 2).Value = "Tên món";
                ws.Cell(6, 3).Value = "SL";
                ws.Cell(6, 4).Value = "Đơn giá";
                ws.Cell(6, 5).Value = "Thành tiền";
                ws.Range(6, 1, 6, 5).Style.Font.Bold = true;

                decimal tong = 0;
                for (int i = 0; i < chiTiets.Count; i++)
                {
                    var item = chiTiets[i];
                    tong += item.ThanhTien;
                    ws.Cell(7 + i, 1).Value = i + 1;
                    ws.Cell(7 + i, 2).Value = item.TenMon;
                    ws.Cell(7 + i, 3).Value = item.SoLuong;
                    ws.Cell(7 + i, 4).Value = item.DonGia;
                    ws.Cell(7 + i, 4).Style.NumberFormat.Format = "#,##0";
                    ws.Cell(7 + i, 5).Value = item.ThanhTien;
                    ws.Cell(7 + i, 5).Style.NumberFormat.Format = "#,##0";
                }

                int sumRow = 7 + chiTiets.Count;
                ws.Cell(sumRow, 4).Value = "Tổng cộng:";
                ws.Cell(sumRow, 4).Style.Font.Bold = true;
                ws.Cell(sumRow, 5).Value = tong;
                ws.Cell(sumRow, 5).Style.Font.Bold = true;
                ws.Cell(sumRow, 5).Style.NumberFormat.Format = "#,##0";

                ws.Columns().AdjustToContents();
                wb.SaveAs(sfd.FileName);
                MessageBox.Show($"Xuất file Excel thành công:\n{sfd.FileName}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

public class MonItemView
{
    public int MaMon { get; set; }
    public string TenMon { get; set; } = "";
    public decimal DonGia { get; set; }
    public string HienThi { get; set; } = "";
}

public class ChiTietIn
{
    public int STT { get; set; }
    public string TenMon { get; set; } = "";
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}
