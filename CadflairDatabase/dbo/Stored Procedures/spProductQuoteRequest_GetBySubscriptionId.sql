CREATE PROCEDURE [dbo].[spProductQuoteRequest_GetBySubscriptionId]
	@SubscriptionId int
AS

--need to figure out if I want to make this a view with more columns from joined tables... and how I should name it

SELECT request.*
FROM [dbo].[ProductQuoteRequest] request
INNER JOIN [dbo].[ProductConfiguration] config ON request.ProductConfigurationId = config.Id
INNER JOIN [dbo].[ProductVersion] vers ON config.ProductVersionId = vers.Id
INNER JOIN [dbo].[Product] product ON vers.ProductId = product.Id
WHERE product.SubscriptionId = @SubscriptionId ORDER BY request.CreatedOn DESC

GO
