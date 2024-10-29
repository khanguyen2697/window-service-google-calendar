#include "stdafx.h"
#include "Event.h"


Event::Event(void)
{
}


Event::~Event(void)
{
}

bool Event::IsEqual(const Event& other) 
{
	bool result = id == other.id &&
		startId == other.startId &&
		endId == other.endId &&
		summary == other.summary &&
		start == other.start &&
		end == other.end;
	return result;
}


