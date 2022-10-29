CREATE TABLE [dbo].[ProductFamily]
(
    [Id]            INT              NOT NULL IDENTITY (1, 1),
    [ParentId]      INT              NULL,
    [AccountId]     INT              NOT NULL,
    [DisplayName]   [dbo].[DisplayName]    NOT NULL,
    [CreatedBy]	    INT              NOT NULL,
    [CreatedOn]     DATETIME         NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK_ProductFamily] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductFamily_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_ProductFamily_ProductFamily] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[ProductFamily] ([Id]),
    CONSTRAINT [UC_AccountId_DisplayName] UNIQUE([AccountId],[DisplayName]),
)

GO
