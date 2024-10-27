#include "stdafx.h"
#include "CommonUtils.h"
#include <chrono>
#include <ctime>
#include <iomanip>
#include <sstream>
#include <random>

/**
* Convert event time from YYYY-MM-DDTHH:MM:SS�HH:MM to YYYY-MM-DD HH:MM
*/
CString CommonUtils::ConvertEventTime(CString eventTime)
{
	if (eventTime.GetLength() < 19)
	{
		return _T("Invalid Time");
	}
	CString date = eventTime.Mid(0, 10);  // Extract date
	CString time = eventTime.Mid(11, 5);  // Extract time

	return date + _T(" ") + time;
}

/**
* Get date formated YYY-MM-DD from YYYY-MM-DDTHH:MM:SS�HH:MM
*/
CString CommonUtils::GetDate(CString& dateTimeString)
{
	// The date part is before the 'T' character
	int pos = dateTimeString.Find(_T('T'));
	if (pos != -1)
	{
		return dateTimeString.Left(pos);  // Return the substring before 'T'
	}
	return _T("InvalidDate");
}

/**
* Get time formated HH:MM:SS from YYYY-MM-DDTHH:MM:SS�HH:MM
*/
CString CommonUtils::GetTime(CString& dateTimeString)
{
	// Find the position of 'T' in the string (which separates date and time)
	int startPos = dateTimeString.Find(_T('T'));
	if (startPos != -1)
	{
		// Find the position of '+' or '-' after the 'T'
		int plusPos = dateTimeString.Find(_T('+'), startPos);
		int minusPos = dateTimeString.Find(_T('-'), startPos);

		// Determine the first position (either '+' or '-')
		int endPos = min(plusPos != -1 ? plusPos : INT_MAX, minusPos != -1 ? minusPos : INT_MAX);

		// If we found the end of the time part (either '+' or '-')
		if (endPos != INT_MAX)
		{
			return dateTimeString.Mid(startPos + 1, endPos - startPos - 1);  // Extract time
		}
	}
	return _T("InvalidTime");
}

/**
* Get timezone �HH:MM from YYYY-MM-DDTHH:MM:SS�HH:MM
*/
CString CommonUtils::GetTimeZone(CString& dateTimeString)
{
	try
	{
		return dateTimeString.Right(6);
	}
	catch(...)
	{
		return _T("InvalidTimeZone");
	}
}

/**
* Get current time with format
*/
CString CommonUtils::GetCurrentTimeInISOFormat(ISOFormatType formatType)
{
	// Get current time and convert to time_t for seconds
	auto now = std::chrono::system_clock::now();
	std::time_t currentTime = std::chrono::system_clock::to_time_t(now);

	// Get milliseconds part
	auto milliseconds = std::chrono::duration_cast<std::chrono::milliseconds>(now.time_since_epoch()) % 1000;

	// Create a string stream for formatting
	std::stringstream ss;

	if (formatType == ISOFormatType::UTC)
	{
		// Convert time to UTC (GMT)
		std::tm gmtTime;
		gmtime_s(&gmtTime, &currentTime);

		// Format the time as "YYYY-MM-DDTHH:MM:SS.sssZ"
		ss << std::put_time(&gmtTime, "%Y-%m-%dT%H:%M:%S")
			<< "." << std::setw(3) << std::setfill('0') << milliseconds.count() << "Z";
	}
	else if (formatType == ISOFormatType::LocalTime)
	{
		// Convert time to local time
		std::tm localTime;
		localtime_s(&localTime, &currentTime);

		// Get UTC time to calculate timezone offset
		std::tm utcTime;
		gmtime_s(&utcTime, &currentTime);

		// Calculate the timezone offset in seconds
		int timezoneOffsetSeconds = static_cast<int>(std::difftime(std::mktime(&localTime), std::mktime(&utcTime)));  // Explicit cast to int
		int hoursOffset = timezoneOffsetSeconds / 3600;
		int minutesOffset = std::abs((timezoneOffsetSeconds % 3600) / 60);

		// Format the time as "YYYY-MM-DDTHH:MM:SS"
		ss << std::put_time(&localTime, "%Y-%m-%dT%H:%M:%S");

		// Append the timezone offset as �HH:MM
		if (hoursOffset >= 0)
		{
			ss << "+" << std::setw(2) << std::setfill('0') << hoursOffset
				<< ":" << std::setw(2) << std::setfill('0') << minutesOffset;
		}
		else
		{
			ss << "-" << std::setw(2) << std::setfill('0') << -hoursOffset
				<< ":" << std::setw(2) << std::setfill('0') << minutesOffset;
		}
	}

	// Convert the result to CString and return
	return CString(ss.str().c_str());
}

