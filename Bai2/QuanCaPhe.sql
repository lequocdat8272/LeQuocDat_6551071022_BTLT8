USE master;
GO

IF DB_ID('QuanCaPhe') IS NOT NULL
BEGIN
    ALTER DATABASE QuanCaPhe SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanCaPhe;
END
GO

CREATE DATABASE QuanCaPhe;
GO

USE QuanCaPhe;
GO

-- 1. Bảng Mon
CREATE TABLE Mon (
    MaMon INT IDENTITY(1,1) PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL,
    NhomMon NVARCHAR(30) NOT NULL,
    DonGia DECIMAL(18, 0) NOT NULL
);
GO

-- 2. Bảng HoaDon
CREATE TABLE HoaDon (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    NgayLap DATETIME NOT NULL,
    SoBan INT NOT NULL CHECK (SoBan >= 1 AND SoBan <= 10),
    HinhThucTT NVARCHAR(20) NOT NULL CHECK (HinhThucTT IN (N'Tiền mặt', N'Chuyển khoản')),
    GhiChu NVARCHAR(200) NULL
);
GO

-- 3. Bảng ChiTietHoaDon
CREATE TABLE ChiTietHoaDon (
    MaCT INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL,
    MaMon INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18, 0) NOT NULL,
    CONSTRAINT FK_ChiTietHoaDon_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietHoaDon_Mon FOREIGN KEY (MaMon) REFERENCES Mon(MaMon)
);
GO

-- Dữ liệu mẫu bảng Mon
SET IDENTITY_INSERT Mon ON;
INSERT INTO Mon (MaMon, TenMon, NhomMon, DonGia) VALUES
(1, N'Cà phê sữa đá', N'Cà phê', 29000),
(2, N'Bạc xỉu', N'Cà phê', 32000),
(3, N'Trà đào cam sả', N'Trà', 45000),
(4, N'Bánh tiramisu', N'Bánh', 39000),
(5, N'Nước suối', N'Nước đóng chai', 12000);
SET IDENTITY_INSERT Mon OFF;
GO

-- Dữ liệu mẫu bảng HoaDon
SET IDENTITY_INSERT HoaDon ON;
INSERT INTO HoaDon (MaHD, NgayLap, SoBan, HinhThucTT, GhiChu) VALUES
(1, '2026-10-09 08:15:00', 3, N'Tiền mặt', NULL),
(2, '2026-10-09 09:40:00', 5, N'Chuyển khoản', NULL);
SET IDENTITY_INSERT HoaDon OFF;
GO

-- Dữ liệu mẫu bảng ChiTietHoaDon
SET IDENTITY_INSERT ChiTietHoaDon ON;
INSERT INTO ChiTietHoaDon (MaCT, MaHD, MaMon, SoLuong, DonGia) VALUES
(1, 1, 1, 2, 29000), -- 2 Cà phê sữa đá
(2, 1, 3, 1, 45000), -- 1 Trà đào cam sả
(3, 1, 4, 1, 39000), -- 1 Bánh tiramisu
(4, 2, 2, 1, 32000), -- 1 Bạc xỉu
(5, 2, 5, 2, 12000);  -- 2 Nước suối
SET IDENTITY_INSERT ChiTietHoaDon OFF;
GO
