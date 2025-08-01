USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[GET_YEARLY_PMC]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GET_YEARLY_PMC]
AS
BEGIN
    SET NOCOUNT ON;

    -- Get the current year
    DECLARE @current_year INT = YEAR(GETDATE());

    -- Create a table of months (1 to 12)
    WITH Months AS (
        SELECT 1 AS Month
        UNION ALL SELECT 2
        UNION ALL SELECT 3
        UNION ALL SELECT 4
        UNION ALL SELECT 5
        UNION ALL SELECT 6
        UNION ALL SELECT 7
        UNION ALL SELECT 8
        UNION ALL SELECT 9
        UNION ALL SELECT 10
        UNION ALL SELECT 11
        UNION ALL SELECT 12
    )

    -- Select the count of entries for each month from January to December for the current year
    SELECT 
        m.month,
        COALESCE(COUNT(p.[date]), 0) AS count  -- Ensure months with no records are shown with 0 count
    FROM Months m
    LEFT JOIN [mst_pofinish] p
        ON MONTH(p.[date]) = m.Month AND YEAR(p.[date]) = @current_year
    GROUP BY m.Month
    ORDER BY m.Month
END
GO
