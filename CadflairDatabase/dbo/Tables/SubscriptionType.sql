CREATE TABLE [dbo].[SubscriptionType] (
    [SubscriptionTypeId] INT          IDENTITY (1, 1) NOT NULL,
    [DisplayName]        VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_SubscriptionType] PRIMARY KEY CLUSTERED ([SubscriptionTypeId] ASC)
);

