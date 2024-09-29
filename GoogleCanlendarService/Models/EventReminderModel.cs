using Google.Apis.Calendar.v3.Data;

namespace GoogleCanlendarService.Models
{
    class EventReminderModel : EventReminder
    {
        public int Id { get; set; }
        public int RemindersDataId { get; set; }

        public EventReminderModel() { }

        public EventReminderModel(EventReminder eventReminder)
        {
            this.Method = eventReminder.Method;
            this.Minutes = eventReminder.Minutes;
            this.ETag = eventReminder.ETag;
        }
        public EventReminderModel(int id, int remindersDataId, string method, int minutes, string eTag)
        {
            this.Id = id;
            this.RemindersDataId = remindersDataId;
            this.Method = method;
            this.Minutes = minutes;
            this.ETag = eTag;
        }
    }
}
