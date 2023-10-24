CREATE PROCEDURE [dbo].[spCategory_Insert]
	@ParentId int,
	@SubscriptionId int,
	@Name nvarchar(50),
	@Description nvarchar(500),
	@ThumbnailUri varchar(200),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Category]
			   ([ParentId]
			   ,[SubscriptionId]
			   ,[Name]
			   ,[Description]
			   ,[ThumbnailUri]
			   ,[CreatedById])
		   VALUES
			   (@ParentId
			   ,@SubscriptionId
			   ,@Name
			   ,@Description
			   ,@ThumbnailUri
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Category] WHERE Id = SCOPE_IDENTITY();

END

GO
