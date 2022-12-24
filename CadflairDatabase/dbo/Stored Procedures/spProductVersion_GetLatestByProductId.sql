CREATE PROCEDURE [dbo].[spProductVersion_GetLatestByProductId]
	@ProductId int
AS

SELECT TOP (1) * FROM [dbo].[ProductVersion] WHERE ProductId = @ProductId ORDER BY CreatedOn DESC

GO
