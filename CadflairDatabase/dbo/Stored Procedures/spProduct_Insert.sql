CREATE PROCEDURE [dbo].[spProduct_Insert]
	@SubscriptionId int,
	@ProductFolderId int,
	@DisplayName nvarchar(50),
	@SubdirectoryName varchar(50),
	@ParameterJson nvarchar(4000),
	@ForgeBucketKey uniqueidentifier,
	@CreatedById int,
	@IsPublic bit,
	@IsConfigurable bit
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Product]
			   (SubscriptionId
			   ,ProductFolderId
			   ,DisplayName
			   ,SubdirectoryName
			   ,ParameterJson
			   ,ForgeBucketKey
			   ,CreatedById
			   ,IsPublic
			   ,IsConfigurable)
		   VALUES
			   (@SubscriptionId
			   ,@ProductFolderId
			   ,@DisplayName
			   ,@SubdirectoryName
			   ,@ParameterJson
			   ,@ForgeBucketKey
			   ,@CreatedById
			   ,@IsPublic
			   ,@IsConfigurable)
		   SELECT * FROM [dbo].[Product] WHERE Id = SCOPE_IDENTITY();

END

GO
