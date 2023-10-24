CREATE PROCEDURE [dbo].[spProductDefinition_UpdateById]
    @Id int,
	@CategoryId int, 
	@Name nvarchar(50),
	@Description nvarchar(500),
	@ThumbnailUri varchar(200),
	@ForgeBucketKey varchar(50)
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[ProductDefinition]

	SET [CategoryId] = @CategoryId,
		[Name] = @Name,
		[Description] = @Description,
		[ThumbnailUri] = @ThumbnailUri,
		[ForgeBucketKey] = @ForgeBucketKey

	WHERE [Id] = @Id;

END

GO
