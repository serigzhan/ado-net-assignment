CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(500) NULL, 
    [Weight] INT NULL, 
    [Height] INT NULL, 
    [Width] INT NULL, 
    [Length] INT NULL
)
