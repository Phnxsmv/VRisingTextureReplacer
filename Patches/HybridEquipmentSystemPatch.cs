using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using ProjectM;
using ProjectM.Hybrid;
using System.Collections;
using UnityEngine;
using VRisingTextureReplacer.Helpers;
using VRisingTextureReplacer.Replacer;

namespace VRisingTextureReplacer.Patches;

[HarmonyPatch]
internal static class HybridEquipmentSystemPatch
{
    static long _signature = 0;
    static bool _hasInitial = false;

    [HarmonyPatch(typeof(HybridEquipmentSystem), nameof(HybridEquipmentSystem.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(HybridEquipmentSystem __instance)
    {
        if (!PlayerState.EnsurePlayerCache()) return;
        var em = PlayerState.GameWorld.EntityManager;
        if (!em.HasComponent<Equipment>(PlayerState.Character)) return;
        // Read equipment from player
        var equip = em.GetComponentData<Equipment>(PlayerState.Character);
        // Hash the entity references in the slot fields
        long sig = ComputeSignature(equip);

        // Fires SwapTexturesForEntity once unconditionally as the entry-point scan, then sets the signature for future comparison
        if (!_hasInitial)
        {
            _signature = sig;
            _hasInitial = true;
            // First sight of equipment — do an initial scan to catch starting gear
            int initial = TextureReplacer.SwapTexturesForEntity(PlayerState.Character);
            if (initial > 0) Plugin.Info($"[TextureReplacer] Initial — {initial}");
            // Schedule a follow-up in case the initial mesh wasn't fully ready
            CoroutineHelper.Instance.StartCoroutine(DelayedRescan(0.1f).WrapToIl2Cpp());
            return;
        }

        if (sig != _signature)
        {
            _signature = sig;
            int immediate = TextureReplacer.SwapTexturesForEntity(PlayerState.Character);
            if (immediate > 0) Plugin.Info($"[TextureReplacer] Immediate — {immediate}");
            CoroutineHelper.Instance.StartCoroutine(DelayedRescan(0.1f).WrapToIl2Cpp());
        }
    }


    static long ComputeSignature(Equipment e)
    {
        // Hash just the Current id of each slot. Queued changes don't matter — 
        // we want to react when items finish streaming and become visible
        // Walks all seven slots and combines their Current HybridEquipmentId into one 64 - bit value
        // Two different equipment loadouts will produce different signatures
        long h = 17;
        // Hash whatever slot fields Equipment has
        // h = h * 31 + (e.WeaponSlotEntity.Index ^ ((long)e.WeaponSlotEntity.Version << 32));
        // ... etc
        // Visible gear slots
        h = h * 31 + HashSlot(e.ArmorHeadgearSlot);
        h = h * 31 + HashSlot(e.ArmorChestSlot);
        h = h * 31 + HashSlot(e.WeaponSlot);
        h = h * 31 + HashSlot(e.ArmorFootgearSlot);
        h = h * 31 + HashSlot(e.ArmorLegsSlot);
        h = h * 31 + HashSlot(e.CloakSlot);
        h = h * 31 + HashSlot(e.ArmorGlovesSlot);
        // Cosmetic slots — these override the visible texture even if base armor is unchanged
        h = h * 31 + HashSlot(e.ChestCosmeticSlot);
        h = h * 31 + HashSlot(e.FootgearCosmeticSlot);
        h = h * 31 + HashSlot(e.LegsCosmeticSlot);
        h = h * 31 + HashSlot(e.CloakCosmeticSlot);
        h = h * 31 + HashSlot(e.GlovesCosmeticSlot);
        return h;
    }
    static long HashSlot(EquipmentSlot s)
    {
        // Hash HybridEquipmentId's fields
        long h = s.SlotId.GuidHash;
        h = (h << 8) | s.TransmogIndex;
        h = (h << 1) | (s.HideEquipment ? 1L : 0L);
        return h;
    }
    internal static IEnumerator DelayedRescan(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!PlayerState.EnsurePlayerCache()) yield break;
        int swapped = TextureReplacer.SwapTexturesForEntity(PlayerState.Character);
        if (swapped > 0) Plugin.Info($"[TextureReplacer] Delayed — {swapped}");
    }
}