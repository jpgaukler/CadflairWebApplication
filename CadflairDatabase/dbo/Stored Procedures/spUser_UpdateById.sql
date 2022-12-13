CREATE PROCEDURE [dbo].[spUser_UpdateById]
	@Id int,
	@ObjectIdentifier uniqueidentifier,
	@SubscriptionId int,
	@FirstName nvarchar(25),
	@LastName nvarchar(25),
	@EmailAddress nvarchar(100)
AS

BEGIN

	SET NOCOUNT ON;

	UPDATE [dbo].[User] 

	SET ObjectIdentifier = @ObjectIdentifier,
		SubscriptionId = @SubscriptionId,
		FirstName = @FirstName,
		LastName = @LastName,
		EmailAddress = @EmailAddress

	WHERE Id = @Id;

END

GO
