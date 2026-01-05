using ErkySSC.Core.SSC;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ErkySSC.Core.Commands;

public class SSCHelp : ModCommand
{
    public override string Command => "sschelp";

    public override string Description => "Shows useful information for using SSC.";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var fileData = Main.ActivePlayerFileData;

        if (fileData == null)
        {
            Main.NewText("No active player data.", Color.Red);
            return;
        }

        var playTime = fileData.GetPlayTime();

        Main.NewText("Server-Side Characters (SSC)", Color.MediumPurple);
        Main.NewText("• Your character is saved on the server, not on your PC.", Color.LightGray);
        Main.NewText("• Your character's items, progress, and stats are shared on this world.", Color.LightGray);
        Main.NewText("• Logging out or switching PCs will not reset your character.", Color.LightGray);
        Main.NewText($"• Playtime on this world: {SSCMainSystem.FormatPlayTime(playTime)}", Color.LightGray);
        Main.NewText("• Server files are stored on server's machine, under tModLoader/ErkySSC/<WorldName>/<YourSteamID>/<YourPlayer>.plr", Color.LightGray);
    }
}
