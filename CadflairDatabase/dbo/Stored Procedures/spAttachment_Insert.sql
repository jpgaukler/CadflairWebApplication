CREATE PROCEDURE [dbo].[spAttachment_Insert]
	@RowId int,
	@ForgeObjectKey varchar(50),
	@CreatedById int
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Attachment]
			   ([RowId]
			   ,[ForgeObjectKey]
			   ,[CreatedById])
		   VALUES
			   (@RowId
			   ,@ForgeObjectKey
			   ,@CreatedById)
		   SELECT * FROM [dbo].[Attachment] WHERE Id = SCOPE_IDENTITY();

END

GO
