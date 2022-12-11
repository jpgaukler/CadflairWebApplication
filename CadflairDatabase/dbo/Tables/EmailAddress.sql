CREATE TABLE [dbo].[EmailAddress]
(
    [Id]                    INT                         NOT NULL IDENTITY (1, 1),
    [UserId]                INT                         NOT NULL,
    [EmailAddress]          NVARCHAR(50)                NOT NULL, 
    [EmailAddressTypeId]    INT                         NOT NULL, 
    [CreatedOn]             DATETIMEOFFSET(7)           NOT NULL DEFAULT sysdatetimeoffset(), 
    CONSTRAINT [PK_EmailAddress] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailAddress_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [FK_EmailAddress_EmailAddressType] FOREIGN KEY ([EmailAddressTypeId]) REFERENCES [dbo].[EmailAddressType]([Id]),
    CONSTRAINT [UC_EmailAddress_EmailAddress] UNIQUE([EmailAddress])
)
