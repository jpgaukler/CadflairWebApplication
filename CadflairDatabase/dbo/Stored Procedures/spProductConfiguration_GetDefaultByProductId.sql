CREATE PROCEDURE [dbo].[spProductConfiguration_GetDefaultByProductId]
	@ProductId int
AS

SELECT * FROM [dbo].[ProductConfiguration] WHERE ProductId = @ProductId AND IsDefault = 'TRUE'

GO
