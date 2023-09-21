using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup_text : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private ConnectorData connectorData;
    private TextMeshProUGUI tmPro;
    private GameObject textObject;
    private bool active = false;
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
            textObject.transform.LookAt(Camera.main.transform.position);
    }
    public void EnableText()
    {
        active = true;
        textObject = Instantiate(canvasPrefab, transform.position, transform.rotation);
        textObject.transform.position.Set(textObject.transform.position.x, textObject.transform.position.y + 0.2f, textObject.transform.position.z);
        tmPro = textObject.GetComponentInChildren<TextMeshProUGUI>();
        tmPro.text = connectorData.Name;
    }
    public void DisableText()
    {
        if (textObject != null)
            Destroy(textObject);
    }
}
