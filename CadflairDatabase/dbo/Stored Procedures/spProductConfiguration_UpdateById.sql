CREATE PROCEDURE [dbo].[spProductConfiguration_UpdateById]
    @Id int,
	@ProductVersionId int, 
	@IsDefault bit,
	@ArgumentJson nvarchar(MAX),
	@ForgeZipKey uniqueidentifier,
	@ForgeStpKey uniqueidentifier,
	@ForgePdfKey uniqueidentifier,
	@ForgeDwgKey uniqueidentifier 
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[ProductConfiguration]

	SET ProductVersionId = @ProductVersionId,
		IsDefault = @IsDefault,
		ArgumentJson = @ArgumentJson,
		ForgeZipKey = @ForgeZipKey,
		ForgeStpKey = @ForgeStpKey,
		ForgeDwgKey = @ForgeDwgKey,
		ForgePdfKey = @ForgePdfKey

	WHERE Id = @Id;

END

GO
