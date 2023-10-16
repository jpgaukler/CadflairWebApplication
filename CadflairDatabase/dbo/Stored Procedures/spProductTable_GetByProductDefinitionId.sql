CREATE PROCEDURE [dbo].[spProductTable_GetByProductDefinitionId]
	@ProductDefinitionId int
AS

SELECT * FROM [dbo].[ProductTable] WHERE [ProductDefinitionId] = @ProductDefinitionId 

GO
