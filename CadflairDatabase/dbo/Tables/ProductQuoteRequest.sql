CREATE TABLE [dbo].[ProductQuoteRequest] (
    [Id]                       INT                  NOT NULL IDENTITY (1, 1), 
    [ProductConfigurationId]   INT                  NOT NULL,
    [FirstName]                NVARCHAR (50)        NOT NULL,
    [LastName]                 NVARCHAR (50)        NOT NULL,
    [EmailAddress]             NVARCHAR (100)       NOT NULL,
    [PhoneNumber]              NVARCHAR (25)        NULL,
    [PhoneExtension]           NVARCHAR (10)        NULL,
    [MessageText]              NVARCHAR (500)       NULL,
    [CreatedOn]                DATETIMEOFFSET(7)    NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_RequestForQuote] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RequestForQuote_ProductConfiguration] FOREIGN KEY ([ProductConfigurationId]) REFERENCES [dbo].[ProductConfiguration] ([Id])
);

