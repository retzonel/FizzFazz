using UnityEditor;
using static System.IO.Directory;
using UnityEngine;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;


public static class Setup
{
    [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders()
    {
        Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "Shaders", "Scenes", "Settings");
        Refresh();
    }

    [MenuItem("Tools/Setup/Import My Fav Assets")]
    public static void ImportMyFavouriteAssets()
    {
        Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant\\Editor ExtensionsAnimation");
    }

    [MenuItem("Tools/Setup/Import Basic Assets (3rd Party)")]
    public static void ImportBasics()
    {
        Assets.ImportAsset("Odin Inspector 3.3.1.12.unitypackage", "OdinInspector");
        Assets.ImportAsset("SceneRefAtrribute.unitypackage", "KyleBanks");
        //!!! dont forget kyle banks scene ref attributes
    }
    
    static class Folders
    {
        public static void CreateDefault(string root, params string[] folders)
        {
            var fullPath = Combine(Application.dataPath, root);
            foreach (var folder in folders)
            {
                var path = Combine(fullPath, folder);
                if (!Exists(path))
                {
                    CreateDirectory(path);
                }
            }
        }
    }
    
    static class Assets
    {
        public static void ImportAsset(string asset, string subFolder, string folder = "C:\\Users\\M O N S U R O H\\AppData\\Roaming\\Unity\\Asset Store-5.x")
        {
            ImportPackage(Combine(folder, subFolder, asset), false);
        }
    }
}