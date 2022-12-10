CREATE TABLE [dbo].[Product] (
    [Id]                 INT                       NOT NULL IDENTITY (1, 1),
    [SubscriptionId]     INT                       NOT NULL,
    [ProductFamilyId]    INT                       NOT NULL,
    [DisplayName]		 NVARCHAR(50)              NOT NULL,
    [ParameterJson]		 NVARCHAR (4000)           NULL, --I am not sure if this should be MAX, might want to try and determine the actual length of this field
    [ForgeBucketKey]	 UNIQUEIDENTIFIER          NOT NULL,
    [CreatedOn]			 DATETIME		    	   NOT NULL DEFAULT getdate(),
    [CreatedById]        INT                       NOT NULL,
    [IsPublic]			 BIT                       NOT NULL DEFAULT 0,
    [IsConfigurable]     BIT                       NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Product_ProductFamily] FOREIGN KEY ([ProductFamilyId]) REFERENCES [dbo].[ProductFamily] ([Id]),
    CONSTRAINT [FK_Product_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[ProductFamily] ([Id]),
    CONSTRAINT [FK_Product_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_SubscriptionId_DisplayName] UNIQUE([SubscriptionId],[DisplayName])
);

