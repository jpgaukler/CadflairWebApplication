CREATE PROCEDURE [dbo].[spColumn_UpdateById]
	@Id int,
	@Header nvarchar(50),
	@SortOrder int 
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Column]

	SET [Header] = @Header,
		[SortOrder] = @SortOrder

	WHERE [Id] = @Id;

END

GO
