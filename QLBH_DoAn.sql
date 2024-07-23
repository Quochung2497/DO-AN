Create database QLBH_DoAn
go
use QLBH_DoAN
go

CREATE TABLE Nhanvien (
    MaNV CHAR(5) PRIMARY KEY NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh Char(3),
    DiaChi NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    SDT INT UNIQUE,
	CONSTRAINT CK_GioiTinh CHECK (GioiTinh IN ('Nam', 'Nữ'))
);

CREATE TABLE [User] (
    MaNV CHAR(5) PRIMARY KEY NOT NULL,
    username NVARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    SDT INT NOT NULL UNIQUE,
    CONSTRAINT FK_User_Nhanvien_MaNV FOREIGN KEY (MaNV) REFERENCES Nhanvien(MaNV),
);
CREATE TABLE NCC (
    MaNCC CHAR(5) PRIMARY KEY NOT NULL,
    TenNCC NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(100),
    SDT INT,
    Email NVARCHAR(100)
);

CREATE TABLE HangHoa 
(
    MaHangHoa CHAR(5) PRIMARY KEY NOT NULL,
    TenHangHoa NVARCHAR(100) NOT NULL,
    Gia FLOAT NOT NULL,
    SoLuong INT NOT NULL,
    NCC CHAR(5),
    CONSTRAINT FK_NhaCungCap FOREIGN KEY (NCC) REFERENCES NCC(MaNCC)
);

CREATE TABLE PhieuNhap (
    MaHangHoa CHAR(5) PRIMARY KEY NOT NULL,
    SoLuongNhap INT NOT NULL,
    NgayNhap DATE NOT NULL,
	GiaNhap Float,
    CONSTRAINT FK_HangHoa FOREIGN KEY (MaHangHoa) REFERENCES HangHoa(MaHangHoa)
);
CREATE TABLE KhachHang (
    MaKH CHAR(5) PRIMARY KEY NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(100),
    SDT INT,
    Email NVARCHAR(100)
);
CREATE TABLE HoaDon (
    MaHD CHAR(5) PRIMARY KEY NOT NULL,
    MaKH CHAR(5) NOT NULL,
    MaNV CHAR(5) NOT NULL,
    NgayLap DATE NOT NULL,
    TongTien FLOAT NOT NULL,
    CONSTRAINT FK_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
    CONSTRAINT FK_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);
CREATE TABLE ChiTietHoaDon (
    MaHD CHAR(5) NOT NULL,
    MaHangHoa CHAR(5) NOT NULL,
    SL INT NOT NULL,
    DonGia FLOAT NOT NULL,
    ThanhTien AS (SL * DonGia) PERSISTED,
    CONSTRAINT FK_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    CONSTRAINT FK_HH FOREIGN KEY (MaHangHoa) REFERENCES HangHoa(MaHangHoa),
    PRIMARY KEY (MaHD, MaHangHoa)
);

Drop database QLBH_DoAn
go
--
CREATE TRIGGER trg_UpdateSoLuongTon
ON PhieuNhap
AFTER INSERT
AS
BEGIN
    UPDATE HangHoa
    SET SoLuong = SoLuong + inserted.SoLuongNhap
    FROM HangHoa
    INNER JOIN inserted ON HangHoa.MaHangHoa = inserted.MaHangHoa;
END;

CREATE TRIGGER trg_UpdateSoLuongTon_Delete
ON PhieuNhap
AFTER DELETE
AS
BEGIN
    UPDATE HangHoa
    SET SoLuong = SoLuong - deleted.SoLuongNhap
    FROM HangHoa
    INNER JOIN deleted ON HangHoa.MaHangHoa = deleted.MaHangHoa;
END;

CREATE TRIGGER trg_UpdateSoLuongTon_Update
ON PhieuNhap
AFTER UPDATE
AS
BEGIN
    DECLARE @OldSoLuongNhap INT;
    DECLARE @NewSoLuongNhap INT;
    DECLARE @MaHangHoa CHAR(5);

    SELECT @OldSoLuongNhap = deleted.SoLuongNhap,
           @NewSoLuongNhap = inserted.SoLuongNhap,
           @MaHangHoa = inserted.MaHangHoa
    FROM inserted
    INNER JOIN deleted ON inserted.MaHangHoa = deleted.MaHangHoa;

    IF @OldSoLuongNhap <> @NewSoLuongNhap
    BEGIN
        UPDATE HangHoa
        SET SoLuong = SoLuong - @OldSoLuongNhap + @NewSoLuongNhap
        WHERE MaHangHoa = @MaHangHoa;
    END
END;
--
CREATE TRIGGER trg_UpdateUserEmail
ON Nhanvien
AFTER UPDATE
AS
BEGIN
    IF UPDATE(Email)
    BEGIN
        UPDATE [User]
        SET email = inserted.Email
        FROM inserted
        WHERE [User].MaNV = inserted.MaNV;
    END
END;
CREATE TRIGGER trg_UpdateUserEmail_Delete
ON Nhanvien
AFTER DELETE
AS
BEGIN
    DELETE FROM [User]
    WHERE MaNV IN (SELECT MaNV FROM deleted);
END;
--
CREATE TRIGGER trg_UpdateUserSDT
ON Nhanvien
AFTER UPDATE
AS
BEGIN
    IF UPDATE(SDT)
    BEGIN
        UPDATE [User]
        SET SDT = inserted.SDT
        FROM inserted
        WHERE [User].MaNV = inserted.MaNV;
    END
