using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "SortDeDango/StageData")]
public class StageData : ScriptableObject
{
    [Tooltip("最小手数")]
    public int minMoveCount;
    [Tooltip("目標の団子数")]
    public int targetDangoCount;
    [Tooltip("食べられる回数")]
    public int maxEatActionCount;
    [Header("串の総数分リストを追加\nリスト0が左上の串に対応。そこからＺ方向に進行")]
    [Tooltip("団子リスト")]
    public List<DangoList> dangoLists;

    [HideInInspector, Tooltip("串の総数")]
    public int SkewerCount => dangoLists.Count;
}
