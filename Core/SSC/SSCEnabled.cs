using ErkySSC.Common;
using Terraria.ModLoader;

namespace ErkySSC.Core.SSC;

/// <summary>
/// Determines if server-side characters (SSC) are enabled in this build.
/// </summary>
public static class SSCEnabled
{
    public static bool IsEnabled
    {
        get
        {
#if DEBUG
            return true;
#else
            return ModContent.GetInstance<ServerConfig>()?.IsSSCEnabled ?? false;
#endif
        }
    }
}
