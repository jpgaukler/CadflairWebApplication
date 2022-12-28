CREATE PROCEDURE [dbo].[spProduct_Insert]
	@SubscriptionId int,
	@ProductFolderId int,
	@DisplayName nvarchar(50),
	@SubdirectoryName varchar(50),
	@ForgeBucketKey varchar(50),
	@IsPublic bit,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Product]
			   (SubscriptionId
			   ,ProductFolderId
			   ,DisplayName
			   ,SubdirectoryName
			   ,ForgeBucketKey
			   ,IsPublic
			   ,CreatedById)
		   VALUES
			   (@SubscriptionId
			   ,@ProductFolderId
			   ,@DisplayName
			   ,@SubdirectoryName
			   ,@ForgeBucketKey
			   ,@IsPublic
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Product] WHERE Id = SCOPE_IDENTITY();

END

GO
