using System.Collections.Generic;
using static Google.Apis.Calendar.v3.Data.Event;

namespace GoogleCanlendarService.Models
{
    internal class RemindersDataModel : RemindersData
    {
        public int Id { get; set; }
        public IList<EventReminderModel> OverridesModel { get; set; }

        public RemindersDataModel() { }

        public RemindersDataModel(RemindersData remindersData)
        {
            this.UseDefault = remindersData.UseDefault;
            EventReminderModel eventReminderModel;
            if (remindersData.Overrides != null)
            {
                this.OverridesModel = new List<EventReminderModel>();
                foreach (var item in remindersData.Overrides)
                {
                    eventReminderModel = new EventReminderModel(item);
                    this.OverridesModel.Add(eventReminderModel);
                }
            }
        }
    }
}
