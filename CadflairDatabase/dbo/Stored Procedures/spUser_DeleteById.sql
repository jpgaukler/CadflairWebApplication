CREATE PROCEDURE [dbo].[spUser_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[User] WHERE Id = @Id

GO
