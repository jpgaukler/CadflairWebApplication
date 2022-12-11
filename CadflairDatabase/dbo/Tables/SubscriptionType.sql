CREATE TABLE [dbo].[SubscriptionType] (
    [Id]              INT                 NOT NULL IDENTITY (1, 1),
    [Name]            varchar(25)         NOT NULL,
    [CreatedOn]       DATETIMEOFFSET(7)   NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_SubscriptionType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_SubscriptionType_Name] UNIQUE([Name])
);

