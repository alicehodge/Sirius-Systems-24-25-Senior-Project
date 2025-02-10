INSERT INTO [Bird] ([SpeciesCode], [CommonName], [ScientificName]) VALUES
('hoocro1', 'Hooded Crow', 'Corvus cornix'),
('bohwax', 'Bohemian Waxwing', 'Bombycilla garrulus'),
('eutspa', 'Eurasian Tree Sparrow', 'Passer montanus'),
('houspa', 'House Sparrow', 'Passer domesticus'),
('gretit1', 'Great Tit', 'Parus major'),
('eurmag1', 'Eurasian Magpie', 'Pica pica'),
('carcro1', 'Carrion Crow', 'Corvus corone'),
('azutit2', 'Azure Tit', 'Cyanistes cyanus'),
('eurbla', 'Eurasian Blackbird', 'Turdus merula'),
('eurgol', 'European Goldfinch', 'Carduelis carduelis');

INSERT INTO [SDUser] ([AspNetIdentityID]) VALUES
('test-user-1-identity-id'),
('test-user-2-identity-id');

INSERT INTO [Sighting] ([SDUserID], [BirdID], [Date], [Latitude], [Longitude], [Notes]) VALUES
(1, 1, '2024-02-01 08:30:00', 51.5074, -0.1278, 'Spotted near the park in the morning'),
(1, 3, '2024-02-02 14:15:00', 48.8566, 2.3522, 'Saw a group near a fountain'),
(1, 5, '2024-02-03 17:45:00', 40.7128, -74.0060, 'One was singing on a tree branch'),
(2, 2, '2024-02-04 09:20:00', 34.0522, -118.2437, 'Flew over a backyard'),
(2, 4, '2024-02-05 12:05:00', 52.5200, 13.4050, 'Two birds seen on a rooftop'),
(2, 6, '2024-02-06 18:30:00', 35.6895, 139.6917, 'Observed near a temple'),
(1, 7, '2024-02-07 07:55:00', 55.7558, 37.6173, 'One was hopping on a sidewalk'),
(2, 8, '2024-02-08 15:40:00', 37.7749, -122.4194, 'Landed briefly on a car'),
(1, 9, '2024-02-09 11:10:00', 45.4642, 9.1900, 'Singing in a bush'),
(2, 10, '2024-02-10 16:25:00', 41.9028, 12.4964, 'Perched on a garden fence');