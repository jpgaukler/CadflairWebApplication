CREATE PROCEDURE [dbo].[spCatalogFolder_UpdateById]
	@Id				 INT,
    @ParentId        INT,
    @DisplayName     NVARCHAR(50)
AS

BEGIN

	SET NOCOUNT ON;

	UPDATE [dbo].[CatalogFolder] 

	SET [ParentId] = @ParentId,
		[DisplayName] = @DisplayName

	WHERE Id = @Id;

END

GO
