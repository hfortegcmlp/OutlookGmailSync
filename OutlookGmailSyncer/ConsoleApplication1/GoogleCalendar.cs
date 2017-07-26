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
using Ical.Net.Interfaces;
using Ical.Net.Interfaces.Components;

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
            CalendarService service = GetService();

            var workCalendar = GetWorkCalendar(service);
            // get work calendar

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

        private static CalendarListEntry GetWorkCalendar(CalendarService service)
        {
            var request = service.CalendarList.List();
            var calendars = request.Execute();
            var workCalendar = calendars.Items.Single(x => x.Summary == "Work");
            return workCalendar;

        }

        private static CalendarService GetService()
        {
            UserCredential credential = GetUserCredentials();

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        private static UserCredential GetUserCredentials()
        {
            UserCredential credential = GetUserCredentials();

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

            return credential;
        }

        public static void AddEventsToWorkCalendar(string icalPath)
        {
            IICalendarCollection calendars = Ical.Net.Calendar.LoadFromFile(icalPath);
            foreach (IEvent eventItem in calendars.First().Events)
            {
                Event googleEvent = new Event();
                googleEvent.ICalUID = eventItem.Uid;
                googleEvent.Organizer = new Event.OrganizerData
                {
                    DisplayName = eventItem.Organizer.CommonName,
                };
                foreach (var attendee in eventItem.Attendees)
                {
                    googleEvent.Attendees.Add(new EventAttendee
                    {
                        DisplayName = attendee.CommonName
                    });
                }
                googleEvent.Start = new EventDateTime
                {
                    DateTime = eventItem.Start.Date,
                    TimeZone = eventItem.Start.TimeZoneName
                };

                googleEvent.End = new EventDateTime
                {
                    DateTime = eventItem.End.Date,
                    TimeZone = eventItem.End.TimeZoneName
                };
            }
        }
    }
}
