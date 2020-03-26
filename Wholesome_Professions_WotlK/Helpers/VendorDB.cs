using robotManager.Helpful;
using wManager.Wow.Class;
using wManager.Wow.Enums;

public static class VendorDB
{
    // ***************** ENCHANTING *****************
    public static Npc OGEnchantingSupplies = new Npc()
    {
        Entry = 3346,
        Position = new Vector3(1918.26, -4436.26, 24.77395),
        ContinentId = ContinentId.Kalimdor,
        GossipOption = 1
    };
    public static Npc OGEnchantingTrainer = new Npc()
    {
        Entry = 3345,
        Position = new Vector3(1912.18, -4436.61, 24.79994),
        ContinentId = ContinentId.Kalimdor,
        GossipOption = 1
    };

    // ***************** TAILORING *****************
    public static Npc OGTailoringSupplies = new Npc()
    {
        Entry = 3364,
        Position = new Vector3(1792.65, -4565.39, 23.0066),
        ContinentId = ContinentId.Kalimdor,
        GossipOption = 1
    };
    public static Npc OGTailoringTrainer = new Npc()
    {
        Entry = 3363,
        Position = new Vector3(1806.85, -4573.32, 23.00661),
        ContinentId = ContinentId.Kalimdor,
        GossipOption = 1
    };
    public static Npc ThrallmarTailoringTrainer = new Npc()
    {
        Entry = 18749,
        Position = new Vector3(204.177, 2617.73, 87.2837),
        ContinentId = ContinentId.Expansion01,
        GossipOption = 1
    };
    public static Npc ThrallmarTailoringSupplies = new Npc()
    {
        Entry = 18749,
        Position = new Vector3(204.177, 2617.73, 87.2837),
        ContinentId = ContinentId.Expansion01,
        GossipOption = 2
    };
    public static Npc ShattrathTailoringSupplies = new Npc()
    {
        Entry = 19213,
        Position = new Vector3(-2077.26, 5270.03, -37.3236),
        ContinentId = ContinentId.Expansion01,
        GossipOption = 1
    };
    public static Npc WarsongHoldTailoringTrainer = new Npc()
    {
        Entry = 26969,
        Position = new Vector3(2842.94, 6170.93, 104.8442),
        ContinentId = ContinentId.Northrend,
        GossipOption = 1
    };
    public static Npc WarsongHoldTailoringSupplies = new Npc()
    {
        Entry = 26941,
        Position = new Vector3(2799.13, 6187.77, 104.9765),
        ContinentId = ContinentId.Northrend,
        GossipOption = 1
    };

    static VendorDB()
    {
    }
}
