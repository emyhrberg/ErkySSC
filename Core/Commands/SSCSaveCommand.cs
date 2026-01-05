using ErkySSC.Core.SSC;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ErkySSC.Core.Commands;

internal class SSCSaveCommand : ModCommand
{
    public override string Command => "save";
    public override string Description => "Manually saves your SSC.";
    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!SSCEnabled.IsEnabled)
        {
            Main.NewText("SSC is not enabled on this server.", Color.Red);
            return;
        }

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            Main.NewText("This command can only be used in multiplayer.", Color.Red);
            return;
        }

        ModContent.GetInstance<SSCSaveSystem>().SendPacketToSavePlayerFile();
    }
}
