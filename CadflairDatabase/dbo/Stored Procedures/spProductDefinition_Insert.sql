CREATE PROCEDURE [dbo].[spProductDefinition_Insert]
	@SubscriptionId int, 
	@CategoryId int, 
	@Name nvarchar(50),
	@Description nvarchar(500),
	@ThumbnailId int,
	@ForgeBucketKey varchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductDefinition]
                ([SubscriptionId]
				,[CategoryId]
				,[Name]
				,[Description]
				,[ThumbnailId]
				,[ForgeBucketKey]
				,[CreatedById])
           VALUES
                (@SubscriptionId
				,@CategoryId
				,@Name
				,@Description
				,@ThumbnailId
				,@ForgeBucketKey
				,@CreatedById)
		   SELECT * FROM [dbo].[ProductDefinition] WHERE Id = SCOPE_IDENTITY();
END

GO
