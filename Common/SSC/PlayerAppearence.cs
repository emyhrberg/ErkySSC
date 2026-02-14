using Microsoft.Xna.Framework;
using System.IO;
using Terraria;

namespace ErkySSC.Common.SSC;

/// <summary>
/// Used for copying the joined player's appearence to the Server Sided Character.
/// </summary>
/// <param name="SkinVariant"></param>
/// <param name="Hair"></param>
/// <param name="SkinColor"></param>
/// <param name="EyeColor"></param>
/// <param name="HairColor"></param>
/// <param name="ShirtColor"></param>
/// <param name="UnderShirtColor"></param>
/// <param name="PantsColor"></param>
/// <param name="ShoeColor"></param>
public readonly record struct PlayerAppearance(
    int SkinVariant,
    int Hair,
    Color SkinColor,
    Color EyeColor,
    Color HairColor,
    Color ShirtColor,
    Color UnderShirtColor,
    Color PantsColor,
    Color ShoeColor)
{
    public static PlayerAppearance Capture(Player p)
    {
        return new PlayerAppearance(
            p.skinVariant,
            p.hair,
            p.skinColor,
            p.eyeColor,
            p.hairColor,
            p.shirtColor,
            p.underShirtColor,
            p.pantsColor,
            p.shoeColor
        );
    }

    public void ApplyTo(Player p)
    {
        p.skinVariant = SkinVariant;
        p.hair = Hair;

        p.skinColor = SkinColor;
        p.eyeColor = EyeColor;
        p.hairColor = HairColor;

        p.shirtColor = ShirtColor;
        p.underShirtColor = UnderShirtColor;
        p.pantsColor = PantsColor;
        p.shoeColor = ShoeColor;
    }

    public void Write(BinaryWriter w)
    {
        w.Write(SkinVariant);
        w.Write(Hair);

        WriteColor(w, SkinColor);
        WriteColor(w, EyeColor);
        WriteColor(w, HairColor);

        WriteColor(w, ShirtColor);
        WriteColor(w, UnderShirtColor);
        WriteColor(w, PantsColor);
        WriteColor(w, ShoeColor);
    }

    public static PlayerAppearance Read(BinaryReader r)
    {
        int skinVariant = r.ReadInt32();
        int hair = r.ReadInt32();

        Color skinColor = ReadColor(r);
        Color eyeColor = ReadColor(r);
        Color hairColor = ReadColor(r);

        Color shirtColor = ReadColor(r);
        Color underShirtColor = ReadColor(r);
        Color pantsColor = ReadColor(r);
        Color shoeColor = ReadColor(r);

        return new PlayerAppearance(
            skinVariant,
            hair,
            skinColor,
            eyeColor,
            hairColor,
            shirtColor,
            underShirtColor,
            pantsColor,
            shoeColor
        );
    }

    private static void WriteColor(BinaryWriter w, Color c)
    {
        w.Write(c.R);
        w.Write(c.G);
        w.Write(c.B);
        w.Write(c.A);
    }

    private static Color ReadColor(BinaryReader r)
    {
        return new Color(
            r.ReadByte(),
            r.ReadByte(),
            r.ReadByte(),
            r.ReadByte()
        );
    }
}
