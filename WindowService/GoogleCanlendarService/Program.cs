using System.ServiceProcess;

namespace GoogleCanlendarService
{
    internal static class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            // Run the service as a console app in debug mode
            InsertEventService service = new InsertEventService();
            service.Start();  // Start the service in debug mode
            Console.WriteLine("Service is running... Press any key to stop.");
            Console.ReadKey();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new InsertEventService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
