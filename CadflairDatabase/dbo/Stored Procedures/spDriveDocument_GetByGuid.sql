CREATE PROCEDURE [dbo].[spDriveDocument_GetByGuid]
	@Guid UNIQUEIDENTIFIER
AS

SELECT TOP 1 * FROM [dbo].[DriveDocument] WHERE [Guid] = @Guid

GO
