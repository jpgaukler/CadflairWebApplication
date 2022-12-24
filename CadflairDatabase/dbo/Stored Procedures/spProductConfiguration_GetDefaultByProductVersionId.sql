CREATE PROCEDURE [dbo].[spProductConfiguration_GetDefaultByProductVersionId]
	@ProductVersionId int
AS

SELECT * FROM [dbo].[ProductConfiguration] WHERE [ProductVersionId] = @ProductVersionId AND IsDefault = 'TRUE'

GO
