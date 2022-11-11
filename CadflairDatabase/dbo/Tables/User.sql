CREATE TABLE [dbo].[User] (
    [Id]                  INT            NOT NULL IDENTITY (1, 1), 
    [AccountId]           INT            NULL,
    [UserRoleId]          INT            NOT NULL,
    [FirstName]           NVARCHAR(25)   NOT NULL,
    [LastName]            NVARCHAR(25)   NOT NULL,
    [EmailAddress]        NVARCHAR(100)  NOT NULL,
    [PasswordHash]        VARCHAR(100)   NOT NULL,
    [CreatedOn]           DATETIME       NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_User_UserRole] FOREIGN KEY ([UserRoleId]) REFERENCES [dbo].[UserRole] ([Id]),
    CONSTRAINT [UC_User_UserName] UNIQUE([EmailAddress])
);


GO

CREATE INDEX [IX_User_EmailAddress] ON [dbo].[User] ([EmailAddress])
