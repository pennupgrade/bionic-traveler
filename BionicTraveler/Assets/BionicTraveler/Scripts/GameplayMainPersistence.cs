namespace BionicTraveler.Scripts
{
    using System.Linq;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    using UnityEngine;

    /// <summary>
    /// Hooks into scene loading and instantiates the GameplayMain prefab for a scene if it is missing.
    /// This only applies to scenes loaded directly from the editor. The script has no effect on
    /// mid-game scene transitions.
    /// </summary>
    internal class GameplayMainPersistence
    {
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            var editorLaunch = EditorPrefs.GetBool("JustLaunchedFromEditor");
            if (editorLaunch)
            {
                Debug.Log("GameplayMainPersistence::OnBeforeSceneLoad: Editor launch detected");
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/BionicTraveler/Prefabs/GameplayMain.prefab");
                var gameObject = Object.FindObjectsOfType<GameplayMain>().FirstOrDefault();
                if (gameObject == null)
                {
                    var obj = GameObject.Instantiate(asset);
                    obj.name = "GameplayMain_Injected";
                    Debug.Log("Added new GameplayMain prefab instance to scene");
                }
                else
                {
                    Debug.Log("GameplayMain already exists on scene");
                }
            }
        }
#endif
    }
}
