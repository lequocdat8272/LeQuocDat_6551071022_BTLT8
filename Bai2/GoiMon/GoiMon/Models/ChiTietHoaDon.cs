using System;
using System.Collections.Generic;

namespace GoiMon.Models;

public partial class ChiTietHoaDon
{
    public int MaCt { get; set; }

    public int MaHd { get; set; }

    public int MaMon { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual Mon MaMonNavigation { get; set; } = null!;
}
