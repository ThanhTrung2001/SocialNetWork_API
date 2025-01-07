USE SocialNetworkDB
;

CREATE TABLE [post_types] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [name] nvarchar(100) NOT NULL
)
;

CREATE TABLE [tags] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [tag_name] nvarchar(100) NOT NULL
)
;

CREATE TABLE [users] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [username] nvarchar(100) NOT NULL,
  [password] nvarchar(100) NOT NULL,
  [email] nvarchar(100) NOT NULL,
  [role] nvarchar(255) NOT NULL CHECK ([role] IN ('Admin', 'Employee'))
)
;

CREATE TABLE [user_details] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [firstname] nvarchar(100) NOT NULL,
  [lastname] nvarchar(100) NOT NULL,
  [phone_number] nvarchar(20) NOT NULL,
  [address] nvarchar(255) NOT NULL,
  [city] nvarchar(100) NOT NULL,
  [country] nvarchar(100) NOT NULL,
  [avatar] nvarchar(255),
  [wallpaper] nvarchar(255),
  [dob] datetime NOT NULL DEFAULT (getdate()),
  [bio] nvarchar(100),
  [user_id] uniqueidentifier NOT NULL
)
;

CREATE TABLE [posts] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [destination_id] uniqueidentifier,
  [in_group] bit NOT NULL DEFAULT (0),
  [content] nvarchar(max),
  [react_count] int NOT NULL DEFAULT (0),
  [user_id] uniqueidentifier NOT NULL,
  [post_type_id] int NOT NULL
)
;

CREATE TABLE [comments] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [content] nvarchar(max) NOT NULL,
  [is_response] bit NOT NULL DEFAULT (0),
  [react_count] int NOT NULL DEFAULT (0),
  [user_id] uniqueidentifier NOT NULL,
  [post_id] uniqueidentifier NOT NULL,
  [is_sharepost] bit NOT NULL DEFAULT (0)
)
;

CREATE TABLE [surveys] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [expired_at] datetime NOT NULL,
  [total_vote] int NOT NULL DEFAULT (0),
  [survey_type_id] int NOT NULL,
  [post_id] uniqueidentifier NOT NULL
)
;

CREATE TABLE [survey_items] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [option_name] nvarchar(max) NOT NULL,
  [total_vote] int NOT NULL DEFAULT (0),
  [survey_id] uniqueidentifier NOT NULL
)
;

CREATE TABLE [share_posts] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [shared_post_id] uniqueidentifier NOT NULL,
  [shared_by_user_id] uniqueidentifier NOT NULL,
  [in_group] bit NOT NULL DEFAULT (0),
  [destination_id] uniqueidentifier,
  [content] nvarchar(max),
  [react_count] int NOT NULL DEFAULT (0)
)
;

CREATE TABLE [attachments] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [media] nvarchar(255) NOT NULL,
  [description] nvarchar(100)
)
;

CREATE TABLE [groups] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [name] nvarchar(100) NOT NULL,
  [avatar] nvarchar(255),
  [wallpaper] nvarchar(255)
)
;

CREATE TABLE [pages] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [name] nvarchar(255) NOT NULL,
  [avatar] nvarchar(255),
  [wallpaper] nvarchar(255)
)
;

CREATE TABLE [notifications] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [user_id] uniqueidentifier NOT NULL,
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [title] nvarchar(255) NOT NULL,
  [description] nvarchar(max) NOT NULL,
  [noti_type] int NOT NULL,
  [destination_id] uniqueidentifier,
  [organization_name] nvarchar(255),
  [startedat] datetime NOT NULL DEFAULT (getdate()),
  [endedat] datetime DEFAULT (getdate())
)
;

CREATE TABLE [chat_groups] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [name] nvarchar(50) NOT NULL,
  [theme] nvarchar(255) NOT NULL CHECK ([theme] IN ('NormalTheme', 'ColorTheme')),
  [group_type] nvarchar(50) NOT NULL
)
;

CREATE TABLE [messages] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT (newid()),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [sender_id] uniqueidentifier NOT NULL,
  [chatgroup_id] uniqueidentifier NOT NULL,
  [content] nvarchar(max) NOT NULL,
  [status] nvarchar(255) NOT NULL CHECK ([status] IN ('Send', 'Read')),
  [type] nvarchar(255) NOT NULL CHECK ([type] IN ('NormalType', 'MediaType')),
  [react_count] int NOT NULL DEFAULT (0),
  [is_reponse] bit NOT NULL DEFAULT (0),
  [is_pinned] bit NOT NULL DEFAULT (0)
)
;

