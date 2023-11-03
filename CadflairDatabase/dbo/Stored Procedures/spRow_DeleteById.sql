CREATE PROCEDURE [dbo].[spRow_DeleteById]
	@Id int
AS

DELETE [Attachment] FROM [Attachment]
INNER JOIN [Row] ON [Attachment].[RowId] = [Row].[Id]
WHERE [RowId] = @Id;

DELETE FROM [dbo].[TableValue] WHERE [RowId] = @Id
DELETE FROM [dbo].[Row] WHERE [Id] = @Id

GO
