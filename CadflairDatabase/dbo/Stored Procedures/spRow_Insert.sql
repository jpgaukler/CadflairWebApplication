CREATE PROCEDURE [dbo].[spRow_Insert]
	@ProductTableId int,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Row]
			   ([ProductTableId]
			   ,[CreatedById])
		   VALUES
			   (@ProductTableId
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Row] WHERE Id = SCOPE_IDENTITY();

END

GO
