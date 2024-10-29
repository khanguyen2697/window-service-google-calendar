
// CalendarView.h : interface of the CCalendarView class
//

#pragma once


class CCalendarView : public CView
{
protected: // create from serialization only
	CCalendarView();
	DECLARE_DYNCREATE(CCalendarView)

// Attributes
public:
	CCalendarDoc* GetDocument() const;

// Operations
public:

// Overrides
public:
	virtual void OnDraw(CDC* pDC);  // overridden to draw this view
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:

// Implementation
public:
	virtual ~CCalendarView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	afx_msg void OnFilePrintPreview();
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // debug version in CalendarView.cpp
inline CCalendarDoc* CCalendarView::GetDocument() const
   { return reinterpret_cast<CCalendarDoc*>(m_pDocument); }
#endif

