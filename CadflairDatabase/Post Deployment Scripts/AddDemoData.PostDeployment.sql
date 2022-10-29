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
if not exists (select * from dbo.[SubscriptionType] where [SubscriptionType].Id = 1)
	begin
		insert into dbo.[SubscriptionType](DisplayName)
		values
			('Basic'),
			('Pro');
	end


--add user roles
if not exists (select * from dbo.[Role] where [Role].Id = 1)
	begin
		insert into dbo.[Role](DisplayName)
		values
			('Admin'),
			('Publisher'),
			('Reviewer')
	end

--populate email address types
if not exists (select * from dbo.[EmailAddressType] where [EmailAddressType].Id = 1)
	begin
		insert into dbo.EmailAddressType(DisplayName)
		values('Primary');

		insert into dbo.EmailAddressType(DisplayName)
		values('Secondary');
	end

--add demo user
if not exists (select * from dbo.[User] where [User].Id = 1)
	begin
		insert into dbo.[User](RoleId, FirstName, LastName, PasswordHash)
		values
			(1, 'Justin', 'Gaukler', 'dhivebisdy');
	end


--add demo account
if not exists (select * from dbo.[Account] where [Account].Id = 1)
	begin
		insert into dbo.Account(CompanyName, SubDirectory, CreatedBy, [Owner], SubscriptionTypeId, SubscriptionExpiresOn)
		values
			('Demo Account', 'demo', 1, 1, 1, dateadd(day,30,getdate()));
	end








