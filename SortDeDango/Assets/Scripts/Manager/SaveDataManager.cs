using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    private static SaveDataManager instance;
    public static SaveDataManager Instance => instance;

    private const string SaveFilePath = "save.json";

    private SaveData currentSaveData;
    public SaveData CurrentSaveData => currentSaveData;

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
        if(saveData != null)
        {
            string json = JsonUtility.ToJson(saveData);
            string path = Path.Combine(Application.persistentDataPath, SaveFilePath);
            File.WriteAllText(path, json);
        }
    }
    /// <summary>
    /// ロード    </summary>
    public SaveData Load()
    {
        SaveData loadedData = new SaveData();
        string path = Path.Combine(Application.persistentDataPath, SaveFilePath);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentSaveData = loadedData = JsonUtility.FromJson<SaveData>(json);
        }

        return loadedData;
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
        if (currentSaveData.isMinMoveClearedList.Count < clearedStageIndex)
            currentSaveData.isMinMoveClearedList.Add(isMinMoveCleared);
        // 既プレイステージを"最小手数"でクリアした場合
        else if(isMinMoveCleared)
            currentSaveData.isMinMoveClearedList[clearedStageIndex - 1] = isMinMoveCleared;

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
        if (currentSaveData.isMinMoveClearedList.Count < stageNumber) return false;
        return currentSaveData.isMinMoveClearedList[stageNumber - 1];
    }
}
