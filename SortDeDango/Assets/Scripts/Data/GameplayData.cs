using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム進行データ    </summary>
public class GameplayData
{
    [Tooltip("到達したステージ番号")]
    public int reachedStageIndex = 0;
    [Tooltip("最後に遊んだステージ番号")]
    public int lastPlayedStageIndex = 0;
    [Tooltip("各ステージの最小手数クリア状況")]
    public List<bool> isMinMoveClearedList = new List<bool>();
}
