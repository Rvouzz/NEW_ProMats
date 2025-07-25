USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[ADD_PURCHASE_REQ]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ADD_PURCHASE_REQ]
    @part_code NVARCHAR(50),
    @material_id NVARCHAR(max),
    @quantity NVARCHAR(50),
	@request_by NVARCHAR(50),
    @ReturnValue INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if material_id already exists
    IF EXISTS (SELECT 1 FROM [DB_PROMATS].[dbo].[mst_purchase_request] WHERE material_id = @material_id AND part_code = @part_code and [status] = 'Waiting')
    BEGIN
        -- Return -1 if material_id already exists
        SET @ReturnValue = -1;
        RETURN;
    END

    -- Insert data if material_id does not exist
    INSERT INTO [DB_PROMATS].[dbo].[mst_purchase_request]
        (part_code, material_id, quantity, date, request_by)
    VALUES
        (@part_code, @material_id, @quantity, GETDATE(), @request_by);

    -- Return 1 if insertion is successful
    SET @ReturnValue = 1;
    RETURN;
END

truncate table [mst_purchase_request]
GO
