using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using iCal.PCL;

namespace Syncer
{
    public static class GoogleCalendar
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = {CalendarService.Scope.Calendar};
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        

        public static void RemoveEventsFromWorkCalendar()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


            var request = service.CalendarList.List();

            // get work calendar
            var calendars = request.Execute();
            var workCalendar = calendars.Items.Single(x => x.Summary == "Work");

            // Define parameters of request.
            EventsResource.ListRequest eventRequest = service.Events.List(workCalendar.Id);
            eventRequest.ShowDeleted = false;

            var events = eventRequest.Execute();

            var tasks = new List<Task>();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    tasks.Add(service.Events.Delete(workCalendar.Id, eventItem.Id).ExecuteAsync());
                    Console.WriteLine("{0}", eventItem.Summary);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }

            Task.WaitAll(tasks.ToArray());
        }

        public static void AddEventsToWorkCalendar(string icalPath)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var events = GetIcalEvents(icalPath);

            var request = service.CalendarList.List();

            // get work calendar
            var calendars = request.Execute();
            var workCalendar = calendars.Items.Single(x => x.Summary == "Work");

            // Define parameters of request.
            EventsResource.ListRequest eventRequest = service.Events.List(workCalendar.Id);
            eventRequest.ShowDeleted = false;

            var events = eventRequest.Execute();

            var tasks = new List<Task>();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    service.Events.Import(new Event {}, workCalendar.Id)
                    tasks.Add(service.Events.Delete(workCalendar.Id, eventItem.Id).ExecuteAsync());
                    Console.WriteLine("{0}", eventItem.Summary);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static object GetIcalEvents(string icalPath)
        {
            iCal.PCL.Serialization.iCalRawSerializer.Deserialize()
        }
    }
}
