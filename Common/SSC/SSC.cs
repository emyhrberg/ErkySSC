using ErkySSC.Common.Commands;
using ErkySSC.Core.Configs;
using ErkySSC.Core.Debug;
using Steamworks;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static ErkySSC.ErkySSC;

namespace ErkySSC.Common.SSC;

/// <summary>
/// Main server-side character (SSC) functionality.
/// Stores player files on the server at ..tModLoader/ErkySSC/[WorldName]/[SteamID]/[PlayerName].plr
/// Stores temporary player data at ..tModLoader/Players/[SteamID].SSC
/// </summary>
[Autoload(Side = ModSide.Both)]
public class SSC : ModSystem
{
    public static bool IsEnabled => ModContent.GetInstance<ServerConfig>()?.IsSSCEnabled ?? false;
    private static string SSCFolder => Path.Combine(Main.SavePath, "ErkySSC");
    private static string MapID => Main.ActiveWorldFileData?.Name ?? "UnknownWorld";
    private static readonly object ioLock = new();

    public static void SendJoinRequest()
    {
        if (!IsEnabled)
            return;

        if (Main.netMode != NetmodeID.MultiplayerClient)
            return;

        Player pLocal = Main.LocalPlayer;

        var p = ModContent.GetInstance<ErkySSC>().GetPacket();
        p.Write((byte)PacketType.ClientJoin);

        p.Write(SteamUser.GetSteamID().m_SteamID.ToString());
        p.Write(pLocal.name);

        // Appearance
        PlayerAppearance.Capture(Main.LocalPlayer).Write(p);

        // Send
        p.Send();
    }

    public static void ClientJoin(BinaryReader reader, int from)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            // Receive data from client
            var steamId = reader.ReadString();
            var name = reader.ReadString();
            PlayerAppearance appearance = PlayerAppearance.Read(reader);

            byte[] data;
            TagCompound root;
            bool isNew;

            // Lock IO operations to prevent race conditions
            lock (ioLock)
            {
                // Create directories for SSC and player if they don't exist
                Directory.CreateDirectory(Path.Combine(SSCFolder, MapID));

                var dir = Path.Combine(SSCFolder, MapID, steamId);
                Directory.CreateDirectory(dir);

                var plrPath = Path.Combine(dir, $"{name}.plr");
                var tplrPath = Path.Combine(dir, $"{name}.tplr");

                isNew = !File.Exists(plrPath) || !File.Exists(tplrPath);

                if (isNew)
                {
                    CreateNewPlayer(plrPath, name, appearance);
                }

                // Read player data from files
                data = File.ReadAllBytes(plrPath);
                root = TagIO.FromFile(tplrPath);
            }

