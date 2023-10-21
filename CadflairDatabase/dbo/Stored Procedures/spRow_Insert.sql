CREATE PROCEDURE [dbo].[spRow_Insert]
	@ProductTableId int,
	@PartNumber nvarchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Row]
			   ([ProductTableId]
			   ,[PartNumber]
			   ,[CreatedById])
		   VALUES
			   (@ProductTableId
			   ,@PartNumber
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Row] WHERE Id = SCOPE_IDENTITY();

END

GO
