CREATE PROCEDURE [dbo].[spProductConfiguration_Insert]
	@ProductId int, 
	@ArgumentJson nvarchar(4000),
	@ForgeObjectKey uniqueidentifier
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductConfiguration]
                (ProductId
                ,ArgumentJson
                ,ForgeObjectKey)
           VALUES
			    (@ProductId
		   	    ,@ArgumentJson
			    ,@ForgeObjectKey)
		   SELECT CAST(SCOPE_IDENTITY() as int);
END

GO
