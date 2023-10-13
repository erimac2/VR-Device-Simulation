using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class EditManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        fillDeviceDropDown();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void fillDeviceDropDown()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Prefabs/Devices");
        FileInfo[] files = dir.GetFiles("*.*");

        foreach (FileInfo file in files)
        {
            if (file.Name.EndsWith(".prefab"))
            {
                string name = file.Name.Replace(".prefab", "");
                dropdown.options.Add(new TMP_Dropdown.OptionData() { text = name });
            }
        }
    }
    
    //objectType need to be the same as the name of prefab folder.
    public void spawnObject(string objectType)
    {
        if (string.IsNullOrEmpty(objectType))
        {
            Debug.LogError("Object type not specified.");
            return;
        }

        DirectoryInfo dir = new DirectoryInfo($"Assets/Resources/Prefabs/{objectType}");
        FileInfo[] files = dir.GetFiles("*.*");

        List<TMP_Dropdown.OptionData> dropdownitems = dropdown.options;
        int selectedData = dropdown.value;

        foreach (FileInfo file in files)
        {
            if (file.Name == dropdownitems[selectedData].text + ".prefab")
            {
                string newFileName = file.Name.Replace(".prefab", "");
                string prefabPath = $"Prefabs/{objectType}/{newFileName}";
                Object pPrefab = Resources.Load(prefabPath);

                if (pPrefab != null)
                {
                    Instantiate(pPrefab, this.transform.position, this.transform.rotation);
                }
                else
                {
                    Debug.LogError($"Prefab not found at path: {prefabPath}");
                }
            }
        }
    }
}
