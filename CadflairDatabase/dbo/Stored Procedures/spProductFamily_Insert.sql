CREATE PROCEDURE [dbo].[spProductFamily_Insert]
	@SubscriptionId int,
	@ParentId int,
	@DisplayName nvarchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductFamily]
			   (SubscriptionId
			   ,ParentId
			   ,DisplayName
			   ,CreatedById)
		   VALUES
			   (@SubscriptionId
			   ,@ParentId
			   ,@DisplayName
			   ,@CreatedById)
		   SELECT * FROM [dbo].[ProductFamily] WHERE Id = SCOPE_IDENTITY();

END

GO
