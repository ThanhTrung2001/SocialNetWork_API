-- Create the database
--DROP DATABASE SocialNetworkSQLDB;
--GO

CREATE DATABASE SocialNetworkSQLDB;
GO

USE SocialNetworkSQLDB;
GO

-- Enums as tables
CREATE TABLE UserRoles (
    Id INT PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

INSERT INTO UserRoles (Id, RoleName) VALUES 
(1, 'CEO'),
(2, 'Admin'),
(3, 'Employee');

CREATE TABLE ReactTypes (
    Id INT PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL
);

INSERT INTO ReactTypes (Id, TypeName) VALUES 
(1, 'Like'),
(2, 'Heart'),
(3, 'Smile');

CREATE TABLE NotificationTypes (
    Id INT PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL
);

INSERT INTO NotificationTypes (Id, TypeName) VALUES 
(1, 'Warning'),
(2, 'Information'),
(3, 'HappyBirthday'),
(4, 'System'),
(5, 'Calendar');

CREATE TABLE FileTypes (
    Id INT PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL
);

INSERT INTO FileTypes (Id, TypeName) VALUES 
(1, 'Image'),
(2, 'Video'),
(3, 'File');

CREATE TABLE PostTypes (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL
)

CREATE TABLE Reacts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReactType NVARCHAR(100) NOT NULL
);

CREATE TABLE SurveyTypes (
	Id INT IDENTITY(1, 1) PRIMARY KEY,
	Name NVARCHAR(50) NOT NULL
)

CREATE TABLE Tags (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	TagName NVARCHAR(100) NOT NULL,
)

CREATE TABLE Themes (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Theme NVARCHAR(50) NOT NULL,
)

CREATE TABLE Statuses (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	StatusName NVARCHAR(50) NOT NULL,
)

CREATE TABLE MessageTypes (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	MessageType NVARCHAR(50) NOT NULL,
)

-- Tables
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    UserName NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Role INT NOT NULL FOREIGN KEY REFERENCES UserRoles(Id)
);

CREATE TABLE UserDetails (
	Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
	Avatar NVARCHAR(255) NULL,
	Wallpaper NVARCHAR(255) NULL,
	DOB DATETIME NOT NULL DEFAULT GETDATE(),
	Bio NVARCHAR(100) NULL,
	UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id)
)

CREATE TABLE Posts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    DestinationId UNIQUEIDENTIFIER NULL,
    InGroup BIT NOT NULL DEFAULT 0,
    Content NVARCHAR(MAX),
    ReactCount INT NOT NULL DEFAULT 0,
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
	PostTypeId  INT NOT NULL FOREIGN KEY REFERENCES PostTypes(Id)
);

CREATE TABLE Comments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    Content NVARCHAR(MAX) NOT NULL,
    IsResponse BIT NOT NULL DEFAULT 0,
    ReactCount INT NOT NULL DEFAULT 0,
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
    PostId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Posts(Id)
);

CREATE TABLE Surveys (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    ExpiredAt DATETIME NOT NULL,
	Total INT NOT NULL DEFAULT 0,
	SurveyTypeId INT NOT NULL FOREIGN KEY REFERENCES SurveyTypes(Id),
    PostId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Posts(Id),
);

CREATE TABLE SurveyItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    OptionName NVARCHAR(MAX) NOT NULL,
	Total INT NOT NULL DEFAULT 0,
    SurveyId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Surveys(Id)
);

CREATE TABLE SharePosts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    SharedPostId UNIQUEIDENTIFIER NOT NULL, -- ID bài viết gốc
    SharedByUserId UNIQUEIDENTIFIER NOT NULL, -- Người chia sẻ bài viết
    InGroup BIT NOT NULL DEFAULT 0,
    DestinationId UNIQUEIDENTIFIER NULL,
    ReactCount INT NOT NULL DEFAULT 0,
    Content NVARCHAR(max), -- Nội dung cá nhân khi share
    FOREIGN KEY (SharedPostId) REFERENCES Posts(Id), -- Liên kết với bài viết gốc
    FOREIGN KEY (SharedByUserId) REFERENCES Users(Id) -- Liên kết với người chia sẻ
);

