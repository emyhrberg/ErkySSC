using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace ErkySSC.Core.Configs;

public class ClientConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("ClientOptions")]
    [BackgroundColor(30, 40, 110)]
    [DefaultValue(true)]
    public bool ShowWhoSummonedBossMessage;

    [BackgroundColor(30, 40, 110)]
    [DefaultValue(true)]
    public bool ShowStatsMessageWhenEnteringWorld;

    [BackgroundColor(30, 40, 110)]
    [DefaultValue(true)]
    public bool ShowPlayerSaveMessage;
}
