CREATE PROCEDURE [dbo].[spProduct_GetBySubscriptionIdAndSubdirectoryName]
	@SubscriptionId int,
	@SubdirectoryName varchar(50)
AS

SELECT * FROM [dbo].[Product] WHERE SubscriptionId = @SubscriptionId AND SubdirectoryName = @SubdirectoryName

GO
