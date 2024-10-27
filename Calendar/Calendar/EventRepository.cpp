#include "stdafx.h"
#include "EventRepository.h"
#include <afxdb.h>
#include "CommonUtils.h"
#include "EventDateTimeRepository.h"


EventRepository::EventRepository(void)
{
}

EventRepository::~EventRepository(void)
{
}

std::vector<Event> EventRepository::FetchAllEvents(CDatabase& database)
{
	CRecordset recordset(&database);
	std::vector<Event> events;

	CString query;
	query.Append(_T("SELECT"));
	query.Append(_T("	E.Id AS Id, "));
	query.Append(_T("	E.StartId AS StartId, "));
	query.Append(_T("	E.EndId AS EndId, "));
	query.Append(_T("	E.Summary AS Summary, "));
	query.Append(_T("	EDTStart.DateTimeRaw AS StartDate, "));
	query.Append(_T("	EDTEnd.DateTimeRaw AS EndDate "));
	query.Append(_T("FROM "));
	query.Append(_T("	Event E "));
	query.Append(_T("INNER JOIN "));
	query.Append(_T("	EventDateTime EDTStart "));
	query.Append(_T("ON "));
	query.Append(_T("	EDTStart.Id = E.StartId "));
	query.Append(_T("INNER JOIN "));
	query.Append(_T("	EventDateTime EDTEnd "));
	query.Append(_T("ON "));
	query.Append(_T("	EDTEnd.Id = E.EndId "));
	query.Append(_T("WHERE "));
	query.Append(_T("	TRY_CAST(EDTStart.DateTimeRaw AS DATETIMEOFFSET) > SYSDATETIMEOFFSET()"));
	query.Append(_T("	AND E.Deleted <> 1"));
	query.Append(_T("ORDER BY "));
	query.Append(_T("StartDate ASC "));

	try
	{
		recordset.Open(CRecordset::forwardOnly, query);

		while (!recordset.IsEOF())
		{
			CDBVariant varStartId, varEndId;
			CString id, name, description, time;

			recordset.GetFieldValue(_T("Id"), id);
			recordset.GetFieldValue(_T("StartId"), varStartId);
			recordset.GetFieldValue(_T("EndId"), varEndId);
			recordset.GetFieldValue(_T("Summary"), name);
			recordset.GetFieldValue(_T("StartDate"), description);
			recordset.GetFieldValue(_T("EndDate"), time);

			events.push_back(Event(id, varStartId.m_lVal, varEndId.m_lVal, name, description, time));

			recordset.MoveNext();
		}

		recordset.Close();
	}
	catch (CDBException* e)
	{
		AfxMessageBox(_T("Failed to fetch events: ") + e->m_strError);
		e->Delete();
	}

	return events;
}

int EventRepository::UpdateEvent(CDatabase& database, Event& updateEvent)
{
	CString updateEventQuery;
	updateEventQuery.Format(
		_T("UPDATE ")
		_T("	Event ")
		_T("SET ")
		_T("	Summary = '%s', ")
		_T("	UpdatedRaw = '%s' ")
		_T("WHERE ")
		_T(" id = '%s'"),
		updateEvent.summary,
		CommonUtils::GetCurrentTimeInISOFormat(),
		updateEvent.id
		);

	try {
		database.BeginTrans();
		database.ExecuteSQL(updateEventQuery);

		EventDateTimeRepository eventDateTimeRepository;
		EventDateTime start = EventDateTime(updateEvent.startId, updateEvent.start);
		EventDateTime end = EventDateTime(updateEvent.endId, updateEvent.end);

		eventDateTimeRepository.Update(database, start);
		eventDateTimeRepository.Update(database, end);

		database.CommitTrans();
		return 1;

	} catch (CDBException* e) {
		database.Rollback();
		AfxMessageBox(_T("Error updating event: ") + e->m_strError);
		e->Delete();
		return -1;
	}
}

CString EventRepository::AddEvent(CDatabase& database, Event& newEvent)
{

	EventDateTimeRepository eventDateTimeRepository;
	EventDateTime start = EventDateTime(newEvent.start);
	EventDateTime end = EventDateTime(newEvent.end);
	CString insertedId;
	try
	{
		database.BeginTrans();
		int newStartId = eventDateTimeRepository.Insert(database, start);
		int newEndId = eventDateTimeRepository.Insert(database, end);
		CString newEventId = CommonUtils::GenerateCustomUuid();

		CString sqlQuery;
		sqlQuery.Format(
			_T("INSERT INTO Event (Id, CalendarId, Summary, StartId, EndId, CreatedRaw, GoogleCreated) ")
			_T("VALUES ('%s', '%s', '%s', %d, %d, '%s', 0)"),
			newEventId,
			GetCalendarId(),
			newEvent.summary,
			newStartId,
			newEndId,
			CommonUtils::GetCurrentTimeInISOFormat()
			);
		database.ExecuteSQL(sqlQuery);

		//TODO update
		CRecordset recordset(&database);
		CString selectIdQuery = _T("SELECT SCOPE_IDENTITY() AS NewId");
		recordset.Open(CRecordset::forwardOnly, selectIdQuery, CRecordset::readOnly);

		if (!recordset.IsEOF())
		{
			recordset.GetFieldValue(_T("NewId"), insertedId);
		}

		recordset.Close();
		database.CommitTrans();
		return newEventId;
	}
	catch (std::exception& e)
	{
		database.Rollback();
		CString errorMsg = _T("Exception on create event: ");
		errorMsg += CA2T(e.what());
		AfxMessageBox(errorMsg);
	}

	return insertedId;
}

bool EventRepository::DeleteEvent(CDatabase& database,CString id)
{
	try
	{
		CString sqlQuery;
		sqlQuery.Format(
			_T("UPDATE Event SET Deleted = 1, UpdatedRaw = '%s' WHERE Id = '%s'"), CommonUtils::GetCurrentTimeInISOFormat(), id);
		database.ExecuteSQL(sqlQuery);
		return true;
	}
	catch (CDBException* e)
	{
		AfxMessageBox(e->m_strError);
		e->Delete();
		return false;
	}
}

CString EventRepository::GetCalendarId()
{
	return CString("anhkha0001@gmail.com");
}
