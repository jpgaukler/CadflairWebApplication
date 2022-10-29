CREATE TABLE [dbo].[ProductConfiguration] (
	[Id]              INT              NOT NULL PRIMARY KEY IDENTITY, 
    [ProductId]       INT              NOT NULL,
    [ArgumentJson]    NVARCHAR (4000)   NOT NULL UNIQUE,
    [ForgeBucketKey]  UNIQUEIDENTIFIER NOT NULL,
    [ForgeObjectKey]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL DEFAULT getdate(),
    CONSTRAINT [FK_Configuration_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id]),
);

