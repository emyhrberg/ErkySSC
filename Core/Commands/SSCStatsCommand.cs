using ErkySSC.Core.SSC;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace ErkySSC.Core.Commands;

public class SSCStatsCommand : ModCommand
{
    public override string Command => "stats";

    public override string Description => "Shows your SSC stats for this world.";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        Stats.PrintStats();
    }
    
}

public static class Stats
{
    public static void PrintStats()
    {
        var fileData = Main.ActivePlayerFileData;
        if (fileData == null)
        {
            Main.NewText("No active player data.", Color.Red);
            return;
        }

        Player p = Main.LocalPlayer;

        // Headers
        Main.NewText($"Character Stats — {p.name}", Color.MediumPurple);
        
        // All stats
        Print($"Difficulty: {GetDifficultyName(p)}");
        Print($"Playtime: {SSCMainSystem.FormatPlayTime(fileData.GetPlayTime())}");
        Print($"Max Health: {p.statLifeMax} | Max Mana: {p.statManaMax} | Defense: {p.statDefense}");
        Print($"Inventory: {p.inventory.Count(i => !i.IsAir)}/50 | Accessories: {GetAccessoryUsage(p)}");
        Print($"PvE Deaths: {p.numberOfDeathsPVE} | PvP Deaths: {p.numberOfDeathsPVP}");
        //PrintBank("Piggy Bank", p.bank);
        //PrintBank("Safe", p.bank2);
        //PrintBank("Defender's Forge", p.bank3);
        //PrintBank("Void Vault", p.bank4);
        //Print($"Coins: {FormatCoins(p)}");
        //Print($"Movement Speed: {p.moveSpeed * 100f:0.#}%");
        //Print($"Max Jump Height: ~{GetJumpTiles(p):0.#} tiles");
        //PrintFlightTime(p);
        //Print($"Last Saved: {GetLastSaveTime(p)}");
        //Print(GetVanillaBossSummary());
    }

    private static void Print(string text)
        => Main.NewText($"• {text}", Color.LightGray);

    private static void PrintBank(string name, Chest bank)
    {
        int count = bank?.item.Count(i => !i.IsAir) ?? 0;
        if (count > 0)
            Print($"{name}: {count} / {bank.item.Length}");
    }

    private static string GetAccessoryUsage(Player p)
    {
        int max = p.extraAccessory ? 7 : 6;
        int used = Enumerable.Range(3, max).Count(i => !p.armor[i].IsAir);
        return $"{used} / {max}";
    }

    private static float GetJumpTiles(Player p)
        => (Player.jumpSpeed + p.jumpSpeedBoost) * 0.75f;

    private static void PrintFlightTime(Player p)
    {
        if (p.wingTimeMax > 0)
            Print($"Flight Time: {p.wingTimeMax / 60f:0.#} seconds");
    }

    private static string GetLastSaveTime(Player p)
    {
        if (p.lastTimePlayerWasSaved <= 0)
            return "Never";

        return DateTime
            .FromBinary(p.lastTimePlayerWasSaved)
            .ToLocalTime()
            .ToString("yyyy-MM-dd HH:mm:ss");
    }

    private static string GetVanillaBossSummary()
    {
        List<string> bosses = [];

        if (NPC.downedSlimeKing) bosses.Add("SlimeKing");
        if (NPC.downedBoss1) bosses.Add("EyeOfCthulhu");
        if (NPC.downedBoss2) bosses.Add("Eater/Brain");
        if (NPC.downedBoss3) bosses.Add("Skeletron");
        if (NPC.downedQueenBee) bosses.Add("QueenBee");
        if (NPC.downedMechBoss1) bosses.Add("MechBoss1");
        if (NPC.downedMechBoss2) bosses.Add("MechBoss2");
        if (NPC.downedMechBoss3) bosses.Add("MechBoss3");
        if (NPC.downedPlantBoss) bosses.Add("Plantera");
        if (NPC.downedGolemBoss) bosses.Add("Golem");
        if (NPC.downedMoonlord) bosses.Add("MoonLord");

        int count = bosses.Count;
        if (count == 0)
            return "Bosses Defeated: 0";

        string list = string.Join(
            ", ",
            bosses.Take(7)
        );

        if (count > 7)
            list += ", ...";

        return $"Bosses Defeated: {count} ({list})";
    }

    private static string FormatCoins(Player player)
    {
        long copper =
            player.inventory.Sum(i => i.type == ItemID.CopperCoin ? i.stack : 0) +
            player.bank.item.Sum(i => i.type == ItemID.CopperCoin ? i.stack : 0) +
            player.bank2.item.Sum(i => i.type == ItemID.CopperCoin ? i.stack : 0) +
            player.bank3.item.Sum(i => i.type == ItemID.CopperCoin ? i.stack : 0) +
            player.bank4.item.Sum(i => i.type == ItemID.CopperCoin ? i.stack : 0);

        long silver = copper / 100;
        copper %= 100;

        long gold = silver / 100;
        silver %= 100;

        long platinum = gold / 100;
        gold %= 100;

        List<string> parts = [];

        if (platinum > 0) parts.Add($"{platinum} platinum");
        if (gold > 0) parts.Add($"{gold} gold");
        if (silver > 0) parts.Add($"{silver} silver");
        if (copper > 0) parts.Add($"{copper} copper");

        return parts.Count > 0 ? string.Join(", ", parts) : "0 coins";
    }

    private static string GetDifficultyName(Player player)
    {
        return player.difficulty switch
        {
            PlayerDifficultyID.SoftCore => "Softcore",
            PlayerDifficultyID.MediumCore => "Mediumcore",
            PlayerDifficultyID.Hardcore => "Hardcore",
            PlayerDifficultyID.Creative => "Journey",
            _ => "Unknown"
        };
    }
}
