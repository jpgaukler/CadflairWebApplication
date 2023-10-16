CREATE PROCEDURE [dbo].[spColumn_Insert]
	@ProductTableId int,
	@Header nvarchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Column]
			   ([ProductTableId]
			   ,[Header]
			   ,[CreatedById])
		   VALUES
			   (@ProductTableId
			   ,@Header
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Column] WHERE Id = SCOPE_IDENTITY();

END

GO
