CREATE PROCEDURE [dbo].[spProductDefinition_DeleteById]
	@Id int
AS

DECLARE @ProductTableId AS INT;
SELECT TOP 1 @ProductTableId = [Id] FROM [ProductTable] WHERE [ProductDefinitionId] = @Id;

DELETE FROM [dbo].[TableValue] WHERE [ProductTableId] = @ProductTableId
DELETE FROM [dbo].[Row] WHERE [ProductTableId] = @ProductTableId
DELETE FROM [dbo].[Column] WHERE [ProductTableId] = @ProductTableId
DELETE FROM [dbo].[ProductTable] WHERE [ProductDefinitionId] = @Id
DELETE FROM [dbo].[ProductDefinition] WHERE [Id] = @Id
-- need to figure out how to handle deleting attachments
-- need to figure out how to delete the thumbnail if there is one

GO
