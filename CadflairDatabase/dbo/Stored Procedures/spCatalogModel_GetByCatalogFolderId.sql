CREATE PROCEDURE [dbo].[spCatalogModel_GetByCatalogFolderId]
	@CatalogFolderId int
AS

SELECT * FROM [dbo].[CatalogModel] WHERE CatalogFolderId = @CatalogFolderId

GO
