-- Drop foreign keys
ALTER TABLE [ChecklistItem] DROP CONSTRAINT FK_ChecklistItem_Bird;

ALTER TABLE [ChecklistItem] DROP CONSTRAINT FK_ChecklistItem_Checklist;

ALTER TABLE [Checklist] DROP CONSTRAINT FK_Checklist_SDUser;

ALTER TABLE [Sighting] DROP CONSTRAINT FK_Sighting_Bird;

ALTER TABLE [Sighting] DROP CONSTRAINT FK_Sighting_SDUser;

ALTER TABLE [Milestone] DROP CONSTRAINT FK_Milestone_SDUser;

-- Drop foreign keys for notifications
ALTER TABLE [Notification] DROP CONSTRAINT [FK_Notification_SDUser];
ALTER TABLE [Notification] DROP CONSTRAINT [CK_Notification_Type];

-- Drop foreign keys for moderation tables
ALTER TABLE [ModeratedContent] DROP CONSTRAINT [FK_ModeratedContent_Bird];
ALTER TABLE [ModeratedContent] DROP CONSTRAINT [FK_ModeratedContent_Submitter];
ALTER TABLE [ModeratedContent] DROP CONSTRAINT [FK_ModeratedContent_Moderator];
ALTER TABLE [ModeratedContent] DROP CONSTRAINT [CK_ModeratedContent_Status];

-- Drop the added columns
ALTER TABLE [ModeratedContent] DROP COLUMN [BirdId];
ALTER TABLE [ModeratedContent] DROP COLUMN [RangeDescription];
ALTER TABLE [ModeratedContent] DROP COLUMN [SubmissionNotes];

ALTER TABLE [UserSettings] DROP CONSTRAINT FK_UserSettings_SDUser;

-- Drop tables
DROP TABLE [ChecklistItem];

DROP TABLE [Checklist];

DROP TABLE [Sighting];

DROP TABLE [SDUser];

DROP TABLE [Bird];

DROP TABLE [Milestone];

DROP TABLE [UserSettings];

DROP TABLE [ModeratedContent];

DROP TABLE [Notification];