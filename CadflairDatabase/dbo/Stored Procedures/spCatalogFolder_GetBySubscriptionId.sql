CREATE PROCEDURE [dbo].[spCatalogFolder_GetBySubscriptionId]
	@SubscriptionId int
AS

SELECT * FROM [dbo].[CatalogFolder] WHERE [SubscriptionId] = @SubscriptionId ORDER BY [DisplayName]

GO
