using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Unity.Entities;
using VRisingTextureReplacer.Replacer;

namespace VRisingTextureReplacer.Patches;

[HarmonyPatch]
internal static class EquipmentSystemHook
{
    [HarmonyPatch(typeof(EquipmentSystem), nameof(EquipmentSystem.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(ref SystemState state)
    {
        var em = state.EntityManager;

        var query = em.CreateEntityQuery(
            ComponentType.ReadOnly<EquipmentChangedEvent>());
        var events = query.ToEntityArray(Allocator.Temp);
        try
        {
            foreach (var e in events)
            {
                var evt = em.GetComponentData<EquipmentChangedEvent>(e);
                var swapped = TextureReplacer.SwapTexturesForEntity(evt.Target);
                if (swapped > 0)
                    Plugin.Info($"[TextureReplacer] Equipment changed (ChangeType={evt.ChangeType}) — swapped {swapped} texture(s)");
            }
        }
        finally
        {
            events.Dispose();
        }
    }
}