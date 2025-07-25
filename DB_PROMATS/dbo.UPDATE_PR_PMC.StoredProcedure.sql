USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[UPDATE_PR_PMC]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_PR_PMC]
	@request_id INT,
    @part_code NVARCHAR(50),
    @material_id NVARCHAR(max),
    @quantity NVARCHAR(50),
    @ReturnValue INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    -- Update data for the existing material_id
    UPDATE [DB_PROMATS].[dbo].[mst_purchase_request]
    SET 
        quantity = @quantity
    WHERE request_id = @request_id;

    -- Return 1 if update is successful
    SET @ReturnValue = 1;
    RETURN;
END
GO
