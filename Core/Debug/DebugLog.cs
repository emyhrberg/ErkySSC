using System;
using System.IO;
using System.Runtime.CompilerServices;
using ErkySSC.Core.Configs;
using log4net;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ErkySSC.Core.Debug;

/// <summary>
/// A static logging helper for ErkySSC.
/// Used to minimize boilerplate 
/// and improve debugging by providing class names to log messages.
/// Example usage: Log.Debug("Your debug message.");
/// Output: [YourCallerFile] Your debug message.
/// </summary>
public static class Log
{
    public static ILog Base
    {
        get
        {
            var m = ModContent.GetInstance<ErkySSC>();
            return (m != null && m.Logger != null) ? m.Logger : LogManager.GetLogger("ErkySSC");
        }
    }

    /// <summary>
    /// Sends a debug Terraria chat message to everyone, e.g. "[DEBUG/FileName]: Your Message"
    /// In DEBUG builds, always enabled. In non-DEBUG builds, gated by ErkySSC.EnableDebugMessages.
    /// </summary>
    public static void Chat(object message, bool showTime = true, [CallerFilePath] string file = "")
    {
        Debug($"[{file}] {message}");

        if (!ShouldSend())
            return;

        string time = showTime ? $"[{DateTime.Now.ToString("HH:mm:ss")}] " : "";
        string fileName = GetFileLabel(file);
        string text = $"{time}[DEBUG/{fileName}]: {message}";

        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.White);
    }

    private static bool ShouldSend()
    {
        var config = ModContent.GetInstance<ClientConfig>();
        return config != null && config.ShowDebugMessages;
    }

    private static string GetFileLabel(string file)
    {
        string name = Path.GetFileNameWithoutExtension(file);
        if (string.IsNullOrWhiteSpace(name))
            return "Unknown";

        // Keep label compact
        const int MaxFileLabelLen = 17;
        if (name.Length > MaxFileLabelLen)
            name = name.Substring(0, MaxFileLabelLen - 2) + "..";

        return name;
    }

    public static void Info(object message, [CallerFilePath] string file = "")
        => Base.Info($"[{Class(file)}] {message}");

    public static void Debug(object message, [CallerFilePath] string file = "")
        => Base.Debug($"[{Class(file)}] {message}");

    public static void Warn(object message, [CallerFilePath] string file = "")
        => Base.Warn($"[{Class(file)}] {message}");

    public static void Error(object message, [CallerFilePath] string file = "")
        => Base.Error($"[{Class(file)}] {message}");

    /// Returns the file name without its extension from the specified file path.
    private static string Class(string file) => Path.GetFileNameWithoutExtension(file);

    /// Provides a logging wrapper that prefixes all log messages with a specified string.
    public readonly struct Prefixed
    {
        private readonly ILog _log;
        private readonly string _p;
        public Prefixed(ILog log, string prefix) { _log = log; _p = prefix; }
        public void Info(object m) => _log.Info($"[{_p}] {m}");
        public void Debug(object m) => _log.Debug($"[{_p}] {m}");
        public void Warn(object m) => _log.Warn($"[{_p}] {m}");
        public void Error(object m) => _log.Error($"[{_p}] {m}");
    }
}