END;

CREATE TRIGGER trg_UpdateUserSDT_Delete
ON Nhanvien
AFTER DELETE
AS
BEGIN
    DELETE FROM [User]
    WHERE MaNV IN (SELECT MaNV FROM deleted);
END;

UPDATE Nhanvien
SET Email = 'new_a@example.com'
WHERE MaNV = 'NV001';

Drop Table ChiTietHoaDon
Delete from [user] where MaNV = 'NV002'
select * from [user]
select * from Nhanvien
select * from NCC
select * from HangHoa
select count(1) from [User] where (UserName = 'quochung2497' or email = 'quochung020497@gmail.com') and password = 'Azusa0908350929'
Insert Into [PhieuNhap] Values ('HH001', 100, GETDATE());
Delete from [PhieuNhap] where MaHangHoa = 'HH001'
Insert into [User] Values('chauyen','chauyen6793','chauyen6793@gmail.com')
select @@SERVERNAME
DROP TRIGGER IF EXISTS trg_UpdateUserEmail;
DROP TRIGGER IF EXISTS trg_UpdateUserSDT;
DROP TRIGGER IF EXISTS trg_UpdateNhanvienEmail;
DROP TRIGGER IF EXISTS trg_UpdateNhanvienSDT;

CREATE TRIGGER trg_UpdateDonGia
ON HangHoa
AFTER UPDATE
AS
BEGIN
    UPDATE c
    SET c.DonGia = i.Gia
    FROM ChiTietHoaDon c
    JOIN inserted i ON c.MaHangHoa = i.MaHangHoa
    JOIN deleted d ON c.MaHangHoa = d.MaHangHoa
    WHERE NOT EXISTS (SELECT 1 FROM deleted WHERE MaHangHoa = c.MaHangHoa)
          AND c.DonGia <> i.Gia; -- Chỉ cập nhật nếu DonGia khác với Giá mới
    -- Sau khi cập nhật DonGia, ThanhTien sẽ được tính toán tự động
END;
CREATE VIEW ChiTietHoaDonView
AS
SELECT MaHD, MaHangHoa, SL, DonGia, SL * DonGia AS ThanhTien
FROM ChiTietHoaDon;
Drop view ChiTietHoaDonView

ALTER TABLE Nhanvien DROP COLUMN GioiTinh;

-- Thêm cột GioiTinh mới với kiểu CHAR(3)
ALTER TABLE Nhanvien ADD GioiTinh CHAR(3);

-- Chỉ cho phép giá trị 'Nam' hoặc 'Nữ'
ALTER TABLE Nhanvien ADD CONSTRAINT CK_GioiTinh CHECK (GioiTinh IN ('Nam', 'Nữ'));

INSERT INTO HoaDon (MaHD, MaKH, MaNV, NgayLap, TongTien)
VALUES ('HD001', 'KH001', 'NV001', '2024-06-17', 1500000.00)

-- Tạo trigger để cập nhật tổng tiền của HoaDon
CREATE OR ALTER TRIGGER trg_UpdateTongTien
ON ChiTietHoaDon
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @MaHD CHAR(5);
    
    -- Lấy danh sách các Mã HĐ bị ảnh hưởng bởi các thay đổi
    SELECT DISTINCT MaHD INTO #AffectedMaHDs
    FROM inserted
    
    -- Cập nhật lại tổng tiền cho từng Hóa đơn bị ảnh hưởng
    WHILE EXISTS (SELECT * FROM #AffectedMaHDs)
    BEGIN
        SELECT TOP 1 @MaHD = MaHD
        FROM #AffectedMaHDs;

        -- Tính tổng tiền mới dựa trên ChiTietHoaDon
        UPDATE HoaDon
        SET TongTien = (
            SELECT SUM(ThanhTien)
            FROM ChiTietHoaDon
            WHERE MaHD = @MaHD
        )
        WHERE MaHD = @MaHD;

        -- Xóa Mã HĐ đã xử lý
        DELETE FROM #AffectedMaHDs WHERE MaHD = @MaHD;
    END;
END;
CREATE OR ALTER TRIGGER trg_UpdateTongTienAfterDelete
ON ChiTietHoaDon
AFTER DELETE
AS
BEGIN
    DECLARE @MaHD CHAR(5);

    -- Lấy danh sách các Mã HĐ bị ảnh hưởng bởi việc xóa
    SELECT DISTINCT MaHD INTO #AffectedMaHDs
    FROM deleted

    -- Cập nhật lại tổng tiền cho từng Hóa đơn bị ảnh hưởng
    WHILE EXISTS (SELECT * FROM #AffectedMaHDs)
    BEGIN
        SELECT TOP 1 @MaHD = MaHD
        FROM #AffectedMaHDs;

        -- Tính tổng tiền mới dựa trên ChiTietHoaDon còn lại
        UPDATE HoaDon
        SET TongTien = (
            SELECT SUM(ThanhTien)
            FROM ChiTietHoaDon
            WHERE MaHD = @MaHD
        )
        WHERE MaHD = @MaHD;

        -- Xóa Mã HĐ đã xử lý
        DELETE FROM #AffectedMaHDs WHERE MaHD = @MaHD;
    END;
END;