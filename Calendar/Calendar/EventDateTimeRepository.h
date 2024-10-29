#pragma once

#include <afxdb.h>
#include "EventDateTime.h"

class EventDateTimeRepository
{
public:
	EventDateTimeRepository(void);
	~EventDateTimeRepository(void);
	int Insert(CDatabase& database, EventDateTime eventDateTime);
	int Update(CDatabase& database, EventDateTime eventDateTime);
};

