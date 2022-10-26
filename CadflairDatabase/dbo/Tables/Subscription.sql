CREATE TABLE [dbo].[Subscription] (
    [SubscriptionId]     INT      IDENTITY (1, 1) NOT NULL,
    [SubscriptionTypeId] INT      NOT NULL,
    [ExpiresOn]          DATETIME NOT NULL,
    [CreatedOn]          DATETIME CONSTRAINT [DF_Subscription_CreatedOn] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED ([SubscriptionId] ASC),
    CONSTRAINT [FK_Subscription_SubscriptionType] FOREIGN KEY ([SubscriptionTypeId]) REFERENCES [dbo].[SubscriptionType] ([SubscriptionTypeId])
);

