#include "stdafx.h"
#include "DetailCalendarView.h"
#include "ListCalendarView.h"
#include "Event.h"
#include "resource.h"
#include "EventRepository.h"
#include "Messages.h"
#include "TCPServerRequest.h"
#include "CommonUtils.h"
#include <CkJsonObject.h>
#include "TCPServerRequestType.h"

BEGIN_MESSAGE_MAP(DetailCalendarView, CView)
	ON_WM_CTLCOLOR()
	ON_BN_CLICKED(IDC_EVENT_DETAIL_BUTTON_SAVE, &DetailCalendarView::OnBnClickedButtonSave)
	ON_BN_CLICKED(IDC_EVENT_DETAIL_BUTTON_DELETE, &DetailCalendarView::OnBnClickedButtonDelete)
	ON_COMMAND(ID_FILE_NEW, &DetailCalendarView::OnFileNew)
END_MESSAGE_MAP()

const CString TITLE_NEW = _T("Create new event");
const CString TITLE_UPDATE = _T("Update event");

DetailCalendarView::DetailCalendarView(void)
{
}


DetailCalendarView::~DetailCalendarView(void)
{
}

IMPLEMENT_DYNCREATE(DetailCalendarView, CView)

	void DetailCalendarView::OnDraw(CDC* pDC)
{
	// TODO: Add your drawing code here
	//pDC->TextOut(10, 10, _T("This is the right view"));
}

void DetailCalendarView::OnInitialUpdate()
{
	CView::OnInitialUpdate();

	m_titleFont.CreateFont(
		30, 0, 0, 0, FW_BOLD, FALSE, FALSE, 0,
		ANSI_CHARSET, OUT_DEFAULT_PRECIS, CLIP_DEFAULT_PRECIS, DEFAULT_QUALITY,
		DEFAULT_PITCH | FF_SWISS, _T("Arial"));

	// Labels
	m_titleLabel.Create(TITLE_NEW, WS_CHILD | WS_VISIBLE, 
		CRect(10, 10, 300, 40), this, IDC_EVENT_DETAIL_TITLE_LABEL);
	m_titleLabel.SetFont(&m_titleFont);
	m_summaryLabel.Create(_T("Summary:"), WS_CHILD | WS_VISIBLE, 
		CRect(10, 80, 100, 110), this, IDC_EVENT_DETAIL_SUMMARY_LABEL);
	m_startLabel.Create(_T("Start:"), WS_CHILD | WS_VISIBLE, 
		CRect(10, 130, 150, 160), this, IDC_EVENT_DETAIL_START_LABEL); 
	m_endLabel.Create(_T("End:"), WS_CHILD | WS_VISIBLE, 
		CRect(10, 180, 100, 210), this, IDC_EVENT_DETAIL_END_LABEL);
	m_timezoneLabel.Create(_T("Timezone:"), WS_CHILD | WS_VISIBLE, 
		CRect(10, 230, 100, 260), this, IDC_EVENT_DETAIL_TIMEZONE_LABEL);

	// Inputs
	m_eventSummaryEdit.Create(WS_CHILD | WS_VISIBLE | WS_BORDER | ES_AUTOHSCROLL, 
		CRect(110, 80, 500, 104), this, IDC_EVENT_DETAIL_EDIT_SUMMARY);
	m_eventStartDateCtrl.Create(WS_CHILD | WS_VISIBLE | DTS_SHORTDATEFORMAT, 
		CRect(110, 130, 230, 154), this, IDC_EVENT_DETAIL_DATEPICKER_START);
	m_eventStartTimeCombo.Create(WS_CHILD | WS_VISIBLE | CBS_DROPDOWN | WS_VSCROLL, 
		CRect(250, 130, 365, 160), this, IDC_EVENT_DETAIL_EDIT_START_TIME);
	m_eventEndDateCtrl.Create(WS_CHILD | WS_VISIBLE | DTS_SHORTDATEFORMAT,
		CRect(110, 180, 230, 204), this, IDC_EVENT_DETAIL_DATEPICKER_END);
	m_eventEndTimeCombo.Create(WS_CHILD | WS_VISIBLE | CBS_DROPDOWN | WS_VSCROLL,
		CRect(250, 180, 365, 210), this, IDC_EVENT_DETAIL_EDIT_END_TIME);
	m_eventTimeZoneCombo.Create(WS_CHILD | WS_VISIBLE | CBS_DROPDOWN | WS_VSCROLL, 
		CRect(110, 230, 230, 260), this, 8);

	// Buttons
	m_saveButton.Create(_T("Save"), WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON, 
		CRect(290, 280, 380, 320), this, IDC_EVENT_DETAIL_BUTTON_SAVE);
	m_deleteButton.Create(_T("Delete"), WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
		CRect(400, 280, 500, 320), this, IDC_EVENT_DETAIL_BUTTON_DELETE);

	// Load data
	CommonUtils::LoadTimeZonesIntoCComboBox(m_eventTimeZoneCombo);
	CommonUtils::LoadTimesIntoCComboBox(m_eventStartTimeCombo);
	CommonUtils::LoadTimesIntoCComboBox(m_eventEndTimeCombo);
	LoadEmptyEvent(false, false);
}

