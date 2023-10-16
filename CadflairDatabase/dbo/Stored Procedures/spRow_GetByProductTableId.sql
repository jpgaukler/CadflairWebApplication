CREATE PROCEDURE [dbo].[spRow_GetByProductTableId]
	@ProductTableId int
AS

SELECT * FROM [dbo].[Row] WHERE [ProductTableId] = @ProductTableId

GO
