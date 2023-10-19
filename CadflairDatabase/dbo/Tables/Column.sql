CREATE TABLE [dbo].[Column]
(
    [Id]                    INT                  NOT NULL IDENTITY (1, 1),
    [ProductTableId]        INT                  NOT NULL,
    [Header]                NVARCHAR(50)         NOT NULL,
    [SortOrder]             INT                  NOT NULL,
    [CreatedById]           INT                  NOT NULL,
    [CreatedOn]             DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_Column] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Column_ProductTable] FOREIGN KEY ([ProductTableId]) REFERENCES [dbo].[ProductTable] ([Id]),
    CONSTRAINT [FK_Column_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_Column_ProductTableId_Header] UNIQUE([ProductTableId],[Header])
)

GO

