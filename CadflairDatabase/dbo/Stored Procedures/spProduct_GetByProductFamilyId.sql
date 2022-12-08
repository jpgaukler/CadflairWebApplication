CREATE PROCEDURE [dbo].[spProduct_GetByProductFamilyId]
	@ProductFamilyId int
AS

SELECT * FROM [dbo].[Product] WHERE ProductFamilyId = @ProductFamilyId

GO
