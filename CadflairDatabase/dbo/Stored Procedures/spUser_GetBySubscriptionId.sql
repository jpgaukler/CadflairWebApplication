CREATE PROCEDURE [dbo].[spUser_GetBySubscriptionId]
	@SubscriptionId int
AS

SELECT * FROM [dbo].[User] WHERE SubscriptionId = @SubscriptionId

GO
