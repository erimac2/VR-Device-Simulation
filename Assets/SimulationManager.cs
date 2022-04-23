using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public List<Device> devices;
    public List<ConnectionRequirement> requiredConnections = new List<ConnectionRequirement>();

    void Start()
    {
        devices = new List<Device>(FindObjectsOfType<Device>());
    }

    void Update()
    {

    }
    public void CheckResults()
    {
        Debug.LogWarning("Check");
        foreach(Device device in devices)
        {
            ConnectionRequirement connectionRequirement = requiredConnections.Find(item => item.device.name == device.name);
            Debug.LogWarning(device.name);
            foreach(Connection connection in device.connections)
            {
                Debug.LogWarning(connection.ToString());
                if(connectionRequirement != null && connectionRequirement.requiredConnections.Contains(connection))
                {
                    Debug.LogWarning("Connection valid: " + connectionRequirement.device.name + " connected to " + connection.otherDevice.name + " via " + connection.connectorData.name);
                }
            }
        }
    }
}
[Serializable]
public class ConnectionRequirement
{
    public Device device;
    public List<Connection> requiredConnections;

    public ConnectionRequirement(Device device)
    {
        this.device = device;
        requiredConnections = new List<Connection>();
    }
}
