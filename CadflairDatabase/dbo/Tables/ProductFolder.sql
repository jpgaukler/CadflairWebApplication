CREATE TABLE [dbo].[ProductFolder]
(
    [Id]              INT                  NOT NULL IDENTITY (1, 1),
    [SubscriptionId]  INT                  NOT NULL,
    [ParentId]        INT                  NULL,
    [DisplayName]     NVARCHAR(50)         NOT NULL,
    [CreatedById]     INT                  NOT NULL,
    [CreatedOn]       DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_ProductFolder] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductFolder_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_ProductFolder_ProductFolder] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[ProductFolder] ([Id]),
    CONSTRAINT [UC_ProductFolder_SubscriptionId_ParentId_DisplayName] UNIQUE([SubscriptionId],[ParentId],[DisplayName]),
)

GO

