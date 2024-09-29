using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;

namespace GoogleCanlendarService.Services
{
    public class GoogleCalendarService : GoogleService
    {

        private readonly CalendarService service;
        public GoogleCalendarService()
        {
            service = GetCalendarService();
        }

        public CalendarService GetCalendarService()
        {
            // Load the credentials from the service account key file
            GoogleCredential credential = GetCredential(CalendarService.Scope.Calendar);

            // Create the Google Calendar API service
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Calendar API Service Account",
            });

            return service;
        }

        /// <summary>
        /// Get all events
        /// </summary>
        /// <param name="calendarId">Calendar identifier. Default is "primary"</param>
        /// <returns>A list of events from the specified calendar.</returns>
        public IList<Event> GetEvents(string calendarId = "primary")
        {

            EventsResource.ListRequest request = service.Events.List(calendarId);
            IList<Event> listEvents = request.Execute().Items;

            return listEvents;
        }

        /// <summary>
        /// Get all up comming events
        /// </summary>
        /// <param name="calendarId">Calendar identifier. Default is "primary"</param>
        /// <returns>A list of upcoming events from the specified calendar.</returns>
        public IList<Event> GetUpcomingEvents(string calendarId = "primary")
        {

            EventsResource.ListRequest request = service.Events.List(calendarId);
            request.TimeMinDateTimeOffset = DateTime.Now;
            IList<Event> listEvents = request.Execute().Items;

            return listEvents;
        }

        /// <summary>
        /// Add new event into the calendar
        /// </summary>
        /// <param name="newEvent">The event to create</param>
        /// <param name="calendarId">Calendar identifier. Default is "primary"</param>
        /// <returns>Creted event</returns>
        public Event CreateEvent(Event newEvent, string calendarId = "primary")
        {
            // Insert the event into the calendar
            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
            Event createdEvent = request.Execute();
            return createdEvent;
        }

        public void DeleteAllEventByCalendarId(string calendarId = "primary")
        {
            IList<Event> events = GetEvents();
            foreach (var item in events)
            {
                service.Events.Delete(calendarId, item.Id).Execute();
            }
        }
    }
}
