CREATE TABLE [dbo].[User] (
    [Id]                  INT                NOT NULL IDENTITY (1, 1), 
    [ObjectIdentifier]    UNIQUEIDENTIFIER   NOT NULL,
    [SubscriptionId]      INT                NULL,
    [FirstName]           NVARCHAR(25)       NOT NULL,
    [LastName]            NVARCHAR(25)       NOT NULL,
    [EmailAddress]        NVARCHAR(100)      NOT NULL,
    [CreatedOn]           DATETIMEOFFSET(7)  NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [UC_User_EmailAddress] UNIQUE([EmailAddress])
);
