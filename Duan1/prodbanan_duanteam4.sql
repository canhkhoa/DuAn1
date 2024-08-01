SELECT f.tenmon, bi.soluong, f.gia, f.gia * bi.soluong as totalPrice FROM dbo.HoaDonChiTiet as bi, dbo.HoaDon as b, dbo.MonAn as f 
WHERE bi.idHD = b.idHD AND b.TrangThai = 0 AND bi.IdMon = f.idMon AND b.idTable =3

--InsertBill
CREATE OR ALTER PROC sp_InsertBIll @idTable INT
AS
BEGIN
	INSERT dbo.HoaDon(dateCheckIn, dateCheckOut, idTable, trangthai)
	VALUES(GETDATE(), NULL, @idTable, 0)
END
GO
-- Select Max
SELECT MAX(idHD) FROM dbo.HoaDon

--InsertBillInfo
CREATE OR ALTER PROC sp_InsertBillInfo @idBill INT, @idFood INT, @count INT
AS
BEGIN
	DECLARE @isExitsBillInfo INT;
	DECLARE @foodCount INT = 1;
	SELECT @isExitsBillInfo = id , @foodCount = b.soluong 
	FROM dbo.HoaDonChiTiet AS b 
	WHERE idHD = @idBill AND IdMon = @idFood
	
	IF(@isExitsBillInfo > 0)
	BEGIN
		DECLARE @newCount INT = @foodCount + @count
		IF (@newCount > 0)
		BEGIN
			UPDATE dbo.HoaDonChiTiet SET  soluong = @foodCount + @count WHERE IdMon = @idFood
		END
		ELSE
		BEGIN
			DELETE dbo.HoaDonChiTiet WHERE idHD = @idBill AND IdMon = @idFood
		END
	END

	ELSE
	BEGIN
		INSERT dbo.HoaDonChiTiet(idHD, IdMon, soluong)
		VALUES (@idBill, @idFood, @count)
	END
END
GO

--Hien Table
CREATE PROC sp_GetTableList
AS
BEGIN
	SELECT * FROM dbo.BanAn
END


select * from LoaiMonAn
SELECT * FROM MonAn WHERE idLoaiMonAn = 2

SELECT idHD, dateCheckIn, dateCheckOut, idTable, trangthai FROM dbo.HoaDon WHERE idTable = 3 AND trangthai = 0
SELECT * FROM dbo.HoaDon WHERE idTable = 6 AND status = 0
-- Thanh Toan Hoa DOon
UPDATE dbo.HoaDon SET trangthai = 1 WHERE idHD = 1

--Trigger update hoadonchitiet

DROP TRIGGER tg_UpdateBillInfo
DROP TRIGGER tg_UpdateBill

CREATE OR ALTER TRIGGER tg_UpdateBillInfo
ON dbo.HoaDonChiTiet FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	SELECT @idBill = idHD FROM inserted
	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.HoaDon WHERE idHD = @idBill AND trangthai = 0
	UPDATE dbo.BanAn SET trangthai = 'Có Người' WHERE maban = @idTable
END
GO

DELETE dbo.BanAn

delete dbo.HoaDon
delete dbo.HoaDonChiTiet

CREATE OR ALTER TRIGGER tg_UpdateBill
ON dbo.HoaDon FOR UPDATE
AS 
BEGIN
	DECLARE @idBill INT
	SELECT @idBill = idHD FROM inserted
	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.HoaDon WHERE idHD = @idBill
	DECLARE @count INT = 0
	SELECT @count = COUNT(*) FROM dbo.HoaDon WHERE idTable = @idTable AND trangthai = 0
	IF(@count = 0)
		UPDATE dbo.BanAn SET trangthai = 'Trống' WHERE maban = @idTable
END
GO

CREATE OR ALTER PROC DanhSachMonAn
AS
BEGIN
	SELECT * FROM dbo.MonAn
