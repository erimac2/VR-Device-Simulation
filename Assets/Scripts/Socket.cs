using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Socket : XRSocketInteractor
{
    private Device device;
    [SerializeField] private ConnectorData connectorData;
    [SerializeField] private SocketType socketType = 0;
    private Connector thisEnd;
    private Connector otherEnd;
    protected override void Start()
    {
        base.Start();
        device = GetComponentInParent<Device>();
        if(connectorData == null)
        {
            List<ConnectorData> connectorObjects = Resources.LoadAll<ConnectorData>("").ToList();
            foreach (ConnectorData connectorObject in connectorObjects)
            {
                if (gameObject.name.ToString().Contains(connectorObject.name))
                {
                    connectorData = connectorObject;
                }
            }
        }
    }
    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && MatchHover(interactable);
    }
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && MatchSelect(interactable);
    }
    private bool MatchHover(IXRHoverInteractable interactable)
    {
        if(interactable.transform.GetComponent<Connector>() != null)
        {
            return interactable.transform.GetComponent<Connector>().CheckType(connectorData);
        }
        else
        {
            return false;
        }
    }
    private bool MatchSelect(IXRSelectInteractable interactable)
    {
        if (interactable.transform.GetComponent<Connector>() != null)
        {
            return interactable.transform.GetComponent<Connector>().CheckType(connectorData);
        }
        else
        {
            return false;
        }
    }
    public void SetConnectedTo()
    {
        thisEnd = this.GetOldestInteractableSelected().transform.GetComponent<Connector>();
        thisEnd.GetComponent<Connector>().connectedToDevice = device;
        Transform parent = thisEnd.originalParent.transform;
        if(parent.childCount > 0)
        {
            foreach (Transform child in parent)
            {
                otherEnd = child.GetComponent<Connector>();
                otherEnd.otherEnd = thisEnd;
                Debug.LogWarning(otherEnd + " " + child.GetComponent<Connector>());
            }
        }
        else
        {
            otherEnd = thisEnd.otherEnd;
            Debug.LogWarning(transform.parent.GetComponent<Device>().deviceName + " is connected to " + otherEnd.connectedToDevice.deviceName);
        }
        thisEnd.gameObject.layer = LayerMask.NameToLayer("Object Ignore Collision");
        Debug.LogWarning(thisEnd.GetComponent<Connector>().connectedToDevice.deviceName);
    }
    public void RemoveConnectedTo()
    {
        thisEnd.gameObject.layer = LayerMask.NameToLayer("Object");
        thisEnd = null;
    }
}
public enum SocketType
{
    Both,
    In,
    Out
}
