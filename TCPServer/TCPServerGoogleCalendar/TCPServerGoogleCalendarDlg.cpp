
// TCPServerGoogleCalendarDlg.cpp : implementation file
//

#include "stdafx.h"
#include "TCPServerGoogleCalendar.h"
#include "TCPServerGoogleCalendarDlg.h"
#include <string>
#include <ctime>
#include <CkJsonObject.h>


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CAboutDlg dialog used for App About

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

	// Dialog Data
	enum { IDD = IDD_ABOUTBOX };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	// Implementation
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CTCPServerGoogleCalendarDlg dialog

CTCPServerGoogleCalendarDlg::CTCPServerGoogleCalendarDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CTCPServerGoogleCalendarDlg::IDD, pParent), m_nextClientId(0), DEFAULT_HOST(_T("127.0.0.1")), DEFAULT_PORT("5000") 
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}
void CTCPServerGoogleCalendarDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT_ACTIVITIES_LOG, m_editActivitiesLogVal);
	DDX_Control(pDX, IDC_EDIT_HOST, m_editHostVal);
	DDX_Control(pDX, IDC_EDIT_PORT, m_editPortVal);
}

BEGIN_MESSAGE_MAP(CTCPServerGoogleCalendarDlg, CDialogEx)
	ON_BN_CLICKED(IDC_MFCBUTTON_START, &CTCPServerGoogleCalendarDlg::OnBnClickedMfcbuttonStart)
	ON_BN_CLICKED(IDC_MFCBUTTON_STOP, &CTCPServerGoogleCalendarDlg::OnBnClickedMfcbuttonStop)
END_MESSAGE_MAP()


// CTCPServerGoogleCalendarDlg message handlers

BOOL CTCPServerGoogleCalendarDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	GetDlgItem(IDC_MFCBUTTON_STOP)->EnableWindow(FALSE);
	m_editActivitiesLogVal.ModifyStyle(ES_AUTOHSCROLL, 0);
	m_editHostVal.SetWindowText(DEFAULT_HOST);
	m_editHostVal.EnableWindow(FALSE);
	m_editPortVal.SetWindowText(DEFAULT_PORT);

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// TODO: Add extra initialization here

	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CTCPServerGoogleCalendarDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CTCPServerGoogleCalendarDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CTCPServerGoogleCalendarDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



void CTCPServerGoogleCalendarDlg::OnBnClickedMfcbuttonStart()
{
	CString strHost;
	CString strPort;
	GetDlgItemText(IDC_EDIT_HOST, strHost);
	GetDlgItemText(IDC_EDIT_PORT, strPort);
	int port = _ttoi(strPort);

	GetDlgItem(IDC_MFCBUTTON_START)->EnableWindow(FALSE);
	GetDlgItem(IDC_MFCBUTTON_STOP)->EnableWindow(TRUE);

	StartTcpServer();
}

void CTCPServerGoogleCalendarDlg::OnBnClickedMfcbuttonStop()
{
	if (m_serverRunning) {
		try {
			m_serverRunning = false;
			closesocket(serverSocket);

			for (const auto& pair : m_mfcClients) {
				closesocket(pair.second.socket);
			}
			for (const auto& pair : m_serviceClients) {
				closesocket(pair.second.socket);
			}
			m_mfcClients.clear();
			m_serviceClients.clear();

			GetDlgItem(IDC_MFCBUTTON_START)->EnableWindow(TRUE);
			GetDlgItem(IDC_MFCBUTTON_STOP)->EnableWindow(FALSE);
			GetDlgItem(IDC_EDIT_PORT)->EnableWindow(TRUE);
		}
		catch (const std::exception& e) {
			CString logMessage;
			logMessage.Format(_T("Exception on StopServer: %S"), e.what());
			SetActivitiesLog(logMessage);
		}
		catch (...) {
			SetActivitiesLog(_T("An unknown error occurred while stopping the server."));
		}
	}
}


void CTCPServerGoogleCalendarDlg::SetActivitiesLog(CString message) {
	// Get current time
	std::time_t now = std::time(nullptr);
	std::tm localTime;
	localtime_s(&localTime, &now);

	// Format the current time as "dd/mm/yyyy HH:MM:SS"
	char timeBuffer[20];
	std::strftime(timeBuffer, sizeof(timeBuffer), "%d/%m/%Y %H:%M:%S", &localTime);

	// Create a formatted log entry
	// Convert char array to CString
	CString formattedLogEntry;
	formattedLogEntry.Format(_T("%s-> %s"), CString(timeBuffer), message);

	CString currentActivitiesLog;
	m_editActivitiesLogVal.GetWindowText(currentActivitiesLog);
	m_editActivitiesLogVal.SetWindowText(formattedLogEntry + _T("\r\n") + currentActivitiesLog);
}

void CTCPServerGoogleCalendarDlg::StartTcpServer()
{

	try {
		// Launch the TCP server in a separate thread
		std::thread serverThread(&CTCPServerGoogleCalendarDlg::HandleTcpServer, this);
		serverThread.detach();  // Detach the thread to let it run independently
	} catch (const std::exception& ex) {
		AfxMessageBox(CString(ex.what()));
	}
}

