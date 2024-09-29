using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GoogleCanlendarService.Utils
{
    class DumpData
    {
        public static List<EventAttachment> EventAttachmentList()
        {
            List<EventAttachment> attachmentList = new List<EventAttachment>();
            EventAttachment eventAttachment1 = new EventAttachment()
            {
                FileId = GenerateRandomId(),
                Title = "Event Attachment 1",
                ETag = "3454671570342000"
            };
            EventAttachment eventAttachment2 = new EventAttachment()
            {
                FileId = GenerateRandomId(),
                Title = "Event Attachment 2",
                ETag = "3454671789276000"
            };
            attachmentList.Add(eventAttachment1);
            attachmentList.Add(eventAttachment2);
            return attachmentList;
        }

        public static List<EventAttendee> EventAttendeeList()
        {
            List<EventAttendee> AttendeeList = new List<EventAttendee>();
            EventAttendee eventAttendee1 = new EventAttendee
            {
                Id = GenerateRandomId(),
                Email = "eventAttendee1@email.com",
                Comment = "Event Attendee comment 1"
            };
            EventAttendee eventAttendee2 = new EventAttendee
            {
                Id = GenerateRandomId(),
                Email = "eventAttendee2@email.com",
                Comment = "Event Attendee comment 2"
            };
            AttendeeList.Add(eventAttendee1);
            AttendeeList.Add(eventAttendee2);
            return AttendeeList;
        }

        public static List<Event> EventList()
        {
            List<Event> events = new List<Event>();
            Event newEvent1 = new Event()
            {
                Summary = "Weekly meeting summary",
                Location = "Chem. de Mornex 3, 1003 Lausanne, Switzerland",
                Description = "Weekly meeting",
                Start = new EventDateTime()
                {
                    DateTimeDateTimeOffset = DateTime.Parse("2024-10-04T10:00:00"),
                    TimeZone = "Europe/Zurich",
                },
                End = new EventDateTime()
                {
                    DateTimeDateTimeOffset = DateTime.Parse("2024-10-04T11:00:00"),
                    TimeZone = "Europe/Zurich",
                },
                Recurrence = new List<string> { "RRULE:FREQ=DAILY;COUNT=3" },
                ////  Required Domain-Wide Delegation of Authority
                //Attendees = new EventAttendee[]
                //{
                //    new EventAttendee() { Email = "example@gmail.com" }
                //},
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[]
                                {
                        new EventReminder() { Method = "email", Minutes = 24 * 60 },
                        new EventReminder() { Method = "popup", Minutes = 10 }
                                }
                }
            };
            Event newEvent2 = new Event()
            {
                Summary = "Monthly meeting summary",
                Location = "Chem. de Mornex 3, 1003 Lausanne, Switzerland",
                Description = "Monthly meeting",
                Start = new EventDateTime()
                {
                    DateTimeDateTimeOffset = DateTime.Parse("2024-10-15T10:00:00"),
                    TimeZone = "Europe/Zurich",
                },
                End = new EventDateTime()
                {
                    DateTimeDateTimeOffset = DateTime.Parse("2024-10-15T11:00:00"),
                    TimeZone = "Europe/Zurich",
                },
                Recurrence = new List<string> { "RRULE:FREQ=DAILY;COUNT=2" },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[]
                    {
                        new EventReminder() { Method = "email", Minutes = 24 * 60 },
                        new EventReminder() { Method = "popup", Minutes = 10 }
                    }
                }
            };

            events.Add(newEvent1);
            events.Add(newEvent2);
            return events;
        }

        public static string GenerateRandomId()
        {
            // Generate a random byte array
            byte[] randomBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(randomBytes);

                StringBuilder hashString = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashString.Append(b.ToString("x2"));
                }
                return hashString.ToString();
            }
        }
        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
