CREATE PROCEDURE [dbo].[spColumn_Insert]
	@ProductTableId int,
	@Header nvarchar(50),
	@SortOrder int,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Column]
			   ([ProductTableId]
			   ,[Header]
			   ,[SortOrder]
			   ,[CreatedById])
		   VALUES
			   (@ProductTableId
			   ,@Header
			   ,@SortOrder
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Column] WHERE Id = SCOPE_IDENTITY();

END

GO
