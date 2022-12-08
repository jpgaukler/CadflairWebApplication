CREATE PROCEDURE [dbo].[spProductConfiguration_GetByProductId]
	@ProductId int
AS

SELECT * FROM [dbo].[ProductConfiguration] WHERE ProductId = @ProductId

GO
