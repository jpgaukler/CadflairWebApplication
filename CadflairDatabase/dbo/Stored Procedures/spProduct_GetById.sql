CREATE PROCEDURE [dbo].[spProduct_GetById]
	@Id int
AS

SELECT * FROM [dbo].[Product] WHERE Id = @Id

GO
