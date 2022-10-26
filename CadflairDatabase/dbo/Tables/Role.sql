CREATE TABLE [dbo].[Role] (
    [RoleId]      INT          IDENTITY (1, 1) NOT NULL,
    [DisplayName] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);

