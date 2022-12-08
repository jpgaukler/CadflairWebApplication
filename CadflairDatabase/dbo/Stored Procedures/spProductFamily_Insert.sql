CREATE PROCEDURE [dbo].[spProductFamily_Insert]
	@ParentId int,
	@AccountId int,
	@DisplayName nvarchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductFamily]
			   (ParentId
			   ,AccountId
			   ,DisplayName
			   ,CreatedById)
		   VALUES
			   (@ParentId
			   ,@AccountId
			   ,@DisplayName
			   ,@CreatedById)
		   SELECT CAST(SCOPE_IDENTITY() as int);

END

GO
