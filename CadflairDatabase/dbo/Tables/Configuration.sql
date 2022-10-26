CREATE TABLE [dbo].[Configuration] (
    [ConfigurationId] INT              IDENTITY (1, 1) NOT NULL,
    [ProductId]       INT              NOT NULL,
    [ArgumentJson]    NVARCHAR (MAX)   NOT NULL,
    [ForgeBucketKey]  UNIQUEIDENTIFIER NOT NULL,
    [ForgeObjectKey]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         CONSTRAINT [DF_Configuration_CreatedOn] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED ([ConfigurationId] ASC),
    CONSTRAINT [FK_Configuration_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId])
);

