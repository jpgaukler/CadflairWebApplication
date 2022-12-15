CREATE PROCEDURE [dbo].[spProductConfiguration_Insert]
	@ProductId int, 
	@IsDefault bit,
	@ArgumentJson nvarchar(4000),
	@ForgeZipKey uniqueidentifier
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProductConfiguration]
                (ProductId
				,IsDefault
                ,ArgumentJson
                ,ForgeZipKey)
           VALUES
			    (@ProductId
				,@IsDefault
		   	    ,@ArgumentJson
			    ,@ForgeZipKey)
		   SELECT * FROM [dbo].[ProductConfiguration] WHERE Id = SCOPE_IDENTITY();
END

GO
