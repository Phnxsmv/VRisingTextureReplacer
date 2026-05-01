/*
using HarmonyLib;
using ProjectM;

namespace VRisingTextureReplacer.Helpers;

[HarmonyPatch]
internal static class GearBuffCheckerDevHelper
{
    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(InputActionSystem __instance)
    {
        if (!PlayerState.EnsurePlayerCache()) return;
        var em = PlayerState.GameWorld.EntityManager;
        if (!em.HasBuffer<BuffBuffer>(PlayerState.Character)) return;

        var buffer = em.GetBuffer<BuffBuffer>(PlayerState.Character);
        Plugin.Info($"[Diagnostic] Player has {buffer.Length} buffs");
        for (int i = 0; i < buffer.Length; i++)
        {
            Plugin.Info($"  GUID {buffer[i].PrefabGuid.GuidHash}");
        }
    }
}
*/