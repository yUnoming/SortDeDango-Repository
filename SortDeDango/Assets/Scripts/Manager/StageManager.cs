using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField, Tooltip("ステージ番号")]
    private int stageNumber = 1;
    [SerializeField, Tooltip("ステージデータリスト")]
    private List<StageData> stageDataList = new List<StageData>();

    /// <summary>
    /// インスタンス    </summary>
    private static StageManager instance;
    public static StageManager Instance
    {
        get
        {
            // インスタンスが無ければ、グローバルマネージャーをシーン内に生成
            if (instance == null)
            {
                GameObject prefab = Resources.Load<GameObject>("GlobalManagersVariant");
                GameObject obj = Instantiate(prefab);
                instance = obj.GetComponentInChildren<StageManager>();
            }
            return instance;
        }
    }
    /// <summary>
    /// 現在のステージデータ  </summary>
    public StageData CurrentStageData => stageDataList[stageNumber - 1];

    protected void Awake()
    {
        // 重複確認
        if (instance != null && instance != this)
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        // インスタンス化
        else if (instance == null) instance = this;
    }

    /// <summary>
    /// ステージ番号をセット    </summary>
    /// <param name="newStageNumber">
    /// セットするステージ番号    </param>
    public void SetStage(int newStageNumber) { stageNumber = newStageNumber; }
    /// <summary>
    /// ステージ番号を一つ進める    </summary>
    public void MoveStage()
    {
        stageNumber++;
        if (stageNumber > stageDataList.Count) stageNumber = 1;
    }
    /// <summary>
    /// ステージ番号を一つ戻す    </summary>
    public void ReturnStage()
    { 
        stageNumber--;
        if (stageNumber <= 0) stageNumber = stageDataList.Count;
    }
}
