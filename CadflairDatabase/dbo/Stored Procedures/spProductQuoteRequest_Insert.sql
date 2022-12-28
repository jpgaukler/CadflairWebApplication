CREATE PROCEDURE [dbo].[spProductQuoteRequest_Insert]
	@ProductConfigurationId int,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@EmailAddress nvarchar(100),
	@PhoneNumber nvarchar(25),
	@PhoneExtension nvarchar(10),
	@MessageText nvarchar(500)
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductQuoteRequest]
			   (ProductConfigurationId
			   ,FirstName
			   ,LastName
			   ,EmailAddress
			   ,PhoneNumber
			   ,PhoneExtension
			   ,MessageText)
		   VALUES
			   (@ProductConfigurationId
			   ,@FirstName
			   ,@LastName
			   ,@EmailAddress
			   ,@PhoneNumber
			   ,@PhoneExtension
			   ,@MessageText)
		   SELECT * FROM [dbo].[ProductQuoteRequest] WHERE Id = SCOPE_IDENTITY();

END

GO
