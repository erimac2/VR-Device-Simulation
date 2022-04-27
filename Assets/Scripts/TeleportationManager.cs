using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private GameObject teleportReticle;
    [SerializeField] private TeleportationProvider teleportationProvider;
    [SerializeField] private InputActionReference activate;
    [SerializeField] private InputActionReference cancel;
    [SerializeField] private InputActionReference teleportMove;
    private bool isActive = false;
    private bool holdingItem = false;


    // Start is called before the first frame update
    void Start()
    {
        rayInteractor.enabled = false;
        teleportReticle.SetActive(false);
        activate.action.performed += OnTeleportActivate;
        cancel.action.performed += OnTeleportCancel;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isActive)
            return;
        if (holdingItem)
            return;
        if (teleportMove.action.ReadValue<Vector2>() != Vector2.zero)
            return;

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider.gameObject.GetComponent<TeleportationArea>() != null)
            {
                TeleportRequest teleportRequest = new TeleportRequest()
                {
                    destinationPosition = hit.point
                };
                teleportationProvider.QueueTeleportRequest(teleportRequest);
                rayInteractor.enabled = false;
                isActive = false;
                teleportReticle.SetActive(false);
                return;
            }
        }
        rayInteractor.enabled = false;
        teleportReticle.SetActive(false);
        isActive = false;
    }
    private void OnEnable()
    {
        activate.action.performed += OnTeleportActivate;
        cancel.action.performed += OnTeleportCancel;
    }
    private void OnDisable()
    {
        activate.action.performed -= OnTeleportActivate;
        cancel.action.performed -= OnTeleportCancel;
    }
    private void OnTeleportActivate(InputAction.CallbackContext context)
    {
        if(rayInteractor != null)
        {
            rayInteractor.enabled = true;
        }
        teleportReticle.SetActive(true);
        isActive = true;
    }
    private void OnTeleportCancel(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = false;
        teleportReticle.SetActive(false);
        isActive = false;
    }
    public void SetHoldingItem(bool hasItem)
    {
        if (hasItem)
        {
            holdingItem = true;
        }
        else
        {
            holdingItem = false;
        }
    }
}
