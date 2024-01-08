CREATE PROCEDURE [dbo].[spDriveDocument_Insert]
    @SubscriptionId      INT,
    @DriveFolderId       INT,
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

	INSERT INTO [dbo].[DriveDocument]
			   ([SubscriptionId]
			   ,[DriveFolderId]
			   ,[CreatedById]
			   ,[DisplayName]
			   ,[Description]
			   ,[BucketKey]
			   ,[ObjectKey]
			   ,[IsZip]
			   ,[RootFileName])
		   VALUES
			   (@SubscriptionId
			   ,@DriveFolderId
			   ,@CreatedById
			   ,@DisplayName
			   ,@Description
			   ,@BucketKey
			   ,@ObjectKey
			   ,@IsZip
			   ,@RootFileName)
		   SELECT * FROM [dbo].[DriveDocument] WHERE Id = SCOPE_IDENTITY();

END

GO
