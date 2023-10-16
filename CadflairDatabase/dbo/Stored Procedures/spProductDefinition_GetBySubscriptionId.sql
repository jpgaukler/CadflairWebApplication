CREATE PROCEDURE [dbo].[spProductDefinition_GetBySubscriptionId]
	@SubscriptionId int
AS

SELECT * FROM [dbo].[ProductDefinition] WHERE [SubscriptionId] = @SubscriptionId ORDER BY [Name]

GO
