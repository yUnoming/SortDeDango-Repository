using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class IsMinMoveClearedList
{
    public List<bool> values;
}

public class SaveDataManager : MonoBehaviour
{
    private static SaveDataManager instance;
    public static SaveDataManager Instance => instance;

    private SaveData currentSaveData;
    public SaveData CurrentSaveData => currentSaveData;

    private IsMinMoveClearedList isMinMoveClearedList = new IsMinMoveClearedList();

    private const string ReachedStageKey = "ReachedStageIndex";
    private const string LastPlayedStageKey = "LastPlayedStageIndex";
    private const string IsMinMoveClearedListKey = "IsMinMoveClearedList";

    private void Awake()
    {
        // 重複確認
        if (instance != null && instance != this)
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        // シングルトン化
        else if (instance == null)
        {
            instance = this;
            isMinMoveClearedList = new IsMinMoveClearedList();
            isMinMoveClearedList.values = new List<bool>();
        }
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
        if(saveData != null)
        {
            PlayerPrefs.SetInt(ReachedStageKey, saveData.reachedStageIndex);
            PlayerPrefs.SetInt(LastPlayedStageKey, saveData.lastPlayedStageIndex);
            
            string json = JsonUtility.ToJson(isMinMoveClearedList);
            PlayerPrefs.SetString(IsMinMoveClearedListKey, json);

            PlayerPrefs.Save();
        }
    }
    /// <summary>
    /// ロード    </summary>
    public SaveData Load()
    {
        // 既にセーブデータがあればセーブデータをロード
        if(PlayerPrefs.HasKey(ReachedStageKey))
        {
            currentSaveData = new SaveData();
            currentSaveData.reachedStageIndex = PlayerPrefs.GetInt(ReachedStageKey, 1);
            currentSaveData.lastPlayedStageIndex = PlayerPrefs.GetInt(LastPlayedStageKey, 1);

            string json = PlayerPrefs.GetString(IsMinMoveClearedListKey, "");
            if(json != "")  isMinMoveClearedList = JsonUtility.FromJson<IsMinMoveClearedList>(json);
        }
        return currentSaveData;
    }

    /// <summary>
    /// ステージクリア時の更新    </summary>
    /// <param name="totalStages">
    /// ステージ総数  </param>
    /// <param name="clearedStageIndex">
    /// クリアしたステージ番号    </param>
    /// <param name="isMinMoveCleared">
    /// 最小手数クリアかどうか    </param>
    public void UpdateOnClear(int totalStages, int clearedStageIndex, bool isMinMoveCleared)
    {
        // 新規ステージをクリアした場合に更新
        int nextStageIndex = clearedStageIndex + 1;
        if (nextStageIndex <= totalStages && nextStageIndex > currentSaveData.reachedStageIndex)
        {
            currentSaveData.reachedStageIndex = nextStageIndex;
            currentSaveData.lastPlayedStageIndex = nextStageIndex;
        }

        //** 最小手数クリア状況の更新
        // 新規ステージをクリアした場合
        if (isMinMoveClearedList.values.Count < clearedStageIndex)
            isMinMoveClearedList.values.Add(isMinMoveCleared);
        // 既プレイステージを"最小手数"でクリアした場合
        else if(isMinMoveCleared)
            isMinMoveClearedList.values[clearedStageIndex - 1] = isMinMoveCleared;

        Save(currentSaveData);
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
    /// <summary>
    /// 最小手数クリア状況を取得    </summary>
    /// <param name="stageNumber">
    /// 取得したいステージ番号 </param>
    public bool GetIsMinMoveCleared(int stageNumber)
    {
        if (isMinMoveClearedList.values.Count < stageNumber) return false;
        return isMinMoveClearedList.values[stageNumber - 1];
    }
}
