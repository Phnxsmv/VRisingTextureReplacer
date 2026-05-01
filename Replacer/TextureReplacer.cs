using Unity.Entities;
using UnityEngine;
using VRisingTextureReplacer.Helpers;

namespace VRisingTextureReplacer.Replacer;

internal static class TextureReplacer
{
    // Called both from the initial scan and from the Harmony postfix on material assignment
    public static int SwapRendererTextures(SkinnedMeshRenderer renderer)
    {
        int count = 0;
        foreach (var material in renderer.materials)
        {
            if (material == null) continue;
            count += SwapMaterialTextures(material);
        }
        return count;
    }

    private static int SwapMaterialTextures(Material material)
    {
        int count = 0;

        // Check each material's textures regardless — streaming may have
        // populated them since the last scan even if we visited before
        foreach (string propName in material.GetTexturePropertyNames())
        {
            Texture tex = material.GetTexture(propName);
            if (tex == null) continue;

            string texName = tex.name;
            if (string.IsNullOrEmpty(texName)) continue;

            if (Plugin.ReplacementTextures.TryGetValue(texName, out Texture2D replacement))
            {
                // Skip if this material+property already has our replacement applied
                if (tex.GetInstanceID() ==
                    replacement.GetInstanceID())
                    continue;

                material.SetTexture(propName, replacement);
                Plugin.Info($"[TextureReplacer] Swapped: {texName} on {propName}");
                count++;
            }
        }

        return count;
    }

    public static int SwapTexturesForEntity(Entity entity)
    {
        var root = PlayerGameObject.GetForEntity(entity);
        if (root == null) return 0;

        int swapped = 0;

        foreach (var r in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            foreach (var m in r.sharedMaterials)
                if (m != null) swapped += SwapMaterialTextures(m);

        foreach (var r in root.GetComponentsInChildren<MeshRenderer>(true))
            foreach (var m in r.sharedMaterials)
                if (m != null) swapped += SwapMaterialTextures(m);

        return swapped;
    }
}