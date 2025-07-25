USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[GET_DONUT_PMC]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GET_DONUT_PMC]
    @date_from DATE = NULL,  -- Allow NULL for date_from
    @date_to DATE = NULL     -- Allow NULL for date_to
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        COUNT(CASE WHEN status = 'OPEN' THEN 1 END) AS waiting, 
        COUNT(CASE WHEN status = 'CLOSE' THEN 1 END) AS approved, 
        COUNT(CASE WHEN status = 'REJECTED' THEN 1 END) AS rejected,
		COUNT(CASE WHEN status = 'IN-PROGRESS' THEN 1 END) AS in_progress
    FROM [mst_pofinish]
    WHERE 
        (@date_from IS NULL OR [date] >= @date_from) AND
        (@date_to IS NULL OR [date] <= @date_to)
END
GO
