#pragma once
class EventDateTime
{
public:
	int id;
	CString date;
	CString dateTimeRaw;
	CString timeZone;
	CString eTag;

	EventDateTime(void);
	~EventDateTime(void);

	EventDateTime(CString dateTimeRaw) : dateTimeRaw(dateTimeRaw) {}
	EventDateTime(int id, CString dateTimeRaw) : id(id), dateTimeRaw(dateTimeRaw) {}
	EventDateTime(int id, CString dateTimeRaw, CString timeZone) : id(id), dateTimeRaw(dateTimeRaw), timeZone(timeZone) {}
	EventDateTime(CString date, CString dateTimeRaw, CString timeZone, CString eTag) : date(date), dateTimeRaw(dateTimeRaw), timeZone(timeZone), eTag(eTag) {}
};

