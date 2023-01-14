CREATE PROCEDURE [dbo].[spNotificationSetting_Insert]
	@NotificationId int,
	@UserId int,
	@IsEnabled bit
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[NotificationSetting]
			   (NotificationId
			   ,UserId
			   ,IsEnabled)
		   VALUES
			   (@NotificationId
			   ,@UserId
			   ,@IsEnabled)
		   SELECT * FROM [dbo].[NotificationSetting] WHERE Id = SCOPE_IDENTITY();

END

GO
