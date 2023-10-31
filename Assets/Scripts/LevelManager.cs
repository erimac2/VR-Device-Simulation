using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private List<string> paths;
    private string objectPath;
    private string conectionPath;
    public void Start()
    {
        paths = new List<string>();
        paths.Add(Application.persistentDataPath + "/ObjectData.json");
        paths.Add(Application.persistentDataPath + "/ConnectionData.json");
    }
    public void Save()
    {
        Connector[] allConnectors = GameObject.FindObjectsOfType<Connector>();
        foreach (string path in paths)
        {
            if (File.Exists(path))
                File.Delete(path);
            else
                File.Create(path);
            Transform transform;
            if (path.Contains("Object"))
                transform = GameObject.Find("Devices").transform;
            else
                transform = GameObject.Find("Connectors").transform;
            //object[] data = transform.GetComponentsInChildren<Transform>();
            File.AppendAllText(path, "{\n\"JSONList\" : [\n");
            int deviceCount = 0;
            int cableCount = 0;
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
                            cableCount++;
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
                            if (ObjectPrefabPaths.GetCableCount() != cableCount)
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
            File.AppendAllText(path, "\n]\n}");
            Debug.Log("Saved to: " + objectPath);
        }
    }
    public void Load()
    {
        foreach (string path in paths)
        {
            if (File.Exists(path))
            {
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
}
