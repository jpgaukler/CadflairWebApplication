CREATE TABLE [dbo].[Row]
(
    [Id]                    INT                  NOT NULL IDENTITY (1, 1),
    [ProductTableId]        INT                  NOT NULL,
    [PartNumber]            NVARCHAR(50)         NOT NULL,
    [CreatedById]           INT                  NOT NULL,
    [CreatedOn]             DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_Row] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Row_ProductTable] FOREIGN KEY ([ProductTableId]) REFERENCES [dbo].[ProductTable] ([Id]),
    CONSTRAINT [FK_Row_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_Row_ProductTableId_PartNumber] UNIQUE([ProductTableId],[PartNumber])
)

GO

