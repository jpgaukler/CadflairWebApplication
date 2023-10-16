CREATE PROCEDURE [dbo].[spTableValue_GetByProductTableId]
	@ProductTableId int
AS

SELECT * FROM [dbo].[TableValue] WHERE [ProductTableId] = @ProductTableId

GO
