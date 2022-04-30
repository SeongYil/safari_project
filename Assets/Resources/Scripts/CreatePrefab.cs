using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// This script creates a new menu item Examples>Create Prefab in the main menu.
// Use it to create Prefab(s) from the selected GameObject(s).
// It is placed in the root Assets folder.
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Assets.Resources.Scripts
{


    public class Example
    {
        // Creates a new menu item 'Examples > Create Prefab' in the main menu.
        [MenuItem("Examples/Create Prefab")]
        static void CreatePrefab()
        {
            // Keep track of the currently selected GameObject(s)
            GameObject[] objectArray = Selection.gameObjects;

            // Loop through every GameObject in the array above
            foreach (GameObject gameObject in objectArray)
            {
                // Create folder Prefabs and set the path as within the Prefabs folder,
                // and name it as the GameObject's name with the .Prefab format
                if (!Directory.Exists("Assets/Prefabs"))
                    AssetDatabase.CreateFolder("Assets", "Prefabs");
                string localPath = "Assets/Prefabs/" + gameObject.name + ".prefab";

                // Make sure the file name is unique, in case an existing Prefab has the same name.
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                // Create the new Prefab and log whether Prefab was saved successfully.
                bool prefabSuccess;
                PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction, out prefabSuccess);
                if (prefabSuccess == true)
                    Debug.Log("Prefab was saved successfully");
                else
                    Debug.Log("Prefab failed to save" + prefabSuccess);
            }
        }

        // Disable the menu item if no selection is in place.
        [MenuItem("Examples/Create Prefab", true)]
        static bool ValidateCreatePrefab()
        {
            return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
        }
    }
}
