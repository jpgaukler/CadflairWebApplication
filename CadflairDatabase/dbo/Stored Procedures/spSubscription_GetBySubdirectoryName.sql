CREATE PROCEDURE [dbo].[spSubscription_GetBySubdirectoryName]
	@SubdirectoryName nvarchar(50)
AS

SELECT * FROM [dbo].[Subscription] WHERE SubdirectoryName = @SubdirectoryName

GO