CREATE TABLE Notifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    NotiType INT NOT NULL FOREIGN KEY REFERENCES NotificationTypes(Id),
    DestinationId UNIQUEIDENTIFIER NULL,
	OrganizationName NVARCHAR(255) NULL, 
    StartedAt DATETIME NOT NULL DEFAULT GETDATE(),
    EndedAt DATETIME NULL,
);

CREATE TABLE Attachments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    Media NVARCHAR(255) NOT NULL,
    Description NVARCHAR(100) NULL
);

CREATE TABLE Groups (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    GroupName NVARCHAR(100) NOT NULL,
	Avatar NVARCHAR(255) NULL,
    Wallpaper NVARCHAR(255) NULL,
);

CREATE TABLE ChatGroups (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
	ChatName NVARCHAR(50) NOT NULL,
	ThemeId INT NOT NULL FOREIGN KEY REFERENCES Themes(Id),
    GroupType NVARCHAR(50) NOT NULL
);


CREATE TABLE Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    SenderId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
    ChatGroupId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES ChatGroups(Id),
    Content NVARCHAR(MAX) NOT NULL,
	StatusId INT NOT NULL FOREIGN KEY REFERENCES Statuses(Id),
	TypeId INT NOT NULL FOREIGN KEY REFERENCES MessageTypes(Id),
	ReactCount INT NOT NULL DEFAULT 0,
	IsReponse BIT NOT NULL DEFAULT 0,	
	IsPinned BIT NOT NULL DEFAULT 0,
);

CREATE TABLE OrganizeNodes (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(500),
    Level INT NOT NULL,
    ParentId UNIQUEIDENTIFIER NULL,
    EmployeeCount INT NOT NULL
);



--Junction Table
CREATE TABLE CommentResponse(
	CommentId UNIQUEIDENTIFIER NOT NULL,
    ResponseId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_Comment_Response PRIMARY KEY
    (
        CommentId,
        ResponseId
    ),
    FOREIGN KEY (CommentId) REFERENCES Comments (Id),
    FOREIGN KEY (ResponseId) REFERENCES Comments (Id)
)

CREATE TABLE MessageResponse(
	MessageId UNIQUEIDENTIFIER NOT NULL,
    ResponseId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_Message_Response PRIMARY KEY
    (
        MessageId,
        ResponseId
    ),
    FOREIGN KEY (MessageId) REFERENCES Messages (Id),
    FOREIGN KEY (ResponseId) REFERENCES Messages (Id)
)

CREATE TABLE UserVote(
	UserId UNIQUEIDENTIFIER NOT NULL,
    SurveyItemId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_User_SurveyItem PRIMARY KEY
    (
        UserId,
        SurveyItemId
    ),
    FOREIGN KEY (UserId) REFERENCES Users (Id),
    FOREIGN KEY (SurveyItemId) REFERENCES SurveyItems (Id)
)

CREATE TABLE UserGroup(
	UserId UNIQUEIDENTIFIER NOT NULL,
    GroupId UNIQUEIDENTIFIER NOT NULL,
	Role INT NOT NULL FOREIGN KEY REFERENCES UserRoles(Id),
	JoinedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0
    CONSTRAINT PK_User_Group PRIMARY KEY
    (
        UserId,
        GroupId
    ),
    FOREIGN KEY (UserId) REFERENCES Users (Id),
    FOREIGN KEY (GroupId) REFERENCES Groups (Id)
)

CREATE TABLE UserChatGroup(
	UserID UNIQUEIDENTIFIER NOT NULL,
    ChatGroupId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_User_ChatGroup PRIMARY KEY
    (
        UserId,
        ChatGroupId
    ),
    FOREIGN KEY (UserId) REFERENCES Users (Id),
    FOREIGN KEY (ChatGroupId) REFERENCES ChatGroups (Id)
)

CREATE TABLE PostTag(
	PostId UNIQUEIDENTIFIER NOT NULL,
    TagId INT NOT NULL,
    CONSTRAINT PK_Post_Tag PRIMARY KEY
    (
        PostId,
        TagId
    ),
    FOREIGN KEY (PostId) REFERENCES Posts (Id),
    FOREIGN KEY (TagId) REFERENCES Tags (Id)
)

