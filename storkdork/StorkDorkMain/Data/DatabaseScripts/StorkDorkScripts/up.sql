CREATE TABLE [Bird]
(
    [ID] int PRIMARY KEY IDENTITY(1,1),
    [ScientificName] [nvarchar](100),
    [CommonName] [nvarchar](100),
    [SpeciesCode] [nvarchar](10),
    [Category] [nvarchar](10),
    [Order] [nvarchar](25) NULL,
    [FamilyCommonName] [nvarchar](50) NULL,
    [FamilyScientificName] [nvarchar](50) NULL,
    [ReportAs] [nvarchar](10) NULL,
    [Range] [nvarchar](1000) NULL
);

CREATE TABLE [BirdPhoto]
(
    [ID] INT PRIMARY KEY IDENTITY(1,1),
    [BirdID] INT NOT NULL,
    [PhotoData] VARBINARY(MAX) NOT NULL,
    [PhotoContentType] NVARCHAR(100) NOT NULL,
    [Caption] NVARCHAR(255) NULL,
    [DateAdded] DATETIME2 NOT NULL
);

CREATE TABLE [SDUser]
(
    [ID] int PRIMARY KEY IDENTITY(1, 1),
    [AspNetIdentityID] nvarchar(450),
    [FirstName] nvarchar(50),
    [LastName] nvarchar(50),
    [DisplayName] nvarchar(25),
    [ProfileImagePath] nvarchar(100)
);

CREATE TABLE [Sighting]
(
    [ID] int PRIMARY KEY IDENTITY(1, 1),
    [SDUserID] int,
    [BirdID] int,
    [Date] datetime2,
    [Latitude] decimal(8,6),
    [Longitude] decimal(9,6),
    [Notes] nvarchar(3000),
    [Country] nvarchar(100),
    [Subdivision] nvarchar(100),
    [PhotoData] VARBINARY(MAX) NULL,
    [PhotoContentType] NVARCHAR(MAX) NULL

);

CREATE TABLE [Checklist]
(
    [ID] int PRIMARY KEY IDENTITY(1, 1),
    [ChecklistName] nvarchar(100),
    [SDUserID] int
);

CREATE TABLE [ChecklistItem]
(
    [ID] int PRIMARY KEY IDENTITY(1, 1),
    [ChecklistID] int,
    [BirdID] int,
    [Sighted] bit
);

CREATE TABLE [Milestone]
(
    [ID] int PRIMARY KEY IDENTITY(1, 1),
    [SDUserID] int,
    [SightingsMade] int,
    [PhotosContributed] int
);

CREATE TABLE [ModeratedContent]
(
    [ID] int PRIMARY KEY IDENTITY(1,1),
    [ContentType] nvarchar(50) NOT NULL,
    [ContentId] int NOT NULL,
    [SubmitterId] int NOT NULL,
    [SubmittedDate] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [ModeratorId] int NULL,
    [ModeratedDate] datetime2 NULL,
    [ModeratorNotes] nvarchar(max) NULL,

    -- Fields for RangeSubmission
    [BirdId] int NULL,
    [RangeDescription] nvarchar(2000) NULL,
    [SubmissionNotes] nvarchar(500) NULL,

    -- Fields for BirdPhotoSubmission
    [PhotoData] VARBINARY(MAX) NULL,
    [PhotoContentType] nvarchar(100) NULL,
    [Caption] nvarchar(255) NULL
);

CREATE TABLE [UserSettings]
(
    [ID] int PRIMARY KEY IDENTITY(1, 1),
    [SDUserID] int,
    [AnonymousSightings] bit NOT NULL DEFAULT 0,
);

CREATE TABLE [Notification]
(
    [ID] int PRIMARY KEY IDENTITY(1,1),
    [UserId] int NOT NULL,
    [Message] nvarchar(500) NOT NULL,
    [Type] nvarchar(20) NOT NULL,
    [IsRead] bit NOT NULL DEFAULT 0,
    [CreatedAt] datetime2 NOT NULL,
    [RelatedUrl] nvarchar(200) NULL
);

ALTER TABLE [Sighting] ADD CONSTRAINT [FK_Sighting_SDUser] 
    FOREIGN KEY ([SDUserID]) REFERENCES [SDUser] ([ID]);

ALTER TABLE [Sighting] ADD CONSTRAINT [FK_Sighting_Bird] 
    FOREIGN KEY ([BirdID]) REFERENCES [Bird] ([ID]);

ALTER TABLE [Checklist] ADD CONSTRAINT [FK_Checklist_SDUser] 
    FOREIGN KEY ([SDUserID]) REFERENCES [SDUser] ([ID]);

ALTER TABLE [ChecklistItem] ADD CONSTRAINT [FK_ChecklistItem_Checklist] 
    FOREIGN KEY ([ChecklistID]) REFERENCES [Checklist] ([ID]);

ALTER TABLE [ChecklistItem] ADD CONSTRAINT [FK_ChecklistItem_Bird] 
    FOREIGN KEY ([BirdID]) REFERENCES [Bird] ([ID]);

ALTER TABLE [Milestone] ADD CONSTRAINT [FK_Milestone_SDUser]
    FOREIGN KEY ([SDUserID]) REFERENCES [SDUser] ([ID]);

ALTER TABLE [UserSettings] ADD CONSTRAINT [FK_UserSettings_SDUser]
    FOREIGN KEY ([SDUserID]) REFERENCES [SDUser] ([ID]);

ALTER TABLE [ModeratedContent] ADD CONSTRAINT [FK_ModeratedContent_Submitter]
    FOREIGN KEY ([SubmitterId]) REFERENCES [SDUser] ([ID]) ON DELETE NO ACTION;

ALTER TABLE [ModeratedContent] ADD CONSTRAINT [FK_ModeratedContent_Moderator]
    FOREIGN KEY ([ModeratorId]) REFERENCES [SDUser] ([ID]) ON DELETE NO ACTION;

ALTER TABLE [ModeratedContent] ADD CONSTRAINT [FK_ModeratedContent_Bird]
    FOREIGN KEY ([BirdId]) REFERENCES [Bird] ([ID]);

ALTER TABLE [ModeratedContent] ADD CONSTRAINT [CK_ModeratedContent_Status]
    CHECK ([Status] IN ('Pending', 'Approved', 'Rejected'));

ALTER TABLE [Notification] ADD CONSTRAINT [FK_Notification_SDUser]
    FOREIGN KEY ([UserId]) REFERENCES [SDUser] ([ID]) ON DELETE CASCADE;

ALTER TABLE [Notification] ADD CONSTRAINT [CK_Notification_Type]
    CHECK ([Type] IN ('Success', 'Warning', 'Info'));

ALTER TABLE [BirdPhoto] ADD CONSTRAINT [FK_BirdPhoto_Bird] 
    FOREIGN KEY ([BirdID]) REFERENCES [Bird] ([ID]);
