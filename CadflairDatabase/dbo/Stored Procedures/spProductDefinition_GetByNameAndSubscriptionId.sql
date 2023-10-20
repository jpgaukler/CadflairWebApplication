CREATE PROCEDURE [dbo].[spProductDefinition_GetByNameAndSubscriptionId]
	@Name nvarchar(50),
	@SubscriptionId int
AS

SELECT TOP 1 * FROM [dbo].[ProductDefinition] WHERE [SubscriptionId] = @SubscriptionId AND [Name] = @Name;

GO
