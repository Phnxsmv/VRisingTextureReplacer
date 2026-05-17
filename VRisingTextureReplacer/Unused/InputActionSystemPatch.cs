/*
using HarmonyLib;
using ProjectM;
using VRisingTextureReplacer.Helpers;
using VRisingTextureReplacer.Replacer;

namespace VRisingTextureReplacer.Patches;

[HarmonyPatch]
internal static class InputActionSystemPatch // Gear buffs drive texture swapping, so we check for changes every update and trigger swaps when detected.
{
    static int _lastSignature = 0;
    static bool _hasInitial = false;

    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(InputActionSystem __instance)
    {
        if (!PlayerState.EnsurePlayerCache()) return;
        var em = PlayerState.GameWorld.EntityManager;
        if (!em.HasBuffer<BuffBuffer>(PlayerState.Character)) return;

        var buffer = em.GetBuffer<BuffBuffer>(PlayerState.Character);
        int sig = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            int hash = buffer[i].PrefabGuid.GuidHash;
            // Combine into running signature; XOR is order-insensitive
            sig ^= hash;
        }

        if (!_hasInitial)
        {
            _lastSignature = sig;
            _hasInitial = true;
            return;
        }

        if (sig != _lastSignature)
        {
            _lastSignature = sig;
            int swapped = TextureReplacer.SwapTexturesForEntity(PlayerState.Character);
            if (swapped > 0)
                Plugin.Info($"[TextureReplacer] Buff signature changed — {swapped} swapped");
        }
    }
}
*/