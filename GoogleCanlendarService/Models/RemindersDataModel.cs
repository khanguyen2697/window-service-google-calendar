using System.Collections.Generic;
using static Google.Apis.Calendar.v3.Data.Event;

namespace GoogleCanlendarService.Models
{
    class RemindersDataModel : RemindersData
    {
        public int Id { get; set; }
        public IList<EventReminderModel> OverridesModel { get; set; }

        public RemindersDataModel() { }

        public RemindersDataModel(RemindersData remindersData)
        {
            this.UseDefault = remindersData.UseDefault;
            this.OverridesModel = new List<EventReminderModel>();
            EventReminderModel eventReminderModel;
            foreach (var item in remindersData.Overrides)
            {
                eventReminderModel = new EventReminderModel(item);
                this.OverridesModel.Add(eventReminderModel);
            }
        }
    }
}
