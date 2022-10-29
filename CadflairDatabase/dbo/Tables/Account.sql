CREATE TABLE [dbo].[Account] (
	[Id]                    INT            NOT NULL PRIMARY KEY IDENTITY, 
    [CompanyName]           NVARCHAR (100) NOT NULL,
    [SubDirectory]          VARCHAR(50)    NOT NULL UNIQUE,
    [CreatedBy]             INT            NOT NULL,
    [CreatedOn]             DATETIME       NOT NULL DEFAULT getdate(),
    [Owner]                 INT            NOT NULL,
    [SubscriptionTypeId]    INT            NOT NULL, 
    [SubscriptionExpiresOn] DATETIME       NOT NULL, 
    CONSTRAINT [FK_Account_SubscriptionType] FOREIGN KEY ([SubscriptionTypeId]) REFERENCES [dbo].[SubscriptionType]([Id]),
    CONSTRAINT [FK_Account_User1] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [FK_Account_User2] FOREIGN KEY ([Owner]) REFERENCES [dbo].[User]([Id]),
);