CREATE TABLE NotificationTag(
	NotificationId UNIQUEIDENTIFIER NOT NULL,
    TagId INT NOT NULL,
    CONSTRAINT PK_Notification_Tag PRIMARY KEY
    (
        NotificationId,
        TagId
    ),
    FOREIGN KEY (NotificationId) REFERENCES Notifications (Id),
    FOREIGN KEY (TagId) REFERENCES Tags (Id)
)

CREATE TABLE PostAttachment(
	PostId UNIQUEIDENTIFIER NOT NULL,
    AttachmentId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_Post_Attachment PRIMARY KEY
    (
        PostId,
        AttachmentId
    ),
    FOREIGN KEY (PostId) REFERENCES Posts (Id),
    FOREIGN KEY (AttachmentId) REFERENCES Attachments (Id)
)

CREATE TABLE CommentAttachment(
	CommentId UNIQUEIDENTIFIER NOT NULL,
    AttachmentId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_Comment_Attachment PRIMARY KEY
    (
        CommentId,
        AttachmentId
    ),
    FOREIGN KEY (CommentId) REFERENCES Comments (Id),
    FOREIGN KEY (AttachmentId) REFERENCES Attachments (Id)
)

CREATE TABLE MessageAttachment(
	MessageId UNIQUEIDENTIFIER NOT NULL,
    AttachmentId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_Message_Attachment PRIMARY KEY
    (
        MessageId,
        AttachmentId
    ),
    FOREIGN KEY (MessageId) REFERENCES Messages (Id),
    FOREIGN KEY (AttachmentId) REFERENCES Attachments (Id)
)

CREATE TABLE UserReactPost(
	UserId UNIQUEIDENTIFIER NOT NULL,
    PostId UNIQUEIDENTIFIER NOT NULL,
	ReactTypeId INT NOT NULL,
	CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_User_React_Post PRIMARY KEY
    (
        UserId,
		PostId
    ),
    FOREIGN KEY (PostId) REFERENCES Posts (Id),
    FOREIGN KEY (UserId) REFERENCES Users (Id),
	FOREIGN KEY (ReactTypeId) REFERENCES ReactTypes (Id)
)

CREATE TABLE UserReactComment(
	UserId UNIQUEIDENTIFIER NOT NULL,
    CommentId UNIQUEIDENTIFIER NOT NULL,
	ReactTypeId INT NOT NULL,
	CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_User_React_Comment PRIMARY KEY
    (
        UserId,
		CommentId
    ),
    FOREIGN KEY (CommentId) REFERENCES Comments (Id),
    FOREIGN KEY (UserId) REFERENCES Users (Id),
	FOREIGN KEY (ReactTypeId) REFERENCES ReactTypes (Id)
)

CREATE TABLE UserReactMessage(
	UserId UNIQUEIDENTIFIER NOT NULL,
    MessageId UNIQUEIDENTIFIER NOT NULL,
	ReactTypeId INT NOT NULL,
	CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_User_React_Message PRIMARY KEY
    (
        UserId,
		MessageId
    ),
    FOREIGN KEY (MessageId) REFERENCES Messages (Id),
    FOREIGN KEY (UserId) REFERENCES Users (Id),
	FOREIGN KEY (ReactTypeId) REFERENCES ReactTypes (Id)
)

-- --Mock Data--
-- Mock data for PostTypes
INSERT INTO PostTypes (Name) VALUES 
('Text Post'),
('Image Post');

-- Mock data for Reacts
INSERT INTO Reacts (ReactType) VALUES 
('Like'),
('Heart');

-- Mock data for SurveyTypes
INSERT INTO SurveyTypes (Name) VALUES 
('Poll'),
('Quiz');

-- Mock data for Tags
INSERT INTO Tags (TagName) VALUES 
('Technology'),
('Lifestyle');

-- Mock data for Themes
INSERT INTO Themes (Theme) VALUES 
('Dark Theme'),
('Light Theme');

