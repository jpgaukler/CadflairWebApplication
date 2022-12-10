CREATE PROCEDURE [dbo].[spUser_GetByObjectIdentifier]
	@ObjectIdentifier uniqueidentifier
AS

SELECT * FROM [dbo].[User] WHERE ObjectIdentifier = @ObjectIdentifier

GO