void CTCPServerGoogleCalendarDlg::HandleTcpServer()
{
	try {
		// Get port
		CString strPort;
		GetDlgItemText(IDC_EDIT_PORT, strPort);
		int port = _ttoi(strPort);

		WSADATA wsaData;
		WSAStartup(MAKEWORD(2, 2), &wsaData);

		SOCKET serverSocket = socket(AF_INET, SOCK_STREAM, 0);
		sockaddr_in serverAddr;
		serverAddr.sin_family = AF_INET;
		serverAddr.sin_port = htons(port);
		serverAddr.sin_addr.s_addr = INADDR_ANY;

		if (bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
			SetActivitiesLog(_T("Binding failed."));
			return;
		}

		listen(serverSocket, SOMAXCONN);

		// Set the server socket to non-blocking mode
		u_long mode = 1;
		ioctlsocket(serverSocket, FIONBIO, &mode); // Make the socket non-blocking

		GetDlgItem(IDC_EDIT_HOST)->EnableWindow(FALSE);
		GetDlgItem(IDC_EDIT_PORT)->EnableWindow(FALSE);

		CString logMessage;
		logMessage.Format(_T("Server is listening on port %d..."), ntohs(serverAddr.sin_port));
		SetActivitiesLog(logMessage);
		m_serverRunning = true;

		while (m_serverRunning) {
			SOCKET clientSocket = accept(serverSocket, nullptr, nullptr);

			if (clientSocket == INVALID_SOCKET) {
				int error = WSAGetLastError();
				if (error != WSAEWOULDBLOCK) {
					SetActivitiesLog(_T("Error accepting client connection."));
					break;
				}
			} else {
				std::thread clientThread([this, clientSocket]() {
					this->OnClientConnect(clientSocket);
				});
				clientThread.detach();
			}
		}

		closesocket(serverSocket);
		WSACleanup();
		SetActivitiesLog(_T("Server stopped."));
	}
	catch (const std::exception& ex) {
		AfxMessageBox(CString(ex.what()));
	}
}

void CTCPServerGoogleCalendarDlg::OnClientConnect(SOCKET clientSocket)
{
	// Lock the mutex for thread safety
	std::lock_guard<std::mutex> lock(m_mutex);

	// Set the socket to blocking mode
	u_long mode = 0; // 0 for blocking mode
	ioctlsocket(clientSocket, FIONBIO, &mode);

	int clientId = m_nextClientId++;

	char buffer[1024];
	bool isMFCClient = true;
	int bytesReceived = recv(clientSocket, buffer, sizeof(buffer) - 1, 0);
	if (bytesReceived > 0)
	{
		buffer[bytesReceived] = '\0';
		// Check if it's an MFC client
		isMFCClient = (strcmp(buffer, "WINDOW_SERVICE") != 0);
	}

	// Store the client socket and ID
	CString logMessage;
	ClientInfo clientInfo = { clientSocket, clientId, isMFCClient };
	if (isMFCClient) {
		m_mfcClients[clientId] = clientInfo;
		logMessage.Format(_T("MFC Client connected! ID: %d"), clientId);
		SetActivitiesLog(logMessage);
	} else {
		m_serviceClients[clientId] = clientInfo;
		logMessage.Format(_T("Windows Service Client connected! ID: %d"), clientId);
		SetActivitiesLog(logMessage);
	}

	// Handle client communication in a separate thread
	std::thread([this, clientSocket, clientId, isMFCClient]() {
		char buffer[1024];
		CString logMessage;
		while (true) {
			int bytesReceived = recv(clientSocket, buffer, sizeof(buffer) - 1, 0);
			if (bytesReceived > 0) {
				buffer[bytesReceived] = '\0';

				// Log received message
				CString receivedMessage(buffer);

				// Create a JSON object from the received message
				CkJsonObject jsonObjReceived;
				bool success = jsonObjReceived.Load(buffer);
				if (success) {
					const char* requester = jsonObjReceived.stringOf("Requester");
					SetActivitiesLog(CString(jsonObjReceived.emit()));
					if (isMFCClient)
					{
						// Sent request to window service
						for (const auto& pair : m_serviceClients) {
							if (pair.second.socket != INVALID_SOCKET) {
								send(pair.second.socket, buffer, strlen(buffer), 0);
							}
						}
					}
					else
					{
						// Sent request to MFC app
						for (const auto& pair : m_mfcClients) {
							if (pair.second.socket != INVALID_SOCKET) {
								send(pair.second.socket, buffer, strlen(buffer), 0);
							}
						}
					}
				}
				/*std::string response = "Message received: " + std::string(buffer);
				send(clientSocket, response.c_str(), response.size(), 0);*/
			}
			else {
				if(isMFCClient) {
					m_mfcClients.erase(clientId);
				}
				else {
					m_serviceClients.erase(clientId);
				}

				logMessage.Format(_T("Client disconnected! ID: %d"), clientId);
				SetActivitiesLog(logMessage);
				break;
			}
		}
		// Clean up after disconnect
		closesocket(clientSocket);
	}).detach();  // Detach client thread to avoid blocking
}
