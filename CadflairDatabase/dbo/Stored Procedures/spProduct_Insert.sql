CREATE PROCEDURE [dbo].[spProduct_Insert]
	@ProductFamilyId int,
	@DisplayName nvarchar(50),
	@ParameterJson nvarchar(4000),
	@ForgeBucketKey uniqueidentifier,
	@CreatedById int,
	@IsPublic bit,
	@IsConfigurable bit
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Product]
			   (ProductFamilyId
			   ,DisplayName
			   ,ParameterJson
			   ,ForgeBucketKey
			   ,CreatedById
			   ,IsPublic
			   ,IsConfigurable)
		   VALUES
			   (@ProductFamilyId
			   ,@DisplayName
			   ,@ParameterJson
			   ,@ForgeBucketKey
			   ,@CreatedById
			   ,@IsPublic
			   ,@IsConfigurable)
		   SELECT CAST(SCOPE_IDENTITY() as int);

END

GO
