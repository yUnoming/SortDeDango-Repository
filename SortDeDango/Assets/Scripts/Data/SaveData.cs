using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    [Tooltip("到達したステージ番号")]
    public int reachedStageIndex = 1;
    [Tooltip("最後に遊んだステージ番号")]
    public int lastPlayedStageIndex = 1;
    [Tooltip("各ステージの最小手数クリア状況")]
    public List<bool> isMinMoveClearedList = new List<bool>();
}