END
EXEC DanhSachMonAn
DROP PROC Insert_MonAn
DROP PROC Delete_MonAn
DROP PROC Update_MonAn
CREATE OR ALTER PROC Insert_MonAn @tenMon NVARCHAR(100), @gia FLOAT, @idLMA INT, @hinhanh image
AS
BEGIN
	INSERT INTO dbo.MonAn(TenMon, Gia, idLoaiMonAn, hinhMonAn)
	VALUES(@tenMon, @gia, @idLMA, @hinhanh)
END
CREATE OR ALTER PROC Delete_MonAn @id INT
AS
BEGIN
	DELETE FROM dbo.MonAn
	WHERE IdMon = @id
END
CREATE OR ALTER PROC Update_MonAn @id INT ,@tenMon NVARCHAR(100), @gia FLOAT, @idLMA INT, @hinhanh image
AS
BEGIN
	UPDATE dbo.MonAn SET TenMon = @tenMon, Gia = @gia , idLoaiMonAn = @idLMA, hinhMonAn = @hinhanh
	WHERE IdMon = @id
END
	
CREATE OR ALTER PROC Insert_BanAn @tenban NVARCHAR(50), @trangthai NVARCHAR(50)
AS
BEGIN
	INSERT INTO dbo.BanAn(tenban, trangthai)
	VALUES(@tenban, @trangthai)
END

CREATE OR ALTER PROC Delete_BanAn @maban INT
AS
BEGIN
	DELETE FROM dbo.BanAn
	WHERE maban = @maban
END

CREATE OR ALTER PROC Update_BanAn @maban INT, @tenban NVARCHAR(50), @trangthai NVARCHAR(50)
AS
BEGIN
	UPDATE dbo.BanAn SET tenban = @tenban , trangthai = @trangthai
	WHERE maban = @maban
END
-- CATEGORY
CREATE OR ALTER PROC GetCategoryList
AS
BEGIN
	select * from LoaiMonAn
END

CREATE OR ALTER PROC Insert_LoaiMonAn @name NVARCHAR(50)
AS
BEGIN
	INSERT INTO dbo.LoaiMonAn(name)
	VALUES(@name)
END
CREATE OR ALTER PROC Update_LoaiMonAn @id INT, @name NVARCHAR(50)
AS
BEGIN
	UPDATE dbo.LoaiMonAn SET name = @name
	WHERE id = @id
END
CREATE OR ALTER PROC Delete_LoaiMonAn @id INT
AS
BEGIN
	DELETE FROM dbo.LoaiMonAn
	WHERE id = @id
END
DELETE FROM dbo.MonAn
DELETE FROM dbo.HoaDon


CREATE OR ALTER PROC GetNameTableByID @id INT
AS
BEGIN
	SELECT * FROM dbo.BanAn WHERE maban = @id
END

SELECT * FROM dbo.HoaDon INNER JOIN dbo.BanAn
	ON dbo.HoaDon.idTable = dbo.BanAn.maban
WHERE dbo.HoaDon.trangthai = 0

SELECT * FROM dbo.HoaDon b INNER JOIN dbo.HoaDonChiTiet bi
	ON b.idHD = bi.idHD
							INNER JOIN dbo.MonAn f
	ON bi.IdMon = f.IdMon

SELECT * FROM LoaiMonAn

CREATE OR ALTER PROC Search_BanAn @tenban NVARCHAR(100)
AS
BEGIN
	SELECT * FROM dbo.BanAn
	WHERE tenban LIKE '%' + @tenban + '%'
END

CREATE OR ALTER PROC Search_LoaiMonAn @tenLMA NVARCHAR(100)
AS
BEGIN
	SELECT * FROM dbo.LoaiMonAn
	WHERE name LIKE '%' + @tenLMA + '%'
END

SELECT * FROM dbo.MonAn INNER JOIN dbo.LoaiMonAn ON dbo.MonAn.idLoaiMonAn = dbo.LoaiMonAn.id
SELECT * FROM dbo.MonAn INNER JOIN dbo.LoaiMonAn ON dbo.MonAn.idLoaiMonAn = dbo.LoaiMonAn.id