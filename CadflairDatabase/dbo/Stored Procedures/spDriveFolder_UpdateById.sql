CREATE PROCEDURE [dbo].[spDriveFolder_UpdateById]
	@Id				 INT,
    @ParentId        INT,
    @DisplayName     NVARCHAR(50)
AS

BEGIN

	SET NOCOUNT ON;

	UPDATE [dbo].[DriveFolder] 

	SET [ParentId] = @ParentId,
		[DisplayName] = @DisplayName

	WHERE Id = @Id;

END

GO
