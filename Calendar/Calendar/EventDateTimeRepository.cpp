#include "stdafx.h"
#include "EventDateTimeRepository.h"
#include "CommonUtils.h"


EventDateTimeRepository::EventDateTimeRepository(void)
{
}

EventDateTimeRepository::~EventDateTimeRepository(void)
{
}

int EventDateTimeRepository::Insert(CDatabase& database, EventDateTime eventDateTime)
{
	int insertedId = -1;
	try
	{
		if (!database.IsOpen())
		{
			AfxMessageBox(_T("Database connection is not open!"));
			return -1;
		}

		CString sqlQuery;
		sqlQuery.Format(
			_T("INSERT INTO EventDateTime (DateTimeRaw) ")
			_T("VALUES ('%s')"),
			eventDateTime.dateTimeRaw);

		database.ExecuteSQL(sqlQuery);

		CRecordset recordset(&database);
		CString selectIdQuery = _T("SELECT TOP 1 Id as NewId from EventDateTime ORDER By Id DESC");
		recordset.Open(CRecordset::forwardOnly, selectIdQuery, CRecordset::readOnly);

		if (!recordset.IsEOF())
		{
			CString idStr;
			recordset.GetFieldValue(_T("NewId"), idStr);
			insertedId = _ttoi(idStr);
		}

		recordset.Close();
	}
	catch (CDBException* e)
	{
		CString errorMsg;
		errorMsg.Format(_T("Error inserting EventDateTime: %s"), e->m_strError);
		AfxMessageBox(errorMsg); 
		e->Delete();
	}

	return insertedId;
}

int EventDateTimeRepository::Update(CDatabase& database, EventDateTime eventDateTime)
{
	int updated = -1;
	try
	{
		if (!database.IsOpen())
		{
			AfxMessageBox(_T("Database connection is not open!"));
			return -1;
		}

		CString updateEventDateTimeQuery;
		updateEventDateTimeQuery.Format(
			_T("UPDATE ")
			_T("    EventDateTime ")
			_T("SET")
			_T("    Date = %s, ")
			_T("    DateTimeRaw = %s, ")
			_T("    TimeZone = %s, ")
			_T("    ETag = %s ")
			_T("WHERE ")
			_T("    Id = %d "),
			CommonUtils::FormatStringValueOrNull(eventDateTime.date),
			CommonUtils::FormatStringValueOrNull(eventDateTime.dateTimeRaw),
			CommonUtils::FormatStringValueOrNull(eventDateTime.timeZone),
			CommonUtils::FormatStringValueOrNull(eventDateTime.eTag),
			eventDateTime.id);

		database.ExecuteSQL(updateEventDateTimeQuery);
		updated = 1;
	}
	catch (CDBException* e)
	{
		CString errorMsg;
		errorMsg.Format(_T("Error inserting EventDateTime: %s"), e->m_strError);
		AfxMessageBox(errorMsg);
		e->Delete();
	}

	return updated;
}
