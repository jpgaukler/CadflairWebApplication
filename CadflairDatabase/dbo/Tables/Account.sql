CREATE TABLE [dbo].[Account] (
    [AccountId]      INT            IDENTITY (1, 1) NOT NULL,
    [SubscriptionId] INT            NULL,
    [CompanyName]    NVARCHAR (100) NOT NULL,
    [CreatedOn]      DATETIME       CONSTRAINT [DF_Account_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]      INT            NOT NULL,
    [Owner]          INT            NOT NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([AccountId] ASC),
    CONSTRAINT [FK_Account_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId])
);

