using ErkySSC.Common.SSC;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ErkySSC.Common.Commands;

public class SSCStatsCommand : ModCommand
{
    public override string Command => "stats";
    public override string Description => Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Description");
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
            Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.NoActivePlayer"), Color.Red);
            return;
        }

        Player p = Main.LocalPlayer;

        Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Header", p.name), Color.MediumPurple);
        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Difficulty", GetDifficultyName(p)));
        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Playtime", SSC.SSC.FormatPlayTime(fileData.GetPlayTime())));
        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.MaxStats", p.statLifeMax, p.statManaMax, p.statDefense));
        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Inventory", p.inventory.Count(i => !i.IsAir), GetAccessoryUsage(p)));
        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Deaths", p.numberOfDeathsPVE, p.numberOfDeathsPVP));
    }

    private static void Print(string text)
        => Main.NewText($"• {text}", Color.LightGray);

    private static string GetAccessoryUsage(Player p)
    {
        int max = p.extraAccessory ? 7 : 6;
        int used = Enumerable.Range(3, max).Count(i => !p.armor[i].IsAir);
        return $"{used} / {max}";
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
