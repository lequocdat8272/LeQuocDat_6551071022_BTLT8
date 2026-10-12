using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastReport;
using FastReport.Export.Pdf;
using GoiMon;
using GoiMon.Models;
using Microsoft.EntityFrameworkCore;

namespace GoiMon;

public static class SamplePdfGen
{
    public static async Task GenerateAsync()
    {
        using var db = new QuanCaPheContext();
        var hd = await db.HoaDons.AsNoTracking().FirstOrDefaultAsync(h => h.MaHd == 1);
        if (hd == null) return;
        var chiTiets = await db.ChiTietHoaDons.AsNoTracking().Where(c => c.MaHd == 1).Include(c => c.MaMonNavigation).ToListAsync();
        var danhSachIn = chiTiets.Select((x, idx) => new ChiTietIn
        {
            STT = idx + 1,
            TenMon = x.MaMonNavigation.TenMon,
            SoLuong = x.SoLuong,
            DonGia = x.DonGia,
            ThanhTien = x.SoLuong * x.DonGia
        }).ToList();
        var report = new Report();
        string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "HoaDon.frx");
        if (!File.Exists(reportPath)) reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "HoaDon.frx");
        report.Load(reportPath);
        report.RegisterData(danhSachIn, "ChiTiet");
        report.GetDataSource("ChiTiet").Enabled = true;
        report.SetParameterValue("TenQuan", "QUÁN CÀ PHÊ GÓC PHỐ");
        report.SetParameterValue("SoHD", $"HD{hd.MaHd:D6}");
        report.SetParameterValue("NgayLap", hd.NgayLap.ToString("dd/MM/yyyy HH:mm"));
        report.SetParameterValue("SoBan", hd.SoBan.ToString());
        report.SetParameterValue("HinhThucTT", hd.HinhThucTt);
        report.Prepare();
        var pdf = new PDFExport();
        string outPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "HoaDon_HD000001.pdf");
        report.Export(pdf, Path.GetFullPath(outPath));
        Console.WriteLine("SUCCESS_EXPORT_PDF: " + Path.GetFullPath(outPath));
    }
}
