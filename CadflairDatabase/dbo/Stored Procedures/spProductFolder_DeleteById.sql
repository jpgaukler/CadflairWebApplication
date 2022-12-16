CREATE PROCEDURE [dbo].[spProductFolder_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[ProductFolder] WHERE Id = @Id

GO
