using static EventType;
using System;

public abstract class Event {
    private EventType type;
    private string description;
    private DateTime date;

    public Event(EventType type, string description) {
        this.type = type;
        this.description = description;
        this.date = DateTime.Now;
    }

    public EventType Type {
        get { return type; }
        set { type = value; }
    }

    public string Description {
        get { return description; }
        set { description = value; }
    }

    public DateTime Date {
        get { return date; }
        set { date = value; }
    }
}