namespace Editor
{
    using System;
    using System.IO;
    using UnityEditor;

    /// <summary>
    /// Fixes the namespace of newly created C# behavior script files.
    /// From: https://stackoverflow.com/questions/39461801/unity-add-default-namespace-to-script-template/39462269.
    /// </summary>
    public class FixNamespace : AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string metaFilePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(metaFilePath);

            if (!fileName.EndsWith(".cs"))
            {
                return;
            }

            var actualFile = $"{Path.GetDirectoryName(metaFilePath)}\\{fileName}";
            var segmentedPath = $"{Path.GetDirectoryName(metaFilePath)}".Split(new[] { '\\' }, StringSplitOptions.None);

            var generatedNamespace = string.Empty;
            string finalNamespace;

            // In case of placing the class at the root of a folder such as (Editor, Scripts, etc...)
            if (segmentedPath.Length <= 2)
            {
                finalNamespace = EditorSettings.projectGenerationRootNamespace;
            }
            else
            {
                // Skipping the Assets folder and a single subfolder (i.e. Scripts, Editor, Plugins, etc...)
                for (var i = 2; i < segmentedPath.Length; i++)
                {
                    generatedNamespace +=
                        i == segmentedPath.Length - 1
                            ? segmentedPath[i]
                            : segmentedPath[i] + "."; // Don't add '.' at the end of the namespace
                }

                finalNamespace = EditorSettings.projectGenerationRootNamespace + "." + generatedNamespace;
            }

            var content = File.ReadAllText(actualFile);
            var newContent = content.Replace("#NAMESPACE#", finalNamespace);

            if (content != newContent)
            {
                File.WriteAllText(actualFile, newContent);
                AssetDatabase.Refresh();
            }
        }
    }
}
