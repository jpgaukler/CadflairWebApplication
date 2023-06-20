CREATE PROCEDURE [dbo].[spProductConfiguration_Insert]
	@ProductVersionId int, 
	@IsDefault bit,
	@ArgumentJson nvarchar(MAX)
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductConfiguration]
                ([ProductVersionId]
				,IsDefault
                ,ArgumentJson)
           VALUES
			    (@ProductVersionId
				,@IsDefault
		   	    ,@ArgumentJson)
		   SELECT * FROM [dbo].[ProductConfiguration] WHERE Id = SCOPE_IDENTITY();
END

GO
