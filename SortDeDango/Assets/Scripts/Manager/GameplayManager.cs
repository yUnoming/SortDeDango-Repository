using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : SceneManagerBase<GameplayManager>
{
    [SerializeField]
    private TextMeshProUGUI clearText;

    [Tooltip("串リスト")]
    private List<SkewerController> skewerList = new List<SkewerController>();
    [Tooltip("クリアしたかどうか")]
    private bool isClear;

    protected override void StateInit()
    {
        skewerList = FindObjectsByType<SkewerController>(FindObjectsSortMode.None).ToList();
        base.StateInit();
    }
    protected override void StateRunning()
    {
        if (IsClear() && !isClear)
        {
            // クリア表示
            Debug.Log("ステージクリア！！");
            clearText.enabled = true;

            PauseGame();
            isClear = true;
        }
    }

    /// <summary>
    /// クリア状態か判定    </summary>
    /// <returns>
    /// 全串が空or完成状態: TRUE / ひとつでも条件を満たさない: FALSE    </returns>
    private bool IsClear()
    {
        // 全串の状態判定
        foreach (var skewer in skewerList)
        {
            // 空or完成状態でなければ、FALSEを返す
            if (skewer.CurrentState != SkewerState.Empty && skewer.CurrentState != SkewerState.Complete)
                return false;
        }
        // ここまで来たら条件達成のため、TRUEを返す
        return true;
    }

    /// <summary>
    /// ゲームを停止    </summary>
    public void PauseGame()
    {
        // 串の機能停止
        foreach(var skewer in skewerList) skewer.enabled = false;
    }
    /// <summary>
    /// ゲームを再開   </summary>
    public void ResumeGame()
    {
        // 串の機能再開
        foreach (var skewer in skewerList) skewer.enabled = true;
    }
    /// <summary>
    /// ステージを初期化    </summary>
    public void ResetStage()
    {
        ChangeScene(SceneType.Gameplay, true, sceneName);
    }
}
