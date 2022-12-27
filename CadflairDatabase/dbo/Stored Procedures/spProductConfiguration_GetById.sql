CREATE PROCEDURE [dbo].[spProductConfiguration_GetById]
	@Id int
AS

SELECT * FROM [dbo].[ProductConfiguration] WHERE [Id] = @Id

GO
