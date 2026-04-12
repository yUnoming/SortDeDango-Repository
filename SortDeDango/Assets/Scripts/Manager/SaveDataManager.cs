using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    private static SaveDataManager instance;
    public static SaveDataManager Instance => instance;

    private SaveData currentSaveData;
    private const string ReachedStageKey = "ReachedStageIndex";
    private const string LastPlayedStageKey = "LastPlayedStageIndex";

    private void Awake()
    {
        // 重複確認
        if (instance != null && instance != this)
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        // シングルトン化
        else if (instance == null) instance = this;
    }
    private void OnApplicationQuit()
    {
        Save(currentSaveData);
    }

    /// <summary>
    /// セーブデータ作成    </summary>
    public SaveData CreateSaveData()
    {
        currentSaveData = new SaveData();
        return currentSaveData;
    }
    /// <summary>
    /// セーブ    </summary>
    public void Save(SaveData saveData)
    {
        PlayerPrefs.SetInt(ReachedStageKey, saveData.reachedStageIndex);
        PlayerPrefs.SetInt(LastPlayedStageKey, saveData.lastPlayedStageIndex);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// ロード    </summary>
    public SaveData Load()
    {
        if(currentSaveData == null)
        {
            currentSaveData = new SaveData();
            currentSaveData.reachedStageIndex = PlayerPrefs.GetInt(ReachedStageKey, 1);
            currentSaveData.lastPlayedStageIndex = PlayerPrefs.GetInt(LastPlayedStageKey, 1);
        }
        return currentSaveData;
    }

    /// <summary>
    /// ステージクリア時にステージ番号を更新    </summary>
    /// <param name="clearedStageIndex">
    /// クリアしたステージ番号    </param>
    public void UpdateStageIndexOnClear(int clearedStageIndex)
    {
        // 新規ステージをクリアした場合に更新
        int nextStageIndex = clearedStageIndex + 1;
        if (nextStageIndex > currentSaveData.reachedStageIndex)
        {
            currentSaveData.reachedStageIndex = nextStageIndex;
            currentSaveData.lastPlayedStageIndex = nextStageIndex;
            Save(currentSaveData);
        }
    }
    /// <summary>
    /// 最後に遊んだステージ番号を更新    </summary>
    /// <param name="currentStageIndex">
    /// 現在のステージ番号    </param>
    public void UpdateLastPlayedStageIndex(int currentStageIndex)
    {
        currentSaveData.lastPlayedStageIndex = currentStageIndex;
        Save(currentSaveData);
    }
}
