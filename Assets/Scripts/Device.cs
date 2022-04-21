using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    public string deviceName;
    public List<GameObject> sockets = new List<GameObject>();
    public List<Connection> connections = new List<Connection>();
    [HideInInspector] public int connectedCount = 0;

    void Start()
    {
        deviceName = gameObject.name;
        foreach(Transform child in transform)
        {
            sockets.Add(child.gameObject);
        }
    }
    public void PrintConnections()
    {
        foreach(Connection connection in connections)
        {
            print(connection.ToString());
        }
    }
}
public class Connection
{
    public Device device;
    public ConnectorData connectorData;

    public Connection(Device device, ConnectorData connectorData)
    {
        this.device = device;
        this.connectorData = connectorData;
    }
    public override string ToString()
    {
        return device.name + " " + connectorData.name;
    }
}
