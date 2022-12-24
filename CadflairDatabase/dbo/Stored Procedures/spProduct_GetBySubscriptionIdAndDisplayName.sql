CREATE PROCEDURE [dbo].[spProduct_GetBySubscriptionIdAndDisplayName]
	@SubscriptionId int,
	@DisplayName nvarchar(50)
AS

SELECT * FROM [dbo].[Product] WHERE SubscriptionId = @SubscriptionId AND DisplayName = @DisplayName

GO
