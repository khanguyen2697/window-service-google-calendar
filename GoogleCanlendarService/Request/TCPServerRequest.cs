using System;
using System.Configuration;

namespace GoogleCanlendarService.Models
{
    public class TCPServerRequest
    {
        private static readonly string DefaultRequester = ConfigurationManager.AppSettings["TCPClientType"];
        public string Requester
        {
            get { return DefaultRequester; }
        }
        public string EventId {get; set;}
        public string Type { get; set; }

        public string Timestamp
        {
            get { return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"); }
        }

        public TCPServerRequest(string type, string eventId)
        { 
            Type = type;
            EventId = eventId;
        }
    }
}
