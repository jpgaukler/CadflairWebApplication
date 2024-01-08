CREATE PROCEDURE [dbo].[spDriveDocument_GetByDriveFolderId]
	@DriveFolderId int
AS

SELECT * FROM [dbo].[DriveDocument] WHERE DriveFolderId = @DriveFolderId

GO
