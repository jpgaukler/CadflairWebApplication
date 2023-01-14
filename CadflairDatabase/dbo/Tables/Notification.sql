CREATE TABLE [dbo].[Notification]
(
    [Id]                INT                 NOT NULL IDENTITY (1, 1),
    [EventName]			varchar(50)         NOT NULL,
    [EnabledByDefault]	BIT                 NOT NULL DEFAULT 1,
    [CreatedOn]			DATETIMEOFFSET(7)   NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_Notification_Name] UNIQUE([EventName])
)
