USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[UPDATE_MATERIAL_DATA]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_MATERIAL_DATA]
	@id_mtl INT,
    @opening_stock INT,
    @incoming INT,
    @ReturnValue INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    -- Update data for the existing material_id
    UPDATE [DB_PROMATS].[dbo].[mst_material]
    SET 
        opening_stock = @opening_stock,
        incoming = @incoming
    WHERE id_mtl = @id_mtl;

    -- Return 1 if update is successful
    SET @ReturnValue = 1;
    RETURN;
END
GO
