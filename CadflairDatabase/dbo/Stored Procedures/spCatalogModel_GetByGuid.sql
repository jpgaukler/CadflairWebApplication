CREATE PROCEDURE [dbo].[spCatalogModel_GetByGuid]
	@Guid UNIQUEIDENTIFIER
AS

SELECT TOP 1 * FROM [dbo].[CatalogModel] WHERE [Guid] = @Guid

GO
