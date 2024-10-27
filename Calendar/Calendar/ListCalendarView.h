#pragma once

#include "Event.h"

class ListCalendarView : public CView
{
private:
	enum ColumnIndex
	{
		COL_SUMMARY = 0,
		COL_START = 1,
		COL_END = 2,
		COL_ID = 3,
		COL_START_ID = 4,
		COL_END_ID = 5,
		COL_START_VAL =6,
		COL_END_VAL = 7
	};

protected:
	DECLARE_DYNCREATE(ListCalendarView)
	virtual void OnDraw(CDC* pDC);
	CListCtrl m_ListCtrl;
	afx_msg void OnRowClick(NMHDR* pNMHDR, LRESULT* pResult);

	DECLARE_MESSAGE_MAP()

public:
	ListCalendarView(void);
	~ListCalendarView(void);
	virtual void OnInitialUpdate();
	void LoadCalendarData();
	void GetEventById(Event& eventObj);
	void OnSize(UINT nType, int cx, int cy);
	void OnFileNew();
};

