using Google.Apis.Calendar.v3.Data;
using GoogleCanlendarService.Data;
using GoogleCanlendarService.Enum;
using GoogleCanlendarService.Models;
using GoogleCanlendarService.Repository;
using GoogleCanlendarService.Services;
using GoogleCanlendarService.Socket;
using GoogleCanlendarService.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace GoogleCanlendarService
{
    public partial class InsertEventService : ServiceBase
    {
        const string SERVICE_SOURCE = "GoogleCalendarServiceSource";
        const string SERVICE_LOG = "GoogleCalendarServiceLog";
        Timer timer = new Timer();
        private TcpClientHandler tcpClientHandler;

        public InsertEventService()
        {
            InitializeComponent();
            eventLogInsertCalendarEvent = new EventLog();
            if (!EventLog.SourceExists(SERVICE_SOURCE))
            {
                EventLog.CreateEventSource(SERVICE_SOURCE, SERVICE_LOG);
            }
            eventLogInsertCalendarEvent.Source = SERVICE_SOURCE;
            eventLogInsertCalendarEvent.Log = SERVICE_LOG;
        }

        protected override void OnStart(string[] args)
        {
            WriteLog($"Service is started at {DateTime.Now}");
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = int.Parse(ConfigurationManager.AppSettings["TimeInterval"]); // Number in miliseconds
            timer.Enabled = true;
            StartTcpClient();
        }

        private void StartTcpClient()
        {
            try
            {
                string tcpServerHost = ConfigurationManager.AppSettings["TCPServerHost"];
                int tcpServerPort = int.Parse(ConfigurationManager.AppSettings["TCPServerPort"]);

                tcpClientHandler = new TcpClientHandler(tcpServerHost, tcpServerPort);

                _ = tcpClientHandler.ListenForMessagesAsync(OnMessageReceived);
            }
            catch (Exception ex)
            {
                WriteLog($"Error on connect to TCP Server: {ex}");
            }
        }

        private void OnMessageReceived(string message)
        {
            WriteLog($"Received request from TCP Server: {message}");
            SyncEventBetweenGoogleCalendarAndDB();
        }

        public void SendMessageToTCPServer(TCPServerRequest request)
        {
            string jsonString = JsonConvert.SerializeObject(request);
            tcpClientHandler.SendMessage(jsonString);
        }

        protected override void OnStop()
        {
            tcpClientHandler.Close();
            WriteLog($"Service is stopped at {DateTime.Now}");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            SyncEventBetweenGoogleCalendarAndDB();
        }

        private void SyncEventBetweenGoogleCalendarAndDB()
        {
            try
            {
                GoogleCalendarService googleCalendarService = new GoogleCalendarService();
                //Retrieve all upcomming event from Google
                IList<Event> eventsFromGoogle = googleCalendarService.GetUpcomingEvents(GetCalendarId());
                var eventsFromGoogleDic = eventsFromGoogle.ToDictionary(entity => entity.Id);

                using (SqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    EventRepository eventRepository = new EventRepository(conn);
                    // Retrieve all events with changes from both sides (Database/Google)
                    IList<EventModel> eventsDiff = eventRepository.GetAllUpcommingEventsDiffFromDB(eventsFromGoogle, GetCalendarId());

                    // Handle create/update/delete event from both side
                    foreach (var eventObj in eventsDiff)
                    {
                        // Event exist on Google
                        if (eventsFromGoogleDic.TryGetValue(eventObj.Id, out var eventFromGoogle))
                        {
                            if (eventObj.UpdatedRaw == null)
                            {
                                // Event doesn't exist in Database -> New event from Google
                                HandleEventCreatedFromGoogle(eventRepository, eventFromGoogle, GetCalendarId());
                                continue;
                            }

                            DateTimeOffset googleUpdated = DateTimeOffset.Parse(eventFromGoogle.UpdatedRaw);
                            DateTimeOffset dbUpdated = DateTimeOffset.Parse(eventObj.UpdatedRaw);

                            // Event change from Database
                            if (dbUpdated > googleUpdated)
                            {
                                if (eventObj.Deleted == true)
                                {
                                    // Event deleted from Database
                                    HandleEventDeletedFromDatabase(googleCalendarService, eventObj);
                                }
                                else
                                {
                                    // Event changed from Database
                                    HandleEventChangedFromDatabase(googleCalendarService, eventRepository, eventObj);
                                }
                            }
                            else
                            {
                                // Event changed from Google
                                HandleEventChangedFromGoogle(eventRepository, conn, eventObj, eventFromGoogle);
                            }
                        }
                        // Event doesn't exist on Google
                        else
                        {
                            if (eventObj.GoogleCreated == true)
                            {
                                // Event deleted from Google
                                HandleEventDeletedFromGoogle(eventRepository, eventObj);
                            }
                            else
                            {
                                // Get event by Id included deleted event
                                if (googleCalendarService.GetEventById(eventObj.Id, eventObj.CalendarId) != null)
                                {
                                    // Event deleted from Database
                                    HandleEventDeletedFromGoogle(eventRepository, eventObj);
                                }
                                else
                                {
                                    // Event created from Database
                                    HandleEventCreatedFromDatabase(googleCalendarService, eventRepository, eventObj);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }

        private void HandleEventCreatedFromGoogle(EventRepository eventRepo, Event eventObj, string calendarId)
        {
            // Insert event to databse
            eventRepo.Insert(eventObj, calendarId);
            WriteLog($"Insert new event in database (Id: {eventObj.Id})");
            TCPServerRequest request = new TCPServerRequest(TCPServerRequestType.CREATE, eventObj.Id);
            SendMessageToTCPServer(request);
        }

        private void HandleEventChangedFromGoogle(EventRepository eventRepo, SqlConnection conn, EventModel eventModelOld, Event eventNew)
        {
            EventModel eventModelNew = new EventModel(eventNew, GetCalendarId());
            EventDateTimeRepository eventDateTimeRepository = new EventDateTimeRepository(conn);
            // Update Start
            if (!eventModelNew.IsStartEqual(eventModelOld.StartEventDateTime))
            {
                eventModelNew.StartEventDateTime.Id = eventModelOld.StartEventDateTime.Id;
                eventDateTimeRepository.Update(eventModelNew.StartEventDateTime);
            }

            // Update End
            if (!eventModelNew.IsEndEqual(eventModelOld.EndEventDateTime))
            {
                eventModelNew.EndEventDateTime.Id = eventModelOld.EndEventDateTime.Id;
                eventDateTimeRepository.Update(eventModelNew.EndEventDateTime);
            }

            // Update OriginalStartTime
            if (!eventModelNew.IsOriginalStartTimeEqual(eventModelOld.OriginalStartTimeEventDateTime))
            {
                eventModelNew.OriginalStartTimeEventDateTime.Id = eventModelOld.OriginalStartTimeEventDateTime.Id;
                eventDateTimeRepository.Update(eventModelNew.OriginalStartTimeEventDateTime);
            }

            // Update Attendees
            if (!eventModelNew.IsAttendeesEqual(eventModelOld.AttendeesModel))
            {
                EventAttendeeRepository eventAttendeeRepository = new EventAttendeeRepository(conn);
                if (eventModelOld.AttendeesModel?.Count() > 0)
                {
                    eventAttendeeRepository.DeleteByEventId(eventModelOld.Id);
                }
                if (eventModelNew.AttendeesModel?.Count() > 0)
                {
                    foreach (var item in eventModelNew.AttendeesModel)
                    {
                        item.EventId = eventModelNew.Id;
                        eventAttendeeRepository.Insert(item);
                    }
                }
            }

            // Update Event
            eventRepo.UpdateSimpleField(eventModelNew);
            WriteLog($"Update event in database (Id: {eventModelNew.Id})");
            TCPServerRequest request = new TCPServerRequest(TCPServerRequestType.UPDATE, eventModelNew.Id);
            SendMessageToTCPServer(request);
        }

        private void HandleEventDeletedFromGoogle(EventRepository eventRepo, Event eventObj)
        {
            // Delete event in databse
            eventRepo.DeleteById(eventObj.Id);
            WriteLog($"Delete event in database (Id: {eventObj.Id})");
            TCPServerRequest request = new TCPServerRequest(TCPServerRequestType.UPDATE, eventObj.Id);
            SendMessageToTCPServer(request);
        }

        private void HandleEventCreatedFromDatabase(GoogleCalendarService service, EventRepository eventRepo, EventModel eventObj)
        {
            Event createdEvent = service.CreateEvent(eventObj.ToEvent(), eventObj.CalendarId);
            eventRepo.UpdateEventAfterHandleOnGoogle(createdEvent);
            WriteLog($"Create event on Google (Id: {createdEvent.Id})");
        }

        private void HandleEventChangedFromDatabase(GoogleCalendarService service, EventRepository eventRepo, EventModel eventObj)
        {
            Event updatedEvent = service.UpdateEvent(eventObj.ToEvent(), eventObj.CalendarId);
            eventRepo.UpdateEventAfterHandleOnGoogle(updatedEvent);
            WriteLog($"Update event on Google (Id: {updatedEvent.Id})");
        }

        private void HandleEventDeletedFromDatabase(GoogleCalendarService service, EventModel eventToDelete)
        {
            service.DeleteEventById(eventToDelete.Id, eventToDelete.CalendarId);
            WriteLog($"Delete event on Google (Id: {eventToDelete.Id})");
        }
        private void WriteLog(string content)
        {
            CommonUtils.WriteLog(content);
            //Console.WriteLine(content);
            //eventLogInsertCalendarEvent.WriteEntry(content);
        }

        private string GetCalendarId()
        {
            return ConfigurationManager.AppSettings["GoogleCalendarId"];
        }

        /// <summary>
        /// This function is for running on console application
        /// </summary>
        public void Start(string[] args = null)
        {
            OnStart(args);
        }
    }
}
