using static Event;
using static ChunkGenerateEvent;
using System;
using UnityEngine;

public class EventProducer {
    private EventRepository eventRepository;
    private bool useKafka = false;

    public EventProducer(EventRepository eventRepository) {
        this.eventRepository = eventRepository;
    }

    public void produceChunkGenerateEvent(int chunkX, int chunkZ) {
        ChunkGenerateEvent chunkGenerateEvent = new ChunkGenerateEvent(chunkX, chunkZ);
        if (useKafka) {
            // TODO: send event to Kafka
        }
        else {
            eventRepository.addEvent(chunkGenerateEvent);
            Debug.Log("Produced event: " + chunkGenerateEvent);
        }
    }

    public void producePlayerFallingIntoVoidEvent(Vector3 position) {
        PlayerFallingIntoVoidEvent playerFallingIntoVoidEvent = new PlayerFallingIntoVoidEvent(position);
        if (useKafka) {
            // TODO: send event to Kafka
        }
        else {
            eventRepository.addEvent(playerFallingIntoVoidEvent);
            Debug.Log("Produced event: " + playerFallingIntoVoidEvent);
        }
    }
}