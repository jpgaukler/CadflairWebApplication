CREATE TABLE [dbo].[Product] (
    [Id]                 INT                       NOT NULL IDENTITY (1, 1),
    [SubscriptionId]     INT                       NOT NULL,
    [DisplayName]		 NVARCHAR(50)              NOT NULL,
    [SubdirectoryName]   VARCHAR(50)               NOT NULL,
    [IsPublic]			 BIT                       NOT NULL DEFAULT 0,
    [CreatedOn]			 DATETIMEOFFSET(7)    	   NOT NULL DEFAULT sysdatetimeoffset(),
    [CreatedById]        INT                       NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Product_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_Product_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_Product_SubscriptionId_DisplayName] UNIQUE([SubscriptionId],[DisplayName]),
    CONSTRAINT [UC_Product_SubscriptionId_SubdirectoryName] UNIQUE([SubscriptionId],[SubdirectoryName])
);

