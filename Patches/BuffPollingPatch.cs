using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using ProjectM;
using VRisingTextureReplacer.Helpers;

namespace VRisingTextureReplacer.Patches;

[HarmonyPatch]
internal static class BuffPollingPatch // AbilityRunScriptsSystem_ClientPatch
{
    // Every character starts with Buff_VBlood_Ability_Replace, which is always active
    const int BUFF_VBLOODABILITYREPLACE = 1171608023;
    static bool _bloodmendActive = false;

    [HarmonyPatch(typeof(AbilityRunScriptsSystem_Client), nameof(AbilityRunScriptsSystem_Client.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(AbilityRunScriptsSystem_Client __instance)
    {
        // Check for buff being applied to player character
        bool bloodmendActiveNow = PlayerState.HasBuff(BUFF_VBLOODABILITYREPLACE);
        if (bloodmendActiveNow && !_bloodmendActive)
        {
            // Do a delayed scan to give the player character renderer time to update
            CoroutineHelper.Instance.StartCoroutine(HybridEquipmentSystemPatch.DelayedRescan(5f).WrapToIl2Cpp());
            Plugin.Info($"[BuffPollingPatch] Bloodmend added to player character.");
        }
        _bloodmendActive = bloodmendActiveNow;
    }
}