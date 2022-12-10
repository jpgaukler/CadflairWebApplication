CREATE PROCEDURE [dbo].[spProductFamily_GetBySubscriptionId]
	@SubscriptionId int
AS

SELECT * FROM [dbo].[ProductFamily] WHERE [SubscriptionId] = @SubscriptionId

GO
