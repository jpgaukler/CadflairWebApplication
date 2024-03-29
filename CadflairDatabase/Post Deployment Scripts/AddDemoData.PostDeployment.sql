﻿/*
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
if not exists (select * from dbo.[SubscriptionType] where [SubscriptionType].[Name] = 'Basic')
	begin
		insert into dbo.[SubscriptionType]([Name])
		values
			('Basic'),
			('Pro');
	end

--add notifications 
if not exists (select * from dbo.[Notification] where [Notification].[EventName] = 'ProductQuoteRequest_Insert')
	begin
		insert into dbo.[Notification]([EventName], [EnabledByDefault])
		values
			('ProductQuoteRequest_Insert', 1)
	end


----populate email address types
--if not exists (select * from dbo.[EmailAddressType] where [EmailAddressType].[Name] = 'Primary')
--	begin
--		insert into dbo.EmailAddressType([Name])
--		values('Primary');

--		insert into dbo.EmailAddressType([Name])
--		values('Secondary');
--	end

----add demo user
--if not exists (select * from dbo.[User] where [User].[EmailAddress] = 'jpgaukler@gmail.com')
--	begin
--		insert into dbo.[User]([UserRoleId], [FirstName], [LastName], [EmailAddress], [PasswordHash])
--		values
--			(1, 'Justin', 'Gaukler', 'jpgaukler@gmail.com', 'password');
--	end


----add demo account
--if not exists (select * from dbo.[Subscription] where [Subscription].[CompanyName] = 'Demo Account')
--	begin
--		insert into dbo.[Subscription]([CompanyName], [SubdirectoryName], [CreatedById], [OwnerId], [SubscriptionTypeId], [ExpiresOn])
--		values
--			('Demo Account', 'demo', 1, 1, 1, dateadd(day,30,getdate()))
--	end








