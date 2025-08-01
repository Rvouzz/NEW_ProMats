USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[ADD_PART_DATA]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ADD_PART_DATA]
    @part_code NVARCHAR(50),
    @part_name NVARCHAR(max),
    @part NVARCHAR(50),
    @runner NVARCHAR(50),
    @ReturnValue INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @po_id INT;
    -- Check if material_id already exists
    IF EXISTS (SELECT 1 FROM [DB_PROMATS].[dbo].[mst_part] WHERE part_code = @part_code)
    BEGIN
        -- Return -1 if material_id already exists
        SET @ReturnValue = -1;
        RETURN;
    END
	ELSE
	BEGIN
		-- Insert data if material_id does not exist
		INSERT INTO [DB_PROMATS].[dbo].[mst_part]
			(part_code, part_name, part, runner)
		VALUES
			(@part_code, @part_name, @part, @runner);
		
		SET @po_id = SCOPE_IDENTITY();

		INSERT INTO [DB_PROMATS].[dbo].tbl_stock_pofinish
			(id_part, part_code, total_stock, update_date)
		VALUES
			(@po_id, @part_code, 0, GETDATE());

		SET @ReturnValue = 1;
		RETURN;
	END


END
GO
