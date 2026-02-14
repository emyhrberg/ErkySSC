using ErkySSC.Core.Configs;
using ErkySSC.Core.Debug;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static ErkySSC.ErkySSC;

namespace ErkySSC.Common.SSC;

/// <summary>
/// Ensures that data is handled by the server rather than saved locally.
/// Intercepts player file save events to redirect saving to the server.
[Autoload(Side = ModSide.Client)]
internal class SSCSaveSystem : ModSystem
{
    public override void Load()
    {
        On_Player.InternalSavePlayerFile += OverrideSavePlayerFile;
    }

    public override void Unload()
    {
        On_Player.InternalSavePlayerFile -= OverrideSavePlayerFile;
    }

    public override void PreSaveAndQuit()
    {
        if (!SSC.IsEnabled)
            return;

        // Save player file before quitting
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendPacketToSavePlayerFile();
        }
    }

    // Do not save SSC player files locally; send to server instead.
    private void OverrideSavePlayerFile(On_Player.orig_InternalSavePlayerFile orig, PlayerFileData fileData)
    {
        if (!SSC.IsEnabled)
        {
            orig(fileData);
            return;
        }

        if (Main.netMode == NetmodeID.MultiplayerClient &&
            fileData.ServerSideCharacter && fileData.Path.EndsWith("SSC"))
        {
            SendPacketToSavePlayerFile();
            return;
        }

        orig(fileData);
    }

    public void SendPacketToSavePlayerFile()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
            return;

        try
        {
            var steamID = SteamUser.GetSteamID().m_SteamID.ToString();
            var fileData = Main.ActivePlayerFileData;
            var name = fileData.Player.name;
            var plr = Player.SavePlayerFile_Vanilla(fileData);
            var tplr = PlayerIO.SaveData(fileData.Player);

            // Add player position to tplr
            var sscTag = new TagCompound();

            // Save player position for this world
            PlayerPositionSystem.SavePlayerPosition(fileData.Player, sscTag);
            tplr["PPP"] = sscTag;

            // Send packet
            var packet = Mod.GetPacket();
            packet.Write((byte)PacketType.SavePlayer);
            packet.Write(steamID);
            packet.Write(name);
            packet.Write(plr.Length);
            packet.Write(plr);
            TagIO.Write(tplr, packet);
            packet.Send();

            Log.Chat("Client sent packet to save " + fileData.Player.name);

            var config = ModContent.GetInstance<ClientConfig>();
            if (config.ShowPlayerSaveMessage)
            {
                Main.NewText($"Saved {name} at {DateTime.Now:HH:mm:ss}",Color.LightSeaGreen);
            }
        }
        catch (Exception e)
        {
            Mod.Logger.Error(e);
            Log.Chat(e);
        }

    }
}
