CREATE PROCEDURE [dbo].[spCatalogFolder_GetById]
	@Id int
AS

SELECT * FROM [dbo].[CatalogFolder] WHERE Id = @Id

GO
