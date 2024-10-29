#include "stdafx.h"
#include "ListCalendarView.h"
#include "DetailCalendarView.h"
#include "Resource.h"
#include "Event.h"
#include "EventRepository.h"
#include "CommonUtils.h"
#include "Messages.h"


IMPLEMENT_DYNCREATE(ListCalendarView, CView)

	BEGIN_MESSAGE_MAP(ListCalendarView, CView)
		ON_WM_SIZE()
		ON_NOTIFY(NM_CLICK, IDC_LIST_CALENDAR, &ListCalendarView::OnRowClick)
		ON_COMMAND(ID_FILE_NEW, &ListCalendarView::OnFileNew)
	END_MESSAGE_MAP()

	ListCalendarView::ListCalendarView(void)
	{
	}


	ListCalendarView::~ListCalendarView(void)
	{
	}


	void ListCalendarView::OnDraw(CDC* pDC)
	{
		pDC->TextOut(10, 10, _T("Calendar list"));
	}

	void ListCalendarView::OnInitialUpdate()
	{
		CView::OnInitialUpdate();
		CRect clientRect;
		GetClientRect(&clientRect);
		m_ListCtrl.Create(WS_CHILD | WS_VISIBLE | LVS_REPORT, clientRect, this, IDC_LIST_CALENDAR);
		m_ListCtrl.InsertColumn(COL_SUMMARY, _T("Summary"), LVCFMT_LEFT, 300);
		m_ListCtrl.InsertColumn(COL_START, _T("Start"), LVCFMT_LEFT, 140);
		m_ListCtrl.InsertColumn(COL_END, _T("End"), LVCFMT_LEFT, 140);
		m_ListCtrl.InsertColumn(COL_ID, _T("EventID"), LVCFMT_LEFT, 0);
		m_ListCtrl.InsertColumn(COL_START_ID, _T("StartID"), LVCFMT_LEFT, 0);
		m_ListCtrl.InsertColumn(COL_END_ID, _T("EndID"), LVCFMT_LEFT, 0);
		m_ListCtrl.InsertColumn(COL_START_VAL, _T("StartVal"), LVCFMT_LEFT, 0);
		m_ListCtrl.InsertColumn(COL_END_VAL, _T("EndVal"), LVCFMT_LEFT, 0);

		// Set extended styles for full-row selection and grid lines
		m_ListCtrl.SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);

		LoadCalendarData();
	}

	void ListCalendarView::LoadCalendarData()
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
				std::vector<Event> events = eventRepository.FetchAllEvents(database);
				m_ListCtrl.DeleteAllItems();

				int index = 0;
				CString startIdStr, endIdStr;
				for (Event event : events)
				{
					startIdStr.Format(_T("%d"), event.startId);
					endIdStr.Format(_T("%d"), event.endId);
					m_ListCtrl.InsertItem(index, event.summary);
					m_ListCtrl.SetItemText(index, COL_START, CommonUtils::ConvertEventTime(event.start));
					m_ListCtrl.SetItemText(index, COL_END, CommonUtils::ConvertEventTime(event.end));
					m_ListCtrl.SetItemText(index, COL_ID, event.id);
					m_ListCtrl.SetItemText(index, COL_START_ID, startIdStr);
					m_ListCtrl.SetItemText(index, COL_END_ID, endIdStr);
					m_ListCtrl.SetItemText(index, COL_START_VAL, event.start);
					m_ListCtrl.SetItemText(index, COL_END_VAL, event.end);
					index++;
				}
			}
			else
			{
				AfxMessageBox(Messages::E0005);
			}
		}
		catch (std::exception& e)
		{
			CString errorMsg = Messages::E0011;
			errorMsg += CA2T(e.what());
			AfxMessageBox(errorMsg);
		}
		catch (...)
		{
			AfxMessageBox(Messages::E0012);
		}
		if (isConnected) {
			dbConnection.Disconnect(database);
		}
	}

	void ListCalendarView::GetEventById(Event& eventObj)
	{
		int nItemCount = m_ListCtrl.GetItemCount();

		for (int i = 0; i < nItemCount; ++i)
		{

			CString id = m_ListCtrl.GetItemText(i, COL_ID);
			if (id == eventObj.id)
			{
				CString startIdStr =  m_ListCtrl.GetItemText(i, COL_START_ID);
				CString endIdStr = m_ListCtrl.GetItemText(i, COL_END_ID);

				eventObj.summary = m_ListCtrl.GetItemText(i, COL_SUMMARY);
				eventObj.start = m_ListCtrl.GetItemText(i, COL_START_VAL);
				eventObj.end = m_ListCtrl.GetItemText(i, COL_END_VAL);
				eventObj.startId = _ttoi(startIdStr);
				eventObj.endId = _ttoi(endIdStr);
				break;
			}
		}
	}

	void ListCalendarView::OnSize(UINT nType, int cx, int cy)
	{
		CView::OnSize(nType, cx, cy);
		if (m_ListCtrl.GetSafeHwnd())
		{
			m_ListCtrl.MoveWindow(0, 0, cx, cy);
		}
	}

	void ListCalendarView::OnRowClick(NMHDR* pNMHDR, LRESULT* pResult)
	{
		LPNMLISTVIEW pNMListView = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);

		int nItem = pNMListView->iItem;

		if (nItem != -1) // Ensure that a valid item is clicked
		{
			// Use the splitter window to get the active view
			CSplitterWnd* pSplitter = static_cast<CSplitterWnd*>(GetParent());
			if (pSplitter != nullptr)
			{
				CWnd* pActiveView = pSplitter->GetPane(0, 1); // Get the right pane (DetailCalendarView)
				DetailCalendarView* pDetailView = dynamic_cast<DetailCalendarView*>(pActiveView);
				if (pDetailView != nullptr)
				{
					CString startIdStr = m_ListCtrl.GetItemText(nItem, COL_START_ID);
					CString endIdStr = m_ListCtrl.GetItemText(nItem, COL_END_ID);
					Event myEvent = Event(
						m_ListCtrl.GetItemText(nItem, COL_ID),
						_ttoi(startIdStr),
						_ttoi(endIdStr),
						m_ListCtrl.GetItemText(nItem, COL_SUMMARY),
						m_ListCtrl.GetItemText(nItem, COL_START_VAL),
						m_ListCtrl.GetItemText(nItem, COL_END_VAL)
						);

					// Load event detail
					pDetailView->LoadEventDetail(myEvent);
				}
				else
				{
					AfxMessageBox(Messages::E0013);
				}
			}
			else
			{
				AfxMessageBox(Messages::E0014);
			}
		}
	}

	void ListCalendarView::OnFileNew()
	{
		CFrameWnd* pFrame = GetParentFrame();
		if (pFrame != nullptr)
		{
			CSplitterWnd* pSplitter = dynamic_cast<CSplitterWnd*>(pFrame->GetDlgItem(AFX_IDW_PANE_FIRST));
			if (pSplitter != nullptr)
			{
				// Get the right pane (DetailCalendarView) and set focus
				CWnd* pRightPane = pSplitter->GetPane(0, 1);
				if (pRightPane != nullptr)
				{
					pRightPane->SetFocus();  // Set focus to the right pane (DetailCalendarView)

					// Trigger the OnFileNew command in the right pane
					pRightPane->SendMessage(WM_COMMAND, ID_FILE_NEW);
				}
			}
		}
	}
