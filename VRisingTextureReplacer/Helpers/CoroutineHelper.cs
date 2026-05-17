using Il2CppInterop.Runtime;
using UnityEngine;

namespace VRisingTextureReplacer.Helpers;

public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper _instance;

    public static CoroutineHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("VRisingTextureReplacer_CoroutineHelper");
                DontDestroyOnLoad(go);

                // IL2CPP requires the non-generic AddComponent with Il2CppType
                _instance = go.AddComponent(Il2CppType.Of<CoroutineHelper>()).Cast<CoroutineHelper>();
            }
            return _instance;
        }
    }
}