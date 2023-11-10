using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using static UnityEngine.UI.GridLayoutGroup;

public class EditManager : MonoBehaviour
{

    [SerializeField] private GameObject taskList;
    [SerializeField] private GameObject devicePrefab;
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private TMP_Dropdown deviceDropdown;
    
    List<GameObject> spawnedGameObjects = new List<GameObject>();
    List<DeviceWithTasks> allDevices;    //used to differentiate between identical tasks on different devices
    RequirementData requirementData;

    Dictionary<string, string> possibleConnections;

    struct DeviceWithTasks
    {
        public Device device;
        public List<GameObject> deviceTasks;
    }

    // Start is called before the first frame update
    void Start()
    {
        possibleConnections = new Dictionary<string, string>();
        possibleConnections.Add("IN", "OUT");
        possibleConnections.Add("USB", "USB");
        possibleConnections.Add("3.5 mm Audio", "3.5 mm Audio");

        

        requirementData = new RequirementData();
        requirementData.requiredConnections = new List<ConnectionRequirement>();
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
                deviceDropdown.options.Add(new TMP_Dropdown.OptionData() { text = name });
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

        List<TMP_Dropdown.OptionData> dropdownitems = deviceDropdown.options;
        int selectedData = deviceDropdown.value;

        foreach (FileInfo file in files)
        {
            if (file.Name == dropdownitems[selectedData].text + ".prefab")
            {
                string newFileName = file.Name.Replace(".prefab", "");
                string prefabPath = $"Prefabs/{objectType}/{newFileName}";
                
                Object pPrefab = Resources.Load(prefabPath);

                if (pPrefab != null)
                {
                    GameObject device = (GameObject)Instantiate(pPrefab, this.transform.position, this.transform.rotation);
                    updateTasks(device);
                }
                else
                {
                    Debug.LogError($"Prefab not found at path: {prefabPath}");
                }
                return;
            }
        }
    }

    void updateTasks(GameObject newObject)//Toks spaghetti, kad WOW
    {
        Device spawnedDevice = newObject.GetComponent<Device>();

        foreach (GameObject gameObjectInScene in spawnedGameObjects)
        {
            ConnectionRequirement requirement = new ConnectionRequirement(spawnedDevice);

            for (int i = 0; i < newObject.transform.childCount; i++)
            {
                GameObject spawnedDevicePort = newObject.transform.GetChild(i).gameObject;
                
                if (!isConnectionPossible(gameObjectInScene, spawnedDevicePort))
                {
                    continue;
                }
                
                ConnectorData connector = new ConnectorData();
                connector.name = spawnedDevicePort.name;

                SocketType socketType = new SocketType();
                socketType = 0;

                Device device1 = gameObjectInScene.GetComponent<Device>();
                
                Connection connection = new Connection(device1, connector, socketType);
                requirement.requiredConnections.Add(connection);
            }
            requirementData.requiredConnections.Add(requirement);
        }
        spawnedGameObjects.Add(newObject);
        FillTaskList();
        //requirementData.requiredConnections.Add();
    }
    
    private bool isConnectionPossible(GameObject gameObjectInScene, GameObject spawnedDevicePort)
    {
        for (int i = 0; i < gameObjectInScene.transform.childCount; i++)
        {
            GameObject oldDevicePort = gameObjectInScene.transform.GetChild(i).gameObject;


            foreach (var possibleConnection in possibleConnections)
            {
                if (oldDevicePort.name.Contains(possibleConnection.Key))
                {
                    if (spawnedDevicePort.name.Contains(possibleConnection.Value))
                    {
                        return true;
                    }
                    else return false;
                }
                else if (oldDevicePort.name.Contains(possibleConnection.Value)) 
                {
                    if (spawnedDevicePort.name.Contains(possibleConnection.Key))
                    {
                        return true;
                    }
                    else return false;
                }
            }
        }
        return false;
    }

    private void FillTaskList()
    {
        GameObject childDevice;
        GameObject childItem;
        allDevices = new List<DeviceWithTasks>();
        foreach (ConnectionRequirement connectionRequirement in requirementData.requiredConnections)
        {
            childDevice = Instantiate(devicePrefab, taskList.transform);
            childDevice.transform.Find("Header").GetComponentInChildren<TextMeshProUGUI>().text = connectionRequirement.device.name;
            List<GameObject> tasks = new List<GameObject>();

            //childDevice.transform.Find("Header").GetComponentInChildren<TextMeshProUGUI>().text = "Something";
            //childDevice.transform.Find("Header").GetComponentInChildren<TextMeshProUGUI>().text = "Something";
            foreach (Connection connection in connectionRequirement.requiredConnections)
            {
                childItem = Instantiate(itemPrefab, childDevice.transform.Find("Connections").transform);
                childItem.GetComponentInChildren<TextMeshProUGUI>().text = connection.ToString();
                childItem.GetComponentInChildren<TextMeshProUGUI>().enableAutoSizing = true;
                childItem.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                tasks.Add(childItem);
            }
            allDevices.Add(new DeviceWithTasks { device = connectionRequirement.device, deviceTasks = tasks });
        }
    }
}
