CREATE TABLE Books(
Id int IDENTITY(1,1) PRIMARY KEY,
BookTitle varchar(255) not NULL,
AuthorFName varchar(255) NULL,
AuthorLName varchar(255) not NULL,
Genre varchar(255) not NULL,
Price decimal(5,2) not NULL,
Status varchar(255) not NULL,
CheckOutDate date NULL,
PatronId int FOREIGN KEY REFERENCES Patrons(Id) ON UPDATE CASCADE
);
CREATE TABLE Patrons (
Id int IDENTITY(1,1) PRIMARY KEY,
StreetAddress varchar(255) not NULL,
FirstName varchar(255) not NULL,
LastName varchar(255) not NULL,
Email varchar(255) not NULL,
City varchar(255) not NULL,
State varchar(255) not NULL,
Zip int not NULL,
PhoneNumber varchar(255) not NULL
);
CREATE TABLE Staff (
Id int IDENTITY(1,1) PRIMARY KEY,
FirstName varchar(255) not NULL,
LastName varchar(255) not NULL,
PhoneNumber varchar(255) NULL,
Position varchar(255) NULL
);
