CREATE PROCEDURE [dbo].[spUser_Insert]
	@ObjectIdentifier uniqueidentifier,
	@FirstName nvarchar(25),
	@LastName nvarchar(25),
	@EmailAddress nvarchar(100)
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[User]
			   (ObjectIdentifier
			   ,FirstName
			   ,LastName
			   ,EmailAddress)
		   VALUES
			   (@ObjectIdentifier
			   ,@FirstName
			   ,@LastName
			   ,@EmailAddress)
		   SELECT * FROM [dbo].[User] WHERE Id = SCOPE_IDENTITY();

END

GO
