CREATE PROCEDURE [dbo].[spProductFamily_GetById]
	@Id int
AS

SELECT * FROM [dbo].[ProductFamily] WHERE Id = @Id

GO
