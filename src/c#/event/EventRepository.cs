using static Event;
using System.Collections.Generic;

public class EventRepository {
    private List<Event> events;

    public EventRepository() {
        events = new List<Event>();
    }

    public void addEvent(Event e) {
        events.Add(e);
    }

    public List<Event> getEvents() {
        return events;
    }
}