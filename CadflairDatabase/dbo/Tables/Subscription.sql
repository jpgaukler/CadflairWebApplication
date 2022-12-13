CREATE TABLE [dbo].[Subscription] (
    [Id]                    INT                 NOT NULL IDENTITY (1, 1),
    [SubscriptionTypeId]    INT                 NOT NULL, 
    [CompanyName]           NVARCHAR(100)       NOT NULL,
    [PageName]              VARCHAR(50)         NOT NULL,
    [ExpiresOn]             DATETIMEOFFSET(7)   NOT NULL, 
    [OwnerId]               INT                 NOT NULL,
    [CreatedById]           INT                 NOT NULL,
    [CreatedOn]             DATETIMEOFFSET(7)   NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Subscription_SubscriptionType] FOREIGN KEY ([SubscriptionTypeId]) REFERENCES [dbo].[SubscriptionType]([Id]),
    CONSTRAINT [FK_Subscription_User1] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [FK_Subscription_User2] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [UC_Subscription_CompanyName] UNIQUE([CompanyName]),
    CONSTRAINT [UC_Subscription_PageName] UNIQUE([PageName])
);

