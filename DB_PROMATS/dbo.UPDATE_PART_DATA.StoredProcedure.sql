USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[UPDATE_PART_DATA]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_PART_DATA]
	@id_part INT,
    @part_code NVARCHAR(50),
    @part_name NVARCHAR(max),
    @part NVARCHAR(50),
    @runner NVARCHAR(50),
    @ReturnValue INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if material_id exists
    IF NOT EXISTS (SELECT 1 FROM [DB_PROMATS].[dbo].[mst_part] WHERE part_code = @part_code)
    BEGIN
        -- If material_id does not exist, return -1
        SET @ReturnValue = -1;
        RETURN;
    END

    -- Update data for the existing material_id
    UPDATE [DB_PROMATS].[dbo].[mst_part]
    SET 
        part_code = @part_code,
        part_name = @part_name,
        part = @part,
        runner = @runner
    WHERE id_part = @id_part;

    -- Return 1 if update is successful
    SET @ReturnValue = 1;
    RETURN;
END
GO
