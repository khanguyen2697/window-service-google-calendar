#pragma once
class Event
{
public:
	CString id;
	int startId;
	int endId;
	CString summary;
	CString start;
	CString end;

	Event(CString id) : id(id) {}
	Event(CString id, int startId, int endId, CString summary, CString start, CString end)
		: id(id), startId(startId), endId(endId), summary(summary), start(start), end(end) {}
	Event(CString summary, CString start, CString end) : summary(summary), start(start), end(end) {}
	Event(CString start, CString end) : start(start), end(end) {}
	Event(void);
	~Event(void);

	bool IsEqual(const Event& other);
};
