CREATE TABLE [dbo].[Product] (
    [ProductId]      INT              IDENTITY (1, 1) NOT NULL,
    [AccountId]      INT              NOT NULL,
    [DisplayName]    NVARCHAR (100)   NOT NULL,
    [ParameterJson]  NVARCHAR (MAX)   NOT NULL,
    [ForgeBucketKey] UNIQUEIDENTIFIER NOT NULL,
    [ForgeObjectKey] UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         CONSTRAINT [DF_Product_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]      INT              NOT NULL,
    [IsPublic]       BIT              CONSTRAINT [DF_Product_IsPublic] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId] ASC),
    CONSTRAINT [FK_Product_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([AccountId]),
    CONSTRAINT [FK_Product_User] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([UserId])
);

