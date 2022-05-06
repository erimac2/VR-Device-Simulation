using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connector : MonoBehaviour
{

    [SerializeField] private ConnectorData connectorData;
    [HideInInspector] public GameObject originalParent;
    [HideInInspector] public Connector otherEnd;
    [HideInInspector] public Device connectedToDevice;
    [HideInInspector] public SocketType socketType;
    public bool isFullDevice;
    void Start()
    {
        originalParent = transform.parent.gameObject;
        if(isFullDevice)
        {
            connectedToDevice = GetComponentInParent<Device>();
            socketType = SocketType.Both;
        }
        if (connectorData == null)
        {
            List<ConnectorData> connectorObjects = Resources.LoadAll<ConnectorData>("").ToList();
            foreach (ConnectorData connectorObject in connectorObjects)
            {
                if (transform.parent.gameObject.name.ToString().Contains(connectorObject.name))
                {
                    connectorData = connectorObject;
                }
            }
        }
    }
    public bool CheckType(ConnectorData data)
    {
        return connectorData.Type.CompareTo(data.Type) == 0;
    }
}
