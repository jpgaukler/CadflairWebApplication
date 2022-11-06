CREATE TABLE [dbo].[ApplicationRole] (
    [Id]             INT                NOT NULL IDENTITY (1, 1), 
    [Name]           VARCHAR(25)        NOT NULL,
    [NormalizedName] VARCHAR(25)        NOT NULL,
    [CreatedOn]      DATETIME           NOT NULL DEFAULT getdate(), 
    CONSTRAINT [PK_ApplicationRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_ApplicationRole_Name] UNIQUE([Name])
);

GO
 
CREATE INDEX [IX_ApplicationRole_NormalizedName] ON [dbo].[ApplicationRole] ([NormalizedName])

