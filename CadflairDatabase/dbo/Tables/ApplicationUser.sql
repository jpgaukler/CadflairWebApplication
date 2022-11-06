CREATE TABLE [dbo].[ApplicationUser] (
    [Id]                  INT            NOT NULL IDENTITY (1, 1), 
    [AccountId]           INT            NULL,
    [ApplicationRoleId]   INT            NOT NULL,
    [UserName]            NVARCHAR(100)  NOT NULL,
    [NormalizedUserName]  NVARCHAR(100)  NOT NULL,
    [PasswordHash]        VARCHAR(100)    NOT NULL,
    [FirstName]           NVARCHAR(25)   NOT NULL,
    [LastName]            NVARCHAR(25)   NOT NULL,
    [CreatedOn]           DATETIME       NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationUser_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_ApplicationUser_ApplicationRole] FOREIGN KEY ([ApplicationRoleId]) REFERENCES [dbo].[ApplicationRole] ([Id]),
    CONSTRAINT [UC_ApplicationUser_UserName] UNIQUE([UserName])
);


GO

CREATE INDEX [IX_ApplicationUser_NormalizedUserName] ON [dbo].[ApplicationUser] ([NormalizedUserName])
