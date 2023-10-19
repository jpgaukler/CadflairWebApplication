CREATE PROCEDURE [dbo].[spColumn_DeleteById]
	@Id int
AS

DECLARE @ProductTableId AS INT;
SELECT TOP 1 @ProductTableId = [ProductTableId] FROM [Column] WHERE [Id] = @Id;

DELETE FROM [dbo].[TableValue] WHERE [ColumnId] = @Id
DELETE FROM [dbo].[Column] WHERE [Id] = @Id

DECLARE @ColumnCount AS INT;
SELECT @ColumnCount = COUNT(*) FROM [Column] WHERE [ProductTableId] = @ProductTableId;

-- if there are no columns remaining in the product table, then delete all the remaining (empty) rows in the table 
IF @ColumnCount = 0
	BEGIN
		DELETE FROM [dbo].[Row] WHERE [ProductTableId] = @ProductTableId
	END

GO