-- Mock data for Statuses
INSERT INTO Statuses (StatusName) VALUES 
('Sent'),
('Read');

-- Mock data for MessageTypes
INSERT INTO MessageTypes (MessageType) VALUES 
('Text'),
('Media');

-- Mock data for Users
INSERT INTO Users (UserName, Password, Email, Role) VALUES 
('john_doe', 'password123', 'john_doe@example.com', 2),
('jane_smith', 'password123', 'jane_smith@example.com', 3);

-- Mock data for UserDetails
INSERT INTO UserDetails (FirstName, LastName, Avatar, Wallpaper, DOB, Bio, UserId) VALUES 
('John', 'Doe', 'avatar1.jpg', 'wallpaper1.jpg', '1990-01-01', 'A software developer', (SELECT TOP 1 Id FROM Users WHERE UserName='john_doe')),
('Jane', 'Smith', 'avatar2.jpg', 'wallpaper2.jpg', '1995-05-15', 'A graphic designer', (SELECT TOP 1 Id FROM Users WHERE UserName='jane_smith'));

-- Mock data for Posts
INSERT INTO Posts (DestinationId, InGroup, Content, UserId, PostTypeId) VALUES 
(NULL, 0, 'Hello, this is my first post!', (SELECT TOP 1 Id FROM Users WHERE UserName='john_doe'), 1),
(NULL, 0, 'Sharing an amazing photo!', (SELECT TOP 1 Id FROM Users WHERE UserName='jane_smith'), 2);

-- Mock data for Comments
INSERT INTO Comments (Content, IsResponse, UserId, PostId) VALUES 
('Great post!', 0, (SELECT TOP 1 Id FROM Users WHERE UserName='jane_smith'), (SELECT TOP 1 Id FROM Posts WHERE Content LIKE 'Hello, this is my first post!')),
('Thank you!', 1, (SELECT TOP 1 Id FROM Users WHERE UserName='john_doe'), (SELECT TOP 1 Id FROM Posts WHERE Content LIKE 'Hello, this is my first post!'));

-- Mock data for Surveys
INSERT INTO Surveys (ExpiredAt, Total, SurveyTypeId, PostId) VALUES 
('2025-01-15', 0, 1, (SELECT TOP 1 Id FROM Posts WHERE Content LIKE 'Hello, this is my first post!')),
('2025-02-01', 0, 2, (SELECT TOP 1 Id FROM Posts WHERE Content LIKE 'Sharing an amazing photo!'));

-- Mock data for SurveyItems
INSERT INTO SurveyItems (OptionName, Total, SurveyId) VALUES 
('Option 1', 0, (SELECT TOP 1 Id FROM Surveys WHERE ExpiredAt = '2025-01-15')),
('Option 2', 0, (SELECT TOP 1 Id FROM Surveys WHERE ExpiredAt = '2025-01-15'));

-- Mock data for Groups
INSERT INTO Groups (GroupName, Avatar, Wallpaper) VALUES 
('Developers Group', 'dev_avatar.jpg', 'dev_wallpaper.jpg'),
('Designers Group', 'design_avatar.jpg', 'design_wallpaper.jpg');

-- Mock data for ChatGroups
INSERT INTO ChatGroups (ChatName, ThemeId, GroupType, Theme) VALUES 
('General Chat', 1, 'Public', 'Dark Theme'),
('Team Chat', 2, 'Private', 'Light Theme');

-- Mock data for Messages
INSERT INTO Messages (SenderId, ChatGroupId, Content, StatusId, TypeId, ReactCount, IsReponse, IsPinned) VALUES 
((SELECT TOP 1 Id FROM Users WHERE UserName='john_doe'), (SELECT TOP 1 Id FROM ChatGroups WHERE ChatName='General Chat'), 'Hello everyone!', 1, 1, 2, 0, 0),
((SELECT TOP 1 Id FROM Users WHERE UserName='jane_smith'), (SELECT TOP 1 Id FROM ChatGroups WHERE ChatName='General Chat'), 'Hi John!', 2, 1, 1, 1, 0);
