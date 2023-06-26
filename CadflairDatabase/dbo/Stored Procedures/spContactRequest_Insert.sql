CREATE PROCEDURE [dbo].[spContactRequest_Insert]
    @FirstName NVARCHAR(30),
    @LastName  NVARCHAR(30),
    @EmailAddress NVARCHAR(100),
    @CompanyName  NVARCHAR(100),
    @Message NVARCHAR(500)             
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ContactRequest]
			   (FirstName
			   ,LastName
			   ,EmailAddress
			   ,CompanyName
			   ,[Message])
		   VALUES
			   (@FirstName
			   ,@LastName
			   ,@EmailAddress
			   ,@CompanyName
			   ,@Message)
		   SELECT * FROM [dbo].[ContactRequest] WHERE Id = SCOPE_IDENTITY();

END

GO
