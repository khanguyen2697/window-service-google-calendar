using static Google.Apis.Calendar.v3.Data.Event;

namespace GoogleCanlendarService.Models
{
    class OrganizerDataModel : OrganizerData
    {
        public OrganizerDataModel() { }

        public OrganizerDataModel(OrganizerData organizerData)
        {
            Id = organizerData.Id;
            Email = organizerData.Email;
            DisplayName = organizerData.DisplayName;
            Self = organizerData.Self;
        }
    }
}
