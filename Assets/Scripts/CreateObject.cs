using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObject : MonoBehaviour
{
    [SerializeField] private GameObject[] objectPrefabs;
    private int selectedPrefabIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        //objectPrefab = 
    }
    private GameObject SelectedPrefab
    {
        get
        {
            return selectedPrefabIndex >= 0 && selectedPrefabIndex < objectPrefabs.Length
                ? objectPrefabs[selectedPrefabIndex] : null;
        }
    }
    // Update is called once per frame
    public void Create()
    {
        if (SelectedPrefab != null)
            Instantiate(SelectedPrefab, this.transform.position, this.transform.rotation);
    }
    public void SelectPrefab(int index)
    {
        selectedPrefabIndex = index;
    }
}
