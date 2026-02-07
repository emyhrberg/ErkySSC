using ErkySSC.Common.SSC;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ErkySSC.Common.Commands;

internal class SSCSaveCommand : ModCommand
{
    public override string Command => "save";
    public override string Description => Language.GetTextValue("Mods.ErkySSC.Commands.SSCSave.Description");
    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!SSC.SSC.IsEnabled)
        {
            Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCSave.NotEnabled"), Color.Red);
            return;
        }

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCSave.MultiplayerOnly"), Color.Red);
            return;
        }

        ModContent.GetInstance<SaveSystem>().SendPacketToSavePlayerFile();
        Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCSave.Success"), Color.LightGreen);
    }
}
