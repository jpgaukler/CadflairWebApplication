CREATE PROCEDURE [dbo].[spCatalogFolder_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[CatalogFolder] WHERE Id = @Id

GO
