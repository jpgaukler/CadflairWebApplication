CREATE PROCEDURE [dbo].[spProductFolder_GetBySubscriptionId]
	@SubscriptionId int
AS

SELECT * FROM [dbo].[ProductFolder] WHERE [SubscriptionId] = @SubscriptionId ORDER BY [DisplayName]

GO
