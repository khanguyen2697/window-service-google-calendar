using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUpdateDatabase
{
    public struct Creator
    {
        public string Id;
        public string Email;
    }

    public struct Event
    {
        public string Id;
        public string Summary;
        public string Start;
        public string End;
        public string CalendarId;
        public string CreatorId;
        public string OrganizerId;
        public int? StartId;
        public int? EndId;
        public Creator Creator;
        public OrganizerData Organizer;
    }

    public struct EventDateTime
    {
        public string DateTimeRaw;
        public string TimeZone;
    }

    public struct OrganizerData
    {
        public string Id;
        public string Email;
    }
}
