CREATE TABLE [dbo].[ProductFamily]
(
    [Id]            INT              NOT NULL IDENTITY (1, 1),
    [ParentId]      INT              NULL,
    [AccountId]     INT              NOT NULL,
    [DisplayName]   NVARCHAR(50)     NOT NULL,
    [CreatedById]   INT              NOT NULL,
    [CreatedOn]     DATETIME         NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK_ProductFamily] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductFamily_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_ProductFamily_ProductFamily] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[ProductFamily] ([Id]),
    CONSTRAINT [UC_ParentId_DisplayName] UNIQUE([ParentId],[DisplayName]),
)

GO

