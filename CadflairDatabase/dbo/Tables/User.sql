CREATE TABLE [dbo].[User] (
	[Id]           INT            NOT NULL PRIMARY KEY IDENTITY, 
    [AccountId]    INT            NULL,
    [RoleId]       INT            NOT NULL,
    [PasswordHash] VARCHAR (30)   NOT NULL,
    [FirstName]    NVARCHAR (25)  NOT NULL,
    [LastName]     NVARCHAR (25)  NOT NULL,
    [CreatedOn]    DATETIME       NOT NULL DEFAULT getdate(),
    CONSTRAINT [FK_User_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]),
    CONSTRAINT [UQ_FirstName_LastName] UNIQUE(FirstName,LastName), 
);

