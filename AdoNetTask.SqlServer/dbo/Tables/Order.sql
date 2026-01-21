CREATE TABLE [dbo].[Order]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Status] NVARCHAR(20) NOT NULL,
    [CreatedDate] DATETIME NOT NULL,
    [UpdatedDate] DATETIME NOT NULL,
    [ProductId] INT NOT NULL,
    CONSTRAINT [FK_Order_To_Product_Table] FOREIGN KEY ([ProductId]) REFERENCES [Product]([Id])
)
