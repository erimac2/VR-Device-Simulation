using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;

public class SceneEditManager : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject itemPrefab;


    // Start is called before the first frame update
    void Start()
    {
        fillDeviceList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void fillDeviceList()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Prefabs/Devices");
        FileInfo[] files = dir.GetFiles("*.*");


        foreach (FileInfo file in files)
        {
            if (file.Name.EndsWith(".prefab"))
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);

                GameObject listItem = Instantiate(itemPrefab, content.transform);
                TextMeshProUGUI newText = listItem.GetComponentInChildren<TextMeshProUGUI>();
                newText.text = name;
            }
        }
    }
}
