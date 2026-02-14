using ErkySSC.Core.Configs.ConfigElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace ErkySSC.Core.Configs;

public class ServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public enum PlayerDifficultyMode
    {
        Softcore,
        Mediumcore,
        Hardcore,
        Journey
    }

    public enum PlayerNameMode
    {
        Default,
        Steam,
        Discord,
        Numbered,
        Random
    }

    [Header("Options")]
    [BackgroundColor(60, 60, 150)]
    [DefaultValue(true)]
    public bool IsSSCEnabled;

    [Header("StartOptions")]
    [BackgroundColor(90, 40, 110)]
    [Expand(false, false)]
    [CustomModConfigItem(typeof(CustomDictionaryElement))]
    public Dictionary<ItemDefinition, int> StartItems = new()
    {
        { new ItemDefinition(ItemID.CopperShortsword), 1 },
        { new ItemDefinition(ItemID.CopperPickaxe), 1 },
        { new ItemDefinition(ItemID.CopperAxe), 1 }
    };

    [BackgroundColor(90, 40, 110)]
    [Slider]
    [Increment(20)]
    [Range(100, 500)]
    [DefaultValue(100)]
    public int StartLife = 100;

    [BackgroundColor(90, 40, 110)]
    [Slider]
    [Increment(20)]
    [Range(20, 200)]
    [DefaultValue(20)]
    public int StartMana = 20;
    
    //[BackgroundColor(90, 40, 110)]
    //[DefaultValue(PlayerDifficultyMode.Softcore)]
    //public PlayerDifficultyMode StartDifficulty = PlayerDifficultyMode.Softcore;

    //[BackgroundColor(90, 40, 110)]
    //[DefaultValue(PlayerNameMode.Default)]
    //public PlayerNameMode PlayerStartName = PlayerNameMode.Default;

    [Header("ClientSidedMods")]
    [BackgroundColor(30, 100, 40, 220)]
    [DefaultValue(false)]
    public bool AllowClientMods;

    [BackgroundColor(30, 100, 40, 220)]
    [Expand(false)]
    public List<string> AllowedClientMods = [];

    [Header("BannedItems")]
    [BackgroundColor(210, 30, 30, 220)]
    [Expand(false)]
    public List<ItemDefinition> BannedItems = [];
}
