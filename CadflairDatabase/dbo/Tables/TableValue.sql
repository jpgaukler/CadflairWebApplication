CREATE TABLE [dbo].[TableValue]
(
    [Id]                    INT                  NOT NULL IDENTITY (1, 1),
    [ProductTableId]        INT                  NOT NULL,
    [ColumnId]              INT                  NOT NULL,
    [RowId]                 INT                  NOT NULL,
    [Value]                 NVARCHAR(50)         NOT NULL,
    [CreatedById]           INT                  NOT NULL,
    [CreatedOn]             DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_TableValue] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TableValue_ProductTable] FOREIGN KEY ([ProductTableId]) REFERENCES [dbo].[ProductTable] ([Id]),
    CONSTRAINT [FK_TableValue_Column] FOREIGN KEY ([ColumnId]) REFERENCES [dbo].[Column] ([Id]),
    CONSTRAINT [FK_TableValue_Row] FOREIGN KEY ([RowId]) REFERENCES [dbo].[Row] ([Id]),
    CONSTRAINT [FK_TableValue_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
)

GO

