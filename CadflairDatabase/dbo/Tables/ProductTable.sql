CREATE TABLE [dbo].[ProductTable]
(
    [Id]                    INT                  NOT NULL IDENTITY (1, 1),
    [ProductDefinitionId]   INT                  NOT NULL,
    [CreatedById]           INT                  NOT NULL,
    [CreatedOn]             DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_ProductTable] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductTable_ProductDefinition] FOREIGN KEY ([ProductDefinitionId]) REFERENCES [dbo].[ProductDefinition] ([Id]),
    CONSTRAINT [FK_ProductTable_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_ProductTable_ProductDefinitionId] UNIQUE([ProductDefinitionId])
)

GO

