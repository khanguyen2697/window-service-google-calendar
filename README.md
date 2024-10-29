# Google Calendar Application

This project consists of three main components that work together to integrate with Google Calendar to handle google calendar on MFC application.

## Components
1. **Windows Service (WindowService)**
   - A service that connects to Google Calendar and database to synchronize data between them.

2. **MFC Application (Calendar)**
   - A MFCapplication that interacts with the database and provides a user interface.

3. **TCP Server (TCPServer)**
   - A server that facilitates communication between the Windows Service and the MFC application for data synchronization.


### Installation window service
1. Build project
2. Update `App.config` file to your credential
3. Open CMD as Administrator
4. Go to bin/Debug (bin/Release) folder
5. `installutil GoogleCanlendarService.exe`

#### Start window service
`net start GoogleCalendarService`
#### Stop window service
`net stop GoogleCalendarService`
#### Delete serivce
`sc delete GoogleCalendarService`

#### Log folder
C:\Windows\System32\winevt\Logs

#### For dev
To running project as Console Application
1. Update Output type
	`Properties`->`Application`->`Output type`-> `Console Application`

## Notes
Make sure the TCP Server is started before starting the Windows Service and MFC application to ensure data synchronization.