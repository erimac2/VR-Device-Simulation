using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    public string deviceName;
    public List<GameObject> sockets;
    public List<Device> connections;
    [HideInInspector] public int connectedCount = 0;

    void Start()
    {
        deviceName = gameObject.name;
        foreach(Transform child in transform)
        {
            sockets.Add(child.gameObject);
        }
    }
}
