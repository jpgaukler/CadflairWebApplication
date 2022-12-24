CREATE PROCEDURE [dbo].[spProductVersion_Insert]
	@ProductId int,
	@RootFileName nvarchar(50),
	@ILogicFormJson nvarchar(MAX),
	@IsConfigurable bit,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	DECLARE @VersionNumber AS INT;
	SELECT @VersionNumber = Count(Id) FROM [dbo].[ProductVersion] WHERE ProductId = @ProductId;

	INSERT INTO [dbo].[ProductVersion]
			   (ProductId
			   ,RootFileName
			   ,ILogicFormJson
			   ,VersionNumber
			   ,IsConfigurable
			   ,CreatedById)
		   VALUES
			   (@ProductId
			   ,@RootFileName
			   ,@ILogicFormJson
			   ,@VersionNumber + 1
			   ,@IsConfigurable
			   ,@CreatedById)
		   SELECT * FROM [dbo].[ProductVersion] WHERE Id = SCOPE_IDENTITY();

END

GO
