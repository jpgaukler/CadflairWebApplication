CREATE TABLE [dbo].[ContactRequest] (
    [Id]                 INT                       NOT NULL IDENTITY (1, 1),
    [FirstName]		     NVARCHAR(30)              NOT NULL,
    [LastName]           NVARCHAR(30)              NOT NULL,
    [EmailAddress]       NVARCHAR(100)             NOT NULL,
    [CompanyName]        NVARCHAR(100)             NULL,
    [Message]            NVARCHAR(500)             NULL,
    [CreatedOn]			 DATETIMEOFFSET(7)    	   NOT NULL DEFAULT sysdatetimeoffset(),
    CONSTRAINT [PK_ContactRequest] PRIMARY KEY CLUSTERED ([Id] ASC)
);

