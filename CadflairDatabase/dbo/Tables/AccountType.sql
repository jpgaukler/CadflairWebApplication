CREATE TABLE [dbo].[AccountType] (
    [Id]              INT                 NOT NULL IDENTITY (1, 1),
    [Name]            varchar(25)         NOT NULL,
    [CreatedOn]       DATETIME            NOT NULL DEFAULT getdate(), 
    CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_AccountType_Name] UNIQUE([Name])
);