void DetailCalendarView::LoadEventDetail(Event& eventObj, bool isReloadLatestData)
{
	if (IsDataChanged())
	{
		CString confirmMessage = isReloadLatestData ? Messages::C0001 : Messages::C0002;
		int result = AfxMessageBox(confirmMessage, 
			MB_YESNO | MB_ICONQUESTION);

		if (result == IDNO) return;
	}

	m_titleLabel.SetWindowText(TITLE_UPDATE);
	m_eventId = eventObj.id;
	m_startId = eventObj.startId;
	m_endId = eventObj.endId;

	CString startDate = CommonUtils::GetDate(eventObj.start);
	CString endDate = CommonUtils::GetDate(eventObj.end);
	CString startTime = CommonUtils::GetTime(eventObj.start);
	CString endTime = CommonUtils::GetTime(eventObj.end);
	CString timeZone = CommonUtils::GetTimeZone(eventObj.start);

	m_eventSummaryEdit.SetWindowText(eventObj.summary);
	CommonUtils::SetDateForCDateTimeCtrl(m_eventStartDateCtrl, startDate);
	CommonUtils::SetDateForCDateTimeCtrl(m_eventEndDateCtrl, endDate);
	CommonUtils::SetValueForCComboBox(m_eventStartTimeCombo, startTime.Mid(0, 5), false);
	CommonUtils::SetValueForCComboBox(m_eventEndTimeCombo, endTime.Mid(0, 5), false);
	CommonUtils::SetValueForCComboBox(m_eventTimeZoneCombo, timeZone, true, false);
	// Set original data
	GetEventDetailFromInputs(m_originalEvent);
}

void DetailCalendarView::LoadEmptyEvent(bool isReloadLatestData, bool isConfirm)
{
	if (isConfirm && IsDataChanged())
	{
		CString confirmMessage = 
			isReloadLatestData ? Messages::C0001 : Messages::C0002;
		int result = AfxMessageBox(confirmMessage, 
			MB_YESNO | MB_ICONQUESTION);

		if (result == IDNO) return;
	}
	m_titleLabel.SetWindowText(TITLE_NEW);
	CString currentDateTime = CommonUtils::GetCurrentTimeInISOFormat(CommonUtils::ISOFormatType::LocalTime);
	CString currentDate = CommonUtils::GetDate(currentDateTime);
	CString currentTime = CommonUtils::GetTime(currentDateTime);
	CString timeZone = CommonUtils::GetTimeZone(currentDateTime);

	m_eventId.Empty();
	m_startId = -1;
	m_endId = -1;
	m_eventSummaryEdit.SetWindowText(_T(""));
	CommonUtils::SetDateForCDateTimeCtrl(m_eventStartDateCtrl, currentDate);
	CommonUtils::SetDateForCDateTimeCtrl(m_eventEndDateCtrl, currentDate);
	CommonUtils::SetValueForCComboBox(m_eventStartTimeCombo, currentTime.Mid(0, 5), false);
	CommonUtils::SetValueForCComboBox(m_eventEndTimeCombo, currentTime.Mid(0, 5), false);
	CommonUtils::SetValueForCComboBox(m_eventTimeZoneCombo, timeZone, true, false);
	// Set original data
	GetEventDetailFromInputs(m_originalEvent);
}

void DetailCalendarView::GetEventDetailFromInputs(Event& eventObj)
{
	CString summary, start, end;
	m_eventSummaryEdit.GetWindowText(summary);
	start = FormatDateTimeFromInputs(m_eventStartDateCtrl, m_eventStartTimeCombo);
	end = FormatDateTimeFromInputs(m_eventEndDateCtrl, m_eventEndTimeCombo);
	eventObj.id = m_eventId;
	eventObj.startId = m_startId;
	eventObj.endId = m_endId;
	eventObj.summary = summary;
	eventObj.start = start;
	eventObj.end = end;
}

