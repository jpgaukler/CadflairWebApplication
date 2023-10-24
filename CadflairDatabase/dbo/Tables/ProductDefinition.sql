CREATE TABLE [dbo].[ProductDefinition]
(
    [Id]               INT                  NOT NULL IDENTITY (1, 1),
    [SubscriptionId]   INT                  NOT NULL,
    [CategoryId]       INT                  NULL,
    [Name]             NVARCHAR(50)         NOT NULL,
    [Description]      NVARCHAR(500)        NULL,
    [ThumbnailUri]     VARCHAR(200)         NULL,
    [ForgeBucketKey]   VARCHAR(50)          NULL,
    [CreatedById]      INT                  NOT NULL,
    [CreatedOn]        DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_ProductDefinition] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductDefinition_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_ProductDefinition_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id]),
    CONSTRAINT [FK_ProductDefinition_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_ProductDefinition_SubscriptionId_Name] UNIQUE([SubscriptionId],[Name])
)

GO

