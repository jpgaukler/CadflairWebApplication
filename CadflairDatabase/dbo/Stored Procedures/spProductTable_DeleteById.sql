CREATE PROCEDURE [dbo].[spProductTable_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[TableValue] WHERE [ProductTableId] = @Id
DELETE FROM [dbo].[Row] WHERE [ProductTableId] = @Id
DELETE FROM [dbo].[Column] WHERE [ProductTableId] = @Id
DELETE FROM [dbo].[ProductTable] WHERE [Id] = @Id
-- need to figure out how to handle deleting attachments

GO
