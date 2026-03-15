using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : SceneManagerBase<GameplayManager>
{
    [Tooltip("串リスト")]
    private List<SkewerController> skewerList = new List<SkewerController>();

    protected override void StateInit()
    {
        skewerList = FindObjectsByType<SkewerController>(FindObjectsSortMode.None).ToList();
        base.StateInit();
    }
    protected override void StateRunning()
    {
        if (IsClear()) Debug.Log("ステージクリア！！");
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
}
