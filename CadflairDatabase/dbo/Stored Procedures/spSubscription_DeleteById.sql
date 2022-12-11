CREATE PROCEDURE [dbo].[spSubscription_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[Subscription] WHERE Id = @Id

GO
