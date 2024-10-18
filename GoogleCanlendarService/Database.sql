CREATE DATABASE GoogleCalendar;
GO

USE GoogleCalendar;

CREATE TABLE OrganizerData (
    Id NVARCHAR(100) NOT NULL PRIMARY KEY,
    DisplayName NVARCHAR(255),
    Email NVARCHAR(255) NOT NULL,
    IsOrganizer BIT DEFAULT 0
);

CREATE TABLE CreatorData (
    Id NVARCHAR(100) NOT NULL PRIMARY KEY,
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

CREATE TABLE EventDateTime (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Date NVARCHAR(255),
    DateTimeRaw NVARCHAR(255),
    TimeZone NVARCHAR(100),
    ETag NVARCHAR(255)
);

CREATE TABLE Event (
    Id NVARCHAR(100) NOT NULL PRIMARY KEY,
    CalendarId NVARCHAR(100) NOT NULL,
    ColorId NVARCHAR(50) NULL,
    CreatorId NVARCHAR(100) NULL,
    OrganizerId NVARCHAR(100) NULL,
    RecurringEventId NVARCHAR(100) NULL,
    ReminderId INT NULL,
    StartId INT NOT NULL,
    EndId INT NOT NULL,
    OriginalStartTimeId INT NULL,
    Sequence INT NULL,
    AnyoneCanAddSelf BIT NULL,
    AttendeesOmitted BIT NULL,
    CreatedRaw NVARCHAR(255) NULL,
    EDescription TEXT NULL,
    EndTimeUnspecified BIT NULL,
    ETag NVARCHAR(255) NULL,
    EventType NVARCHAR(50) NULL,
    GuestsCanInviteOthers BIT NULL,
    GuestsCanModify BIT NULL,
    GuestsCanSeeOtherGuests BIT NULL,
    HangoutLink NVARCHAR(255) NULL,
    HtmlLink NVARCHAR(255) NULL,
    ICalUID NVARCHAR(255) NULL,
    Location NVARCHAR(255) NULL,
    Kind NVARCHAR(50) NULL,
    Locked BIT NULL,
    PrivateCopy BIT NULL,
    EStatus NVARCHAR(50) NULL,
    Summary TEXT NULL,
    Transparency NVARCHAR(50) NULL,
    UpdatedRaw NVARCHAR(255) NULL,
    Visibility NVARCHAR(50) NULL,
    GoogleCreated BIT DEFAULT 0,
    Deleted BIT DEFAULT 0,
    CONSTRAINT FK_Event_RemindersData FOREIGN KEY (ReminderId) REFERENCES RemindersData(Id),
    CONSTRAINT FK_Event_CreatorData FOREIGN KEY (CreatorId) REFERENCES CreatorData(Id),
    CONSTRAINT FK_Event_OrganizerData FOREIGN KEY (OrganizerId) REFERENCES OrganizerData(Id),
    CONSTRAINT FK_Event_Start FOREIGN KEY (StartId) REFERENCES EventDateTime(Id),
    CONSTRAINT FK_Event_End FOREIGN KEY (EndId) REFERENCES EventDateTime(Id),
    CONSTRAINT FK_Event_Original FOREIGN KEY (OriginalStartTimeId) REFERENCES EventDateTime(Id),
);

CREATE TABLE EventAttendee (
    Id NVARCHAR(100) NULL,
    EventId NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    DisplayName NVARCHAR(255),
    AdditionalGuests INT NULL DEFAULT 0,
    Comment NVARCHAR(MAX) NULL,
    Optional BIT NULL DEFAULT 0,
    Organizer BIT NULL DEFAULT 0,
    Resource BIT NULL DEFAULT 0,
    ResponseStatus NVARCHAR(50) NULL,
    RepresentsCalendar BIT NULL DEFAULT 0,
    ETag NVARCHAR(255) NULL,
    PRIMARY KEY (EventId, Email),
    CONSTRAINT FK_EventAttendee_Event FOREIGN KEY (EventId) REFERENCES Event(Id),
);

CREATE TABLE Recurrence (
    EventId NVARCHAR(100) NOT NULL,
    Content NVARCHAR(255) NULL,
    CONSTRAINT FK_Recurrence_Event FOREIGN KEY (EventId) REFERENCES Event(Id),
);

CREATE TABLE EventAttachment (
    FileId NVARCHAR(100) NOT NULL,
    EventId NVARCHAR(100) NOT NULL,
    FileUrl NVARCHAR(MAX) NULL,
    IconLink NVARCHAR(MAX) NULL,
    MimeType NVARCHAR(100) NULL,
    Title NVARCHAR(255) NULL,
    ETag NVARCHAR(255) NULL,
    PRIMARY KEY (FileId, EventId),
    CONSTRAINT FK_EventAttachment_Event FOREIGN KEY (EventId) REFERENCES Event(Id),
);

CREATE TYPE UT_EventCheck AS TABLE
(
    Id NVARCHAR(100),
    UpdatedRaw NVARCHAR(255)
);