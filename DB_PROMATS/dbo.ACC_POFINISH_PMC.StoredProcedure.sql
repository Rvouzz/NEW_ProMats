USE [DB_PROMATS]
GO
/****** Object:  StoredProcedure [dbo].[ACC_POFINISH_PMC]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ACC_POFINISH_PMC]
    @po_id NVARCHAR(50),
    @part_code NVARCHAR(50),
    @part_name NVARCHAR(100),
    @qty INT,
    @date DATE,
    @request_by NVARCHAR(50),
    @part NVARCHAR(50),
    @runner NVARCHAR(50),
    @shot NVARCHAR(50),
	@file_order NVARCHAR(MAX),
    @ReturnValue INT OUTPUT
AS
BEGIN
    BEGIN TRY
        -- Insert new data into the table
        INSERT INTO mst_accpofinish (
            po_id, part_code, part_name, qty, date, request_by, part, runner, shot, status, file_order
        )
        VALUES (
            @po_id, @part_code, @part_name, @qty, @date, @request_by, @part, @runner, @shot, 'IN-PROGRESS', @file_order
        )

        -- Update the status of the existing po_id to 'Completed'
        UPDATE mst_pofinish
        SET status = 'IN-PROGRESS'
        WHERE po_id = @po_id

        -- Success
        SET @ReturnValue = 1
    END TRY
    BEGIN CATCH
        -- Handle errors
        SET @ReturnValue = 0
    END CATCH
END
GO
