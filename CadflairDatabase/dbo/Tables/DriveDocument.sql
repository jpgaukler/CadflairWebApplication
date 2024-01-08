CREATE TABLE [dbo].[DriveDocument] (
    [Id]                 INT                       NOT NULL IDENTITY (1, 1),
    [Guid]	             UNIQUEIDENTIFIER          NOT NULL DEFAULT NEWID(),
    [SubscriptionId]     INT                       NOT NULL,
    [DriveFolderId]      INT                       NOT NULL,
    [CreatedById]        INT                       NOT NULL,
    [DisplayName]		 NVARCHAR(50)              NOT NULL,
    [Description]		 NVARCHAR(500)             NULL,
    [BucketKey]	         VARCHAR(50)               NOT NULL,
    [ObjectKey]	         VARCHAR(50)               NOT NULL,
    [IsZip]			     BIT                       NOT NULL DEFAULT 0,
    [RootFileName]		 NVARCHAR(50)              NULL,
    [CreatedOn]			 DATETIMEOFFSET(7)    	   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT [PK_DriveDocument] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DriveDocument_DriveFolder] FOREIGN KEY ([DriveFolderId]) REFERENCES [dbo].[DriveFolder] ([Id]),
    CONSTRAINT [FK_DriveDocument_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_DriveDocument_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_DriveDocument_SubscriptionId_DriveFolderId_DisplayName] UNIQUE([SubscriptionId],[DriveFolderId],[DisplayName])
);

