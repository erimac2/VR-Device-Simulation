using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Socket : XRSocketInteractor
{
    private Device device;
    [SerializeField] private AudioClip connectedSound;
    [SerializeField] private ConnectorData connectorData;
    [SerializeField] private SocketType socketType = 0;
    private Connector thisEnd;
    private Connector otherEnd;
    private Connection currentConnection;
    private Connection currentOtherEndConnection;
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
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        SoundManager.instance.PlaySoundEffectAtPosition(connectedSound, transform.position);
        device.connectedCount++;
        if(device.gameObject.GetComponent<XRGrabInteractable>() != null)
        {
            device.gameObject.GetComponent<XRGrabInteractable>().enabled = false;
        }
        SetConnectedTo();
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        RemoveConnectedTo();
        device.connectedCount--;
        if(device.connectedCount == 0)
        {
            if(device.gameObject.GetComponent<XRGrabInteractable>() != null)
            {
                device.gameObject.GetComponent<XRGrabInteractable>().enabled = true;
            }
        }
    }
    public void SetConnectedTo()
    {
        thisEnd = this.GetOldestInteractableSelected().transform.GetComponent<Connector>();
        thisEnd.connectedToDevice = device;
        thisEnd.socketType = socketType;
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
            if(thisEnd.socketType == SocketType.Both && otherEnd.socketType == SocketType.Both 
                || thisEnd.socketType == SocketType.In && otherEnd.socketType == SocketType.Out 
                || thisEnd.socketType == SocketType.Out && otherEnd.socketType == SocketType.In)
            {
                currentConnection = new Connection(device, connectorData);
                currentOtherEndConnection = new Connection(otherEnd.connectedToDevice, connectorData);
                print(currentConnection.device);
                print(currentOtherEndConnection.device);
                print(device);
                device.connections.Add(currentOtherEndConnection);
                otherEnd.connectedToDevice.connections.Add(currentConnection);
                device.PrintConnections();
                otherEnd.connectedToDevice.PrintConnections();
            }
            Debug.LogWarning(transform.parent.GetComponent<Device>().deviceName + " is connected to " + otherEnd.connectedToDevice.deviceName);
        }
        thisEnd.gameObject.layer = LayerMask.NameToLayer("Object Ignore Collision");
        Debug.LogWarning(thisEnd.GetComponent<Connector>().connectedToDevice.deviceName);
    }
    public void RemoveConnectedTo()
    {
        thisEnd.gameObject.layer = LayerMask.NameToLayer("Object");
        if(otherEnd.connectedToDevice != null)
        {
            device.connections.Remove(currentOtherEndConnection);
            otherEnd.connectedToDevice.connections.Remove(currentConnection);
            currentConnection = null;
            currentOtherEndConnection = null;
        }
        thisEnd.connectedToDevice = null;
        thisEnd.socketType = 0;
        thisEnd = null;
        otherEnd = null;
    }
}
public enum SocketType
{
    Both,
    In,
    Out
}
