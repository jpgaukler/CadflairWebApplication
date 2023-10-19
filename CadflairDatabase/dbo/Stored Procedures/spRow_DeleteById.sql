CREATE PROCEDURE [dbo].[spRow_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[TableValue] WHERE [RowId] = @Id
DELETE FROM [dbo].[Row] WHERE [Id] = @Id

GO
