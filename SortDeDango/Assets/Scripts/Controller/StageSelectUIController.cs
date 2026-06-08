using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectUIController : MonoBehaviour
{
    [SerializeField]
    private List<Button> stageSelectButtons = new List<Button>();
    [SerializeField]
    private List<GameObject> cherryBlossomPetals;

    /// <summary>
    /// ステージ選択ボタンの状態更新    </summary>
    /// <param name="reachedStageIndex">
    /// 到達したステージ番号    </param>
    public void UpdateStageSelectButtons(int reachedStageIndex)
    {
        SaveDataManager manager = SaveDataManager.Instance;

        // 到達済みステージの数だけループ
        for (int i = 0; i < reachedStageIndex; i++)
        {
            // ステージ選択ボタンのロック解除
            stageSelectButtons[i].interactable = true;
            // 最小手数クリアによる装飾
            if (manager.GetIsMinMoveCleared(i + 1))
                cherryBlossomPetals[i].SetActive(true);
        }
    }
}
