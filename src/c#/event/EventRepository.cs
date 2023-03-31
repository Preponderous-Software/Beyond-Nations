using static Event;
using System.Collections.Generic;

/**
 * This is a temporary repository for events until we get Kafka up and running.
 */
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