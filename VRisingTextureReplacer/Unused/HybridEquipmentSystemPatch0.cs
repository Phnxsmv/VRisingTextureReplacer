/*
using HarmonyLib;
using ProjectM;
using ProjectM.Hybrid;
using Unity.Collections;
using Unity.Entities;
using VRisingTextureReplacer.Helpers;
using VRisingTextureReplacer.Replacer;

namespace VRisingTextureReplacer.Patches;

[HarmonyPatch]
internal static class HybridEquipmentSystemPatch
{
    static long _signature = 0;
    static bool _hasInitial = false;
    static World _clientWorld;
    static HybridModelSystem _hybridModelSystem;

    [HarmonyPatch(typeof(HybridEquipmentSystem), nameof(HybridEquipmentSystem.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(HybridEquipmentSystem __instance)
    {
        if (!PlayerState.EnsurePlayerCache()) return;
        var em = __instance.EntityManager;

        bool hasEquipment = em.HasComponent<Equipment>(PlayerState.Character);
        bool hasHybridCurrentEquipment = em.HasComponent<HybridCurrentEquipment>(PlayerState.Character);
        bool hasHybridModelUser = em.HasComponent<ProjectM.Hybrid.HybridModelUser>(PlayerState.Character);
        bool hasHybridModelPlayerTransform = em.HasComponent<ProjectM.Hybrid.HybridModelPlayerTransformData>(PlayerState.Character);

        Plugin.Info($"Player {PlayerState.Character.Index}v{PlayerState.Character.Version}: " +
                    $"Equipment={hasEquipment} HybridCurrentEquipment={hasHybridCurrentEquipment} " +
                    $"HybridModelUser={hasHybridModelUser} PlayerTransformData={hasHybridModelPlayerTransform}");
        var query = em.CreateEntityQuery(
            ComponentType.ReadOnly<HybridCurrentEquipment>(),
            ComponentType.ReadOnly<HybridModelPlayerTransformData>());
        var query = em.CreateEntityQuery(ComponentType.ReadOnly<PlayerCharacter>());
        var entities = query.ToEntityArray(Allocator.Temp);

        try
        {
            PlayerState.EnsurePlayerCache();
            Plugin.Info($"PlayerState world: {PlayerState.GameWorld?.Name}, entity: {PlayerState.Character.Index}v{PlayerState.Character.Version}");

            // And the system's own world's player query:
            // foreach (var e in entities)
               // Plugin.Info($"In system world {__instance.World.Name}: PlayerCharacter at {e.Index}v{e.Version}");

            if (entities.Length == 0) return;
            var visualEntity = entities[0];  // SP: only one player visual
            // var equip = em.GetComponentData<HybridCurrentEquipment>(visualEntity);
            // long sig = ComputeSignature(equip);

            // Cache the client world
            if (_clientWorld == null || !_clientWorld.IsCreated)
            {
                _clientWorld = null;
                foreach (var w in World.All)
                {
                    if (w.IsCreated && w.Name == "Client_0") { _clientWorld = w; break; }
                }
                if (_clientWorld == null) return;
                _hybridModelSystem = null;  // world changed, invalidate
            }
            // Cache the HybridModelSystem instance
            if (_hybridModelSystem == null)
            {
                _hybridModelSystem = _clientWorld.GetExistingSystemManaged<HybridModelSystem>();
                if (_hybridModelSystem == null) return;
            }
            var goMap = _hybridModelSystem.GetEntityToGameObjectMap();
            foreach (var kvp in goMap)
            {
                if (kvp.Value != null && kvp.Value.name.Contains("VampireFemale"))
                {
                    Plugin.Info($"[Diag] GO-keyed player entity: {kvp.Key.Index}v{kvp.Key.Version}");
                    var types = em.GetComponentTypes(kvp.Key);
                    foreach (var t in types)
                    {
                        Plugin.Info($"  component: {t}");
                    }
                    types.Dispose();
                    break;
                }
            }
            if (!_hasInitial)
            {
                _signature = sig;
                _hasInitial = true;
                int initial = TextureReplacer.SwapTexturesForEntity(visualEntity);
                if (initial > 0)
                    Plugin.Info($"[TextureReplacer] Initial scan — {initial} swapped");
                return;
            }

            if (sig != _signature)
            {
                _signature = sig;
                int swapped = TextureReplacer.SwapTexturesForEntity(visualEntity);
                if (swapped > 0)
                    Plugin.Info($"[TextureReplacer] Equipment changed — {swapped} swapped");
            }
        }
        finally
        {
            entities.Dispose();
        }
    }

    static long ComputeSignature(HybridCurrentEquipment e)
    {
        long h = 17;
        h = h * 31 + HashId(e.HeadgearSlot.Current);
        h = h * 31 + HashId(e.ChestSlot.Current);
        h = h * 31 + HashId(e.FootgearSlot.Current);
        h = h * 31 + HashId(e.WeaponSlot.Current);
        h = h * 31 + HashId(e.LegsSlot.Current);
        h = h * 31 + HashId(e.CloakSlot.Current);
        h = h * 31 + HashId(e.GlovesSlot.Current);
        return h;
    }

    static long HashId(HybridEquipmentId id)
    {
        long h = id.PrefabGUID.GuidHash;
        h = (h << 8) | id.TransmogIndex;
        h = (h << 1) | (id.FemaleVariant ? 1L : 0L);
        return h;
    }

    [HarmonyPatch(typeof(HybridEquipmentSystem), nameof(HybridEquipmentSystem.OnUpdate))]
    [HarmonyPostfix]
    static void DiagnosticPostfix(HybridEquipmentSystem __instance)
    {
        var em = __instance.EntityManager;
        var query = em.CreateEntityQuery(ComponentType.ReadOnly<HybridCurrentEquipment>());
        int count = query.CalculateEntityCount();
        Plugin.Info($"[Diag] {count} entities have HybridCurrentEquipment");

        if (count == 0) return;

        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
        try
        {
            // Just dump the components on the first entity to see what we're dealing with
            var first = entities[0];
            Plugin.Info($"[Diag] First entity: {first.Index}v{first.Version}");
            var second = entities[1];
            Plugin.Info($"[Diag] Second entity: {second.Index}v{second.Version}");

            var types1 = em.GetComponentTypes(first);
            foreach (var t in types1)
            {
                Plugin.Info($"  component: {t}");
            }
            types1.Dispose();

            var types2 = em.GetComponentTypes(second);
            foreach (var t in types2)
            {
                Plugin.Info($"  component: {t}");
            }
            types2.Dispose();
        }
        finally
        {
            entities.Dispose();
        }        
    }
}
*/