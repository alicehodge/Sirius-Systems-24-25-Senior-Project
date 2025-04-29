-- Run bird_seed.sql first to populate the Bird table

INSERT INTO [SDUser] ([AspNetIdentityID], [FirstName], [LastName], [DisplayName]) VALUES
('17c420c9-e12c-44fd-b3c1-208881e92fb9', 'Marjorie', 'Caldwell', 'Marjorie'),
('99e94994-3dce-4577-b4ee-0f7e2775eedc', 'Patricia', 'Rivers', 'Patricia'),
('1948774a-9e90-4f21-8aed-df02b7a57ee5', 'Emily', 'Carter', 'Emily')

INSERT INTO [Sighting] ([SDUserID], [BirdID], [Date], [Latitude], [Longitude], [Notes]) VALUES
(1, (SELECT ID FROM Bird WHERE CommonName = 'American Herring Gull'), '2024-12-01 08:30:00', 44.8481, -123.2324, 'Spotted near the university park in the morning'),
(1, (SELECT ID FROM Bird WHERE CommonName = 'Glaucous-winged Gull'), '2024-12-02 14:15:00', 44.8495, -123.2302, ''),
(1, (SELECT ID FROM Bird WHERE CommonName = 'gull sp.'), '2024-12-03 17:45:00', 44.8478, -123.2356, 'One was singing on a tree branch'),
(2, (SELECT ID FROM Bird WHERE CommonName = 'Northern Flicker'), '2024-12-04 09:20:00', 44.8502, -123.2380, 'Flew over a backyard near campus'),
(2, (SELECT ID FROM Bird WHERE CommonName = 'Northern Shrike'), '2024-12-05 12:05:00', 44.8520, -123.2345, 'Seen in leaf compost area near sport fields, disc course.'),
(2, (SELECT ID FROM Bird WHERE CommonName = 'California Scrub-Jay'), '2024-12-06 18:30:00', 44.8465, -123.2392, ''),
(1, (SELECT ID FROM Bird WHERE CommonName = 'Common Raven'), '2024-12-07 07:55:00', 44.8490, -123.2378, 'One was hopping on a sidewalk near the library'),
(2, (SELECT ID FROM Bird WHERE CommonName = 'House Finch'), '2024-12-08 15:40:00', 44.8512, -123.2299, 'Landed briefly on a car in the parking lot'),
(1, (SELECT ID FROM Bird WHERE CommonName = 'White-crowned Sparrow'), '2024-12-09 11:10:00', 44.8475, -123.2311, 'Singing in a bush near the science building'),
(2, (SELECT ID FROM Bird WHERE CommonName = 'Song Sparrow'), '2024-12-10 16:25:00', 44.8508, -123.2333, 'Perched on a garden fence near the quad'),
(2, (SELECT ID FROM Bird WHERE CommonName = 'Brewer''s Blackbird'), '2024-12-26 14:14:00', 44.8516, -123.2400, '');

INSERT INTO [Milestone] ([SDUserID], [SightingsMade], [PhotosContributed]) VALUES
(1, 136, 23),
(1, 12, 0);