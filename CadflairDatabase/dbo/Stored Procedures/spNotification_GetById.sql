CREATE PROCEDURE [dbo].[spNotification_GetById]
	@Id int
AS

SELECT * FROM [dbo].[Notification] WHERE Id = @Id


GO