void DetailCalendarView::LoadEventList()
{
	// Get the active view
	CSplitterWnd* pSplitter = static_cast<CSplitterWnd*>(GetParent());
	if (pSplitter != nullptr)
	{
		CWnd* pActiveView = pSplitter->GetPane(0, 0); // Get the left pane (ListCalendarView)
		ListCalendarView* pListView = dynamic_cast<ListCalendarView*>(pActiveView);
		if (pListView != nullptr)
		{
			pListView->LoadCalendarData();
		}
		else
		{
			AfxMessageBox(Messages::E0002);
		}
	}
}

bool DetailCalendarView::validateForm(Event& eventObj)
{
	if(eventObj.summary.IsEmpty())
	{
		AfxMessageBox(Messages::E0001);
		return false;
	}

	// Trim to "YYYY-MM-DDTHH:MM:SS"
	int timeZoneIndex = 19;
	CString startTimeTrimmed = eventObj.start.Left(timeZoneIndex);
	CString endTimeTrimmed = eventObj.end.Left(timeZoneIndex);

	startTimeTrimmed.Replace(_T("T"), _T(" "));
	endTimeTrimmed.Replace(_T("T"), _T(" "));

	COleDateTime startTime, endTime;
	if (!startTime.ParseDateTime(startTimeTrimmed) || !endTime.ParseDateTime(endTimeTrimmed))
	{
		AfxMessageBox(Messages::E0003);
		return false;
	}

	if (startTime > endTime)
	{
		AfxMessageBox(Messages::E0004);
		return false;
	}

	return true;
}

void DetailCalendarView::OnBnClickedButtonSave()
{
	Event eventObj;
	GetEventDetailFromInputs(eventObj);

	if (!validateForm(eventObj)) return;

	if (m_eventId.IsEmpty())
	{
		AddEvent(eventObj);
	}
	else
	{
		UpdateEvent(eventObj);
	}
}

void DetailCalendarView::OnBnClickedButtonDelete()
{
	int result = AfxMessageBox(Messages::C0003, 
		MB_YESNO | MB_ICONQUESTION);

	if (result == IDYES)
	{
		CDatabase database;
		DBConnection dbConnection;
		EventRepository eventRepository;
		BOOL isConnected = false;

		try
		{
			isConnected = dbConnection.Connect(database);
			if (isConnected)
			{
				bool deleted = eventRepository.DeleteEvent(database, m_eventId);
				if (deleted)
				{
					AfxMessageBox(Messages::I0003, MB_ICONQUESTION);
					// Reload after delete
					LoadEventList();
					OnFileNew();

					// Send request to TCP server
					std::string eventId = CStringA(m_eventId);
					TCPServerRequest request(TCPServerRequestType::REMOVE, eventId);
					SendRequestToTCPServer(request);
				}
			}
			else
			{
				AfxMessageBox(Messages::E0005);
			}
		}
		catch (std::exception& e)
		{
			CString errorMsg = Messages::E0007;
			errorMsg += CA2T(e.what());
			AfxMessageBox(errorMsg);
		}
		catch (...)
		{
			AfxMessageBox(Messages::E0015);
		}
		if(isConnected)
		{
			dbConnection.Disconnect(database);
		}
	}
}

void DetailCalendarView::AddEvent(Event& newEvent)
{
	CDatabase database;
	DBConnection dbConnection;
	EventRepository eventRepository;
	BOOL isConnected = false;
	try
	{
		isConnected = dbConnection.Connect(database);
		if (isConnected)
		{
			CString insertedId = eventRepository.AddEvent(database, newEvent);

			if (!insertedId.IsEmpty())
			{
				AfxMessageBox(Messages::I0001, MB_ICONQUESTION);
				LoadEventList();
				LoadEmptyEvent(false, false);

				// Send request to TCP server
				std::string eventId = CStringA(insertedId);
				TCPServerRequest request(TCPServerRequestType::CREATE, eventId);
				SendRequestToTCPServer(request);
			}
			else 
			{
				AfxMessageBox(Messages::E0010);
			}
		}
		else
		{
			AfxMessageBox(Messages::E0005);
		}
	}
	catch (std::exception& e)
	{
		CString errorMsg = Messages::E0006;
		errorMsg += CA2T(e.what());
		AfxMessageBox(errorMsg);
	}
	catch (...)
	{
		AfxMessageBox(Messages::E0008);
	}
	if (isConnected)
	{
		dbConnection.Disconnect(database);
	}
}

