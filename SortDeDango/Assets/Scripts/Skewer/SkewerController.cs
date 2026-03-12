using System.Collections.Generic;
using UnityEngine;

public class SkewerController : MonoBehaviour
{
    [SerializeField, Tooltip("最大で刺せる団子数")]
    private int maxDango;
    [SerializeField, Tooltip("団子の基準点")]
    private GameObject dangoAnchor;
    [SerializeField, Tooltip("団子の配置間隔")]
    private float dangoSpacing;
    [SerializeField, Tooltip("団子リスト")]
    private List<Dango> dangoList = new List<Dango>();

    [Tooltip("串の現在の状態")]
    private SkewerState currentState;

    private void Awake()
    {
        //=====
        // 初期状態の設定
        if (dangoList.Count == 0) ChangeState(SkewerState.Empty);
        else if (dangoList.Count >= maxDango) ChangeState(SkewerState.Full);
        else ChangeState(SkewerState.Stack);
    }
    private void OnMouseDown()
    {
        Debug.Log(gameObject.name + " clicked!");
        /* 団子が刺さっていて、完成していない状態なら判定成功
         * → 選択中の串として、自身をGameplayControllerに渡す  */
        if(currentState != SkewerState.Empty && currentState != SkewerState.Complete)
            GameplayController.Instance.OnSelectedSkewer(this);
    }

    /// <summary>
    /// 状態を変更    </summary>
    /// <param name="nextState">
    /// 変更先の状態  </param>
    private void ChangeState(SkewerState nextState){ currentState = nextState; }
    /// <summary>
    /// 団子の座標設定    </summary>
    /// <param name="dango">
    /// 設定する団子    </param>
    /// <param name="index">
    /// 設定する団子のリスト内インデックス番号 </param>
    private void SetDangoPosition(Dango dango, int index)
    {
        dango.transform.parent = dangoAnchor.transform;
        dango.transform.position = dangoAnchor.transform.position + Vector3.up * dangoSpacing * index;
    }

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
        if (dango == null) return;
        dangoList.Add(dango);
        SetDangoPosition(dango, dangoList.Count - 1);
        // 団子の数が最大数に達したら、状態遷移
        if (dangoList.Count >= maxDango) ChangeState(SkewerState.Full);
        else ChangeState(SkewerState.Stack);
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
        if (currentState == SkewerState.Empty) return null;
        Dango topDango = dangoList[dangoList.Count - 1];
        dangoList.Remove(topDango);
        /* 団子が無くなれば、Empty状態へ遷移
         * それ以外は、Stack状態へ遷移 */
        if (dangoList.Count == 0) ChangeState(SkewerState.Empty);
        else ChangeState(SkewerState.Stack);

        return topDango;
    }
}
