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
        if (connectorData == null)
        {
            List<ConnectorData> connectorObjects = Resources.LoadAll<ConnectorData>("ConnectorData").ToList();
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
        if (interactable.transform.GetComponent<Connector>() != null)
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
        if (device.gameObject.GetComponent<XRGrabInteractable>() != null)
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
        if (device.connectedCount == 0)
        {
            if (device.gameObject.GetComponent<XRGrabInteractable>() != null)
            {
                device.gameObject.GetComponent<XRGrabInteractable>().enabled = true;
            }
        }
    }
    public void SetConnectedTo()
    {
        thisEnd = this.GetOldestInteractableSelected().transform.GetComponent<Connector>();
        if (thisEnd != null && thisEnd.isFullDevice)
        {
            currentConnection = new Connection(thisEnd.connectedToDevice, connectorData, SocketType.Both);
            currentOtherEndConnection = new Connection(device, connectorData, SocketType.Both);
            device.connections.Add(currentConnection);
            thisEnd.connectedToDevice.connections.Add(currentOtherEndConnection);
            thisEnd.gameObject.layer = LayerMask.NameToLayer("Object Ignore Collision");
        }
        else
        {
            thisEnd.connectedToDevice = device;
            thisEnd.socketType = socketType;
            print(thisEnd.socketType);
            Transform parent = thisEnd.originalParent.transform;
            if (parent.childCount > 0)
            {
                foreach (Transform child in parent)
                {
                    otherEnd = child.GetComponent<Connector>();
                    thisEnd.otherEnd = otherEnd;
                    otherEnd.otherEnd = thisEnd;
                }
            }
            else
            {
                print(thisEnd.socketType);
                
                otherEnd = thisEnd.otherEnd;
                print(otherEnd.socketType);

                if (thisEnd.socketType == SocketType.Both && (otherEnd.socketType == SocketType.Both || otherEnd.socketType == SocketType.In || otherEnd.socketType == SocketType.Out)
                    || (thisEnd.socketType == SocketType.Both || thisEnd.socketType == SocketType.In || thisEnd.socketType == SocketType.Out) && otherEnd.socketType == SocketType.Both
                    || thisEnd.socketType == SocketType.In && otherEnd.socketType == SocketType.Out
                    || thisEnd.socketType == SocketType.Out && otherEnd.socketType == SocketType.In)
                {
                    currentConnection = new Connection(otherEnd.connectedToDevice, connectorData, otherEnd.socketType);
                    currentOtherEndConnection = new Connection(device, otherEnd.GetConnectorType(), socketType);
                    device.connections.Add(currentConnection);
                    otherEnd.connectedToDevice.connections.Add(currentOtherEndConnection);
                }
            }
            thisEnd.gameObject.layer = LayerMask.NameToLayer("Object Ignore Collision");
        }
    }
    public void RemoveConnectedTo()
    {
        thisEnd.gameObject.layer = LayerMask.NameToLayer("Object");
        if (otherEnd != null && otherEnd.connectedToDevice != null)
        {
            device.connections.Remove(currentConnection);
            otherEnd.connectedToDevice.connections.Remove(currentOtherEndConnection);
            currentConnection = null;
            currentOtherEndConnection = null;
        }
        else
        {
            device.connections.Remove(currentConnection);
            thisEnd.connectedToDevice.connections.Remove(currentOtherEndConnection);
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