/**
* Load all time in a day to CComboBox
*/
void CommonUtils::LoadTimesIntoCComboBox(CComboBox& comboBox)
{
	comboBox.ResetContent();

	CStringArray timeZones;

	for (int hour = 0; hour < 24; ++hour)
	{
		// Loop through minutes (0, 15, 30, 45)
		for (int minute = 0; minute < 60; minute += 15)
		{
			CString timeString;
			timeString.Format(_T("%02d:%02d"), hour, minute);
			timeZones.Add(timeString);
		}
	}

	// Add the times to the combo box
	for (int i = 0; i < timeZones.GetSize(); ++i)
	{
		comboBox.AddString(timeZones[i]);
	}
}

void CommonUtils::LoadTimeZonesIntoCComboBox(CComboBox& comboBox)
{
	{
		comboBox.ResetContent();
		CStringArray timeZones;
		timeZones.Add(_T("+00:00 (UTC)"));
		timeZones.Add(_T("+01:00 (CET)"));
		timeZones.Add(_T("+02:00 (EET)"));
		timeZones.Add(_T("+03:00 (MSK)"));
		timeZones.Add(_T("+04:00 (GST)"));
		timeZones.Add(_T("+05:00 (PKT)"));
		timeZones.Add(_T("+06:00 (BST)"));
		timeZones.Add(_T("+07:00 (ICT)"));
		timeZones.Add(_T("+08:00 (CST)"));
		timeZones.Add(_T("+09:00 (JST)"));
		timeZones.Add(_T("+10:00 (AEST)"));
		timeZones.Add(_T("+11:00 (SBT)"));
		timeZones.Add(_T("+12:00 (NZST)"));
		timeZones.Add(_T("-01:00 (AZOT)"));
		timeZones.Add(_T("-02:00 (GST-2)"));
		timeZones.Add(_T("-03:00 (ART)"));
		timeZones.Add(_T("-04:00 (AST)"));
		timeZones.Add(_T("-05:00 (EST)"));
		timeZones.Add(_T("-06:00 (CST)"));
		timeZones.Add(_T("-07:00 (MST)"));
		timeZones.Add(_T("-08:00 (PST)"));
		timeZones.Add(_T("-09:00 (AKST)"));
		timeZones.Add(_T("-10:00 (HST)"));
		timeZones.Add(_T("-11:00 (SST)"));
		timeZones.Add(_T("-12:00 (BIT)"));

		for (int i = 0; i < timeZones.GetSize(); i++)
		{
			comboBox.AddString(timeZones[i]);
		}

		// Get the local time zone using GetTimeZoneInformation
		TIME_ZONE_INFORMATION tzInfo;
		GetTimeZoneInformation(&tzInfo);

		// Calculate the time zone offset in hours and minutes
		int biasMinutes = -(tzInfo.Bias);
		int hours = biasMinutes / 60;
		int minutes = abs(biasMinutes % 60);

		// Format the bias as a string (+HH:MM or -HH:MM)
		CString localTimeZoneStr;
		if (hours >= 0)
			localTimeZoneStr.Format(_T("+%02d:%02d"), hours, minutes);
		else
			localTimeZoneStr.Format(_T("-%02d:%02d"), -hours, minutes);

		int index = comboBox.FindString(0, localTimeZoneStr);
		if (index != CB_ERR)
		{
			comboBox.SetCurSel(index);
		}
		else
		{
			comboBox.SetCurSel(0);
		}
	}
}

