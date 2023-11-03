CREATE PROCEDURE [dbo].[spProductTable_DeleteById]
	@Id int
AS

DELETE [Attachment] FROM [Attachment]
INNER JOIN [Row] ON [Attachment].[RowId] = [Row].[Id]
INNER JOIN [ProductTable] ON [Row].[ProductTableId] = [ProductTable].[Id]
WHERE [ProductTableId] = @Id;
DELETE FROM [dbo].[TableValue] WHERE [ProductTableId] = @Id
DELETE FROM [dbo].[Row] WHERE [ProductTableId] = @Id
DELETE FROM [dbo].[Column] WHERE [ProductTableId] = @Id
DELETE FROM [dbo].[ProductTable] WHERE [Id] = @Id

GO
