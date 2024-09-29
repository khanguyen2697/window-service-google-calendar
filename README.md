# Google Calendar Service

This project is a Windows Service that integrates with Google Calendar to get Event from Google Calendar then install into database

### Installation window service
1. Build project
2. Update `App.config` file to your credential
3. Open CMD as Administrator
4. Go to bin/Debug (bin/Release) folder
5. `install GoogleCanlendarService.exe`

### Start window service
`net start GoogleCalendarService`
### Stop window service
`net stop GoogleCalendarService`
### Delete serivce
`sc delete GoogleCalendarService`

### Log folder
C:\Windows\System32\winevt\Logs