void CommonUtils::SetDateForCDateTimeCtrl(CDateTimeCtrl& ctrl, const CString& datetimeString)
{
	// Parse the date string (format: "YYYY-MM-DD")
	int year, month, day;
	if (_stscanf_s(datetimeString, _T("%d-%d-%d"), &year, &month, &day) == 3)
	{
		COleDateTime date(year, month, day, 0, 0, 0); // Date with 00:00:00 time

		ctrl.SetTime(date);
	}
	else
	{
		AfxMessageBox(_T("Invalid date format!"));
	}
}

void CommonUtils::SetTimeForCDateTimeCtrl(CDateTimeCtrl& ctrl, const CString& datetimeString)
{
	// Parse the time string (format: "HH:MM:SS")
	int hour, minute, second;
	if (_stscanf_s(datetimeString, _T("%d:%d:%d"), &hour, &minute, &second) == 3)
	{
		// Create a COleDateTime object with today's date and the specified time
		COleDateTime time(1970, 1, 1, hour, minute, second);

		// Set the time in the CDateTimeCtrl control
		ctrl.SetTime(time);
	}
	else
	{
		AfxMessageBox(_T("Invalid time format!"));
	}
}

void CommonUtils::SetValueForCComboBox(CComboBox& comboBox, CString value, bool setValueIfNotFound, bool findExact)
{
	int index = findExact ? comboBox.FindStringExact(-1, value) : comboBox.FindString(-1, value);
	if (index != CB_ERR)
	{
		comboBox.SetCurSel(index);
		return;
	}

	if (setValueIfNotFound)
	{
		comboBox.AddString(value);
	}
	else {
		comboBox.SetWindowTextW(_T(""));
	}
}

CString CommonUtils::GenerateCustomUuid(int length)
{
	const char Base32HexChars[] = "0123456789abcdefghijklmnopqrstuv";

	// Validate length
	if (length < 5 || length > 1024)
	{
		throw std::out_of_range("Length must be between 5 and 1024 characters.");
	}

	// Calculate the number of bits and bytes required
	int numBits = length * 5; // Each base32hex character represents 5 bits
	int numBytes = (numBits + 7) / 8; // Convert bits to bytes, rounding up

	// Generate random bytes
	std::vector<unsigned char> randomBytes(numBytes);
	std::random_device rd;
	std::mt19937 rng(rd());
	std::uniform_int_distribution<int> dist(0, 255);

	for (int i = 0; i < numBytes; ++i)
	{
		randomBytes[i] = static_cast<unsigned char>(dist(rng));
	}

	// Convert the bytes to base32hex string
	CString uuidBuilder;

	// Process each byte and convert it to base32hex characters
	int bitBuffer = 0;
	int bitsInBuffer = 0;

	for (unsigned char b : randomBytes)
	{
		bitBuffer = (bitBuffer << 8) | b;
		bitsInBuffer += 8;

		while (bitsInBuffer >= 5)
		{
			bitsInBuffer -= 5;
			int index = (bitBuffer >> bitsInBuffer) & 31; // Extract 5 bits at a time
			uuidBuilder.AppendChar(Base32HexChars[index]);
		}
	}

	// If there are remaining bits, pad them with zeros
	if (bitsInBuffer > 0)
	{
		int index = (bitBuffer << (5 - bitsInBuffer)) & 31;
		uuidBuilder.AppendChar(Base32HexChars[index]);
	}

	// Trim the result to the desired length
	return uuidBuilder.Left(length);
}

CString CommonUtils::FormatStringValueOrNull(const CString& value) {
	if (value.IsEmpty()) {
		return _T("NULL");
	} else {
		CString formattedValue;
		formattedValue.Format(_T("'%s'"), value);
		return formattedValue;
	}
}