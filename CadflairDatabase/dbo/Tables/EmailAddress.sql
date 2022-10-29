CREATE TABLE [dbo].[EmailAddress]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL,
    [EmailAddress] NVARCHAR(50) NOT NULL, 
    [EmailAddressTypeId] INT NOT NULL, 
    [CreatedOn] DATETIME NOT NULL DEFAULT getdate(), 
    CONSTRAINT [FK_EmailAddress_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [FK_EmailAddress_EmailAddressType] FOREIGN KEY ([EmailAddressTypeId]) REFERENCES [dbo].[EmailAddressType]([Id]),
)
