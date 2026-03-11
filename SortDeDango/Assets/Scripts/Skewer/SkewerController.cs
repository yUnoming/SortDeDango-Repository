using System.Collections.Generic;
using UnityEngine;

public class SkewerController : MonoBehaviour
{
    [SerializeField, Tooltip("最大で刺せる団子数")]
    private int maxDango;
    [SerializeField, Tooltip("団子リスト")]
    private List<Dango> dangoList = new List<Dango>();

    [Tooltip("串の現在の状態")]
    private SkewerState currentState;

    /// <summary>
    /// 状態を変更    </summary>
    /// <param name="nextState">
    /// 変更先の状態  </param>
    public void ChangeState(SkewerState nextState){ currentState = nextState; }

    /// <summary>
    /// 団子を追加できるか判定    </summary>
    /// <returns>
    /// 空きあり; TRUE / 空き無し: FALSE    </returns>
    public bool CanAddDango() { return currentState != SkewerState.Full && currentState != SkewerState.Complete; }
    /// <summary>
    /// 団子を追加    </summary>
    /// <param name="dango">
    /// 追加する団子    </param>
    public void AddDango(Dango dango)
    {
        dangoList.Add(dango);
        // 団子の数が最大数に達したら、状態遷移
        if (dangoList.Count >= maxDango) ChangeState(SkewerState.Full);
    }
    /// <summary>
    /// 一番上の団子を取得    </summary>
    /// <returns>
    /// 取得した一番上の団子  </returns>
    public Dango GetTopDango()
    {
        if(currentState == SkewerState.Empty) return null;
        return dangoList[dangoList.Count - 1];
    }
    /// <summary>
    /// 一番上の団子を除外    </summary>
    /// <returns>
    /// 除外する一番上の団子    </returns>
    public Dango RemoveTopDango()
    {
        Dango topDango = dangoList[dangoList.Count - 1];
        dangoList.Remove(topDango);
        /* 団子が無くなれば、Empty状態へ遷移
         * それ以外は、Stack状態へ遷移 */
        if (dangoList.Count == 0) ChangeState(SkewerState.Empty);
        else ChangeState(SkewerState.Stack);

        return topDango;
    }
}
