CREATE TABLE [dbo].[DriveFolder]
(
    [Id]              INT                  NOT NULL IDENTITY (1, 1),
    [SubscriptionId]  INT                  NOT NULL,
    [ParentId]        INT                  NULL,
    [DisplayName]     NVARCHAR(50)         NOT NULL,
    [CreatedById]     INT                  NOT NULL,
    [CreatedOn]       DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_DriveFolder] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DriveFolder_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_DriveFolder_DriveFolder] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[DriveFolder] ([Id]),
    CONSTRAINT [CK_DriveFolder_Id_ParentId] CHECK ([Id] <> [ParentId]),
    CONSTRAINT [UC_DriveFolder_SubscriptionId_ParentId_DisplayName] UNIQUE([SubscriptionId],[ParentId],[DisplayName])
)

GO

