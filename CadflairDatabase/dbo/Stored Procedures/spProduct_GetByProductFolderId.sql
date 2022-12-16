CREATE PROCEDURE [dbo].[spProduct_GetByProductFolderId]
	@ProductFolderId int
AS

SELECT * FROM [dbo].[Product] WHERE ProductFolderId = @ProductFolderId

GO
