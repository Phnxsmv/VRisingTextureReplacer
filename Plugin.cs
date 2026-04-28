using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using VRisingTextureReplacer.Helpers;
using VRisingTextureReplacer.Replacer;

namespace VRisingTextureReplacer;

[BepInPlugin("VRisingTextureReplacer", "VRisingTextureReplacer", "1.0.0")]
public class Plugin : BasePlugin
{
    internal static ManualLogSource Logger;
    internal static ConfigEntry<bool> LoggingEnabledConfig;
    internal static bool LoggingEnabled => LoggingEnabledConfig?.Value ?? false;
    internal static Dictionary<string, Texture2D> ReplacementTextures = new();
    internal static Harmony Harmony = new("VRisingTextureReplacer");

    internal static void Info(string msg)
    {
        if (LoggingEnabled) Logger.LogInfo(msg);
    }
    internal static void Warning(string msg) => Logger.LogWarning(msg);

    internal static void Error(string msg) => Logger.LogError(msg);

    public override void Load()
    {
        Logger = Log;

        LoggingEnabledConfig = Config.Bind("Debug", "EnableInfoLogging", false,
        "Enable info-level logging. Useful for diagnosing issues; disable for cleaner logs.");

        string texturesFolder = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Textures");

        if (!Directory.Exists(texturesFolder))
        {
            Error($"Textures folder not found: {texturesFolder}");
            return;
        }

        foreach (string file in Directory.GetFiles(texturesFolder, "*.png"))
        {
            string textureName = Path.GetFileNameWithoutExtension(file);
            bool isNormalMap = textureName.EndsWith("_n", StringComparison.OrdinalIgnoreCase);

            byte[] bytes = File.ReadAllBytes(file);

            // Load as RGBA32 first — ImageConversion.LoadImage requires uncompressed
            Texture2D tex = isNormalMap
                ? new Texture2D(2, 2, TextureFormat.RGBA32, true, true)
                : new Texture2D(2, 2, TextureFormat.RGBA32, true, false);

            if (!ImageConversion.LoadImage(tex, (Il2CppStructArray<byte>)bytes))
            {
                Error($"Failed to load image: {file}");
                continue;
            }

            // Compress to BC7 to match the game's original texture format
            // true = high quality compression (slower load, better result)
            tex.Compress(true);

            // Compress() changes the format but doesn't rename — restore the name
            tex.name = textureName;
            ((UnityEngine.Object)tex).hideFlags = HideFlags.DontUnloadUnusedAsset;
            UnityEngine.Object.DontDestroyOnLoad(tex);

            ReplacementTextures[textureName] = tex;
            Info($"Loaded: {textureName} ({tex.width}x{tex.height}) fmt={tex.format}{(isNormalMap ? " [normal map]" : "")}");
        }

        if (ReplacementTextures.Count == 0)
        {
            Warning("No replacement textures loaded — skipping patch.");
            return;
        }

        Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<CoroutineHelper>();
        Harmony.PatchAll();
        
        TextureReplacer.Init();

        Info($"Patched — {ReplacementTextures.Count} replacement texture(s) ready.");
    }

    public override bool Unload()
    {
        TextureReplacer.Shutdown();
        foreach (var tex in ReplacementTextures.Values)
            UnityEngine.Object.Destroy(tex);
        ReplacementTextures.Clear();
		Harmony.UnpatchSelf();
        return true;
    }
}