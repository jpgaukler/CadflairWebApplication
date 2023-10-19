CREATE PROCEDURE [dbo].[spTableValue_UpdateById]
    @Id int,
	@RowId int, 
	@ColumnId int, 
	@Value nvarchar(50)
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[TableValue]

	SET [RowId] = @RowId,
		[ColumnId] = @ColumnId,
		[Value] = @Value

	WHERE [Id] = @Id;

END

GO
