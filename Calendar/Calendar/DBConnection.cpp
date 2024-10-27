#include "stdafx.h"
#include "DBConnection.h"


DBConnection::DBConnection(void)
{
}


DBConnection::~DBConnection(void)
{
}

BOOL DBConnection::Connect(CDatabase& database)
{
	CString connectionString = _T("Driver={SQL Server};Server=localhost\\SQLEXPRESS;Database=GoogleCalendar;Uid=khanta;Pwd=123456;");
	try
	{
		database.Open(NULL, FALSE, FALSE, connectionString, TRUE);
		return TRUE;
	}
	catch (CDBException* e)
	{
		// Handle connection failure
		AfxMessageBox(_T("Database connection failed: ") + e->m_strError);
		e->Delete();
		return FALSE;
	}
}

void DBConnection::Disconnect(CDatabase& database)
{
	if (database.IsOpen())
	{
		database.Close();
	}
}