            // Send player data back to client
            var p = ModContent.GetInstance<ErkySSC>().GetPacket();
            p.Write((byte)PacketType.LoadPlayer);
            p.Write(isNew);
            p.Write(data.Length);
            p.Write(data);
            TagIO.Write(root, p);
            p.Send(toClient: from);
        }
    }

    public static void LoadPlayer(BinaryReader reader)
    {
        // Receive data from server, load the player and spawn into the world
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            bool isNew = reader.ReadBoolean();
            int len = reader.ReadInt32();
            byte[] data = reader.ReadBytes(len);
            TagCompound root = TagIO.Read(reader);

            string steamId = SteamUser.GetSteamID().m_SteamID.ToString();

            var ms = new MemoryStream();
            TagIO.ToStream(root, ms);

            var fileData = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{steamId}.SSC"), cloudSave: false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            };

            Player.LoadPlayerFromStream(fileData, data, ms.ToArray());

            // Force hotbar locked
            fileData.Player.hbLocked = true;

            fileData.MarkAsServerSide();
            fileData.SetAsActive();

            // Enter world
            fileData.Player.Spawn(PlayerSpawnContext.SpawningIntoWorld);
            Player.Hooks.EnterWorld(Main.myPlayer);

            // Notify server that my player data (appearance) has changed
            NetMessage.SendData(MessageID.SyncPlayer, number: Main.myPlayer);

            // Try to restore player position from previous session in this world
            TagCompound sscData = null;
            if (root.ContainsKey("PPP"))
            {
                sscData = root.GetCompound("PPP");
            }

            bool positionRestored = PlayerPositionSystem.TryLoadPlayerPosition(fileData.Player, sscData);

            // Apply max life and mana again to ensure
            //if (fileData.Player.statLife != fileData.Player.statLifeMax)
            //    fileData.Player.statLife = fileData.Player.statLifeMax;
            //if (fileData.Player.statMana != fileData.Player.statManaMax)
            //    fileData.Player.statMana = fileData.Player.statManaMax;

            MapLoadSystem.Request(delayTicks: 30);

            // Get world coordinates for logging
            int worldX = (int)fileData.Player.position.X / 16;
            int worldY = (int)fileData.Player.position.Y / 16;

            // Debug log new/existing player
            Log.Chat(isNew ? "Loaded new SSC player " : "Loaded existing SSC player " + fileData.Player.name);

            // Log player position with coordinates
            string positionStatus = positionRestored ? "(restored)" : "(world spawn)";
            Log.Chat($"SSC player position: ({worldX}, {worldY}) {positionStatus}");

            // Print chat to players with player and playtime
            var config = ModContent.GetInstance<ClientConfig>();
            if (config.ShowStatsMessageWhenEnteringWorld)
            {
                Stats.PrintFullStats();
            }
        }
    }

    private static void CreateNewPlayer(string plrPath, string name, PlayerAppearance appearance)
    {

        // Create a brand new empty player on the server
        var fileData = new PlayerFileData(plrPath, cloudSave: false)
        {
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            Player = new()
            {
                name = name,
                difficulty = PlayerDifficultyID.SoftCore,
            }
        };
        ApplyAppearance(fileData.Player, appearance);

        // Force hotbar locked
        fileData.Player.hbLocked = true;

        // Apply config options
        StarterItems.ApplyStartItems(fileData.Player);
        StarterItems.ApplyStartLife(fileData.Player);
        StarterItems.ApplyStartMana(fileData.Player);

        // Save the player
        //fileData.MarkAsServerSide();
        Player.InternalSavePlayerFile(fileData);

        Log.Chat("Created and saved new player " + name);
    }

    private static void ApplyAppearance(Player p, PlayerAppearance a)
    {
        p.skinVariant = a.SkinVariant;
        p.hair = a.Hair;

        p.skinColor = a.SkinColor;
        p.eyeColor = a.EyeColor;
        p.hairColor = a.HairColor;
        p.shirtColor = a.ShirtColor;
        p.underShirtColor = a.UnderShirtColor;
        p.pantsColor = a.PantsColor;
        p.shoeColor = a.ShoeColor;
    }

    public static void SavePlayer(BinaryReader reader)
    {
        // Receive player data from client save system and save to server disk
        if (Main.netMode == NetmodeID.Server)
        {
            // Read data from client
            var steamID = reader.ReadString();
            var name = reader.ReadString();
            var data = reader.ReadBytes(reader.ReadInt32());
            var root = TagIO.Read(reader);

            // Ensure directory exists
            Utils.TryCreatingDirectory(Path.Combine(SSCFolder, MapID, steamID));

            // Write to file
            File.WriteAllBytes(Path.Combine(SSCFolder, MapID, steamID, $"{name}.plr"), data);
            TagIO.ToFile(root, Path.Combine(SSCFolder, MapID, steamID, $"{name}.tplr"));

            // Flush to ensure data is written
            var stream = new MemoryStream();
            TagIO.ToStream(root, stream);
            stream.Flush();

            Log.Chat("Server saved player " + name);
        }
    }
}
