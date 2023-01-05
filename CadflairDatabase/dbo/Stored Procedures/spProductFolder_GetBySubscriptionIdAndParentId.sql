CREATE PROCEDURE [dbo].[spProductFolder_GetBySubscriptionIdAndParentId]
	@SubscriptionId int,
	@ParentId int
AS

IF @ParentId IS NULL
	BEGIN
		SELECT * FROM [dbo].[ProductFolder] WHERE [SubscriptionId] = @SubscriptionId AND [ParentId] IS NULL ORDER BY [DisplayName]
	END
ELSE
	BEGIN
		SELECT * FROM [dbo].[ProductFolder] WHERE [SubscriptionId] = @SubscriptionId AND [ParentId] = @ParentId ORDER BY [DisplayName]
	END


GO
