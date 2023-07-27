CREATE PROCEDURE [dbo].[spCatalogModel_DeleteById]
	@Id INT
AS

DELETE FROM [dbo].[CatalogModel] WHERE [Id] = @Id

GO
