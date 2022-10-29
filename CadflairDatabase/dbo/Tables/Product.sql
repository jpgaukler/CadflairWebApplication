CREATE TABLE [dbo].[Product] (
	[Id]                INT              NOT NULL PRIMARY KEY IDENTITY, 
    [ProductFamilyId]	INT              NOT NULL,
    [DisplayName]		[dbo].[DisplayName]    NOT NULL,
    [ParameterJson]		 NVARCHAR (MAX)   NULL, --I am not sure if this should be MAX, might want to try and determine the actual length of this field
    [ForgeBucketKey]	 UNIQUEIDENTIFIER NOT NULL,
    [ForgeObjectKey]	 UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]			 DATETIME         NOT NULL DEFAULT getdate(),
    [CreatedBy]			 INT              NOT NULL,
    [IsPublic]			 BIT              NOT NULL DEFAULT 0,
    [IsConfigurable]     BIT              NOT NULL DEFAULT 0,
    CONSTRAINT [FK_Product_ProductFamily] FOREIGN KEY ([ProductFamilyId]) REFERENCES [dbo].[ProductFamily] ([Id]),
    CONSTRAINT [FK_Product_User] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_ProductFamilyId_DisplayName] UNIQUE([ProductFamilyId],[DisplayName])
);

