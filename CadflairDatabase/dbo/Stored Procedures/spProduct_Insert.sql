CREATE PROCEDURE [dbo].[spProduct_Insert]
	@SubscriptionId int,
	@DisplayName nvarchar(50),
	@SubdirectoryName varchar(50),
	@IsPublic bit,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Product]
			   (SubscriptionId
			   ,DisplayName
			   ,SubdirectoryName
			   ,IsPublic
			   ,CreatedById)
		   VALUES
			   (@SubscriptionId
			   ,@DisplayName
			   ,@SubdirectoryName
			   ,@IsPublic
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Product] WHERE Id = SCOPE_IDENTITY();

END

GO
