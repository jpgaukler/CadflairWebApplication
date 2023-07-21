CREATE TABLE [dbo].[CadModel] (
    [Id]                 INT                       NOT NULL IDENTITY (1, 1),
    [Guid]	             UNIQUEIDENTIFIER          NOT NULL DEFAULT NEWID(),
    [SubscriptionId]     INT                       NOT NULL,
    [ProductFolderId]    INT                       NOT NULL,
    [CreatedById]        INT                       NOT NULL,
    [DisplayName]		 NVARCHAR(50)              NOT NULL,
    [Description]		 NVARCHAR(MAX)             NULL,
    [BucketKey]	         VARCHAR(50)               NOT NULL,
    [ObjectKey]	         VARCHAR(50)               NOT NULL,
    [IsZip]			     BIT                       NOT NULL DEFAULT 0,
    [RootFileName]		 NVARCHAR(50)              NULL,
    [CreatedOn]			 DATETIMEOFFSET(7)    	   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT [PK_CadModel] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CadModel_ProductFolder] FOREIGN KEY ([ProductFolderId]) REFERENCES [dbo].[ProductFolder] ([Id]),
    CONSTRAINT [FK_CadModel_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_CadModel_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_CadModel_ProductFolderId_DisplayName] UNIQUE([ProductFolderId],[DisplayName])
);

