CREATE TABLE [dbo].[Category]
(
    [Id]               INT                  NOT NULL IDENTITY (1, 1),
    [ParentId]         INT                  NULL,
    [SubscriptionId]   INT                  NOT NULL,
    [Name]             NVARCHAR(50)         NOT NULL,
    [Description]      NVARCHAR(500)        NULL,
    [ThumbnailUri]     VARCHAR(200)         NULL,
    [CreatedById]      INT                  NOT NULL,
    [CreatedOn]        DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Category_Category] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Category] ([Id]),
    CONSTRAINT [FK_Category_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_Category_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [CK_Category_Id_ParentId] CHECK ([Id] <> [ParentId]),
    CONSTRAINT [UC_Category_SubscriptionId_Name] UNIQUE([SubscriptionId],[Name])
)

GO

