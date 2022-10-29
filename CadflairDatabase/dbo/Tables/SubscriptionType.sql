CREATE TABLE [dbo].[SubscriptionType] (
	[Id]              INT          NOT NULL PRIMARY KEY IDENTITY, 
    [DisplayName]     [dbo].[TypeName] NOT NULL UNIQUE,
    [CreatedOn]       DATETIME     NOT NULL DEFAULT getdate(), 
);

