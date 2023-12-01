using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private List<string> paths;
    private string objectPath;
    private string conectionPath;
    public void Start()
    {

    }
    public void Save()
    {/*
        string path = Application.persistentDataPath + "/LevelData.json";
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
        /*            prefabPath = ObjectPrefabPaths.paths[go.name];
                        StorableObject storableObject = new StorableObject(go.name, prefabPath, go.transform.position, go.transform.rotation);
                        string json = storableObject.Serialize((GameObject)obj);
                        File.AppendAllText(path, json);
                        if (ObjectPrefabPaths.paths.Keys.Count != i)
                            File.AppendAllText(path, ",\n");
                    }
                }
                File.AppendAllText(path, "\n]\n}");
                Debug.Log("Saved to: " + path);
        */



        List<string> deviceType = new List<string>();
        deviceType.Add("Devices");
        deviceType.Add("Connectors");
        Connector[] allConnectors = GameObject.FindObjectsOfType<Connector>();
        string path = Application.persistentDataPath + "/LevelData.json";
        if (File.Exists(path))
            File.Delete(path);
        else
            File.Create(path);
        File.AppendAllText(path, "{");
        foreach (string type in deviceType)
        {
            Transform transform;
            transform = GameObject.Find(type).transform;
            //object[] data = transform.GetComponentsInChildren<Transform>();
            string jsonList = "\n\"JSONList" + type.TrimStart('"').TrimEnd('"') + "\" : [\n";
            File.AppendAllText(path, jsonList);
            int deviceCount = 0;
            int connectorCount = 0;
            foreach (Transform obj in transform)
            {
                GameObject go = obj.gameObject;
                string prefabPath = "";
                if (ObjectPrefabPaths.paths.ContainsKey(go.name))
                {
                    string json;
                    // -- Alternative --
                    /*
                    string some_path_for_asset = "Assets/LevelAssets/" + go.name + ".prefab";
                    AssetDatabase.CreateAsset(go, some_path_for_asset);
                    prefabPath = AssetDatabase.GetAssetPath(go);
                    */
                    prefabPath = ObjectPrefabPaths.paths[go.name];
                    StorableObject storable;
                    if (prefabPath.Contains("Devices"))
                    {
                        deviceCount++;
                        storable = new StorableObject(go.name, prefabPath, go.transform.position, go.transform.rotation);
                        json = storable.Serialize(obj.gameObject);
                        File.AppendAllText(path, json);
                        if (ObjectPrefabPaths.GetDeviceCount() != deviceCount)
                            File.AppendAllText(path, ",\n");
                    }
                    else
                    {
                        if (prefabPath.Contains("Cables"))
                        {
                            connectorCount++;
                            List<Connector> thisConn = new List<Connector>();
                            foreach (Connector conn in allConnectors)
                            {
                                if (conn.originalParent.name == go.name)
                                {
                                    thisConn.Add(conn);
                                }
                            }
                            storable = new StorableCable(go.name, prefabPath, go.transform.position, go.transform.rotation, thisConn[0].gameObject.transform.position, thisConn[0].gameObject.transform.rotation, thisConn[1].gameObject.transform.position, thisConn[1].gameObject.transform.rotation);
                            json = storable.Serialize(obj.gameObject);
                            File.AppendAllText(path, json);
                            if (ObjectPrefabPaths.GetCableCount() != connectorCount)
                                File.AppendAllText(path, ",\n");
                        }
                        else
                        {
                            storable = new StorableObject(go.name, prefabPath, go.transform.position, go.transform.rotation);
                            json = storable.Serialize(obj.gameObject);
                            File.AppendAllText(path, json);
                            File.AppendAllText(path, ",\n");

                        }
                    }
                    //todo: fix so that components loaded from json are will be saved when trying to save again
                }
            }
            if (type.Equals(deviceType.Last()))
            {
                File.AppendAllText(path, "\n]\n}");
            }
            else
            {
                File.AppendAllText(path, "\n],");
            }
        }
        Debug.Log("Saved to: " + path);
    }

    public static void Load() // sceneName - scene, where objects should be loaded
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        //Debug.Log(SceneManager.GetActiveScene().name);


        /*
        
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





        */

        string path = Application.persistentDataPath + "/LevelData.json";

        if (File.Exists(path))
        {
            JsonAllObjectsArray test = JsonUtility.FromJson<JsonAllObjectsArray>(path);





            StorableObject[] jsonObjects = JsonUtility.FromJson<StorableObject[]>(path);
            StorableCable[] jsonCables = JsonUtility.FromJson<StorableCable[]>(path);



            string data = File.ReadAllText(path);
            if (path.Contains("Object"))
            {
                StorableObjectList list = JsonUtility.FromJson<StorableObjectList>(data);
                Debug.Log("List length: " + list.JSONList.Count);
                foreach (StorableObject obj in list.JSONList)
                {
                    //Debug.Log("Prefab path in load: " + obj.prefabPath);
                    var prefab = AssetDatabase.LoadAssetAtPath(obj.prefabPath, typeof(GameObject));
                    Instantiate(prefab, obj.position, obj.rotation);
                    if (!ObjectPrefabPaths.paths.ContainsKey(obj.objectName))
                    {
                        ObjectPrefabPaths.paths.Add(obj.objectName, obj.prefabPath);
                    }
                    ObjectPrefabPaths.IncreaseDeviceCount();
                }
            }
            else if (path.Contains("Connection"))
            {
                StorableCableList list = JsonUtility.FromJson<StorableCableList>(data);
                Debug.Log("List length: " + list.JSONList.Count);
                foreach (StorableCable obj in list.JSONList)
                {
                    //Debug.Log("Prefab path in load: " + obj.prefabPath);
                    var prefab = AssetDatabase.LoadAssetAtPath(obj.prefabPath, typeof(GameObject));
                    GameObject go = Instantiate(prefab, obj.position, obj.rotation) as GameObject;
                    go.transform.GetChild(0).position = obj.positionOfFirstPlug;
                    go.transform.GetChild(0).rotation = obj.rotationOfFirstPlug;
                    go.transform.GetChild(1).position = obj.positionOfSecondPlug;
                    go.transform.GetChild(1).rotation = obj.rotationOfSecondPlug;
                    if (!ObjectPrefabPaths.paths.ContainsKey(obj.objectName))
                    {
                        ObjectPrefabPaths.paths.Add(obj.objectName, obj.prefabPath);
                    }
                    ObjectPrefabPaths.IncreaseCableCount();
                }

            }
        }
        else
            Debug.Log("Saved file does not exist!");
    }

}

[System.Serializable]
public class JsonAllObjectsArray
{
    public StorableObject[] JSONListDevices;
    public StorableCable[] JSONListConnectors;
}

