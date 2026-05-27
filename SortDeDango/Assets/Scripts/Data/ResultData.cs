using UnityEngine;

public class ResultData
{
    [Tooltip("実際の手数")]
    public int moveCount;
    [Tooltip("最小手数")]
    public int minMoveCount;

    /// <summary>
    /// 最小手数でクリアしたかどうか    </summary>
    public bool IsMinMoveCleared() { return moveCount == minMoveCount; }
}
