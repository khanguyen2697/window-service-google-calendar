using System.Security.Cryptography;
using System.Text;
using TestUpdateDatabase;

internal class Program
{
    static DBUtils dBUtils = new DBUtils();

    static void Main(string[] args)
    {
        //CreateNewEvent("Test create from database", "2024-11-26T13:15:00+07:00", "2024-11-26T14:15:00+07:00");

        string eventId = "hscfm9jvo1pna0dahjgh1de2g6";
        //UpdateEvent(eventId, "Test event from GOOGLE Updated FROM DB 999");
        //UpdateEvent(eventId, "Test event from GOOGLE Updated FROM DB 1", 575, "2024-11-26T10:30:00+07:00", 576, "2024-11-26T17:45:00+07:00");
        DeleteEvent(eventId);
    }

    static void DeleteEvent(string id)
    {
        dBUtils.DeleteById(id);
        Console.WriteLine($"Deleted event `{id}` successfully");
    }

    static void CreateNewEvent(string summary, string start, string end)
    {
        // Sample time: 2024-11-25T16:30:00+07:00
        string eventId = GenerateCustomUuid();
        Event eventObj = new Event()
        {
            Id = eventId,
            Summary = summary,
            CalendarId = "anhkha0001@gmail.com",
            Start = start,
            End = end,
            Creator = new Creator()
            {
                Id = eventId,
                Email = "anhkha0001@gmail.com"
            },
            Organizer = new OrganizerData()
            {
                Id = eventId,
                Email = "anhkha0001@gmail.com"
            }
        };

        int startId = dBUtils.InsertEventDateTime(eventObj.Start);
        int endId = dBUtils.InsertEventDateTime(eventObj.End);
        string creatorId = dBUtils.InsertCreator(eventObj.Creator);
        string organizerId = dBUtils.InsertOrganizer(eventObj.Organizer);
        eventObj.StartId = startId;
        eventObj.EndId = endId;
        eventObj.CreatorId = creatorId;
        eventObj.OrganizerId = organizerId;

        string createdId = dBUtils.InsertEvent(eventObj);
        Console.WriteLine($"Created event `{createdId}` successfully");
    }

    static void UpdateEvent(string id, string summary, int? startId = null, string start = "", int? endId = null, string end = "")
    {
        Event eventObj = new Event()
        {
            Id = id,
            Summary = summary,
            StartId = startId,
            EndId = endId,
            Start = start,
            End = end
        };
        if (eventObj.StartId != null)
        {
            dBUtils.UpdateEventDateTime(eventObj.StartId, eventObj.Start);
        }
        if (eventObj.EndId != null)
        {
            dBUtils.UpdateEventDateTime(eventObj.EndId, eventObj.End);
        }

        if (dBUtils.UpdateEvent(eventObj) > 0)
        {
            Console.WriteLine("Update successfully");
        }

    }

    int UpdateStartAndEndTime(string start, string end)
    {
        return 1;
    }

    void createCreator(string creator)
    {

    }


    static string GenerateRandomId(int length = 20)
    {
        Random _random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder result = new StringBuilder(length);
        result.Append("khanta");
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[_random.Next(chars.Length)]);
        }

        return result.ToString();
    }

    public static string GenerateCustomUuid(int length = 26)
    {
        char[] Base32HexChars = "0123456789abcdefghijklmnopqrstuv".ToCharArray();
        if (length < 5 || length > 1024)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be between 5 and 1024 characters.");
        }

        // Calculate how many bytes we need (5 bits per base32hex character)
        int numBits = length * 5; // Each base32hex character represents 5 bits
        int numBytes = (numBits + 7) / 8; // Convert bits to bytes, rounding up

        // Generate random bytes
        byte[] randomBytes = new byte[numBytes];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
        }

        // Convert the bytes to base32hex string
        StringBuilder uuidBuilder = new StringBuilder(length);

        // Process each byte and convert it to base32hex characters
        int bitBuffer = 0;
        int bitsInBuffer = 0;

        foreach (byte b in randomBytes)
        {
            bitBuffer = (bitBuffer << 8) | b;
            bitsInBuffer += 8;

            while (bitsInBuffer >= 5)
            {
                bitsInBuffer -= 5;
                int index = (bitBuffer >> bitsInBuffer) & 31; // Extract 5 bits at a time
                uuidBuilder.Append(Base32HexChars[index]);
            }
        }

        // If there are remaining bits, pad them with zeros
        if (bitsInBuffer > 0)
        {
            int index = (bitBuffer << (5 - bitsInBuffer)) & 31;
            uuidBuilder.Append(Base32HexChars[index]);
        }

        // Trim the result to the desired length
        return uuidBuilder.ToString().Substring(0, length);
    }

}