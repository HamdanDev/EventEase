using System;
using System.Collections.Generic;
using System.Linq;

namespace EventEaseApp.Services
{
    public class EventService
    {
        private readonly List<Event> _events = new()
        {
            new Event { Id = 1, Name = "Corporate Gala", Date = new DateTime(2025, 12, 1), Location = "New York City" },
            new Event { Id = 2, Name = "Tech Conference", Date = new DateTime(2025, 11, 15), Location = "San Francisco" },
            new Event { Id = 3, Name = "Music Festival", Date = new DateTime(2025, 10, 20), Location = "Los Angeles" },
            new Event { Id = 4, Name = "DJ Festival", Date = new DateTime(2025, 10, 20), Location = "Paris" },
            new Event { Id = 5, Name = "Fête de la Musique", Date = new DateTime(2025, 10, 20), Location = "Nice" },
            new Event { Id = 6, Name = "Festival of Arts", Date = new DateTime(2025, 10, 20), Location = "California" },
            new Event { Id = 7, Name = "Music Festival", Date = new DateTime(2025, 10, 20), Location = "Berlin" },
            new Event { Id = 8, Name = "Music Festival", Date = new DateTime(2025, 10, 20), Location = "Tokyo" },
            new Event { Id = 9, Name = "Music Festival", Date = new DateTime(2025, 10, 20), Location = "Osaka" }
        };

        public List<Event> GetAllEvents()
        {
            return _events;
        }

        public Event? GetEventById(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }
    }

    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}