CREATE TABLE [dbo].[ProductConfiguration] (
    [Id]              INT              NOT NULL IDENTITY (1, 1),
    [ProductId]       INT              NOT NULL,
    [ArgumentJson]    NVARCHAR (4000)  NOT NULL,
    [ForgeObjectKey]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK_ProductConfigutation] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Configuration_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id]),
    --CONSTRAINT [UC_ProductConfiguration_ArgumentJson] UNIQUE([ArgumentJson])
);

