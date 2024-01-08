CREATE PROCEDURE [dbo].[spDriveDocument_DeleteById]
	@Id INT
AS

DELETE FROM [dbo].[DriveDocument] WHERE [Id] = @Id

GO
