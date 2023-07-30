CREATE TABLE [dbo].[CatalogFolder]
(
    [Id]              INT                  NOT NULL IDENTITY (1, 1),
    [SubscriptionId]  INT                  NOT NULL,
    [ParentId]        INT                  NULL,
    [DisplayName]     NVARCHAR(50)         NOT NULL,
    [CreatedById]     INT                  NOT NULL,
    [CreatedOn]       DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_CatalogFolder] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CatalogFolder_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_CatalogFolder_CatalogFolder] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[CatalogFolder] ([Id]),
    CONSTRAINT [CK_CatalogFolder_Id_ParentId] CHECK ([Id] <> [ParentId]),
    CONSTRAINT [UC_CatalogFolder_SubscriptionId_ParentId_DisplayName] UNIQUE([SubscriptionId],[ParentId],[DisplayName])
)

GO

