CREATE PROCEDURE [dbo].[spNotification_GetByEventName]
	@EventName varchar(50)
AS

SELECT * FROM [dbo].[Notification] WHERE EventName = @EventName

GO
