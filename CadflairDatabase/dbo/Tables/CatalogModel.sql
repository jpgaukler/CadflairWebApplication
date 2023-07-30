CREATE TABLE [dbo].[CatalogModel] (
    [Id]                 INT                       NOT NULL IDENTITY (1, 1),
    [Guid]	             UNIQUEIDENTIFIER          NOT NULL DEFAULT NEWID(),
    [SubscriptionId]     INT                       NOT NULL,
    [CatalogFolderId]    INT                       NOT NULL,
    [CreatedById]        INT                       NOT NULL,
    [DisplayName]		 NVARCHAR(50)              NOT NULL,
    [Description]		 NVARCHAR(500)             NULL,
    [BucketKey]	         VARCHAR(50)               NOT NULL,
    [ObjectKey]	         VARCHAR(50)               NOT NULL,
    [IsZip]			     BIT                       NOT NULL DEFAULT 0,
    [RootFileName]		 NVARCHAR(50)              NULL,
    [CreatedOn]			 DATETIMEOFFSET(7)    	   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT [PK_CatalogModel] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CatalogModel_CatalogFolder] FOREIGN KEY ([CatalogFolderId]) REFERENCES [dbo].[CatalogFolder] ([Id]),
    CONSTRAINT [FK_CatalogModel_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_CatalogModel_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_CatalogModel_SubscriptionId_CatalogFolderId_DisplayName] UNIQUE([SubscriptionId],[CatalogFolderId],[DisplayName])
);

