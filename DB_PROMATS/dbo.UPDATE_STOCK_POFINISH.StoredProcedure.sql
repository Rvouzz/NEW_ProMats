USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[UPDATE_STOCK_POFINISH]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_STOCK_POFINISH]
	@id_det INT,
    @part_code NVARCHAR(50),
    @total_stock NVARCHAR(50),
    @ReturnValue INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [DB_PROMATS].[dbo].[tbl_stock_pofinish]
    SET 
        total_stock = @total_stock,
		update_date = GETDATE()
    WHERE id_det = @id_det;

    SET @ReturnValue = 1;
    RETURN;
END
GO
