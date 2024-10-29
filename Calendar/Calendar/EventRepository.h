#pragma once

#include <vector>
#include "Event.h"
#include "DBConnection.h"

class EventRepository
{
public:
	EventRepository(void);
	~EventRepository(void);
	std::vector<Event> FetchAllEvents(CDatabase& database);
	int UpdateEvent(CDatabase& database, Event& eventObj);
	CString AddEvent(CDatabase& database, Event& eventObj);
	bool DeleteEvent(CDatabase& database, CString id);

private:
	CString GetCalendarId();
};

