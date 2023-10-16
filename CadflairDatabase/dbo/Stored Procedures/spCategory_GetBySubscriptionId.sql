CREATE PROCEDURE [dbo].[spCategory_GetBySubscriptionId]
	@SubscriptionId int
AS

SELECT * FROM [dbo].[Category] WHERE [SubscriptionId] = @SubscriptionId ORDER BY [Name]

GO
