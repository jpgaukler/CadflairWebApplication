CREATE PROCEDURE [dbo].[spColumn_GetByProductTableId]
	@ProductTableId int
AS

SELECT * FROM [dbo].[Column] WHERE [ProductTableId] = @ProductTableId

GO
