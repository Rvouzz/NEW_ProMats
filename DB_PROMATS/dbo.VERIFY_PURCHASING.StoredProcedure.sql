USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[VERIFY_PURCHASING]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[VERIFY_PURCHASING]
    @request_id NVARCHAR(50),
    @supplier NVARCHAR(255),
    @lead_time NVARCHAR(50),
    @shipment NVARCHAR(50),
    @total_lead NVARCHAR(50),
    @acc_by NVARCHAR(255),
    @ReturnValue INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Cek apakah data dengan request_id sudah ada di tbl_detail_pr
    IF EXISTS (SELECT 1 FROM tbl_detail_pr WHERE request_id = @request_id)
    BEGIN
        SET @ReturnValue = -1;  -- Data sudah ada
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insert data baru ke tabel tbl_detail_pr
        INSERT INTO tbl_detail_pr (request_id, supplier_name, lead_time, shipment, total_time, acc_by)
        VALUES (@request_id, @supplier, @lead_time, @shipment, @total_lead, @acc_by);

        -- Update status menjadi 'Completed' di tabel mst_purchase_request berdasarkan request_id
        UPDATE mst_purchase_request
        SET status = 'SHIPPING'
        WHERE request_id = @request_id;

        SET @ReturnValue = 1;  -- Data berhasil diverifikasi dan status diupdate

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @ReturnValue = 0;  -- Terjadi kesalahan saat proses
    END CATCH
END
GO
