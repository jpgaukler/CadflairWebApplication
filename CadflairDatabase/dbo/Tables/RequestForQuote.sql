CREATE TABLE [dbo].[RequestForQuote] (
    [RequestForQuoteId] INT            IDENTITY (1, 1) NOT NULL,
    [ConfigurationId]   INT            NOT NULL,
    [FirstName]         NVARCHAR (50)  NOT NULL,
    [LastName]          NVARCHAR (50)  NOT NULL,
    [EmailAddress]      NVARCHAR (100) NOT NULL,
    [PhoneNumber]       NVARCHAR (25)  NULL,
    [PhoneExtension]    NVARCHAR (25)  NULL,
    [CreatedOn]         DATETIME       CONSTRAINT [DF_RequestForQuote_RequestedOn] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_RequestForQuote] PRIMARY KEY CLUSTERED ([RequestForQuoteId] ASC),
    CONSTRAINT [FK_RequestForQuote_Configuration] FOREIGN KEY ([ConfigurationId]) REFERENCES [dbo].[ProductConfiguration] ([Id])
);

