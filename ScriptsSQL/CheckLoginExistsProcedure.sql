-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[LoginExists](@loginname nvarchar(128))
RETURNS bit
AS
BEGIN
	declare @result bit = 0 

	 SELECT @result = CAST(
        CASE WHEN Exists (select loginname from master.dbo.syslogins where name = @loginname ) THEN 1 
        ELSE 0 
        END 
    AS BIT)
    return @result

END
GO

