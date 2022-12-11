CREATE TABLE [dbo].[ProductFamily]
(
    [Id]              INT                  NOT NULL IDENTITY (1, 1),
    [SubscriptionId]  INT                  NOT NULL,
    [ParentId]        INT                  NULL,
    [DisplayName]     NVARCHAR(50)         NOT NULL,
    [CreatedById]     INT                  NOT NULL,
    [CreatedOn]       DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_ProductFamily] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductFamily_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([Id]),
    CONSTRAINT [FK_ProductFamily_ProductFamily] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[ProductFamily] ([Id]),
    CONSTRAINT [UC_ParentId_DisplayName] UNIQUE([ParentId],[DisplayName]),
)

GO

