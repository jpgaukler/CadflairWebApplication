CREATE PROCEDURE [dbo].[spNotificationSetting_GetByUserId]
	@UserId int
AS

SELECT * FROM [dbo].[NotificationSetting] WHERE [UserId] = @UserId

GO
