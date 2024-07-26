-- 14072020
-- Sql Update

-- Update The Roles
  Update [dbo].[webpages_Roles] Set RoleName = 'User' Where RoleId = 2;
  INSERT INTO [dbo].[webpages_Roles](RoleName) VALUES('Manager')