using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/RequirementData")]
public class RequirementData : ScriptableObject
{
    public List<ConnectionRequirement> requiredConnections;
}
