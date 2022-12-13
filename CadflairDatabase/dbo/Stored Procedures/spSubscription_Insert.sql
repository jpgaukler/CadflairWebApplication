CREATE PROCEDURE [dbo].[spSubscription_Insert]
	@SubscriptionTypeId int,
	@CompanyName nvarchar(100),
	@PageName varchar(50),
	@OwnerId int,
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Subscription]
			   (SubscriptionTypeId
			   ,CompanyName
			   ,PageName
			   ,ExpiresOn
			   ,OwnerId
			   ,CreatedById)
		   VALUES
			   (@SubscriptionTypeId
			   ,@CompanyName
			   ,@PageName
			   ,DATEADD(DAY,30,SYSDATETIMEOFFSET()) --ExpiresOn
			   ,@OwnerId
			   ,@CreatedById)
		   SELECT CAST(SCOPE_IDENTITY() as int);

END

GO
