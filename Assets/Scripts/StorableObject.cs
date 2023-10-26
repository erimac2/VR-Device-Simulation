using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
public interface IStorableComponent
{
    string Serialize();
    void Deserialize(string payload);
}
[System.Serializable]
public class StorableObject
{

    public string objectName;
    public string prefabPath;
    public Vector3 position;
    public Quaternion rotation;
    public Dictionary<string, string> storedComponents; // not working yet
    //public List<SerializableComponent> components;

    public StorableObject(string name, string path, Vector3 pos, Quaternion rot)
    {
        objectName = name;
        prefabPath = path;
        position = pos;
        rotation = rot;
    }
    public string Serialize(GameObject obj)
    {
        storedComponents = new Dictionary<string, string>();

        //IStorableComponent[] components = obj.GetComponents<IStorableComponent>();
        //Component[] components = obj.GetComponents<Component>();
        //foreach (Component comp in components)
        //{
            //storedComponents.Add(comp.GetType().Name, JsonUtility.ToJson(comp));
        //}

        return JsonUtility.ToJson(this);
    }
}
[System.Serializable]
public class StorableObjectList
{
    public List<StorableObject> JSONList;
}