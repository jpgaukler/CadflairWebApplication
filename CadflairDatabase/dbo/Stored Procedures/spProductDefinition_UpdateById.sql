CREATE PROCEDURE [dbo].[spProductDefinition_UpdateById]
    @Id int,
	@CategoryId int, 
	@Name nvarchar(50),
	@Description nvarchar(500),
	@ThumbnailId int, 
	@ForgeBucketKey varchar(50)
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[ProductDefinition]

	SET [CategoryId] = @CategoryId,
		[Name] = @Name,
		[Description] = @Description,
		[ThumbnailId] = @ThumbnailId,
		[ForgeBucketKey] = @ForgeBucketKey

	WHERE [Id] = @Id;

END

GO
