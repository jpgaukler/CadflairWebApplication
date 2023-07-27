CREATE PROCEDURE [dbo].[spCatalogModel_Insert]
    @SubscriptionId      INT,
    @CatalogFolderId     INT,
    @CreatedById         INT,
    @DisplayName		 NVARCHAR(50),
    @Description		 NVARCHAR(500),
    @BucketKey	         VARCHAR(50),
    @ObjectKey	         VARCHAR(50),
    @IsZip			     BIT,
    @RootFileName		 NVARCHAR(50)
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[CatalogModel]
			   ([SubscriptionId]
			   ,[CatalogFolderId]
			   ,[CreatedById]
			   ,[DisplayName]
			   ,[Description]
			   ,[BucketKey]
			   ,[ObjectKey]
			   ,[IsZip]
			   ,[RootFileName])
		   VALUES
			   (@SubscriptionId
			   ,@CatalogFolderId
			   ,@CreatedById
			   ,@DisplayName
			   ,@Description
			   ,@BucketKey
			   ,@ObjectKey
			   ,@IsZip
			   ,@RootFileName)
		   SELECT * FROM [dbo].[CatalogModel] WHERE Id = SCOPE_IDENTITY();

END

GO
