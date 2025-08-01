USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[ADD_MATERIAL_DATA]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ADD_MATERIAL_DATA]
    @part_code NVARCHAR(50),
    @material_id NVARCHAR(max),
	@material_name NVARCHAR(max),
    @opening_stock NVARCHAR(50),
    @incoming NVARCHAR(50),
    @ReturnValue INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if material_id already exists
    IF EXISTS (SELECT 1 FROM [DB_PROMATS].[dbo].[mst_material] WHERE material_id = @material_id AND part_code = @part_code)
    BEGIN
        -- Return -1 if material_id already exists
        SET @ReturnValue = -1;
        RETURN;
    END

    -- Insert data if material_id does not exist
    INSERT INTO [DB_PROMATS].[dbo].[mst_material]
        (part_code, material_id, material_name, opening_stock, incoming)
    VALUES
        (@part_code, @material_id, @material_name, @opening_stock, @incoming);

    -- Return 1 if insertion is successful
    SET @ReturnValue = 1;
    RETURN;
END
GO
