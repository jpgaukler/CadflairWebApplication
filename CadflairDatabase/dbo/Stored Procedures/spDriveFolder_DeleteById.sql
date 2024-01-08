CREATE PROCEDURE [dbo].[spDriveFolder_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[DriveFolder] WHERE Id = @Id

GO
