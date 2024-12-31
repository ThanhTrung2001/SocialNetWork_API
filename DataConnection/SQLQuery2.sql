

-- Create the database
CREATE DATABASE DapperASPNetCore;
GO

USE DapperASPNetCore;
GO

-- Enums as tables
CREATE TABLE UserRole (
    Id INT PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

INSERT INTO UserRole (Id, RoleName) VALUES 
(1, 'CEO'),
(2, 'Admin'),
(3, 'Employee');

CREATE TABLE ReactType (
    Id INT PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL
);

INSERT INTO ReactType (Id, TypeName) VALUES 
(1, 'Like'),
(2, 'Heart'),
(3, 'Smile');

CREATE TABLE NotificationType (
    Id INT PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL
);

INSERT INTO NotificationType (Id, TypeName) VALUES 
(0, 'Warning'),
(1, 'Information'),
(2, 'HappyBirthday'),
(3, 'System'),
(4, 'Calendar');

CREATE TABLE FileType (
    Id INT PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL
);

INSERT INTO FileType (Id, TypeName) VALUES 
(1, 'Image'),
(2, 'Video'),
(3, 'File');

-- Tables
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    UserName NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    AvatarUrl NVARCHAR(255),
    Role INT NOT NULL FOREIGN KEY REFERENCES UserRole(Id)
);

CREATE TABLE Posts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    DestinationId NVARCHAR(100),
    IsNotification BIT NOT NULL DEFAULT 0,
    PostType NVARCHAR(50) NOT NULL,
    PostDestination NVARCHAR(255),
    Content NVARCHAR(MAX),
    OwnerId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id)
);

CREATE TABLE Comments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    Content NVARCHAR(MAX) NOT NULL,
    MediaURL NVARCHAR(255),
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
    PostId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Posts(Id)
);

CREATE TABLE Reacts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    ReactType INT NOT NULL FOREIGN KEY REFERENCES ReactType(Id),
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
    PostId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Posts(Id)
);

CREATE TABLE Surveys (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    ExpiredIn DATETIME NOT NULL,
    PostId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Posts(Id)
);

CREATE TABLE SurveyItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    Content NVARCHAR(MAX) NOT NULL,
    SurveyId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Surveys(Id)
);

CREATE TABLE MediaItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    FileType INT NOT NULL FOREIGN KEY REFERENCES FileType(Id),
    URL NVARCHAR(255) NOT NULL,
    PostId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Posts(Id)
);

CREATE TABLE Groups (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    GroupName NVARCHAR(100) NOT NULL,
    WallpaperURL NVARCHAR(255)
);

CREATE TABLE ChatBoxes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    BoxType NVARCHAR(50) NOT NULL,
    Theme NVARCHAR(50)
);

CREATE TABLE Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    Content NVARCHAR(MAX) NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
    ChatBoxId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES ChatBoxes(Id)
);

CREATE TABLE Notifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    NotiType INT NOT NULL FOREIGN KEY REFERENCES NotificationType(Id),
    DestinationId UNIQUEIDENTIFIER
);

ALTER TABLE Notifications
ADD 
    StartedAt DATETIME NULL, -- DateTime when the notification starts
    EndedAt DATETIME NULL,   -- DateTime when the notification ends
    OrganizeName NVARCHAR(255) NULL; -- Name of the organizing entity


--Create Table UserChatBox
--(
--	UserId Uniqueidentifier NOT NULL,
--    ChatBoxId Uniqueidentifier NOT NULL,
--    CONSTRAINT PK_UserChatBox PRIMARY KEY
--    (
--        UserId,
--        ChatBoxId
--    ),
--    FOREIGN KEY (UserId) REFERENCES Users (Id),
--    FOREIGN KEY (ChatBoxId) REFERENCES Chatboxes (Id)
--)

CREATE TABLE UserGroup (
    UserId UNIQUEIDENTIFIER NOT NULL,    -- Foreign Key to Users table
    GroupId UNIQUEIDENTIFIER NOT NULL,   -- Foreign Key to Groups table
    RoleId INT NOT NULL,                 -- Foreign Key to Role table
    JoinedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Tracks when the user joined the group
    PRIMARY KEY (UserId, GroupId),       -- Composite Primary Key
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (GroupId) REFERENCES Groups(Id),
    FOREIGN KEY (RoleId) REFERENCES UserRole(Id) -- Links to Role table
);

CREATE TABLE UserGroups (
    UserId UNIQUEIDENTIFIER NOT NULL,    -- Foreign Key to Users table
    GroupId UNIQUEIDENTIFIER NOT NULL,   -- Foreign Key to Groups table
    RoleId INT NOT NULL,                 -- Foreign Key to Role table
    JoinedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Tracks when the user joined the group
    PRIMARY KEY (UserId, GroupId),       -- Composite Primary Key
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (GroupId) REFERENCES Groups(Id),
    FOREIGN KEY (RoleId) REFERENCES UserRole(Id) -- Links to Role table
);

ALTER TABLE Messages
ADD IsPinned BIT NOT NULL DEFAULT 0;

CREATE TABLE SharePosts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SharedPostId UNIQUEIDENTIFIER NOT NULL, -- ID bài viết gốc
    SharedByUserId UNIQUEIDENTIFIER NOT NULL, -- Người chia sẻ bài viết
    TargetType NVARCHAR(50) NOT NULL, -- Loại đối tượng chia sẻ (User, Group, Page)
    TargetId UNIQUEIDENTIFIER, -- ID của đối tượng nhận bài viết
    SharedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Thời điểm chia sẻ
    FOREIGN KEY (SharedPostId) REFERENCES Posts(Id), -- Liên kết với bài viết gốc
    FOREIGN KEY (SharedByUserId) REFERENCES Users(Id) -- Liên kết với người chia sẻ
);
