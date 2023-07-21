CREATE PROCEDURE [dbo].[spCadModel_GetByProductFolderId]
	@ProductFolderId int
AS

SELECT * FROM [dbo].[CadModel] WHERE ProductFolderId = @ProductFolderId

GO
