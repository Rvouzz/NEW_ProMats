USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[GET_MASTER_PART]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GET_MASTER_PART]
AS
BEGIN
    SELECT * FROM mst_part 
    ORDER BY material_id ASC;
END;
GO
