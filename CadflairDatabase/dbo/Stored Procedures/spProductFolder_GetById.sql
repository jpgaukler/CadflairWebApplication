CREATE PROCEDURE [dbo].[spProductFolder_GetById]
	@Id int
AS

SELECT * FROM [dbo].[ProductFolder] WHERE Id = @Id

GO
