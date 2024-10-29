#include "stdafx.h"
#include "Messages.h"

// Information messages
const CString Messages::I0001 = _T("Add event successfully");
const CString Messages::I0002 = _T("Update successfully");
const CString Messages::I0003 = _T("Delete event successfully");

// Confirmation messages
const CString Messages::C0001 = _T("New data available. Reload now?");
const CString Messages::C0002 = _T("You have unsaved changes, Do you want to reload?");
const CString Messages::C0003 = _T("Are you sure you want to delete this event?");

// Error messages
const CString Messages::E0001 = _T("Summary is empty!");
const CString Messages::E0002 = _T("Active view is not ListCalendarView");
const CString Messages::E0003 = _T("Invalid date and time format!");
const CString Messages::E0004 = _T("The end time must be later than the start time!");
const CString Messages::E0005 = _T("Failed to connect to the database.");
const CString Messages::E0006 = _T("Exception on create event: ");
const CString Messages::E0007 = _T("Exception on update event: ");
const CString Messages::E0008 = _T("An unknown error occurred on create event.");
const CString Messages::E0009 = _T("An unknown error occurred on update event.");
const CString Messages::E0010 = _T("Something wrong !");
const CString Messages::E0011 = _T("Exception on load List Calendar: ");
const CString Messages::E0012 = _T("An unknown error occurred on load list event.");
const CString Messages::E0013 = _T("Active view is not DetailCalendarView");
const CString Messages::E0014 = _T("No splitter found");
const CString Messages::E0015 = _T("An unknown error occurred on delete event.");
const CString Messages::E0016 = _T("Failed to send message to TCP server.");