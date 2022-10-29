CREATE TABLE [dbo].[Account] (
    [Id]                    INT            NOT NULL IDENTITY (1, 1),
    [CompanyName]           NVARCHAR (100) NOT NULL,
    [SubDirectory]          VARCHAR(50)    NOT NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedOn]             DATETIME       NOT NULL DEFAULT getdate(),
    [Owner]                 INT            NOT NULL,
    [AccountTypeId]         INT            NOT NULL, 
    [SubscriptionExpiresOn] DATETIME       NOT NULL, 
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Account_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [dbo].[AccountType]([Id]),
    CONSTRAINT [FK_Account_User1] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [FK_Account_User2] FOREIGN KEY ([Owner]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [UC_Account_SubDirectory] UNIQUE([SubDirectory])
);

