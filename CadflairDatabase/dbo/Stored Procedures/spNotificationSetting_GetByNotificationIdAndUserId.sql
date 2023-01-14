CREATE PROCEDURE [dbo].[spNotificationSetting_GetByNotificationIdAndUserId]
	@NotificationId int,
	@UserId int
AS

SELECT * FROM [dbo].[NotificationSetting] WHERE [NotificationId] = @NotificationId AND [UserId] = @UserId

GO
