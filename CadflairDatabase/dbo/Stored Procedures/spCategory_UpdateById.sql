CREATE PROCEDURE [dbo].[spCategory_UpdateById]
    @Id int,
	@ParentId int, 
	@Name nvarchar(50),
	@Description nvarchar(500),
	@ThumbnailId int
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Category]

	SET [ParentId] = @ParentId,
		[Name] = @Name,
		[Description] = @Description,
		[ThumbnailId] = @ThumbnailId

	WHERE [Id] = @Id;

END

GO
