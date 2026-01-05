using ErkySSC.Common;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ErkySSC.Core;

internal class BannedItemsPlayer : ModPlayer
{
    public static bool IsItemBanned(Item item)
    {
        if (item == null || item.IsAir)
            return false;

        return ModContent.GetInstance<ServerConfig>()
            .BannedItems
            .Contains(new ItemDefinition(item.type));
    }

    public override void PostUpdate()
    {
        var banned = ModContent.GetInstance<ServerConfig>().BannedItems;

        for (int i = 0; i < Player.inventory.Length; i++)
        {
            Item item = Player.inventory[i];
            if (item.IsAir)
                continue;

            if (banned.Contains(new ItemDefinition(item.type)))
            {
                Player.inventory[i].TurnToAir();
            }
        }
    }
}

internal class BannedItemsGlobalItem : GlobalItem
{
    public override bool CanPickup(Item item, Player player)
    {
        return !BannedItemsPlayer.IsItemBanned(item);
    }

}



