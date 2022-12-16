CREATE PROCEDURE [dbo].[spSubscription_Insert]
	@SubscriptionTypeId int,
	@CompanyName nvarchar(50),
	@SubdirectoryName varchar(50),
	@OwnerId int,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Subscription]
			   (SubscriptionTypeId
			   ,CompanyName
			   ,[SubdirectoryName]
			   ,ExpiresOn
			   ,OwnerId
			   ,CreatedById)
		   VALUES
			   (@SubscriptionTypeId
			   ,@CompanyName
			   ,@SubdirectoryName
			   ,DATEADD(DAY,30,SYSDATETIMEOFFSET()) --ExpiresOn
			   ,@OwnerId
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Subscription] WHERE Id = SCOPE_IDENTITY();

END

GO
