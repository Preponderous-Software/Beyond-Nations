class GameConfig {
    private int chunkSize;
    private int locationScale;
    private int updateInterval;

    public GameConfig() {
        chunkSize = 9;
        locationScale = 9;
        updateInterval = 10;
    }

    public int getChunkSize() {
        return chunkSize;
    }

    public int getLocationScale() {
        return locationScale;
    }

    public int getUpdateInterval() {
        return updateInterval;
    }
}