using ProjectM.Hybrid;
using Unity.Entities;
using UnityEngine;

namespace VRisingTextureReplacer.Helpers;

public static class PlayerGameObject
{
    static World _clientWorld;
    static HybridModelSystem _hybridModelSystem;

    public static GameObject GetForEntity(Entity entity)
    {
        // Cache the client world
        if (_clientWorld == null || !_clientWorld.IsCreated)
        {
            _clientWorld = null;
            foreach (var w in World.All)
            {
                if (w.IsCreated && w.Name == "Client_0") { _clientWorld = w; break; }
            }
            if (_clientWorld == null) return null;
            _hybridModelSystem = null;  // world changed, invalidate
        }

        // Cache the HybridModelSystem instance
        if (_hybridModelSystem == null)
        {
            _hybridModelSystem = _clientWorld.GetExistingSystemManaged<HybridModelSystem>();
            if (_hybridModelSystem == null) return null;
        }

        var map = _hybridModelSystem.GetEntityToGameObjectMap();
        /*Plugin.Info($"[PGO] map has {map.Count} entries");
        foreach (var kvp in map)
        {
            Plugin.Info($"  key={kvp.Key.Index}v{kvp.Key.Version} go={(kvp.Value == null ? "null" : kvp.Value.name)}");
        }
        */
        if (map == null) return null;

        return map.TryGetValue(entity, out var go) ? go : null;
    }
}