#include "stdafx.h"
#include "TCPServerRequest.h"
#include <ctime>
#include <iomanip>
#include <sstream>


const std::string TCPServerRequest::DefaultRequester = "MFC_CLIENT";

TCPServerRequest::TCPServerRequest(const std::string& type, const std::string& eventId)
	: m_type(type), m_eventId(eventId) {
}

std::string TCPServerRequest::GetRequester() const {
	return DefaultRequester;
}

std::string TCPServerRequest::GetEventId() const {
	return m_eventId;
}

void TCPServerRequest::SetEventId(const std::string& eventId) {
	m_eventId = eventId;
}

std::string TCPServerRequest::GetType() const {
	return m_type;
}

void TCPServerRequest::SetType(const std::string& type) {
	m_type = type;
}

std::string TCPServerRequest::GetTimestamp() const {
	// Get the current time in UTC
    std::time_t now = std::time(nullptr);
    std::tm tm;
    gmtime_s(&tm, &now);

    std::ostringstream oss;
    // Format the timestamp as "yyyy-MM-ddTHH:mm:ss.fffZ"
    oss << std::put_time(&tm, "%Y-%m-%dT%H:%M:%S");
    // Append milliseconds
    oss << '.' << std::setw(3) << std::setfill('0') << (now % 1000) << 'Z';

	return oss.str();
}