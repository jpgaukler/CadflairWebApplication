CREATE PROCEDURE [dbo].[spProductFamily_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[ProductFamily] WHERE Id = @Id

GO
