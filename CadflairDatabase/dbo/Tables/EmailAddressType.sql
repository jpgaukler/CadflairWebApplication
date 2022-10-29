CREATE TABLE [dbo].[EmailAddressType]
(
	[Id]             INT                NOT NULL PRIMARY KEY IDENTITY, 
    [DisplayName]    [dbo].[TypeName]   NOT NULL UNIQUE,
    [CreatedOn]       DATETIME          NOT NULL DEFAULT getdate(),
)
