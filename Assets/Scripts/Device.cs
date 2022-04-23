using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    public string deviceName;
    public List<Connection> connections = new List<Connection>();
    [HideInInspector] public int connectedCount = 0;

    void Start()
    {
        deviceName = gameObject.name;
    }
    public void PrintConnections()
    {
        foreach(Connection connection in connections)
        {
            print(connection.ToString());
        }
    }
}
[Serializable]
public class Connection : IEquatable<Connection>
{
    public Device otherDevice;
    public ConnectorData connectorData;

    public Connection(Device otherDevice, ConnectorData connectorData)
    {
        this.otherDevice = otherDevice;
        this.connectorData = connectorData;
    }
    public override string ToString()
    {
        return otherDevice.name + " " + connectorData.name;
    }
    public bool Equals(Connection other)
    {
        if (other == null)
        {
            return false;
        }
        else
        {
            return otherDevice == other.otherDevice && connectorData == other.connectorData;
        }
    }
}
