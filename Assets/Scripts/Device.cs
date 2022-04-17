using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    public string deviceName;
    public List<GameObject> sockets;
    // Start is called before the first frame update
    void Start()
    {
        deviceName = gameObject.name;
        foreach(Transform child in transform)
        {
            sockets.Add(child.gameObject);
        }
    }
    public void DisableSockets()
    {
        foreach(GameObject socket in sockets)
        {
            socket.GetComponent<Socket>().socketActive = false;
        }
    }
    public void EnableSockets()
    {
        foreach (GameObject socket in sockets)
        {
            socket.GetComponent<Socket>().socketActive = true;
        }
    }
}