CREATE TABLE [organizations] (
  [id] uniqueidentifier PRIMARY KEY,
  [name] nvarchar(255) NOT NULL,
  [description] nvarchar(500),
  [department] nvarchar(255) NOT NULL CHECK ([department] IN ('Director', 'Accounting', 'HR', 'IT', 'Business', 'Ticket')),
  [email] nvarchar(100) NOT NULL,
  [phone_number] nvarchar(20) NOT NULL,
  [address] nvarchar(255) NOT NULL,
  [city] nvarchar(100) NOT NULL,
  [country] nvarchar(100) NOT NULL,
  [level] int NOT NULL,
  [parentid] uniqueidentifier,
  [employee_count] int NOT NULL DEFAULT (0)
)
;

CREATE TABLE [comment_response] (
  [comment_id] uniqueidentifier NOT NULL,
  [response_id] uniqueidentifier NOT NULL,
  PRIMARY KEY ([comment_id], [response_id])
)
;

CREATE TABLE [message_response] (
  [message_id] uniqueidentifier NOT NULL,
  [response_id] uniqueidentifier NOT NULL,
  PRIMARY KEY ([message_id], [response_id])
)
;

CREATE TABLE [user_surveyitem_vote] (
  [user_id] uniqueidentifier NOT NULL,
  [surveyitem_id] uniqueidentifier NOT NULL,
  PRIMARY KEY ([user_id], [surveyitem_id])
)
;

