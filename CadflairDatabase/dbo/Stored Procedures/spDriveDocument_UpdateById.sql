CREATE PROCEDURE [dbo].[spDriveDocument_UpdateById]
    @Id                  INT,
    @SubscriptionId      INT,
    @DriveFolderId       INT,
    @DisplayName		 NVARCHAR(50),
    @Description		 NVARCHAR(500),
    @BucketKey	         VARCHAR(50),
    @ObjectKey	         VARCHAR(50),
    @IsZip			     BIT,
    @RootFileName		 NVARCHAR(50)
AS

BEGIN

	SET NOCOUNT ON;

	UPDATE [dbo].[DriveDocument] 

	SET [SubscriptionId] = @SubscriptionId,
		[DriveFolderId] = @DriveFolderId,
		[DisplayName] = @DisplayName,
		[Description] = @Description,
		[BucketKey] = @BucketKey,
		[ObjectKey] = @ObjectKey,
		[IsZip] = @IsZip,
		[RootFileName] = @RootFileName

	WHERE [Id] = @Id;

END

GO
