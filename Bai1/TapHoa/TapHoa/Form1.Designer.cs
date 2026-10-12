namespace TapHoa
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gbThongTin = new GroupBox();
            chkConKinhDoanh = new CheckBox();
            dtpNgayNhap = new DateTimePicker();
            numSoLuongTon = new NumericUpDown();
            numGiaBan = new NumericUpDown();
            cboDonVi = new ComboBox();
            cboLoaiHang = new ComboBox();
            txtTenHang = new TextBox();
            lblNgayNhap = new Label();
            lblSoLuongTon = new Label();
            lblGiaBan = new Label();
            lblDonVi = new Label();
            lblLoaiHang = new Label();
            lblTenHang = new Label();
            gbTimKiem = new GroupBox();
            chkLocConKinhDoanh = new CheckBox();
            cboLocLoaiHang = new ComboBox();
            txtTimKiem = new TextBox();
            dgvHangHoa = new DataGridView();
            colMaHang = new DataGridViewTextBoxColumn();
            colTenHang = new DataGridViewTextBoxColumn();
            colLoaiHang = new DataGridViewTextBoxColumn();
            colDonVi = new DataGridViewTextBoxColumn();
            colGiaBan = new DataGridViewTextBoxColumn();
            colSoLuongTon = new DataGridViewTextBoxColumn();
            colNgayNhap = new DataGridViewTextBoxColumn();
            colConKinhDoanh = new DataGridViewCheckBoxColumn();
            btnThem = new Button();
            btnSua = new Button();
            btnXoa = new Button();
            btnLamMoi = new Button();
            btnXemBangGia = new Button();
            btnXuatPdf = new Button();
            btnXuatExcelLuoi = new Button();
            btnXuatExcelTonKho = new Button();
            lblTrangThai = new Label();
            timerDebounce = new System.Windows.Forms.Timer(components);
            gbThongTin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSoLuongTon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numGiaBan).BeginInit();
            gbTimKiem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHangHoa).BeginInit();
            SuspendLayout();
            // 
            // gbThongTin
            // 
            gbThongTin.Controls.Add(chkConKinhDoanh);
            gbThongTin.Controls.Add(dtpNgayNhap);
            gbThongTin.Controls.Add(numSoLuongTon);
            gbThongTin.Controls.Add(numGiaBan);
            gbThongTin.Controls.Add(cboDonVi);
            gbThongTin.Controls.Add(cboLoaiHang);
            gbThongTin.Controls.Add(txtTenHang);
            gbThongTin.Controls.Add(lblNgayNhap);
            gbThongTin.Controls.Add(lblSoLuongTon);
            gbThongTin.Controls.Add(lblGiaBan);
            gbThongTin.Controls.Add(lblDonVi);
            gbThongTin.Controls.Add(lblLoaiHang);
            gbThongTin.Controls.Add(lblTenHang);
            gbThongTin.Location = new Point(14, 12);
            gbThongTin.Name = "gbThongTin";
            gbThongTin.Size = new Size(590, 160);
            gbThongTin.TabIndex = 0;
            gbThongTin.TabStop = false;
            gbThongTin.Text = "Thông tin hàng hóa";
            // 
            // chkConKinhDoanh
            // 
            chkConKinhDoanh.AutoSize = true;
            chkConKinhDoanh.Checked = true;
            chkConKinhDoanh.CheckState = CheckState.Checked;
            chkConKinhDoanh.Location = new Point(420, 125);
            chkConKinhDoanh.Name = "chkConKinhDoanh";
            chkConKinhDoanh.Size = new Size(110, 19);
            chkConKinhDoanh.TabIndex = 12;
            chkConKinhDoanh.Text = "Còn kinh doanh";
            chkConKinhDoanh.UseVisualStyleBackColor = true;
            // 
            // dtpNgayNhap
            // 
            dtpNgayNhap.CustomFormat = "dd/MM/yyyy";
            dtpNgayNhap.Format = DateTimePickerFormat.Custom;
            dtpNgayNhap.Location = new Point(420, 90);
            dtpNgayNhap.Name = "dtpNgayNhap";
            dtpNgayNhap.Size = new Size(150, 23);
            dtpNgayNhap.TabIndex = 11;
            // 
            // numSoLuongTon
            // 
            numSoLuongTon.Location = new Point(420, 56);
            numSoLuongTon.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numSoLuongTon.Name = "numSoLuongTon";
            numSoLuongTon.Size = new Size(150, 23);
            numSoLuongTon.TabIndex = 10;
            // 
            // numGiaBan
            // 
            numGiaBan.Increment = new decimal(new int[] { 500, 0, 0, 0 });
            numGiaBan.Location = new Point(95, 124);
            numGiaBan.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numGiaBan.Name = "numGiaBan";
            numGiaBan.Size = new Size(180, 23);
            numGiaBan.TabIndex = 9;
            numGiaBan.ThousandsSeparator = true;
            // 
            // cboDonVi
            // 
            cboDonVi.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDonVi.FormattingEnabled = true;
            cboDonVi.Location = new Point(95, 90);
            cboDonVi.Name = "cboDonVi";
            cboDonVi.Size = new Size(180, 23);
            cboDonVi.TabIndex = 8;
            // 
            // cboLoaiHang
            // 
            cboLoaiHang.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLoaiHang.FormattingEnabled = true;
            cboLoaiHang.Location = new Point(95, 56);
            cboLoaiHang.Name = "cboLoaiHang";
            cboLoaiHang.Size = new Size(180, 23);
            cboLoaiHang.TabIndex = 7;
            // 
            // txtTenHang
            // 
            txtTenHang.Location = new Point(95, 23);
            txtTenHang.Name = "txtTenHang";
            txtTenHang.Size = new Size(475, 23);
            txtTenHang.TabIndex = 6;
            // 
            // lblNgayNhap
            // 
            lblNgayNhap.AutoSize = true;
            lblNgayNhap.Location = new Point(310, 94);
            lblNgayNhap.Name = "lblNgayNhap";
            lblNgayNhap.Size = new Size(68, 15);
            lblNgayNhap.TabIndex = 5;
            lblNgayNhap.Text = "Ngày nhập:";
            // 
            // lblSoLuongTon
            // 
            lblSoLuongTon.AutoSize = true;
            lblSoLuongTon.Location = new Point(310, 60);
            lblSoLuongTon.Name = "lblSoLuongTon";
            lblSoLuongTon.Size = new Size(78, 15);
            lblSoLuongTon.TabIndex = 4;
            lblSoLuongTon.Text = "Số lượng tồn:";
            // 
            // lblGiaBan
            // 
            lblGiaBan.AutoSize = true;
            lblGiaBan.Location = new Point(15, 127);
            lblGiaBan.Name = "lblGiaBan";
            lblGiaBan.Size = new Size(50, 15);
            lblGiaBan.TabIndex = 3;
            lblGiaBan.Text = "Giá bán:";
            // 
            // lblDonVi
            // 
            lblDonVi.AutoSize = true;
            lblDonVi.Location = new Point(15, 94);
            lblDonVi.Name = "lblDonVi";
            lblDonVi.Size = new Size(44, 15);
            lblDonVi.TabIndex = 2;
            lblDonVi.Text = "Đơn vị:";
            // 
            // lblLoaiHang
            // 
            lblLoaiHang.AutoSize = true;
            lblLoaiHang.Location = new Point(15, 60);
            lblLoaiHang.Name = "lblLoaiHang";
            lblLoaiHang.Size = new Size(62, 15);
            lblLoaiHang.TabIndex = 1;
            lblLoaiHang.Text = "Loại hàng:";
            // 
            // lblTenHang
            // 
            lblTenHang.AutoSize = true;
            lblTenHang.Location = new Point(15, 26);
            lblTenHang.Name = "lblTenHang";
            lblTenHang.Size = new Size(58, 15);
            lblTenHang.TabIndex = 0;
            lblTenHang.Text = "Tên hàng:";
            // 
            // gbTimKiem
            // 
            gbTimKiem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbTimKiem.Controls.Add(chkLocConKinhDoanh);
            gbTimKiem.Controls.Add(cboLocLoaiHang);
            gbTimKiem.Controls.Add(txtTimKiem);
            gbTimKiem.Location = new Point(615, 12);
            gbTimKiem.Name = "gbTimKiem";
            gbTimKiem.Size = new Size(355, 160);
            gbTimKiem.TabIndex = 1;
            gbTimKiem.TabStop = false;
            gbTimKiem.Text = "Tìm kiếm";
            // 
            // chkLocConKinhDoanh
            // 
            chkLocConKinhDoanh.AutoSize = true;
            chkLocConKinhDoanh.Location = new Point(16, 93);
            chkLocConKinhDoanh.Name = "chkLocConKinhDoanh";
            chkLocConKinhDoanh.Size = new Size(181, 19);
            chkLocConKinhDoanh.TabIndex = 2;
            chkLocConKinhDoanh.Text = "Chỉ hiện hàng còn kinh doanh";
            chkLocConKinhDoanh.UseVisualStyleBackColor = true;
            // 
            // cboLocLoaiHang
            // 
            cboLocLoaiHang.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboLocLoaiHang.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLocLoaiHang.FormattingEnabled = true;
            cboLocLoaiHang.Location = new Point(16, 56);
            cboLocLoaiHang.Name = "cboLocLoaiHang";
            cboLocLoaiHang.Size = new Size(325, 23);
            cboLocLoaiHang.TabIndex = 1;
            // 
            // txtTimKiem
            // 
            txtTimKiem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTimKiem.Location = new Point(16, 23);
            txtTimKiem.Name = "txtTimKiem";
            txtTimKiem.Size = new Size(325, 23);
            txtTimKiem.TabIndex = 0;
            // 
            // dgvHangHoa
            // 
            dgvHangHoa.AllowUserToAddRows = false;
            dgvHangHoa.AllowUserToDeleteRows = false;
            dgvHangHoa.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvHangHoa.BackgroundColor = SystemColors.ControlDark;
            dgvHangHoa.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHangHoa.Columns.AddRange(new DataGridViewColumn[] { colMaHang, colTenHang, colLoaiHang, colDonVi, colGiaBan, colSoLuongTon, colNgayNhap, colConKinhDoanh });
            dgvHangHoa.Location = new Point(14, 185);
            dgvHangHoa.MultiSelect = false;
            dgvHangHoa.Name = "dgvHangHoa";
            dgvHangHoa.ReadOnly = true;
            dgvHangHoa.RowHeadersWidth = 35;
            dgvHangHoa.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHangHoa.Size = new Size(956, 310);
            dgvHangHoa.TabIndex = 2;
            // 
            // colMaHang
            // 
            colMaHang.DataPropertyName = "MaHang";
            colMaHang.HeaderText = "Mã";
            colMaHang.Name = "colMaHang";
            colMaHang.ReadOnly = true;
            colMaHang.Width = 55;
            // 
            // colTenHang
            // 
            colTenHang.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTenHang.DataPropertyName = "TenHang";
            colTenHang.HeaderText = "Tên hàng";
            colTenHang.Name = "colTenHang";
            colTenHang.ReadOnly = true;
            // 
            // colLoaiHang
            // 
            colLoaiHang.DataPropertyName = "LoaiHang";
            colLoaiHang.HeaderText = "Loại hàng";
            colLoaiHang.Name = "colLoaiHang";
            colLoaiHang.ReadOnly = true;
            colLoaiHang.Width = 115;
            // 
            // colDonVi
            // 
            colDonVi.DataPropertyName = "DonVi";
            colDonVi.HeaderText = "Đơn vị";
            colDonVi.Name = "colDonVi";
            colDonVi.ReadOnly = true;
            colDonVi.Width = 80;
            // 
            // colGiaBan
            // 
            colGiaBan.DataPropertyName = "GiaBan";
            colGiaBan.HeaderText = "Giá bán";
            colGiaBan.Name = "colGiaBan";
            colGiaBan.ReadOnly = true;
            colGiaBan.Width = 110;
            // 
            // colSoLuongTon
            // 
            colSoLuongTon.DataPropertyName = "SoLuongTon";
            colSoLuongTon.HeaderText = "Tồn";
            colSoLuongTon.Name = "colSoLuongTon";
            colSoLuongTon.ReadOnly = true;
            colSoLuongTon.Width = 80;
            // 
            // colNgayNhap
            // 
            colNgayNhap.DataPropertyName = "NgayNhap";
            colNgayNhap.HeaderText = "Ngày nhập";
            colNgayNhap.Name = "colNgayNhap";
            colNgayNhap.ReadOnly = true;
            colNgayNhap.Width = 110;
            // 
            // colConKinhDoanh
            // 
            colConKinhDoanh.DataPropertyName = "ConKinhDoanh";
            colConKinhDoanh.HeaderText = "Kinh doanh";
            colConKinhDoanh.Name = "colConKinhDoanh";
            colConKinhDoanh.ReadOnly = true;
            colConKinhDoanh.Width = 85;
            // 
            // btnThem
            // 
            btnThem.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnThem.Location = new Point(14, 510);
            btnThem.Name = "btnThem";
            btnThem.Size = new Size(88, 32);
            btnThem.TabIndex = 3;
            btnThem.Text = "Thêm";
            btnThem.UseVisualStyleBackColor = true;
            // 
            // btnSua
            // 
            btnSua.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSua.Location = new Point(108, 510);
            btnSua.Name = "btnSua";
            btnSua.Size = new Size(88, 32);
            btnSua.TabIndex = 4;
            btnSua.Text = "Sửa";
            btnSua.UseVisualStyleBackColor = true;
            // 
            // btnXoa
            // 
            btnXoa.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnXoa.Location = new Point(202, 510);
            btnXoa.Name = "btnXoa";
            btnXoa.Size = new Size(88, 32);
            btnXoa.TabIndex = 5;
            btnXoa.Text = "Xóa";
            btnXoa.UseVisualStyleBackColor = true;
            // 
            // btnLamMoi
            // 
            btnLamMoi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLamMoi.Location = new Point(296, 510);
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Size = new Size(88, 32);
            btnLamMoi.TabIndex = 6;
            btnLamMoi.Text = "Làm mới";
            btnLamMoi.UseVisualStyleBackColor = true;
            // 
            // btnXemBangGia
            // 
            btnXemBangGia.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnXemBangGia.Location = new Point(480, 510);
            btnXemBangGia.Name = "btnXemBangGia";
            btnXemBangGia.Size = new Size(110, 32);
            btnXemBangGia.TabIndex = 7;
            btnXemBangGia.Text = "Xem bảng giá";
            btnXemBangGia.UseVisualStyleBackColor = true;
            // 
            // btnXuatPdf
            // 
            btnXuatPdf.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnXuatPdf.Location = new Point(596, 510);
            btnXuatPdf.Name = "btnXuatPdf";
            btnXuatPdf.Size = new Size(100, 32);
            btnXuatPdf.TabIndex = 8;
            btnXuatPdf.Text = "Xuất PDF";
            btnXuatPdf.UseVisualStyleBackColor = true;
            // 
            // btnXuatExcelLuoi
            // 
            btnXuatExcelLuoi.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnXuatExcelLuoi.Location = new Point(702, 510);
            btnXuatExcelLuoi.Name = "btnXuatExcelLuoi";
            btnXuatExcelLuoi.Size = new Size(125, 32);
            btnXuatExcelLuoi.TabIndex = 9;
            btnXuatExcelLuoi.Text = "Xuất Excel (lưới)";
            btnXuatExcelLuoi.UseVisualStyleBackColor = true;
            // 
            // btnXuatExcelTonKho
            // 
            btnXuatExcelTonKho.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnXuatExcelTonKho.Location = new Point(833, 510);
            btnXuatExcelTonKho.Name = "btnXuatExcelTonKho";
            btnXuatExcelTonKho.Size = new Size(137, 32);
            btnXuatExcelTonKho.TabIndex = 10;
            btnXuatExcelTonKho.Text = "Xuất Excel tồn kho";
            btnXuatExcelTonKho.UseVisualStyleBackColor = true;
            // 
            // lblTrangThai
            // 
            lblTrangThai.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTrangThai.AutoSize = true;
            lblTrangThai.Location = new Point(14, 555);
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Size = new Size(123, 15);
            lblTrangThai.TabIndex = 11;
            lblTrangThai.Text = "Tìm thấy 0 mặt hàng";
            // 
            // timerDebounce
            // 
            timerDebounce.Interval = 350;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 581);
            Controls.Add(lblTrangThai);
            Controls.Add(btnXuatExcelTonKho);
            Controls.Add(btnXuatExcelLuoi);
            Controls.Add(btnXuatPdf);
            Controls.Add(btnXemBangGia);
            Controls.Add(btnLamMoi);
            Controls.Add(btnXoa);
            Controls.Add(btnSua);
            Controls.Add(btnThem);
            Controls.Add(dgvHangHoa);
            Controls.Add(gbTimKiem);
            Controls.Add(gbThongTin);
            MinimumSize = new Size(950, 550);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý hàng hóa - Tạp hóa UTC2";
            gbThongTin.ResumeLayout(false);
            gbThongTin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSoLuongTon).EndInit();
            ((System.ComponentModel.ISupportInitialize)numGiaBan).EndInit();
            gbTimKiem.ResumeLayout(false);
            gbTimKiem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHangHoa).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox gbThongTin;
        private Label lblTenHang;
        private Label lblLoaiHang;
        private Label lblDonVi;
        private Label lblGiaBan;
        private Label lblSoLuongTon;
        private Label lblNgayNhap;
        private TextBox txtTenHang;
        private ComboBox cboLoaiHang;
        private ComboBox cboDonVi;
        private NumericUpDown numGiaBan;
        private NumericUpDown numSoLuongTon;
        private DateTimePicker dtpNgayNhap;
        private CheckBox chkConKinhDoanh;
        private GroupBox gbTimKiem;
        private TextBox txtTimKiem;
        private ComboBox cboLocLoaiHang;
        private CheckBox chkLocConKinhDoanh;
        private DataGridView dgvHangHoa;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnXemBangGia;
        private Button btnXuatPdf;
        private Button btnXuatExcelLuoi;
        private Button btnXuatExcelTonKho;
        private Label lblTrangThai;
        private System.Windows.Forms.Timer timerDebounce;
        private DataGridViewTextBoxColumn colMaHang;
        private DataGridViewTextBoxColumn colTenHang;
        private DataGridViewTextBoxColumn colLoaiHang;
        private DataGridViewTextBoxColumn colDonVi;
        private DataGridViewTextBoxColumn colGiaBan;
        private DataGridViewTextBoxColumn colSoLuongTon;
        private DataGridViewTextBoxColumn colNgayNhap;
        private DataGridViewCheckBoxColumn colConKinhDoanh;
    }
}
