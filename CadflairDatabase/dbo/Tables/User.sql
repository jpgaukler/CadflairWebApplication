CREATE TABLE [dbo].[User] (
    [Id]           INT            NOT NULL IDENTITY (1, 1), 
    [AccountId]    INT            NULL,
    [UserTypeId]       INT            NOT NULL,
    [PasswordHash] VARCHAR (30)   NOT NULL,
    [FirstName]    NVARCHAR (25)  NOT NULL,
    [LastName]     NVARCHAR (25)  NOT NULL,
    [UserName]     NVARCHAR (25)  NOT NULL,
    [CreatedOn]    DATETIME       NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([UserTypeId]) REFERENCES [dbo].[UserType] ([Id]),
    CONSTRAINT [UC_User_UserName] UNIQUE(UserName), 
    --CONSTRAINT [UC_FirstName_LastName] UNIQUE(FirstName,LastName), 
);

