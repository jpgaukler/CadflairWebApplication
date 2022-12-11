CREATE TABLE [dbo].[EmailAddressType]
(
    [Id]             INT                NOT NULL IDENTITY (1, 1),
    [Name]           varchar(25)        NOT NULL,
    [CreatedOn]      DATETIMEOFFSET(7)  NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_EmailAddressType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_EmailAddressType_Name] UNIQUE([Name])
)
