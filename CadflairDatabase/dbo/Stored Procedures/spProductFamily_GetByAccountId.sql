CREATE PROCEDURE [dbo].[spProductFamily_GetByAccountId]
	@AccountId int
AS

SELECT * FROM [dbo].[ProductFamily] WHERE AccountId = @AccountId

GO
