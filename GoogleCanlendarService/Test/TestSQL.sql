SELECT
	E.Summary,
	E.Location,
	EDTStart.DateTimeRaw AS StartTime,
	EDTEnd.DateTimeRaw AS EndTime,
	StartId,
	E.EDescription,
	EndId,
	E.Id,
	E.ColorId,
	E.Deleted
FROM
	Event E
INNER JOIN
	EventDateTime EDTStart
ON
	E.StartId = EDTStart.Id
INNER JOIN
	EventDateTime EDTEnd
ON
	E.EndId = EDTEnd.Id;

SELECT * FROM EventAttendee;