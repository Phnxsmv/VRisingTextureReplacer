using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRisingTextureReplacer.Helpers;

namespace VRisingTextureReplacer.Replacer;

public static class TextureReplacer
{
    private static System.Action<Scene, LoadSceneMode> _sceneLoadedDelegate;
    public static void Init()
    {
        _sceneLoadedDelegate = new System.Action<Scene, LoadSceneMode>(OnSceneLoaded);
        SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
    }

    private static bool _scanning = true;

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "UIEntryPoint" || scene.name == "Boot")
            return;

        Plugin.Info($"[TextureReplacer] Scene loaded: {scene.name}");
        _scanning = true; // reset on each world load
        CoroutineHelper.Instance.StartCoroutine(KeepScanning().WrapToIl2Cpp());
    }

    private static IEnumerator KeepScanning()
    {
        // Scene-load polling because equipment events don't fire for initial-load gear; runs once until first hit then stops
        // Wait for shader prewarming and initial asset streaming to finish
        // Poll until we get at least one successful swap, then stop after a minute of no swaps to avoid infinite scanning in case of issues
        Plugin.Info("[TextureReplacer] Starting KeepScanning()...");

        int consecutiveEmptyScans = 0;

        while (_scanning)
        {
            int swapped = ScanAllRenderers();

            if (swapped > 0)
            {
                // Found some — slow down to catch late-spawning characters
                consecutiveEmptyScans = 0;
                Plugin.Info($"[TextureReplacer] Swapped {swapped} texture(s) this scan.");
                yield return new WaitForSeconds(5f);
                _scanning = false;
            }
            else
            {
                consecutiveEmptyScans++;

                if (consecutiveEmptyScans >= 30)
                {
                    Plugin.Info("[TextureReplacer] No textures found after 30 attempts, stopping scan.");
                    _scanning = false;
                    // The yield break would also work here, but setting _scanning=false is consistent
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                    Plugin.Info($"[TextureReplacer] No textures swapped this scan. Consecutive empty scans: {consecutiveEmptyScans}");
                }
            }
        }
    }

    public static IEnumerator ScanAfterDelay()
    {
        // Small delay for inventory/gear to finish loading
        yield return new WaitForSeconds(0.1f);
        int swapped = ScanAllRenderers();
        Plugin.Info($"[TextureReplacer] Scan complete — {swapped} texture(s) swapped.");
    }

    public static int ScanAllRenderers()
    {
        var renderers = Object.FindObjectsOfType<SkinnedMeshRenderer>();
        int swapCount = 0;

        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material == null) continue;
                swapCount += SwapMaterialTextures(material);
            }
        }

        var meshRenderers = Object.FindObjectsOfType<MeshRenderer>();
        
        foreach (var renderer in meshRenderers)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material == null) continue;
                swapCount += SwapMaterialTextures(material);
            }
        }

        return swapCount;
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

    public static void Shutdown()
    {
        if (_sceneLoadedDelegate != null)
        {
            SceneManager.remove_sceneLoaded(_sceneLoadedDelegate);
            _sceneLoadedDelegate = null;
        }
        _scanning = false;  // stop any running KeepScanning coroutine
    }
}