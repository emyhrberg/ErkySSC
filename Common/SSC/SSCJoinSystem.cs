using Steamworks;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace ErkySSC.Common.SSC;

/// <summary>
/// Joins the world as a ghost.
/// Adds a small delay after joining, to allow some time for systems to initialize,
/// before sending a request to join as a proper SSC character.
/// TODO: Hopefully reworked in the future for smoother player experience.
/// </summary>
[Autoload(Side = ModSide.Client)]
public class SSCJoinSystem : ModSystem
{
    private bool _sent;
    private int _delayTicks;
    public override void OnWorldLoad()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
            return;

        if (!SSC.IsEnabled)
            return;

        _sent = false;
        _delayTicks = 60; // 1 second

        // Enter as a ghost
        Main.LocalPlayer.ghost = true;
    }

    public override void PostUpdateEverything()
    {
        if (_sent)
            return;

        if (Main.netMode != NetmodeID.MultiplayerClient)
            return;

        if (_delayTicks > 0)
        {
            _delayTicks--;
            return;
        }

        _sent = true;
        SSC.SendJoinRequest();
    }

    public override void OnWorldUnload()
    {
        _sent = false;
        _delayTicks = 0;
    }
}
