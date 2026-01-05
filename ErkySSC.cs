using ErkySSC.Core;
using ErkySSC.Core.SSC;
using System.IO;
using Terraria.ModLoader;

namespace ErkySSC;

public class ErkySSC : Mod
{
    // The different types of packets that can be sent/received by the client/server.
    public enum PacketType : byte
    {
        ClientJoin,
        LoadPlayer,
        SavePlayer,
        ClientModCheck
    }

    // The main packet handler for the mod.
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        var msg = (PacketType)reader.ReadByte();

        switch (msg)
        {
            case PacketType.ClientJoin:
                SSCMainSystem.ClientJoin(reader, whoAmI);
                break;
            case PacketType.LoadPlayer:
                SSCMainSystem.LoadPlayer(reader);
                break;
            case PacketType.SavePlayer:
                SSCMainSystem.SavePlayer(reader);
                break;
            case PacketType.ClientModCheck:
                ClientModHandler.HandlePacket(reader, whoAmI);
                break;
        }
    }
}
