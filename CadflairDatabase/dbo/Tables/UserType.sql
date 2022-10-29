CREATE TABLE [dbo].[UserType] (
    [Id]        INT                NOT NULL IDENTITY (1, 1), 
    [Name]      [dbo].[TypeName]   NOT NULL,
    [CreatedOn] DATETIME           NOT NULL DEFAULT getdate(), 
    CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_UserType_Name] UNIQUE([Name])
);

