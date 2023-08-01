CREATE PROCEDURE [dbo].[spCatalogModel_UpdateById]
    @Id                  INT,
    @SubscriptionId      INT,
    @CatalogFolderId     INT,
    @DisplayName		 NVARCHAR(50),
    @Description		 NVARCHAR(500),
    @BucketKey	         VARCHAR(50),
    @ObjectKey	         VARCHAR(50),
    @IsZip			     BIT,
    @RootFileName		 NVARCHAR(50)
AS

BEGIN

	SET NOCOUNT ON;

	UPDATE [dbo].[CatalogModel] 

	SET [SubscriptionId] = @SubscriptionId,
		[CatalogFolderId] = @CatalogFolderId,
		[DisplayName] = @DisplayName,
		[Description] = @Description,
		[BucketKey] = @BucketKey,
		[ObjectKey] = @ObjectKey,
		[IsZip] = @IsZip,
		[RootFileName] = @RootFileName

	WHERE [Id] = @Id;

END

GO
