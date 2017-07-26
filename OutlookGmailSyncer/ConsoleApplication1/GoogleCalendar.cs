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
    public class GoogleCalendar
    {
        private readonly UploadToGoogleOptions _options;

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = {CalendarService.Scope.Calendar};
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        public GoogleCalendar(UploadToGoogleOptions options)
        {
            _options = options;
        }

        public void RemoveEventsFromWorkCalendar()
        {
            CalendarService service = GetService();

            // get work calendar
            var workCalendar = GetCalendar(service);
            
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
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public void AddEventsToWorkCalendar(string icalPath)
        {
            IICalendarCollection calendars = Ical.Net.Calendar.LoadFromFile(icalPath);
            List<Event> eventsToCreate = GetEventsFromIcal(calendars);
            UploadEvents(eventsToCreate);

        }

        private void UploadEvents(List<Event> eventsToCreate)
        {
            var googleService = GetService();
            var calendar = GetCalendar(googleService);
            var tasks = new List<Task>();
            foreach (var googleEvent in eventsToCreate)
            {
                tasks.Add(googleService.Events.Import(googleEvent, calendar.Id).ExecuteAsync());
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                // ignored
            }

        }

        private List<Event> GetEventsFromIcal(IICalendarCollection calendars)
        {
            var eventsToCreate = new List<Event>();
            foreach (IEvent eventItem in calendars.First().Events)
            {
                Event googleEvent = new Event();
                googleEvent.ICalUID = eventItem.Uid;
                googleEvent.Organizer = new Event.OrganizerData
                {
                    DisplayName = eventItem.Organizer.CommonName,
                };
                googleEvent.Attendees = new List<EventAttendee>();
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

                googleEvent.Description = eventItem.Description;

                eventsToCreate.Add(googleEvent);
            }

            return eventsToCreate;
        }

        private UserCredential GetUserCredentials()
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


        private CalendarListEntry GetCalendar(CalendarService service)
        {
            var request = service.CalendarList.List();
            var calendars = request.Execute();
            var workCalendar = calendars.Items.Single(x => x.Summary == _options.Calendar);
            return workCalendar;

        }

        private CalendarService GetService()
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
    }
}
