using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts : EditorWindow
{
    [MenuItem("Tools/Remove Missing Scripts")]
    static void ShowWindow()
    {
        GetWindow<RemoveMissingScripts>("Remove Missing Scripts");
    }

    void OnGUI()
    {
        GUILayout.Label("Remove Missing Script References", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Remove from Selected GameObjects", GUILayout.Height(30)))
        {
            RemoveFromSelected();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Remove from All GameObjects in Scene", GUILayout.Height(30)))
        {
            RemoveFromScene();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Remove from Selected Prefabs", GUILayout.Height(30)))
        {
            RemoveFromPrefabs();
        }
    }

    static void RemoveFromSelected()
    {
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected!");
            return;
        }

        int count = 0;
        foreach (GameObject obj in Selection.gameObjects)
        {
            count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
        }
        Debug.Log($"Removed {count} missing scripts from {Selection.gameObjects.Length} selected object(s)");
    }

    static void RemoveFromScene()
    {
        // Updated method for Unity 2023.1+
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        int count = 0;
        foreach (GameObject obj in allObjects)
        {
            count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
        }
        Debug.Log($"Removed {count} missing scripts from {allObjects.Length} GameObjects in scene");
    }

    static void RemoveFromPrefabs()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No prefabs selected!");
            return;
        }

        int totalCount = 0;
        foreach (Object obj in Selection.objects)
        {
            if (obj is GameObject prefab && PrefabUtility.IsPartOfPrefabAsset(prefab))
            {
                string path = AssetDatabase.GetAssetPath(prefab);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

                int count = 0;
                foreach (Transform child in prefabRoot.GetComponentsInChildren<Transform>(true))
                {
                    count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
                }

                if (count > 0)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                    totalCount += count;
                    Debug.Log($"Removed {count} missing scripts from prefab: {prefab.name}");
                }

                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        if (totalCount > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"Total: Removed {totalCount} missing scripts from prefabs");
        }
    }
}