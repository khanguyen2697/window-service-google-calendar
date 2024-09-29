using Google.Apis.Calendar.v3.Data;
using GoogleCanlendarService.Data;
using GoogleCanlendarService.Services;
using GoogleCanlendarService.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace GoogleCanlendarService
{
    public partial class InsertEventService : ServiceBase
    {

        Timer timer = new Timer();
        const string SERVICE_SOURCE = "GoogleCalendarServiceSource";
        const string SERVICE_LOG = "GoogleCalendarServiceLog";

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
            eventLogInsertCalendarEvent.WriteEntry("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = int.Parse(ConfigurationManager.AppSettings["TimeInterval"]); // Number in miliseconds
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            eventLogInsertCalendarEvent.WriteEntry("Service is stopped at " + DateTime.Now);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            InsertEventsFromGoogleCalendarToDB();
        }

        private void InsertEventsFromGoogleCalendarToDB()
        {
            try
            {
                GoogleCalendarService googleCalendarService = new GoogleCalendarService();
                IList<Event> events = googleCalendarService.GetUpcomingEvents();

                using (SqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    EventRepository eventRepository = new EventRepository(conn);
                    List<string> eventIdsFromDB = eventRepository.GetUpcomingEventIds();
                    List<string> insertedIds = new List<string>();

                    foreach (var eventObj in events)
                    {
                        // Only insert if event do not exist in database
                        if (!eventIdsFromDB.Contains(eventObj.Id))
                        {
                            eventRepository.Insert(eventObj);
                            insertedIds.Add(eventObj.Id);
                        }
                    }

                    if (insertedIds.Count > 0)
                    {
                        string insertedIdsString = string.Join(", ", insertedIds);
                        eventLogInsertCalendarEvent.WriteEntry($"Insert new event with Id: {insertedIdsString}");
                    }
                }
            }
            catch (Exception ex)
            {
                eventLogInsertCalendarEvent.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
