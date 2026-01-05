using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ErkySSC.Core;

/// <summary>
/// Prints a chat message when a boss summoning item is used.
/// </summary>
internal class BossSummonChat : GlobalItem
{
    public override void OnConsumeItem(Item item, Player player)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;

        // Only run on server
        //if (Main.netMode != NetmodeID.Server)
            //return;

        // Must be consumable
        if (!item.consumable)
            return;

        // Filter boss summoning items
        if (!IsBossSummon(item))
            return;

        string itemName = item.Name;
        string itemTag = GetItemTag(item);
        string playerName = player.name;

        ChatHelper.BroadcastChatMessage(
            NetworkText.FromLiteral($"{playerName} used {itemTag}!"),
            Color.OrangeRed
        );
    }

    private static string GetItemTag(Item item)
    {
        // Vanilla
        if (item.ModItem == null)
        {
            return $"[i:{item.type}]";
        }

        // Modded
        return $"[i:{item.ModItem.Mod.Name}/{item.ModItem.Name}]";
    }

    private static bool IsBossSummon(Item item)
    {
        // Vanilla
        if (IsVanillaBossSummon(item))
            return true;

        // Calamity
        if (IsCalamityBossSummon(item))
            return true;

        // Generic modded fallback
        if (IsGenericBossSummon(item))
            return true;

        return false;
    }

    private static bool IsVanillaBossSummon(Item item)
    {
        // Vanilla boss summons usually have a shoot NPC or useStyle with boss logic
        return item.type switch
        {
            ItemID.SlimeCrown or
            ItemID.SuspiciousLookingEye or
            ItemID.WormFood or
            ItemID.BloodySpine or
            ItemID.Abeemination or
            ItemID.ClothierVoodooDoll or
            ItemID.MechanicalEye or
            ItemID.MechanicalWorm or
            ItemID.MechanicalSkull or
            ItemID.LihzahrdPowerCell or
            ItemID.CelestialSigil
                => true,

            _ => false
        };
    }

    private static bool IsGenericBossSummon(Item item)
    {
        return item.consumable &&
               item.shoot > 0 &&
               ContentSamples.NpcsByNetId.TryGetValue(item.shoot, out var npc) &&
               npc.boss;
    }

    private static bool IsCalamityBossSummon(Item item)
    {
        if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            return false;

        return item.type == calamity.Find<ModItem>("Abombination")?.Type
            || item.type == calamity.Find<ModItem>("AstralChunk")?.Type
            || item.type == calamity.Find<ModItem>("BloodyWormFood")?.Type
            || item.type == calamity.Find<ModItem>("CosmicWorm")?.Type
            || item.type == calamity.Find<ModItem>("DecapoditaSprout")?.Type
            || item.type == calamity.Find<ModItem>("DesertMedallion")?.Type
            || item.type == calamity.Find<ModItem>("EyeofDesolation")?.Type
            || item.type == calamity.Find<ModItem>("RuneofKos")?.Type
            || item.type == calamity.Find<ModItem>("Seafood")?.Type;
    }
}
