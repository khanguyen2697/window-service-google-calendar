#include "stdafx.h"
#include "TCPClient.h"
#include <ws2tcpip.h>


TCPClient::TCPClient(void)
{
	// Initialize Winsock
	WSADATA wsaData;
	WSAStartup(MAKEWORD(2, 2), &wsaData);
}


TCPClient::~TCPClient(void)
{
	Disconnect();
	WSACleanup();
}

bool TCPClient::Connect(const CString& host, int port)
{
	// Initialize Winsock
	WSADATA wsa_data;
	if (WSAStartup(MAKEWORD(2, 2), &wsa_data) != 0) {
		AfxMessageBox(_T("WSAStartup failed!"));
		closesocket(m_socket);
		WSACleanup();
		return false;
	}

	// Create a socket
	m_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (m_socket == INVALID_SOCKET) {
		AfxMessageBox(_T("Invalid socket!"));
		closesocket(m_socket);
		WSACleanup();
		return false;
	}

	// Set up the sockaddr_in structure
	sockaddr_in addr;
	addr.sin_family = AF_INET;
	addr.sin_port = htons(5000);
	if (InetPton(AF_INET, _T("127.0.0.1"), &addr.sin_addr.s_addr) <= 0) {
		AfxMessageBox(_T("Invalid address!"));
		closesocket(m_socket);
		WSACleanup();
		return false;
	}

		// Connect to the server
	if (connect(m_socket, reinterpret_cast<SOCKADDR *>(&addr), sizeof(addr)) != 0) {
		AfxMessageBox(_T("Connection to server failed!"));
		closesocket(m_socket);
		WSACleanup();
		return false;
	}

	// Send the initial message
	CStringA ansiMessage("MFC_CLIENT");
	send(m_socket, ansiMessage, static_cast<int>(ansiMessage.GetLength()), 0);

	m_connected = true;
	return true;
}

void TCPClient::Disconnect()
{
	if (m_connected) {
		closesocket(m_socket);
		m_connected = false;
	}
}

bool TCPClient::Send(const CString& message)
{
	if (!m_connected) {
		return false;
	}

	CStringA ansiMessage(message); // Convert to ANSI
	int bytesSent = send(m_socket, ansiMessage, static_cast<int>(ansiMessage.GetLength()), 0);
	return bytesSent != SOCKET_ERROR;
}

CString TCPClient::Receive()
{
	if (!m_connected) {
		return _T("");
	}

	char recvBuffer[1024];
	int bytesReceived = recv(m_socket, recvBuffer, sizeof(recvBuffer) - 1, 0);
	if (bytesReceived > 0) {
		recvBuffer[bytesReceived] = '\0'; // Null-terminate the string
		return CString(recvBuffer);
	}
	return _T("");
}

bool TCPClient::IsConnected() {
	return m_connected;
}

SOCKET TCPClient::GetSocket() {
	return m_socket;
}