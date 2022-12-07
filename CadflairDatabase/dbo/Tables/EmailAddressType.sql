CREATE TABLE [dbo].[EmailAddressType]
(
    [Id]             INT                NOT NULL IDENTITY (1, 1),
    [Name]           varchar(25)        NOT NULL,
    [CreatedOn]      DATETIME           NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK_EmailAddressType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_EmailAddressType_Name] UNIQUE([Name])
)
