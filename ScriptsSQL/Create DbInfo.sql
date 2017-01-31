use FastSqlIdentity CREATE TABLE DbInfos (
	Id INT IDENTITY(1,1) Not null primary key,
	Name NVARCHAR(200) not null,
	DateOfCreating Date not null,
	ConnectionString NVARCHAR(500) not null,
	UserKey NVARCHAR(128) not null,

	Foreign key (userkey) references dbo.AspNetUsers(Id)
)