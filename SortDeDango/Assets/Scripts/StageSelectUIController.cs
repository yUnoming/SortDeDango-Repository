using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectUIController : MonoBehaviour
{
    [SerializeField]
    private List<Button> stageSelectButtons = new List<Button>();

    /// <summary>
    /// ステージ選択ボタンのロック状態更新    </summary>
    /// <param name="reachedStageIndex">
    /// 到達したステージ番号    </param>
    public void UpdateStageSelectButtonsLock(int reachedStageIndex)
    {
        // 到達済みのステージ選択ボタンのロック解除
        for (int i = 0; i < stageSelectButtons.Count; i++)
        {
            if (i >= reachedStageIndex) break;
            stageSelectButtons[i].interactable = true;
        }
    }
}
