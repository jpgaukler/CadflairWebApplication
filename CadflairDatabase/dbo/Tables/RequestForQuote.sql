CREATE TABLE [dbo].[RequestForQuote] (
    [Id] INT            NOT NULL IDENTITY (1, 1), 
    [ProductConfigurationId]   INT            NOT NULL,
    [FirstName]         NVARCHAR (50)  NOT NULL,
    [LastName]          NVARCHAR (50)  NOT NULL,
    [EmailAddress]      NVARCHAR (100) NOT NULL,
    [PhoneNumber]       NVARCHAR (25)  NULL,
    [PhoneExtension]    NVARCHAR (25)  NULL,
    [CreatedOn]         DATETIME       NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK_RequestForQuote] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RequestForQuote_ProductConfiguration] FOREIGN KEY ([ProductConfigurationId]) REFERENCES [dbo].[ProductConfiguration] ([Id])
);

