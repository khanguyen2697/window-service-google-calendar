#pragma once

#include <string>

class TCPServerRequest
{
private:
	std::string m_type;
	std::string m_eventId;
	static const std::string DefaultRequester;

public:
	TCPServerRequest(const std::string& type, const std::string& eventId);

	std::string GetRequester() const;
	std::string GetEventId() const;
	void SetEventId(const std::string& eventId);
	std::string GetType() const;
	void SetType(const std::string& type);
	std::string GetTimestamp() const;
};

