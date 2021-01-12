using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using baseTemplate;

namespace Assets.Editor
{
    public class SceneItem : UnityEditor.Editor
    {
        [MenuItem("Open Scene/PolicyPopup")]
        public static void OpenScenePolitic()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Imported/Template/PolicyPopup/_PolicyPopup.unity");
            }
        }

        [MenuItem("Open Scene/Game")]
        public static void OpenGame()
        {
            OpenScene("game");
        }
        
        [MenuItem("Open Scene/Prototype")]
        public static void OpenPrototype()
        {
            OpenScene("prototype");
        }

        static void OpenScene(string name)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
            }
        }

        [MenuItem("Tools/DeleteSaveGame")]
        public static void DeleteSave()
        {
            var manager = FindObjectOfType<ManagersCreator>();
            if (manager != null)
            {
               string path =  Path.Combine(Application.persistentDataPath, manager.ProfileName + ".json");
               PlayerPrefs.DeleteAll();
               if (File.Exists(path))
               {
                   File.Delete(path);
               }
            }
            else
            {
                Debug.LogWarning("manager is null");
            }

            string path2 = Path.Combine(Application.persistentDataPath, "MainProfile.json");
            PlayerPrefs.DeleteAll();
            if (File.Exists(path2))
            {
                File.Delete(path2);
            }

            Debug.LogWarning("Delete save");
        }


    }
}
