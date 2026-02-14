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
        Stats.PrintFullStats();
    }
}

public static class Stats
{
    public static void PrintMinimalStats()
    {
        var fileData = Main.ActivePlayerFileData;
        if (fileData == null)
        {
            Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.NoActivePlayer"), Color.Red);
            return;
        }

        Player p = Main.LocalPlayer;

        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Header", p.name, FormatPlayTime(fileData.GetPlayTime())));
    }

    public static void PrintFullStats()
    {
        var fileData = Main.ActivePlayerFileData;
        if (fileData == null)
        {
            Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.NoActivePlayer"), Color.Red);
            return;
        }

        Player p = Main.LocalPlayer;

        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Header", p.name, FormatPlayTime(fileData.GetPlayTime())));
        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.MaxStats", p.statLifeMax, p.statManaMax, p.statDefense));
        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Inventory", GetInventoryUsage(p), GetAccessoryUsage(p)));
        Print(Language.GetTextValue("Mods.ErkySSC.Commands.SSCStats.Deaths", p.numberOfDeathsPVE, p.numberOfDeathsPVP));
    }

    #region Helpers
    public static string FormatPlayTime(TimeSpan t)
    {
        int hours = (int)t.TotalHours;
        return $"{hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
    }

    private static void Print(string text)
    {
        Main.NewText(text, Color.LightGreen);
    }

    private static string GetInventoryUsage(Player p)
    {
        int usedInventoryItems = p.inventory.Count(i => !i.IsAir);

        return usedInventoryItems.ToString() + "/" + 50;
    }

    private static string GetAccessoryUsage(Player p)
    {
        int max = p.extraAccessory ? 7 : 6;
        int used = Enumerable.Range(3, max).Count(i => !p.armor[i].IsAir);
        return $"{used}/{max}";
    }
    #endregion
}
