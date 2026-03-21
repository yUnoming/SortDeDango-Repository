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
    
    [Tooltip("現在のステージデータ")]
    public StageData CurrentStageData => stageDataList[stageNumber - 1];
    [Tooltip("現在のステージ番号")]
    public int CurrentStageNumber { get { return stageNumber; } }

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
    /// 次のステージに進む    </summary>
    public void GoToNextStage()
    {
        stageNumber++;
        if (stageNumber > stageDataList.Count) stageNumber = 1;
    }
    /// <summary>
    /// 前のステージに戻る    </summary>
    public void GoToPreviousStage()
    { 
        stageNumber--;
        if (stageNumber <= 0) stageNumber = stageDataList.Count;
    }
}
