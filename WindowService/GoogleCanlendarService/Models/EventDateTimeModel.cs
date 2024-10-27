using Google.Apis.Calendar.v3.Data;

namespace GoogleCanlendarService.Models
{
    internal class EventDateTimeModel : EventDateTime
    {
        public int? Id { get; set; }

        public EventDateTimeModel() { }

        public EventDateTimeModel(EventDateTime eventDateTime)
        {
            this.Date = eventDateTime.Date;
            this.DateTimeRaw = eventDateTime.DateTimeRaw;
            this.TimeZone = eventDateTime.TimeZone;
            this.ETag = eventDateTime.ETag;
        }

        public EventDateTimeModel(int id, string dateTimeRaw, string timeZone, string eTag)
        {
            this.Id = id;
            this.DateTimeRaw = dateTimeRaw;
            this.TimeZone = timeZone;
            this.ETag = eTag;
        }

        public EventDateTime ToEventDateTime()
        {
            return new EventDateTime()
            {
                Date = this.Date,
                DateTimeRaw = this.DateTimeRaw,
                TimeZone = this.TimeZone,
                ETag = this.ETag,
            };
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            EventDateTimeModel other = (EventDateTimeModel)obj;

            return this.Date == other.Date &&
                   this.DateTimeRaw == other.DateTimeRaw &&
                   this.TimeZone == other.TimeZone &&
                   this.ETag == other.ETag;
        }

        public override int GetHashCode()
        {
            return (Date != null ? Date.GetHashCode() : 0) ^
                   (DateTimeRaw != null ? DateTimeRaw.GetHashCode() : 0) ^
                   (TimeZone != null ? TimeZone.GetHashCode() : 0) ^
                   (ETag != null ? ETag.GetHashCode() : 0);
        }
    }
}
