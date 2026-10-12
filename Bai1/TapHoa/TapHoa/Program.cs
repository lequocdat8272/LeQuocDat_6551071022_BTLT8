using Microsoft.EntityFrameworkCore;

namespace TapHoa
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            EnsureUnicodeData();

            if (args.Length > 0 && args[0] == "--test")
            {
                RunSelfTest();
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        public static void EnsureUnicodeData()
        {
            try
            {
                using var context = new Models.TiemTapHoaContext();
                var items = context.HangHoas.ToList();
                // Kiểm tra xem có dòng nào bị lỗi mã hóa UTF-8 thành ANSI/Windows-1252 không
                bool hasCorruptedData = items.Any(h => h.TenHang.Contains("Æ") || h.TenHang.Contains("Ã") || h.TenHang.Contains("") || (h.LoaiHang != null && h.LoaiHang.Contains("")));

                if (hasCorruptedData || items.Count == 0)
                {
                    context.HangHoas.RemoveRange(items);
                    context.SaveChanges();

                    context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('HangHoa', RESEED, 0);");

                    context.HangHoas.AddRange(
                        new Models.HangHoa { TenHang = "Nước suối Aquafina 500ml", LoaiHang = "Đồ uống", DonVi = "Chai", GiaBan = 6000, SoLuongTon = 120, NgayNhap = new DateTime(2026, 10, 2), ConKinhDoanh = true },
                        new Models.HangHoa { TenHang = "Mì Hảo Hảo tôm chua cay", LoaiHang = "Thực phẩm khô", DonVi = "Gói", GiaBan = 4500, SoLuongTon = 200, NgayNhap = new DateTime(2026, 10, 2), ConKinhDoanh = true },
                        new Models.HangHoa { TenHang = "Dầu ăn Tường An 1L", LoaiHang = "Gia vị", DonVi = "Chai", GiaBan = 48000, SoLuongTon = 35, NgayNhap = new DateTime(2026, 10, 5), ConKinhDoanh = true },
                        new Models.HangHoa { TenHang = "Bột giặt OMO 800g", LoaiHang = "Đồ gia dụng", DonVi = "Túi", GiaBan = 52000, SoLuongTon = 40, NgayNhap = new DateTime(2026, 10, 5), ConKinhDoanh = true },
                        new Models.HangHoa { TenHang = "Sữa tươi Vinamilk 1L", LoaiHang = "Đồ uống", DonVi = "Hộp", GiaBan = 32000, SoLuongTon = 60, NgayNhap = new DateTime(2026, 10, 8), ConKinhDoanh = true },
                        new Models.HangHoa { TenHang = "Nước mắm Nam Ngư 500ml", LoaiHang = "Gia vị", DonVi = "Chai", GiaBan = 38000, SoLuongTon = 25, NgayNhap = new DateTime(2026, 10, 1), ConKinhDoanh = false }
                    );
                    context.SaveChanges();
                }
            }
            catch
            {
                // Bỏ qua nếu chưa kết nối được CSDL lúc design
            }
        }

        static void RunSelfTest()
        {
            try
            {
                Console.WriteLine("=== BẮT ĐẦU KIỂM THỬ TỰ ĐỘNG ===");

                // 1. Kiểm tra EF Core
                using (var context = new Models.TiemTapHoaContext())
                {
                    var count = context.HangHoas.Count();
                    Console.WriteLine($"[EF Core] Số lượng hàng hóa trong CSDL: {count} dòng");
                    var activeCount = context.HangHoas.Count(h => h.ConKinhDoanh);
                    Console.WriteLine($"[EF Core] Số lượng hàng hóa còn kinh doanh: {activeCount} dòng");
                }

                // 2. Kiểm tra FastReport
                Console.WriteLine("[FastReport] Đang kiểm tra load mẫu và chuẩn bị report...");
                using (var context = new Models.TiemTapHoaContext())
                {
                    var list = context.HangHoas.Where(h => h.ConKinhDoanh).OrderBy(h => h.MaHang).ToList();
                    var dt = new System.Data.DataTable("HangHoa");
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

                    string frxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "BangGia.frx");
                    var report = new FastReport.Report();
                    report.Load(frxPath);
                    report.RegisterData(dt, "HangHoa");
                    var ds = report.GetDataSource("HangHoa");
                    if (ds != null) ds.Enabled = true;
                    report.SetParameterValue("NgayIn", DateTime.Now.ToString("dd/MM/yyyy"));
                    report.Prepare();

                    string testPdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BangGia_Test.pdf");
                    using var pdfExport = new FastReport.Export.Pdf.PDFExport();
                    report.Export(pdfExport, testPdf);
                    Console.WriteLine($"[FastReport] Xuất PDF thành công tại: {testPdf} (Size: {new FileInfo(testPdf).Length} bytes)");
                }

                // 3. Kiểm tra ClosedXML Tồn kho
                Console.WriteLine("[ClosedXML] Đang kiểm tra xuất Excel tồn kho...");
                using (var context = new Models.TiemTapHoaContext())
                {
                    var list = context.HangHoas.Where(h => h.ConKinhDoanh).OrderBy(h => h.MaHang).ToList();
                    using var wb = new ClosedXML.Excel.XLWorkbook();
                    var ws = wb.Worksheets.Add("TonKho");
                    string[] headers = { "STT", "Mã hàng", "Tên hàng", "Loại hàng", "Đơn vị", "Giá bán", "Số lượng tồn", "Giá trị tồn" };
                    for (int c = 0; c < headers.Length; c++)
                    {
                        ws.Cell(1, c + 1).Value = headers[c];
                    }
                    int row = 2;
                    for (int i = 0; i < list.Count; i++)
                    {
                        var it = list[i];
                        ws.Cell(row, 1).SetValue(i + 1);
                        ws.Cell(row, 2).SetValue(it.MaHang);
                        ws.Cell(row, 3).SetValue(it.TenHang);
                        ws.Cell(row, 4).SetValue(it.LoaiHang ?? "");
                        ws.Cell(row, 5).SetValue(it.DonVi ?? "");
                        ws.Cell(row, 6).SetValue((double)it.GiaBan);
                        ws.Cell(row, 7).SetValue(it.SoLuongTon);
                        ws.Cell(row, 8).FormulaA1 = $"=F{row}*G{row}";
                        row++;
                    }
                    int lastRow = row - 1;
                    ws.Cell(row, 1).SetValue("Tổng");
                    ws.Cell(row, 8).FormulaA1 = $"=SUM(H2:H{lastRow})";
                    ws.Range(1, 1, lastRow, 8).SetAutoFilter();
                    ws.SheetView.FreezeRows(1);
                    ws.Columns().AdjustToContents();

                    string testXlsx = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TonKho_Test.xlsx");
                    wb.SaveAs(testXlsx);
                    Console.WriteLine($"[ClosedXML] Xuất Excel tồn kho thành công tại: {testXlsx} (Size: {new FileInfo(testXlsx).Length} bytes)");

                    // Đọc lại kiểm tra nội dung
                    using var verifyWb = new ClosedXML.Excel.XLWorkbook(testXlsx);
                    var vws = verifyWb.Worksheet(1);
                    Console.WriteLine("--- Nội dung các dòng trong file Excel: ---");
                    for (int r = 1; r <= 7; r++)
                    {
                        var line = string.Join(" | ", Enumerable.Range(1, 8).Select(c =>
                        {
                            var cell = vws.Cell(r, c);
                            return cell.HasFormula ? $"Formula: {cell.FormulaA1}" : cell.Value.ToString();
                        }));
                        Console.WriteLine($"Dòng {r}: {line}");
                    }
                }

                Console.WriteLine("=== TẤT CẢ KIỂM THỬ ĐÃ THÀNH CÔNG RỰC RỠ! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI]: {ex}");
            }
        }
    }
}