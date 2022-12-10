CREATE PROCEDURE [dbo].[spProduct_Insert]
	@SubscriptionId int,
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
			   (SubscriptionId
			   ,ProductFamilyId
			   ,DisplayName
			   ,ParameterJson
			   ,ForgeBucketKey
			   ,CreatedById
			   ,IsPublic
			   ,IsConfigurable)
		   VALUES
			   (@SubscriptionId
			   ,@ProductFamilyId
			   ,@DisplayName
			   ,@ParameterJson
			   ,@ForgeBucketKey
			   ,@CreatedById
			   ,@IsPublic
			   ,@IsConfigurable)
		   SELECT CAST(SCOPE_IDENTITY() as int);

END

GO
