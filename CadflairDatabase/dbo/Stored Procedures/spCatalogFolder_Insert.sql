CREATE PROCEDURE [dbo].[spCatalogFolder_Insert]
	@SubscriptionId int,
	@ParentId int,
	@DisplayName nvarchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[CatalogFolder]
			   (SubscriptionId
			   ,ParentId
			   ,DisplayName
			   ,CreatedById)
		   VALUES
			   (@SubscriptionId
			   ,@ParentId
			   ,@DisplayName
			   ,@CreatedById)
		   SELECT * FROM [dbo].[CatalogFolder] WHERE Id = SCOPE_IDENTITY();

END

GO
