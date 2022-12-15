CREATE TABLE [dbo].[ProductConfiguration] (
    [Id]                    INT                  NOT NULL IDENTITY (1, 1),
    [ProductId]             INT                  NOT NULL,
    [IsDefault]             BIT                  NOT NULL,
    [ArgumentJson]          NVARCHAR (4000)      NOT NULL,
    [ForgeZipKey]           UNIQUEIDENTIFIER     NOT NULL,
    [ForgeStpKey]           UNIQUEIDENTIFIER     NULL,
    [ForgePdfKey]           UNIQUEIDENTIFIER     NULL,
    [ForgeDwgKey]           UNIQUEIDENTIFIER     NULL,
    [CreatedOn]             DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_ProductConfigutation] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Configuration_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id]),
    --CONSTRAINT [UC_ProductConfiguration_ArgumentJson] UNIQUE([ArgumentJson])
);

