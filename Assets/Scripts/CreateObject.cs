using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateObject : MonoBehaviour
{
    [SerializeField] private GameObject[] objectPrefabs;
    private int selectedPrefabIndex = -1;
    private int objectIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        //objectPrefab = 
    }
    private GameObject SelectedPrefab
    {
        get
        {
            return selectedPrefabIndex >= 0 && selectedPrefabIndex < objectPrefabs.Length
                ? objectPrefabs[selectedPrefabIndex] : null;
        }
    }
    // Only objects created by this function will be saved to json
    public void Create()
    {
        if (SelectedPrefab != null)
        {
            GameObject obj = Instantiate(SelectedPrefab, this.transform.position, this.transform.rotation);
            MonoScript targetScript = MonoScript.FromMonoBehaviour(obj.GetComponent<Device>());

            if (targetScript != null)
            {
                obj.transform.parent = GameObject.Find("Devices").transform;
                ObjectPrefabPaths.IncreaseDeviceCount();
            }
            else
            {
                targetScript = MonoScript.FromMonoBehaviour(obj.GetComponent<Wire>());
                if (targetScript != null)
                {
                    obj.transform.parent = GameObject.Find("Connectors").transform;
                    ObjectPrefabPaths.IncreaseCableCount();
                }
                else
                {
                    Debug.Log("Unknown device type");
                }
            }

            string prefabPath = AssetDatabase.GetAssetPath(SelectedPrefab);

            Debug.Log("Prefab path: " + prefabPath);
            if (ObjectPrefabPaths.paths.ContainsKey(obj.name))
            {
                obj.name = obj.name + objectIndex;
                objectIndex++;
                ObjectPrefabPaths.paths.Add(obj.name, prefabPath);
            }
            else
            {
                ObjectPrefabPaths.paths.Add(obj.name, prefabPath);
            }
        }
    }
    public void SelectPrefab(int index)
    {
        selectedPrefabIndex = index;
    }
}

public static class ObjectPrefabPaths
{
    public static Dictionary<string, string> paths = new Dictionary<string, string>(); // key - object_name, value - prefab_path
    private static int DeviceCount = 0;
    private static int CableCount = 0;

    public static void IncreaseDeviceCount()
    {
        DeviceCount++;
    }

    public static void IncreaseCableCount()
    {
        CableCount++;
    }

    public static int GetDeviceCount()
    {
        return DeviceCount;
    }

    public static int GetCableCount()
    {
        return CableCount;
    }
}