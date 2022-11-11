CREATE TABLE [dbo].[UserRole] (
    [Id]             INT                NOT NULL IDENTITY (1, 1), 
    [Name]           VARCHAR(25)        NOT NULL,
    [CreatedOn]      DATETIME           NOT NULL DEFAULT getdate(), 
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_UserRole_Name] UNIQUE([Name])
);
