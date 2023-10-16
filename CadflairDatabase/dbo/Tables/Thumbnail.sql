CREATE TABLE [dbo].[Thumbnail]
(
    [Id]               INT                  NOT NULL IDENTITY (1, 1),
    [Base64String]     NVARCHAR(MAX)        NOT NULL,
    [CreatedById]      INT                  NOT NULL,
    [CreatedOn]        DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_Thumbnail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Thumbnail_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
)

GO

