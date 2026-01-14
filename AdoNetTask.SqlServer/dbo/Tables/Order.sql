CREATE TABLE [dbo].[Order]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Status] INT NOT NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [UpdatedDate] DATETIME NOT NULL, 
    [ProductId] INT NOT NULL, 
    CONSTRAINT [FK_Order_To_Product_Table] FOREIGN KEY ([ProductId]) REFERENCES [Product]([Id])
)
