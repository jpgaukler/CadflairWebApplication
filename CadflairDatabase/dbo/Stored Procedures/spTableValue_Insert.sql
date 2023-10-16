CREATE PROCEDURE [dbo].[spTableValue_Insert]
	@ProductTableId int,
	@ColumnId int,
	@RowId int,
	@Value nvarchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TableValue]
			   ([ProductTableId]
			   ,[ColumnId]
			   ,[RowId]
			   ,[Value]
			   ,[CreatedById])
		   VALUES
			   (@ProductTableId
			   ,@ColumnId
			   ,@RowId
			   ,@Value
			   ,@CreatedById)
		   SELECT * FROM [dbo].[TableValue] WHERE Id = SCOPE_IDENTITY();

END

GO
