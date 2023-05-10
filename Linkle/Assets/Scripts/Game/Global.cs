using Game.Config;
using Game.Define;

public static class Global
{
    public const int GamingTimerGroupId = 1000;

    public static GameSettingsConfig GameSettingsConfig;

    public static int GetItemPrice(ItemType itemType)
    {
        return 10;
    }
}
