CREATE TABLE [dbo].[NotificationSetting]
(
    [Id]               INT                 NOT NULL IDENTITY (1, 1),
    [NotificationId]   INT                 NOT NULL,
    [UserId]           INT                 NOT NULL,
    [IsEnabled]        BIT                 NOT NULL,
    [CreatedOn]        DATETIMEOFFSET(7)   NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_NotificationSetting] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NotificationSetting_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [dbo].[Notification]([Id]),
    CONSTRAINT [FK_NotificationSetting_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id]),
    CONSTRAINT [UC_NotificationSetting_NotificationId_UserId] UNIQUE([NotificationId],[UserId])
)
