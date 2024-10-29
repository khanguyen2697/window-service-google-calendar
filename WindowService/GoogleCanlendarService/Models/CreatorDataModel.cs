using static Google.Apis.Calendar.v3.Data.Event;

namespace GoogleCanlendarService.Models
{
    internal class CreatorDataModel : CreatorData
    {
        public CreatorDataModel() { }

        public CreatorDataModel(CreatorData creatorData)
        {
            Id = creatorData.Id;
            Email = creatorData.Email;
            DisplayName = creatorData.DisplayName;
            Self = creatorData.Self;
        }
    }
}
