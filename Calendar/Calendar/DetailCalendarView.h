#pragma once

#include "Event.h"
#include "TCPClient.h"
#include "TCPServerRequest.h"

class DetailCalendarView : public CView
{

private:
	int m_startId;
	int m_endId;
	Event m_originalEvent;
	TCPClient* m_pTCPClient;

public:
	CString m_eventId;

protected:
	CFont m_titleFont;
	CStatic m_titleLabel;
	CStatic m_summaryLabel;
	CStatic m_startLabel;
	CStatic m_endLabel;
	CStatic m_timezoneLabel;
	CEdit m_eventSummaryEdit;
	CDateTimeCtrl m_eventStartDateCtrl;
	CDateTimeCtrl m_eventEndDateCtrl;
	CComboBox m_eventStartTimeCombo;
	CComboBox m_eventEndTimeCombo;
	CComboBox m_eventTimeZoneCombo;
	CButton m_saveButton;
	CButton m_deleteButton;

protected:
	virtual void OnInitialUpdate();
	DECLARE_MESSAGE_MAP()
	DECLARE_DYNCREATE(DetailCalendarView)

public:
	DetailCalendarView(void);
	~DetailCalendarView(void);
	virtual void OnDraw(CDC* pDC);
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	void OnBnClickedButtonSave();
	void OnBnClickedButtonDelete();
	void LoadEventDetail(Event& eventObj, bool isReloadLatestData = false);
	void GetEventDetailFromInputs(Event& eventObj);
	void LoadEmptyEvent(bool isReloadLatestData = false, bool isConfirm = true);
	void LoadEventList();
	void OnFileNew();
	bool IsDataChanged();
	void SetTCPClient(TCPClient* client);
	void SendRequestToTCPServer(TCPServerRequest& request);

private:

	void UpdateEvent(Event& newEvent);
	void AddEvent(Event& newEvent);
	bool validateForm(Event& eventObj);
	CString FormatDateTimeFromInputs(CDateTimeCtrl& dateCtrl, CComboBox& timeCtrl);
};

