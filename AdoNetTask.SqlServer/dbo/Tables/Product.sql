CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(500) NULL, 
    [Weight] DECIMAL(18, 3) NULL, 
    [Height] DECIMAL(18, 2) NULL, 
    [Width] DECIMAL(18, 2) NULL, 
    [Length] DECIMAL(18, 3) NULL
)
