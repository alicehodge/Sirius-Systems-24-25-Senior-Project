-- Drop foreign keys
ALTER TABLE [ChecklistItem] DROP CONSTRAINT FK_ChecklistItem_Bird;

ALTER TABLE [ChecklistItem] DROP CONSTRAINT FK_ChecklistItem_Checklist;

ALTER TABLE [Checklist] DROP CONSTRAINT FK_Checklist_SDUser;

ALTER TABLE [Sighting] DROP CONSTRAINT FK_Sighting_Bird;

ALTER TABLE [Sighting] DROP CONSTRAINT FK_Sighting_SDUser;

ALTER TABLE [Milestone] DROP CONSTRAINT FK_Milestone_SDUser;

-- Drop tables
DROP TABLE [ChecklistItem];

DROP TABLE [Checklist];

DROP TABLE [Sighting];

DROP TABLE [SDUser];

DROP TABLE [Bird];

DROP TABLE [Milestone];