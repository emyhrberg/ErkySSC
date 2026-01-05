using Steamworks;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace ErkySSC.Core.SSC;

/// <summary>
/// Joins the world as a ghost.
/// A a small delay sends a request to join as a proper SSC character.
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

        if (!SSCEnabled.IsEnabled)
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
        SSCMainSystem.SendJoinRequest();
    }

    public override void OnWorldUnload()
    {
        _sent = false;
        _delayTicks = 0;
    }
}
