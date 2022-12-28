CREATE TABLE [dbo].[ProductConfiguration] (
    [Id]                    INT                  NOT NULL IDENTITY (1, 1),
    [ProductVersionId]      INT                  NOT NULL,
    [IsDefault]             BIT                  NOT NULL,
    [ArgumentJson]          NVARCHAR (MAX)       NOT NULL, 
    [ForgeZipKey]           VARCHAR(50)          NULL,
    [ForgeStpKey]           VARCHAR(50)          NULL,
    [ForgePdfKey]           VARCHAR(50)          NULL,
    [ForgeDwgKey]           VARCHAR(50)          NULL,
    [CreatedOn]             DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_ProductConfigutation] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductConfiguration_ProductVersion] FOREIGN KEY ([ProductVersionId]) REFERENCES [dbo].[ProductVersion] ([Id]),
    --CONSTRAINT [UC_ProductConfiguration_ArgumentJson] UNIQUE([ArgumentJson])
);

