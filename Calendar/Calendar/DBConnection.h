#pragma once

#include <afxdb.h>

class DBConnection
{
public:
	DBConnection(void);
	~DBConnection(void);
	BOOL Connect(CDatabase& database);
	void Disconnect(CDatabase& database);
};

