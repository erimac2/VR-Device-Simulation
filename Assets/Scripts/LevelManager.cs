using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private string path;
    public void Start()
    {
        path = Application.persistentDataPath + "/LevelData.json";
    }
    public void Save()
    {
        if (File.Exists(path))
            File.Delete(path);
        else
            File.Create(path);
        object[] data = FindObjectsOfType<GameObject>();
        File.AppendAllText(path, "{\n\"JSONList\" : [\n");
        int i = 0;
        foreach (object obj in data)
        {
            GameObject go = (GameObject)obj;
            string prefabPath = "";
            if (ObjectPrefabPaths.paths.ContainsKey(go.name))
            {
                i++;
                // -- Alternative --
                /*
                string some_path_for_asset = "Assets/LevelAssets/" + go.name + ".prefab";
                AssetDatabase.CreateAsset(go, some_path_for_asset);
                prefabPath = AssetDatabase.GetAssetPath(go);
                */
                prefabPath = ObjectPrefabPaths.paths[go.name];
                StorableObject storableObject = new StorableObject(go.name, prefabPath, go.transform.position, go.transform.rotation);
                string json = storableObject.Serialize((GameObject)obj);
                File.AppendAllText(path, json);
                if (ObjectPrefabPaths.paths.Keys.Count != i)
                    File.AppendAllText(path, ",\n");
            }
        }
        File.AppendAllText(path, "\n]\n}");
        Debug.Log("Saved to: " + path);
    }
    public static void Load() // sceneName - scene, where objects should be loaded
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        //Debug.Log(SceneManager.GetActiveScene().name);
        string path = Application.persistentDataPath + "/LevelData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            StorableObjectList list = JsonUtility.FromJson<StorableObjectList>(data);
            Debug.Log("List length: " + list.JSONList.Count);
            foreach (StorableObject obj in list.JSONList)
            {
                //Debug.Log("Prefab path in load: " + obj.prefabPath);
                var prefab = AssetDatabase.LoadAssetAtPath(obj.prefabPath, typeof(GameObject));
                Instantiate(prefab, obj.position, obj.rotation);
                //foreach ()
            }
        }
        else
            Debug.Log("Saved file does not exist!");
    }
}
