using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkewerSpawnTable", menuName = "SortDeDango/SkewerSpawnTable")]
public class SkewerSpawnTable : ScriptableObject
{
    public List<DangoList> entries = new List<DangoList>();
}
