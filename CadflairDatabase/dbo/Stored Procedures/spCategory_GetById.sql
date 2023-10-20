CREATE PROCEDURE [dbo].[spCategory_GetById]
	@Id int
AS

SELECT * FROM [dbo].[Category] WHERE [Id] = @Id

GO
