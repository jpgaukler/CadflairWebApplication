CREATE PROCEDURE [dbo].[spCategory_DeleteById]
	@Id int
AS

UPDATE [dbo].[ProductDefinition] SET [CategoryId] = NULL WHERE [CategoryId] = @Id;
DELETE FROM [dbo].[Category] WHERE [Id] = @Id;

GO