void DetailCalendarView::UpdateEvent(Event& updateEvent)
{
	CDatabase database;
	DBConnection dbConnection;
	EventRepository eventRepository;
	BOOL isConnected = false;
	try
	{
		isConnected = dbConnection.Connect(database);
		if (isConnected)
		{
			int result = eventRepository.UpdateEvent(database, updateEvent);

			if (result > 0)
			{
				AfxMessageBox(Messages::I0002, MB_ICONQUESTION);
				LoadEventList();
				// Update latest data for original event after update
				GetEventDetailFromInputs(m_originalEvent);

				// Send request to TCP server
				std::string eventId = CStringA(updateEvent.id);
				TCPServerRequest request(TCPServerRequestType::UPDATE, eventId);
				SendRequestToTCPServer(request);
			}
		}
		else
		{
			AfxMessageBox(Messages::E0005);
		}

	}
	catch (std::exception& e)
	{
		CString errorMsg = Messages::E0007;
		errorMsg += CA2T(e.what());
		AfxMessageBox(errorMsg);
	}
	catch (...)
	{
		AfxMessageBox(Messages::E0009);
	}
	if (isConnected)
	{
		dbConnection.Disconnect(database);
	}
}

bool DetailCalendarView::IsDataChanged()
{
	CString summary, start, end;
	m_eventSummaryEdit.GetWindowText(summary);
	start = FormatDateTimeFromInputs(m_eventStartDateCtrl, m_eventStartTimeCombo);
	end = FormatDateTimeFromInputs(m_eventEndDateCtrl, m_eventEndTimeCombo);
	Event currentEvent = Event(
		m_eventId,
		m_startId,
		m_endId,
		summary,
		start,
		end
		);
	return !currentEvent.IsEqual(m_originalEvent);
}

void DetailCalendarView::OnFileNew()
{
	LoadEmptyEvent();
}

/**
* Format datetime as YYYY-MM-DDTHH:MM:SS�HH:MM from inputs
*/
CString DetailCalendarView::FormatDateTimeFromInputs(CDateTimeCtrl& dateCtrl, CComboBox& timeCombo)
{
	COleDateTime date;
	CString time, timeZone;

	dateCtrl.GetTime(date);
	timeCombo.GetWindowText(time);
	m_eventTimeZoneCombo.GetWindowText(timeZone);

	// Format the date as "YYYY-MM-DD"
	CString formattedDate;
	formattedDate.Format(_T("%04d-%02d-%02d"), date.GetYear(), date.GetMonth(), date.GetDay());
	// Format the time as "HH:MM:00"
	CString formattedTime = time + _T(":00");

	CString formattedDateTime;
	formattedDateTime.Format(_T("%sT%s%s"), formattedDate, formattedTime, timeZone.Mid(0, 6));

	return formattedDateTime;
}

/**
* Set backgroup for static label
*/
HBRUSH DetailCalendarView::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hbr = CView::OnCtlColor(pDC, pWnd, nCtlColor);

	// Check if the control is the static label
	if (nCtlColor == CTLCOLOR_STATIC)
	{
		// Set the background mode to opaque
		pDC->SetBkMode(OPAQUE);

		// Set the background color
		pDC->SetBkColor(RGB(255, 255, 255));

		// Create a brush with the desired background color
		static HBRUSH hbrBackground = CreateSolidBrush(RGB(255, 255, 255));

		// Return the brush to paint the background
		return hbrBackground;
	}
	return hbr;
}

void DetailCalendarView::SetTCPClient(TCPClient* client) {
	m_pTCPClient = client;
}

void DetailCalendarView::SendRequestToTCPServer(TCPServerRequest& request)
{
	if (m_pTCPClient && m_pTCPClient->IsConnected()) {

		CkJsonObject jsonObjSend;
		jsonObjSend.AppendString("Requester", request.GetRequester().c_str());
		jsonObjSend.AppendString("EventId", request.GetEventId().c_str());
		jsonObjSend.AppendString("Type", request.GetType().c_str());
		jsonObjSend.AppendString("Timestamp", request.GetTimestamp().c_str());

		const char* jsonString = jsonObjSend.emit();
		CString message(jsonString);

		if (!m_pTCPClient->Send(message)) {
			AfxMessageBox(Messages::E0016);
		}
	}
}
