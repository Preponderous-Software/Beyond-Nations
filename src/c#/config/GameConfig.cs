class GameConfig {
    private int chunkSize;
    private int locationScale;
    private int updateInterval;
    private int statusExpirationTicks;

    public GameConfig() {
        chunkSize = 9;
        locationScale = 9;
        updateInterval = 10;
        statusExpirationTicks = 500;
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

    public int getStatusExpirationTicks() {
        return statusExpirationTicks;
    }
}