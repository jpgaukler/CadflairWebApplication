CREATE PROCEDURE [dbo].[spProductConfiguration_UpdateById]
    @Id int,
	@ProductVersionId int, 
	@IsDefault bit,
	@ArgumentJson nvarchar(MAX),
	@BucketKey varchar(50),
	@ZipObjectKey varchar(50),
	@StpObjectKey varchar(50)
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[ProductConfiguration]

	SET ProductVersionId = @ProductVersionId,
		IsDefault = @IsDefault,
		ArgumentJson = @ArgumentJson,
		BucketKey = @BucketKey,
		ZipObjectKey = @ZipObjectKey,
		StpObjectKey = @StpObjectKey

	WHERE Id = @Id;

END

GO
