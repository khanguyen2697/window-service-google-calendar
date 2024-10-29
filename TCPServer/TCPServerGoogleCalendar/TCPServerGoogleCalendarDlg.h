// TCPServerGoogleCalendarDlg.h : header file
//

#pragma once

#include <future>
#include <unordered_map>

struct ClientInfo {
	SOCKET socket;
	int id;
	bool isMFCClient;
};

// CTCPServerGoogleCalendarDlg dialog
class CTCPServerGoogleCalendarDlg : public CDialogEx
{
	// Construction
public:
	// Constructor
	CTCPServerGoogleCalendarDlg(CWnd* pParent = NULL);

	// Dialog Data
	enum { IDD = IDD_TCPSERVERGOOGLECALENDAR_DIALOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX); // DDX/DDV support

private:
	const CString DEFAULT_HOST;
	const CString DEFAULT_PORT;

	/*std::vector<ClientInfo> m_mfcClients;
	std::vector<ClientInfo> m_serviceClients;*/
	std::unordered_map<int, ClientInfo> m_mfcClients;
	std::unordered_map<int, ClientInfo> m_serviceClients;
	std::mutex m_mutex;
	int m_nextClientId;
	bool m_serverRunning;
	SOCKET serverSocket;

// Implementation
protected:
	HICON m_hIcon;

	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()

public:
	afx_msg void OnBnClickedMfcbuttonStart();
	afx_msg void OnBnClickedMfcbuttonStop();
	void SetActivitiesLog(CString message);
	void HandleTcpServer();
	void StartTcpServer();
	void StopTcpServer();
	void OnClientConnect(SOCKET client);
	CEdit m_editActivitiesLogVal;
	CEdit m_editHostVal;
	CEdit m_editPortVal;

};
