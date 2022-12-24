CREATE TABLE [dbo].[ProductVersion]
(
    [Id]                 INT                       NOT NULL IDENTITY (1, 1),
    [ProductId]          INT                       NOT NULL,
    [RootFileName]		 NVARCHAR(50)              NOT NULL,
    [ILogicFormJson]	 NVARCHAR (MAX)            NULL, 
    [VersionNumber]      INT                       NOT NULL DEFAULT 1,
    [IsConfigurable]     BIT                       NOT NULL DEFAULT 0,
    [CreatedOn]			 DATETIMEOFFSET(7)    	   NOT NULL DEFAULT sysdatetimeoffset(),
    [CreatedById]        INT                       NOT NULL,
    CONSTRAINT [PK_ProductVersion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductVersion_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id]),
    CONSTRAINT [FK_ProductVersion_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id]),
)
