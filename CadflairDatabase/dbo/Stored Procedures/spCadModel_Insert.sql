CREATE PROCEDURE [dbo].[spCadModel_Insert]
    @SubscriptionId      INT,
    @ProductFolderId     INT,
    @CreatedById         INT,
    @DisplayName		 NVARCHAR(50),
    @Description		 NVARCHAR(MAX),
    @BucketKey	         VARCHAR(50),
    @ObjectKey	         VARCHAR(50),
    @IsZip			     BIT,
    @RootFileName		 NVARCHAR(50)
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[CadModel]
			   ([SubscriptionId]
			   ,[ProductFolderId]
			   ,[CreatedById]
			   ,[DisplayName]
			   ,[Description]
			   ,[BucketKey]
			   ,[ObjectKey]
			   ,[IsZip]
			   ,[RootFileName])
		   VALUES
			   (@SubscriptionId
			   ,@ProductFolderId
			   ,@CreatedById
			   ,@DisplayName
			   ,@Description
			   ,@BucketKey
			   ,@ObjectKey
			   ,@IsZip
			   ,@RootFileName)
		   SELECT * FROM [dbo].[CadModel] WHERE Id = SCOPE_IDENTITY();

END

GO
