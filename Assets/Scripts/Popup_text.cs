using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Popup_text : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject tooltipPrefab;
    [SerializeField] private ConnectorData connectorData;
    private TextMeshProUGUI tmPro;
    private GameObject textObject;
    private bool active = false;
    private XRGrabInteractable interactable;
    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();
        interactable.hoverEntered.AddListener(EnableText);
        interactable.hoverExited.AddListener(DisableText);
        interactable.selectEntered.AddListener(DisableText);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            textObject.transform.LookAt(Camera.main.transform, Vector3.up);
            //textObject.transform.position.Set(textObject.transform.position.x, textObject.transform.position.y + 0.5f, textObject.transform.position.z);
            //textObject.transform.localPosition.Set(textObject.transform.localPosition.x, textObject.transform.localPosition.y + 0.5f, textObject.transform.localPosition.z);

        }
    }
    public void EnableText(HoverEnterEventArgs arg0)
    {
        if (!active && !interactable.isSelected && GameObject.Find("Tooltip(Clone)") == null)
        {
            active = true;
            Vector3 offset = new Vector3(0.1f, 0.2f, 0);
            textObject = Instantiate(tooltipPrefab, transform.position + offset, transform.rotation);
            TwoPointsLine script = textObject.GetComponentInChildren<TwoPointsLine>();
            script.pointA = transform;
            tmPro = textObject.GetComponentInChildren<TextMeshProUGUI>();
            tmPro.text = connectorData.Name + " " + connectorData.Version + " " + connectorData.Type;
        }
    }
    public void DisableText(HoverExitEventArgs arg0)
    {
        if (textObject != null)
        {
            Destroy(textObject);
            active = false;
        }
    }
    public void DisableText(SelectEnterEventArgs arg0)
    {
        if (textObject != null)
        {
            Destroy(textObject);
            active = false;
        }
    }
}
