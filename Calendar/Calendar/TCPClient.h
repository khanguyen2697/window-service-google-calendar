#pragma once
class TCPClient
{
public:
	TCPClient(void);
	~TCPClient(void);

	bool Connect(const CString& host, int port);
	void Disconnect();
	bool Send(const CString& message);
	CString Receive();
	bool IsConnected();
	SOCKET GetSocket();

private:
	SOCKET m_socket;
	bool m_connected;
};

