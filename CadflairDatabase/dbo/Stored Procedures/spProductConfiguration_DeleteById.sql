CREATE PROCEDURE [dbo].[spProductConfiguration_DeleteById]
	@Id int
AS

DELETE FROM [dbo].[ProductConfiguration] WHERE Id = @Id

GO
