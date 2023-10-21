CREATE PROCEDURE [dbo].[spRow_UpdateById]
    @Id int,
	@PartNumber nvarchar(50)
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Row]

	SET [PartNumber] = @PartNumber

	WHERE [Id] = @Id;

END

GO
