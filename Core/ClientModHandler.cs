using ErkySSC.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static ErkySSC.ErkySSC;

namespace ErkySSC.Core;

/// <summary>
/// Checks for client mods when joining.
/// </summary>
internal class ClientModHandler
{
    public static void HandlePacket(BinaryReader reader, int from)
    {
        List<string> allowedClientMods = [];

        allowedClientMods.AddRange(
            from mod in ModLoader.Mods
            where mod.Side == ModSide.Client
            select mod.Name
        );

        allowedClientMods.AddRange(
            ModContent.GetInstance<ServerConfig>().AllowedClientMods
        );

        List<string> unallowedClientMods = [];

        int num = reader.ReadInt32();
        for (int i = 0; i < num; i++)
        {
            string name = reader.ReadString();
            if (!allowedClientMods.Contains(name))
            {
                unallowedClientMods.Add(name);
            }
        }

        if (unallowedClientMods.Count > 0)
        {
            string names = string.Join(", ", unallowedClientMods);
            NetMessage.BootPlayer(
                from,
                NetworkText.FromLiteral($"Unallowed client mods: {names}")
            );
        }
    }
}


public class ClientModCheckPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
            return;

        if (ModContent.GetInstance<ServerConfig>().AllowClientMods)
            return;

        List<string> clientMods =
            ModLoader.Mods
                .Where(m => m.Side == ModSide.Client)
                .Select(m => m.Name)
                .ToList();

        var packet = Mod.GetPacket();
        packet.Write((byte)PacketType.ClientModCheck);
        packet.Write(clientMods.Count);

        foreach (string name in clientMods)
            packet.Write(name);

        packet.Send();
    }
}
