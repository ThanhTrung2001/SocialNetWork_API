-- Insert seed data into the users table for an admin user
INSERT INTO [users] ([id], [created_at], [updated_at], [is_deleted], [username], [password], [email], [role])
VALUES 
(
    NEWID(),                        -- Unique identifier for the admin user
    GETDATE(),                      -- Current date for created_at
    GETDATE(),                      -- Current date for updated_at
    0,                              -- is_deleted = false
    'admin',                        -- Username for admin
    'Admin@123',                    -- Password for admin
    'admin@example.com',            -- Admin email
    'Admin'                         -- Role
);

-- Optionally retrieve the user_id for the newly created admin user
DECLARE @AdminUserId UNIQUEIDENTIFIER;
SET @AdminUserId = (SELECT TOP 1 [id] FROM [users] WHERE [username] = 'admin');

-- Insert seed data into the user_details table for the admin user
INSERT INTO [user_details] ([id], [created_at], [updated_at], [is_deleted], [firstname], [lastname], [phone_number], [address], [city], [country], [avatar], [wallpaper], [dob], [bio], [user_id])
VALUES 
(
    NEWID(),                        -- Unique identifier for the user details
    GETDATE(),                      -- Current date for created_at
    GETDATE(),                      -- Current date for updated_at
    0,                              -- is_deleted = false
    'Admin',                        -- First name
    'User',                         -- Last name
    '123456789',                    -- Phone number
    '123 Admin Street',             -- Address
    'Admin City',                   -- City
    'Admin Country',                -- Country
    NULL,                           -- Avatar (NULL for default)
    NULL,                           -- Wallpaper (NULL for default)
    '1980-01-01',                   -- Date of birth
    'This is the admin user.',      -- Bio
    @AdminUserId                    -- Foreign key referencing the admin user
);