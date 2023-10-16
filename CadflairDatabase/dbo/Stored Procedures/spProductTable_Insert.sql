CREATE PROCEDURE [dbo].[spProductTable_Insert]
	@ProductDefinitionId int,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductTable]
			   ([ProductDefinitionId]
			   ,[CreatedById])
		   VALUES
			   (@ProductDefinitionId
			   ,@CreatedById)
		   SELECT * FROM [dbo].[ProductTable] WHERE Id = SCOPE_IDENTITY();

END

GO
