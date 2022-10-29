CREATE TABLE [dbo].[ProductFamily]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [ParentId]      INT              NOT NULL,
    --[RootId]      INT              NOT NULL,
    [AccountId]      INT              NOT NULL,
    [DisplayName]    [dbo].[DisplayName]    NOT NULL,
    [CreatedBy]	      INT              NOT NULL,
    [CreatedOn]			 DATETIME         NOT NULL DEFAULT getdate(),
    CONSTRAINT [FK_ProductFamily_Account] FOREIGN KEY ([Id]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_ProductFamily_ProductFamily1] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[ProductFamily] ([Id]),
    --CONSTRAINT [FK_ProductFamily_ProductFamily2] FOREIGN KEY ([RootId]) REFERENCES [dbo].[ProductFamily] ([Id]),
    CONSTRAINT [UC_AccountId_DisplayName] UNIQUE([AccountId],[DisplayName]),
)
