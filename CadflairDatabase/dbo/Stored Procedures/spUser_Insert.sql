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
		   SELECT CAST(SCOPE_IDENTITY() as int);

END

GO
