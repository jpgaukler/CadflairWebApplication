CREATE TABLE [dbo].[SubscriptionType] (
    [Id]              INT                 NOT NULL IDENTITY (1, 1),
    [Name]            varchar(25)         NOT NULL,
    [CreatedOn]       DATETIME            NOT NULL DEFAULT getdate(), 
    CONSTRAINT [PK_SubscriptionType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_SubscriptionType_Name] UNIQUE([Name])
);

