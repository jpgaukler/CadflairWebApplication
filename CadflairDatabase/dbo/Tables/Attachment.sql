CREATE TABLE [dbo].[Attachment]
(
    [Id]               INT                  NOT NULL IDENTITY (1, 1),
    [RowId]            INT                  NOT NULL,
    [ForgeObjectKey]   VARCHAR(50)          NOT NULL,
    [CreatedById]      INT                  NOT NULL,
    [CreatedOn]        DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Attachment_Row] FOREIGN KEY ([RowId]) REFERENCES [dbo].[Row] ([Id]),
    CONSTRAINT [FK_Attachment_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_Attachment_RowId_ForgeObjectKey] UNIQUE([RowId],[ForgeObjectKey])
)

GO

