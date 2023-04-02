using System;
using UnityEngine;

namespace osg {

    public class EventProducer {
        private EventRepository eventRepository;

        public EventProducer(EventRepository eventRepository) {
            this.eventRepository = eventRepository;
        }

        public void produceChunkGenerateEvent(int chunkX, int chunkZ) {
            ChunkGenerateEvent chunkGenerateEvent = new ChunkGenerateEvent(chunkX, chunkZ);
            eventRepository.addEvent(chunkGenerateEvent);
            Debug.Log("Produced event: " + chunkGenerateEvent);
        }

        public void producePlayerFallingIntoVoidEvent(Vector3 position) {
            PlayerFallingIntoVoidEvent playerFallingIntoVoidEvent = new PlayerFallingIntoVoidEvent(position);
            eventRepository.addEvent(playerFallingIntoVoidEvent);
            Debug.Log("Produced event: " + playerFallingIntoVoidEvent);
        }

        public void produceNationCreationEvent(Nation nation) {
            NationCreationEvent nationCreationEvent = new NationCreationEvent(nation);
            eventRepository.addEvent(nationCreationEvent);
            Debug.Log("Produced event: " + nationCreationEvent);
        }
    }
}