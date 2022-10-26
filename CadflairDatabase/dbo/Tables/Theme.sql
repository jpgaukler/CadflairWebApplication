CREATE TABLE [dbo].[Theme] (
    [ThemeId]         INT        IDENTITY (1, 1) NOT NULL,
    [AccountId]       INT        NOT NULL,
    [MainColor]       NCHAR (10) NULL,
    [BackgroundColor] NCHAR (10) NULL,
    [Font]            NCHAR (10) NULL,
    [RoundedCorners]  BIT        CONSTRAINT [DF_Theme_RoundedCorners] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Theme] PRIMARY KEY CLUSTERED ([ThemeId] ASC),
    CONSTRAINT [FK_Theme_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([AccountId])
);

