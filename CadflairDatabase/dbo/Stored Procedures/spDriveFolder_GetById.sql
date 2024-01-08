CREATE PROCEDURE [dbo].[spDriveFolder_GetById]
	@Id int
AS

SELECT * FROM [dbo].[DriveFolder] WHERE Id = @Id

GO
