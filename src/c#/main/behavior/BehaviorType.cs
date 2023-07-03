namespace osg {

    public enum BehaviorType {
        NONE,

        // environment modification
        GATHER_RESOURCES,
        PLANT_SAPLING,

        // movement
        WANDER,
        GO_TO_HOME_SETTLEMENT,
        EXIT_SETTLEMENT,

        // trading
        SELL_RESOURCES, // TODO: reimplement
        PURCHASE_FOOD, // TODO: reimplement
        PURCHASE_STALL, // TODO: implement
        TRANSFER_ITEMS_TO_STALL, // TODO: implement
        COLLECT_PROFIT_FROM_STALL, // TODO: implement

        // construction
        CONSTRUCT_SETTLEMENT,        
        CONSTRUCT_STALL,
    }
}