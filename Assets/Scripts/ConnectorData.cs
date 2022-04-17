using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ConnectorData")]
public class ConnectorData : ScriptableObject
{
    public string Name;
    public string Type;
    public string Version;
    public bool HasPower;
    public bool HasEthernet;

}
