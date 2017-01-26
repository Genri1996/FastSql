use master
go

create procedure dbo.DropDatabase(@dbname nvarchar(128))
as
begin
	ALTER DATABASE [@dbname] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE [@dbname] 
end