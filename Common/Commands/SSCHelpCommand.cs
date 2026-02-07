using ErkySSC.Common.SSC;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ErkySSC.Common.Commands;

public class SSCHelp : ModCommand
{
    public override string Command => "sschelp";
    public override string Description => Language.GetTextValue("Mods.ErkySSC.Commands.SSCHelp.Description");
    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var fileData = Main.ActivePlayerFileData;
        if (fileData == null)
        {
            Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCHelp.NoActivePlayer"), Color.Red);
            return;
        }

        var playTime = fileData.GetPlayTime();

        Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCHelp.Header"), Color.MediumPurple);
        Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCHelp.Bullet_ServerSaved"), Color.LightGray);
        Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCHelp.Bullet_SharedWorld"), Color.LightGray);
        Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCHelp.Bullet_Persistence"), Color.LightGray);
        Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCHelp.Bullet_Playtime", SSC.SSC.FormatPlayTime(playTime)), Color.LightGray);
        Main.NewText(Language.GetTextValue("Mods.ErkySSC.Commands.SSCHelp.Bullet_ServerPath"), Color.LightGray);
    }
}
