CREATE PROCEDURE [dbo].[spCategory_Insert]
	@ParentId int,
	@SubscriptionId int,
	@Name nvarchar(50),
	@Description nvarchar(500),
	@ThumbnailId int,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Category]
			   ([ParentId]
			   ,[SubscriptionId]
			   ,[Name]
			   ,[Description]
			   ,[ThumbnailId]
			   ,[CreatedById])
		   VALUES
			   (@ParentId
			   ,@SubscriptionId
			   ,@Name
			   ,@Description
			   ,@ThumbnailId
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Category] WHERE Id = SCOPE_IDENTITY();

END

GO
