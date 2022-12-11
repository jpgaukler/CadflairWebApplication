CREATE PROCEDURE [dbo].[spSubscription_GetById]
	@Id int
AS

SELECT * FROM [dbo].[Subscription] WHERE Id = @Id

GO
