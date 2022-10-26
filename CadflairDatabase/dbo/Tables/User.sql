CREATE TABLE [dbo].[User] (
    [UserId]       INT            IDENTITY (1, 1) NOT NULL,
    [AccountId]    INT            NULL,
    [RoleId]       INT            NOT NULL,
    [EmailAddress] NVARCHAR (100) NOT NULL,
    [PhoneNumber]  NVARCHAR (25)  NULL,
    [PasswordHash] VARCHAR (30)   NOT NULL,
    [FirstName]    NVARCHAR (50)  NOT NULL,
    [LastName]     NVARCHAR (50)  NOT NULL,
    [CreatedOn]    DATETIME       CONSTRAINT [DF_User_CreatedOn] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_User_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([AccountId]),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId])
);

