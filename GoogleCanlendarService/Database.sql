CREATE DATABASE GoogleCalendar;
GO

USE GoogleCalendar;

CREATE TABLE OrganizerData (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    DisplayName NVARCHAR(255),
    Email NVARCHAR(255) NOT NULL,
    IsOrganizer BIT DEFAULT 0
);

CREATE TABLE CreatorData (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    DisplayName NVARCHAR(255),
    Email NVARCHAR(255)NOT NULL,
    IsCreator BIT DEFAULT 0
);

CREATE TABLE RemindersData (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UseDefault BIT NULL
);

CREATE TABLE EventReminder (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RemindersDataId INT NOT NULL,
    Method NVARCHAR(255) NOT NULL,
    Minutes INT NULL,
    ETag NVARCHAR(255) NULL,
    CONSTRAINT FK_RemindersData_EventReminder FOREIGN KEY (RemindersDataId) REFERENCES RemindersData(Id)
);

CREATE TABLE Event (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    AnyoneCanAddSelf BIT NULL,
    AttendeesOmitted BIT NULL,
    CreatedRaw VARCHAR(255) NULL,
    ColorId VARCHAR(50) NULL,
    CreatedDateTimeOffset DATETIMEOFFSET NULL,
    CreatorId VARCHAR(255) NULL,
    EDescription TEXT NULL,
    EndDate DATETIME2 NOT NULL,
    EndTimeUnspecified BIT NULL,
    ETag VARCHAR(255) NULL,
    EventType VARCHAR(50) NULL,
    GuestsCanInviteOthers BIT NULL,
    GuestsCanModify BIT NULL,
    GuestsCanSeeOtherGuests BIT NULL,
    HangoutLink VARCHAR(255) NULL,
    HtmlLink VARCHAR(255) NULL,
    ICalUID VARCHAR(255) NULL,
    Location VARCHAR(255) NULL,
    Kind VARCHAR(50) NULL,
    Locked BIT NULL,
    OrganizerId VARCHAR(255) NULL,
	PrivateCopy BIT NULL,
    OriginalStartTime DATETIME2 NULL,
    RecurringEventId VARCHAR(50) NULL,
    ReminderId INT NULL,
    StartDate DATETIME2 NOT NULL,
    EStatus VARCHAR(50) NULL,
    Summary TEXT NULL,
	Transparency VARCHAR(50) NULL,
    UpdatedRaw VARCHAR(255) NULL,
	Visibility VARCHAR(50) NULL,
	CONSTRAINT FK_Event_RemindersData FOREIGN KEY (ReminderId) REFERENCES RemindersData(Id),
	CONSTRAINT FK_Event_CreatorData FOREIGN KEY (CreatorId) REFERENCES CreatorData(Id),
	CONSTRAINT FK_Event_OrganizerData FOREIGN KEY (OrganizerId) REFERENCES OrganizerData(Id)
);

CREATE TABLE EventAttendee (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    EventId VARCHAR(255) NOT NULL,
    DisplayName NVARCHAR(255),
    Email NVARCHAR(255) NULL,
    AdditionalGuests INT NULL DEFAULT 0,
    Comment NVARCHAR(MAX) NULL,
    Optional BIT NULL DEFAULT 0,
    Organizer BIT NULL DEFAULT 0,
    Resource BIT NULL DEFAULT 0,
    ResponseStatus NVARCHAR(50) NULL,
    RepresentsCalendar BIT NULL DEFAULT 0,
    ETag NVARCHAR(255) NULL,
	CONSTRAINT FK_EventAttendee_Event FOREIGN KEY (EventId) REFERENCES Event(Id),
);

CREATE TABLE EventAttachment (
    FileId VARCHAR(255) NOT NULL,
    EventId VARCHAR(255) NOT NULL,
    FileUrl NVARCHAR(MAX) NULL,
    IconLink NVARCHAR(MAX) NULL,
    MimeType NVARCHAR(100) NULL,
    Title NVARCHAR(255) NULL,
    ETag NVARCHAR(255) NULL,
    PRIMARY KEY (FileId, EventId),
	CONSTRAINT FK_EventAttachment_Event FOREIGN KEY (EventId) REFERENCES Event(Id),
);