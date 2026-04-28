/*
using UnityEngine;

namespace VRisingTextureReplacer.Helpers
{
    internal class DebugHelper
    {
        // Dump various methods to the log so we can find the right names
        public static void DumpSkinnedMeshRendererMethods()
        {
            var skinnedMeshRendererMethods = typeof(SkinnedMeshRenderer).GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            foreach (var method in skinnedMeshRendererMethods)
                Plugin.Info($"[MethodDump] {method.ReturnType.Name} {method.Name}({string.Join(", ", System.Array.ConvertAll(method.GetParameters(), p => p.ParameterType.Name + " " + p.Name))})");
        }
        public static void DumpRendererMethods()
        {
            var rendererMethods = typeof(Renderer).GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly); // ← only methods declared on Renderer itself
            foreach (var method in rendererMethods)
                Plugin.Info($"[MethodDump] {method.ReturnType.Name} {method.Name}({string.Join(", ", System.Array.ConvertAll(method.GetParameters(), p => p.ParameterType.Name + " " + p.Name))})");
        }
        public static void DumpAssetBundleMethods()
        {
            var assetBundleMethods = typeof(AssetBundle).GetMethods(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.DeclaredOnly); // ← only methods declared on Renderer itself
            foreach (var method in assetBundleMethods)
                Plugin.Info($"[MethodDump] {method.ReturnType.Name} {method.Name}({string.Join(", ", System.Array.ConvertAll(method.GetParameters(), p => p.ParameterType.Name + " " + p.Name))})");
        }
    }
}
*/