CREATE PROCEDURE [dbo].[spProduct_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[Product] WHERE Id = @Id

GO
