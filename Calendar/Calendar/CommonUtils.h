#pragma once
class CommonUtils
{
public:
	enum class ISOFormatType {
        UTC,        // YYYY-MM-DDTHH:MM:SS.sssZ (UTC format)
        LocalTime   // YYYY-MM-DDTHH:MM:SS�HH:MM (Local time with offset)
    };

	static CString ConvertEventTime(CString eventTime);
	static CString GetCurrentTimeInISOFormat(ISOFormatType formatType = ISOFormatType::UTC);
	static CString GetDate(CString& dateTimeStr);
	static CString GetTime(CString& dateTimeStr);
	static CString GetTimeZone(CString& dateTimeStr);
	static void LoadTimeZonesIntoCComboBox(CComboBox& comboBox);
	static void LoadTimesIntoCComboBox(CComboBox& comboBox);
	static void SetDateForCDateTimeCtrl(CDateTimeCtrl& ctrl, const CString& dateString);
	static void SetTimeForCDateTimeCtrl(CDateTimeCtrl& ctrl, const CString& dateString);
	static void SetValueForCComboBox(CComboBox& comboBox, CString value, bool setValueIfNotFound = true, bool findExact = true);
	static CString GenerateCustomUuid(int length = 26);
	static CString FormatStringValueOrNull(const CString& value);
};

