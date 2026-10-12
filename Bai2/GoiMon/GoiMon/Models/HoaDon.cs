using System;
using System.Collections.Generic;

namespace GoiMon.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public DateTime NgayLap { get; set; }

    public int SoBan { get; set; }

    public string HinhThucTt { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
}
