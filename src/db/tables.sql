CREATE TABLE IF NOT EXISTS Actors
(
    id int AUTO_INCREMENT PRIMARY KEY,
    firstname NVARCHAR(64) NOT NULL,
    lastname NVARCHAR(64) NOT NULL,
    bio NVARCHAR(4096),
    rating float
)

CREATE TABLE IF NOT EXISTS Movies
(
    id int AUTO_INCREMENT PRIMARY KEY,
    title NVARCHAR(256) NOT NULL,
    year int NOT NULL,
    description NVARCHAR(4096),
    rating float
)