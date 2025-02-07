CREATE TABLE [Bird] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [CommonName] nvarchar(100),
  [ScientificName] nvarchar(100),
  [SpeciesCode] nvarchar(50)
);

CREATE TABLE [SDUser] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [AspNetIdentityID] nvarchar(450)
);

CREATE TABLE [Sighting] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [SDUserID] int,
  [BirdID] int,
  [Date] datetime2,
  [Latitude] decimal(8,6),
  [Longitude] decimal(9,6),
  [Notes] nvarchar(3000)
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
