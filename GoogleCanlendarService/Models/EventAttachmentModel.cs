using Google.Apis.Calendar.v3.Data;

namespace GoogleCanlendarService.Models
{
    class EventAttachmentModel : EventAttachment
    {
        public string EventId { get; set; }

        public EventAttachmentModel() { }

        public EventAttachmentModel(EventAttachment eventAttachment)
        {
            FileId = eventAttachment.FileId;
            FileUrl = eventAttachment.FileUrl;
            IconLink = eventAttachment.IconLink;
            MimeType = eventAttachment.MimeType;
            Title = eventAttachment.Title;
            ETag = eventAttachment.ETag;
        }
    }
}
