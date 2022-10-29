/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--add subscription types 
if not exists (select * from dbo.[AccountType] where [AccountType].[Name] = 'Basic')
	begin
		insert into dbo.[AccountType]([Name])
		values
			('Basic'),
			('Pro');
	end


--add user roles
if not exists (select * from dbo.[UserType] where [UserType].[Name] = 'Admin')
	begin
		insert into dbo.[UserType]([Name])
		values
			('Admin'),
			('Publisher'),
			('Reviewer')
	end

--populate email address types
if not exists (select * from dbo.[EmailAddressType] where [EmailAddressType].[Name] = 'Primary')
	begin
		insert into dbo.EmailAddressType([Name])
		values('Primary');

		insert into dbo.EmailAddressType([Name])
		values('Secondary');
	end

--add demo user
if not exists (select * from dbo.[User] where [User].[FirstName] = 'Demo')
	begin
		insert into dbo.[User](RoleId, UserName, FirstName, LastName, PasswordHash)
		values
			(1, 'jpgaukler', 'Demo', 'User', 'dhivebisdy');
	end


--add demo account
if not exists (select * from dbo.[Account] where [Account].CompanyName = 'Demo Account')
	begin
		insert into dbo.Account(CompanyName, SubDirectory, CreatedBy, [Owner], [AccountTypeId], SubscriptionExpiresOn)
		values
			('Demo Account', 'demo', 1, 1, 1, dateadd(day,30,getdate()));
	end


--add demo product family
if not exists (select * from dbo.[ProductFamily] where [ProductFamily].[DisplayName] = 'Test')
	begin
		insert into dbo.ProductFamily(DisplayName, AccountId, CreatedBy)
		values
			 ('Test', 1, 1);
	end