CREATE TABLE [user_group] (
  [user_id] uniqueidentifier NOT NULL,
  [group_id] uniqueidentifier NOT NULL,
  [role] nvarchar(255) NOT NULL CHECK ([role] IN ('Owner', 'Admin', 'Mod', 'Member')),
  [is_follow] bit NOT NULL DEFAULT (0),
  [joined_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  PRIMARY KEY ([user_id], [group_id])
)
;

CREATE TABLE [user_page] (
  [user_id] uniqueidentifier NOT NULL,
  [page_id] uniqueidentifier NOT NULL,
  [role] nvarchar(255) NOT NULL CHECK ([role] IN ('Owner', 'Admin', 'Mod', 'Follower')),
  [is_follow] bit NOT NULL DEFAULT (0),
  [joined_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  PRIMARY KEY ([user_id], [page_id])
)
;

CREATE TABLE [user_chatgroup] (
  [user_id] uniqueidentifier NOT NULL,
  [chatgroup_id] uniqueidentifier NOT NULL,
  [is_not_notification] bit NOT NULL DEFAULT (0),
  [delay_until] datetime NOT NULL DEFAULT (getdate()),
  [role] nvarchar(255) NOT NULL CHECK ([role] IN ('Owner', 'Admin', 'Mod', 'Member')),
  PRIMARY KEY ([user_id], [chatgroup_id])
)
;

CREATE TABLE [post_tag] (
  [post_id] uniqueidentifier NOT NULL,
  [tag_id] int NOT NULL,
  PRIMARY KEY ([post_id], [tag_id])
)
;

CREATE TABLE [notification_tag] (
  [notification_id] uniqueidentifier NOT NULL,
  [tag_id] int NOT NULL,
  PRIMARY KEY ([notification_id], [tag_id])
)
;

CREATE TABLE [post_attachment] (
  [post_id] uniqueidentifier NOT NULL,
  [attachmentid] uniqueidentifier NOT NULL,
  PRIMARY KEY ([post_id], [attachmentid])
)
;

CREATE TABLE [comment_attachment] (
  [comment_id] uniqueidentifier NOT NULL,
  [attachment_id] uniqueidentifier NOT NULL,
  PRIMARY KEY ([comment_id], [attachment_id])
)
;

CREATE TABLE [message_attachment] (
  [message_id] uniqueidentifier NOT NULL,
  [attachment_id] uniqueidentifier NOT NULL,
  PRIMARY KEY ([message_id], [attachment_id])
)
;

CREATE TABLE [user_react_post] (
  [user_id] uniqueidentifier NOT NULL,
  [post_id] uniqueidentifier NOT NULL,
  [is_sharepost] bit NOT NULL DEFAULT (0),
  [react_type] nvarchar(255) NOT NULL CHECK ([react_type] IN ('Like', 'Heart', 'Smile', 'Angry', 'Sad')),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  PRIMARY KEY ([user_id], [post_id])
)
;

CREATE TABLE [user_react_comment] (
  [user_id] uniqueidentifier NOT NULL,
  [commentid] uniqueidentifier NOT NULL,
  [react_type] nvarchar(255) NOT NULL CHECK ([react_type] IN ('Like', 'Heart', 'Smile', 'Angry', 'Sad')),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  PRIMARY KEY ([user_id], [commentid])
)
;

CREATE TABLE [user_react_message] (
  [user_id] uniqueidentifier NOT NULL,
  [message_id] uniqueidentifier NOT NULL,
  [react_type] nvarchar(255) NOT NULL CHECK ([react_type] IN ('Like', 'Heart', 'Smile', 'Angry', 'Sad')),
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  PRIMARY KEY ([user_id], [message_id])
)
;

CREATE TABLE [user_organization] (
  [user_id] uniqueidentifier NOT NULL,
  [node_id] uniqueidentifier NOT NULL,
  [created_at] datetime NOT NULL DEFAULT (getdate()),
  [updated_at] datetime NOT NULL DEFAULT (getdate()),
  [is_deleted] bit NOT NULL DEFAULT (0),
  [organization_role] nvarchar(255) NOT NULL CHECK ([organization_role] IN ('CEO', 'COO', 'VP', 'Manager', 'Lead', 'Employee', 'Probation', 'Intern')),
  PRIMARY KEY ([user_id], [node_id])
);


ALTER TABLE [user_details] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [posts] ADD FOREIGN KEY ([post_type_id]) REFERENCES [post_types] ([id])
GO

ALTER TABLE [comments] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [surveys] ADD FOREIGN KEY ([post_id]) REFERENCES [posts] ([id])
GO

ALTER TABLE [survey_items] ADD FOREIGN KEY ([survey_id]) REFERENCES [surveys] ([id])
GO

ALTER TABLE [share_posts] ADD FOREIGN KEY ([shared_post_id]) REFERENCES [posts] ([id])
GO

ALTER TABLE [share_posts] ADD FOREIGN KEY ([shared_by_user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [notifications] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [messages] ADD FOREIGN KEY ([sender_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [messages] ADD FOREIGN KEY ([chatgroup_id]) REFERENCES [chat_groups] ([id])
GO

ALTER TABLE [comment_response] ADD FOREIGN KEY ([comment_id]) REFERENCES [comments] ([id])
GO

ALTER TABLE [comment_response] ADD FOREIGN KEY ([response_id]) REFERENCES [comments] ([id])
GO

ALTER TABLE [message_response] ADD FOREIGN KEY ([message_id]) REFERENCES [messages] ([id])
GO

ALTER TABLE [message_response] ADD FOREIGN KEY ([response_id]) REFERENCES [messages] ([id])
GO

ALTER TABLE [user_surveyitem_vote] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_surveyitem_vote] ADD FOREIGN KEY ([surveyitem_id]) REFERENCES [survey_items] ([id])
GO

ALTER TABLE [user_group] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_group] ADD FOREIGN KEY ([group_id]) REFERENCES [groups] ([id])
GO

ALTER TABLE [user_page] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_page] ADD FOREIGN KEY ([page_id]) REFERENCES [pages] ([id])
GO

ALTER TABLE [user_chatgroup] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_chatgroup] ADD FOREIGN KEY ([chatgroup_id]) REFERENCES [chat_groups] ([id])
GO

ALTER TABLE [post_tag] ADD FOREIGN KEY ([post_id]) REFERENCES [posts] ([id])
GO

ALTER TABLE [post_tag] ADD FOREIGN KEY ([tag_id]) REFERENCES [tags] ([id])
GO

ALTER TABLE [notification_tag] ADD FOREIGN KEY ([notification_id]) REFERENCES [notifications] ([id])
GO

ALTER TABLE [notification_tag] ADD FOREIGN KEY ([tag_id]) REFERENCES [tags] ([id])
GO

ALTER TABLE [post_attachment] ADD FOREIGN KEY ([post_id]) REFERENCES [posts] ([id])
GO

ALTER TABLE [post_attachment] ADD FOREIGN KEY ([attachmentid]) REFERENCES [attachments] ([id])
GO

ALTER TABLE [comment_attachment] ADD FOREIGN KEY ([comment_id]) REFERENCES [comments] ([id])
GO

ALTER TABLE [comment_attachment] ADD FOREIGN KEY ([attachment_id]) REFERENCES [attachments] ([id])
GO

ALTER TABLE [message_attachment] ADD FOREIGN KEY ([message_id]) REFERENCES [messages] ([id])
GO

ALTER TABLE [message_attachment] ADD FOREIGN KEY ([attachment_id]) REFERENCES [attachments] ([id])
GO

ALTER TABLE [user_react_post] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_react_comment] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_react_comment] ADD FOREIGN KEY ([commentid]) REFERENCES [comments] ([id])
GO

ALTER TABLE [user_react_message] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_react_message] ADD FOREIGN KEY ([message_id]) REFERENCES [messages] ([id])
GO

ALTER TABLE [user_organization] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_organization] ADD FOREIGN KEY ([node_id]) REFERENCES [organizations] ([id])
GO

