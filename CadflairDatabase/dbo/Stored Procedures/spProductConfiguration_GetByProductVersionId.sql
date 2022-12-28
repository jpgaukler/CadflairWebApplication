CREATE PROCEDURE [dbo].[spProductConfiguration_GetByProductVersionId]
	@ProductVersionId int
AS

SELECT * FROM [dbo].[ProductConfiguration] WHERE [ProductVersionId] = @ProductVersionId 

GO
