CREATE TABLE [dbo].[Account] (
    [Id]                    INT            NOT NULL IDENTITY (1, 1),
    [CompanyName]           NVARCHAR (100) NOT NULL,
    [SubDirectory]          VARCHAR(50)    NOT NULL,
    [CreatedById]           INT            NOT NULL,
    [CreatedOn]             DATETIME       NOT NULL DEFAULT getdate(),
    [OwnerId]               INT            NOT NULL,
    [AccountTypeId]         INT            NOT NULL, 
    [SubscriptionExpiresOn] DATETIME       NOT NULL, 
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Account_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [dbo].[AccountType]([Id]),
    CONSTRAINT [FK_Account_User1] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [FK_Account_User2] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [UC_Account_CompanyName] UNIQUE([CompanyName]),
    CONSTRAINT [UC_Account_SubDirectory] UNIQUE([SubDirectory])
);

