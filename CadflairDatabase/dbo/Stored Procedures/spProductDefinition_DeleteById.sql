CREATE PROCEDURE [dbo].[spProductDefinition_DeleteById]
	@Id int
AS

DECLARE @ProductTableId AS INT;
SELECT TOP 1 @ProductTableId = [Id] FROM [ProductTable] WHERE [ProductDefinitionId] = @Id;

DELETE [Attachment] FROM [Attachment]
INNER JOIN [Row] ON [Attachment].[RowId] = [Row].[Id]
INNER JOIN [ProductTable] ON [Row].[ProductTableId] = [ProductTable].[Id]
INNER JOIN [ProductDefinition] ON [ProductTable].[ProductDefinitionId] = [ProductDefinition].[Id]
WHERE [ProductDefinitionId] = @Id;

DELETE FROM [dbo].[TableValue] WHERE [ProductTableId] = @ProductTableId;
DELETE FROM [dbo].[Row] WHERE [ProductTableId] = @ProductTableId;
DELETE FROM [dbo].[Column] WHERE [ProductTableId] = @ProductTableId;
DELETE FROM [dbo].[ProductTable] WHERE [ProductDefinitionId] = @Id;
DELETE FROM [dbo].[ProductDefinition] WHERE [Id] = @Id;

GO
