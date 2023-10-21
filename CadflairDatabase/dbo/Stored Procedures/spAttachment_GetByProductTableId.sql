CREATE PROCEDURE [dbo].[spAttachment_GetByProductTableId]
	@ProductTableId int
AS

SELECT * FROM [Attachment]
INNER JOIN [Row] ON [Attachment].[RowId] = [Row].[Id]
INNER JOIN [ProductTable] ON [Row].[ProductTableId] = [ProductTable].[Id]
WHERE [ProductTableId] = @ProductTableId;

GO
