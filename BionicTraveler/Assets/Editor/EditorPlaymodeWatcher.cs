namespace BionicTraveler
{
    using UnityEditor;

    /// <summary>
    /// Editor script that watches play mode state changes to inform game objects.
    /// </summary>
    [InitializeOnLoad] // ensure class initializer is called whenever scripts recompile
    public static class EditorPlaymodeWatcher
    {
        static EditorPlaymodeWatcher()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorPrefs.SetBool("JustLaunchedFromEditor", true);
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                EditorPrefs.SetBool("JustLaunchedFromEditor", false);
            }
        }
    }
}
