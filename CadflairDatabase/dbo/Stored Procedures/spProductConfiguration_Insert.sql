CREATE PROCEDURE [dbo].[spProductConfiguration_Insert]
	@ProductVersionId int, 
	@IsDefault bit,
	@ArgumentJson nvarchar(MAX),
	@ForgeZipKey varchar(50) 
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductConfiguration]
                ([ProductVersionId]
				,IsDefault
                ,ArgumentJson
                ,ForgeZipKey)
           VALUES
			    (@ProductVersionId
				,@IsDefault
		   	    ,@ArgumentJson
			    ,@ForgeZipKey)
		   SELECT * FROM [dbo].[ProductConfiguration] WHERE Id = SCOPE_IDENTITY();
END

GO
