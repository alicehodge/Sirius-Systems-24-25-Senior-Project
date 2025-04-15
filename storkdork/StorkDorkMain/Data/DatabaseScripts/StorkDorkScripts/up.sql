CREATE TABLE [Bird](
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

CREATE TABLE [SDUser] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [AspNetIdentityID] nvarchar(450),
  [FirstName] nvarchar(50),
  [LastName] nvarchar(50)
);

CREATE TABLE [Sighting] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [SDUserID] int,
  [BirdID] int,
  [Date] datetime2,
  [Latitude] decimal(8,6),
  [Longitude] decimal(9,6),
  [Notes] nvarchar(3000),
  [Country] nvarchar(100),
  [Subdivision] nvarshar(100)
);

CREATE TABLE [Checklist] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [ChecklistName] nvarchar(100),
  [SDUserID] int
);

CREATE TABLE [ChecklistItem] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [ChecklistID] int,
  [BirdID] int,
  [Sighted] bit
);

CREATE TABLE [Milestone] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [SDUserID] int,
  [SightingsMade] int,
  [PhotosContributed] int
)

CREATE TABLE [UserSettings] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [SDUserID] int,
  [AnonymousSightings] bit NOT NULL DEFAULT 0,
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
    FOREIGN KEU ([SDUserID]) REFERENCES [SDUser] ([ID]);
