﻿CREATE PROCEDURE [dbo].[spDriveFolder_Insert]
	@SubscriptionId int,
	@ParentId int,
	@DisplayName nvarchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[DriveFolder]
			   (SubscriptionId
			   ,ParentId
			   ,DisplayName
			   ,CreatedById)
		   VALUES
			   (@SubscriptionId
			   ,@ParentId
			   ,@DisplayName
			   ,@CreatedById)
		   SELECT * FROM [dbo].[DriveFolder] WHERE Id = SCOPE_IDENTITY();

END

GO
