CREATE PROCEDURE [dbo].[spDriveFolder_GetBySubscriptionId]
	@SubscriptionId int
AS

SELECT * FROM [dbo].[DriveFolder] WHERE [SubscriptionId] = @SubscriptionId ORDER BY [DisplayName]

GO
