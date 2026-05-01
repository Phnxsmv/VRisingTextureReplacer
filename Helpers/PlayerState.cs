using ProjectM;
using Unity.Entities;
using Unity.Collections;

namespace VRisingTextureReplacer.Helpers;

internal static class PlayerState
{
    public static World GameWorld { get; private set; }
    public static Entity Character { get; private set; } = Entity.Null;
    static EntityQuery _playerQuery;

    public static bool EnsurePlayerCache()
    {
        if (GameWorld == null || !GameWorld.IsCreated)
        {
            GameWorld = null;
            Character = Entity.Null;
            foreach (var w in World.All)
            {
                if (!w.IsCreated) continue;
                if (w.Name.Contains("Loading")) continue;
                try
                {
                    var q = w.EntityManager.CreateEntityQuery(
                        ComponentType.ReadOnly<PlayerCharacter>());
                    if (q.CalculateEntityCount() > 0)
                    {
                        GameWorld = w;
                        _playerQuery = q;
                        break;
                    }
                }
                catch { }
            }
            if (GameWorld == null) return false;
        }

        var em = GameWorld.EntityManager;
        if (Character == Entity.Null || !em.Exists(Character))
        {
            var players = _playerQuery.ToEntityArray(Allocator.Temp);
            Character = players.Length > 0 ? players[0] : Entity.Null;
            players.Dispose();
            if (Character == Entity.Null) return false;
        }
        return true;
    }
    public static bool HasBuff(int buffGuidHash)
    {
        if (!EnsurePlayerCache()) return false;
        var em = GameWorld.EntityManager;

        if (!em.HasBuffer<BuffBuffer>(Character)) return false;
        var buffer = em.GetBuffer<BuffBuffer>(Character);

        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].PrefabGuid.GuidHash == buffGuidHash) return true;
        }
        return false;
    }
    public static bool HasComponent<T>() where T : struct
    {
        if (!EnsurePlayerCache()) return false;
        return GameWorld.EntityManager.HasComponent<T>(Character);
    }
    public static T GetComponent<T>() where T : struct
    {
        return GameWorld.EntityManager.GetComponentData<T>(Character);
    }
}