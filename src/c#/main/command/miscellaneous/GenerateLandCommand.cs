using UnityEngine;

namespace osg {

    public class GenerateLandCommand {
        private Environment environment;
        private WorldGenerator worldGenerator;

        public GenerateLandCommand(Environment environment, WorldGenerator worldGenerator) {
            this.environment = environment;
            this.worldGenerator = worldGenerator;
        }

        public void execute(Player player) {
            Vector3 playerPosition = player.getGameObject().transform.position;
            Chunk chunk = environment.getChunkAtPosition(playerPosition);
            if (chunk != null) {
                Vector3 chunkPosition = chunk.getGameObject().transform.position;
                int numChunksGenerated = 0;
                for (int x = -50; x < 51; x++) {
                    for (int z = -50; z < 51; z++) {
                        int chunkSize = environment.getChunkSize();
                        Vector3 position = new Vector3(chunkPosition.x + x * chunkSize, 0, chunkPosition.z + z * chunkSize);
                        bool generatedNewChunk = worldGenerator.generateChunkAtPosition(position);
                        if (generatedNewChunk) {
                            numChunksGenerated++;
                        }
                    }
                }
                player.getStatus().update("Generated " + numChunksGenerated + " chunks.");
            }
        }
    }
}