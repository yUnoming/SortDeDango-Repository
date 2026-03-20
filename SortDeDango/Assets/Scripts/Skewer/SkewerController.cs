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
    public SkewerState CurrentState { get { return currentState; } }

    private void Awake()
    {
        // 初期状態の設定
        if (dangoList.Count == 0) ChangeState(SkewerState.Empty);
        else if (dangoList.Count >= maxDango)
        {
            if (IsComplete()) ChangeState(SkewerState.Complete);
            else ChangeState(SkewerState.Full);

        }
        else ChangeState(SkewerState.Stack);
    }
    private void OnMouseDown()
    {
        if(!enabled) return;
        Debug.Log(gameObject.name + " clicked!");
        // 完成していない状態なら、選択中の串として自身をGameplayControllerに渡す
        if(currentState != SkewerState.Complete)
            GameplayController.Instance.OnSelectedSkewer(this);
    }

    /// <summary>
    /// 状態を変更    </summary>
    /// <param name="nextState">
    /// 変更先の状態  </param>
    private void ChangeState(SkewerState nextState){ currentState = nextState; }
    /// <summary>
    /// 完成状態であるか判定    </summary>
    /// <returns>
    /// 団子の色が全一致; TRUE / いつでも色が一致しない; FALSE    </returns>
    private bool IsComplete()
    {
        // 団子同士の色を確認し、一致しなければFALSEを返す
        for(int i = 0; i < dangoList.Count - 1; i++)
        {
            if (dangoList[i].Color != dangoList[i + 1].Color) return false;
        }
        // ここまで到達したら、全ての団子の色が一致しているのでTRUEを返す
        return true;
    }

    /// <summary>
    /// 団子を移動可能か判定    </summary>
    /// <param name="from">
    /// 現在、追加予定の団子が刺さっている串    </param>
    /// <returns>
    /// 空きあり; TRUE / 空き無し: FALSE    </returns>
    public bool CanMoveDango(SkewerController from)
    {
        // 同じ串だったら、FALSEを返す
        if (from == this) return false;
        // 団子で埋まっている / 完成している状態なら、FALSEを返す
        else if (currentState == SkewerState.Full || currentState == SkewerState.Complete) return false;
        // 団子が刺さっていないなら、TRUEを返す
        else if(currentState == SkewerState.Empty) return true;
        // 団子の色が合致するかどうかで、真偽値を返す
        return from.GetTopDango().Color == GetTopDango().Color;
    }
    /// <summary>
    /// 団子を追加    </summary>
    /// <param name="dango">
    /// 追加する団子    </param>
    public void AddDango(Dango dango)
    {
        if (dango == null) return;

        dangoList.Add(dango);
        dango.transform.parent = dangoAnchor.transform;
        // 団子の数が最大数に達したら、状態遷移
        if (dangoList.Count >= maxDango)
        {
            if (IsComplete()) ChangeState(SkewerState.Complete);
            else ChangeState(SkewerState.Full);
        }
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

    /// <summary>
    /// 一番上の団子の座標を設定    </summary>
    public void SetTopDangoPosition(Dango topDango)
    {
        topDango.transform.position = dangoAnchor.transform.position + Vector3.up * dangoSpacing * (dangoList.Count - 1);
    }
    /// <summary>
    /// 一番上の団子の座標を取得    </summary>
    public Vector3 GetTopDangoPosition()
    {
        return dangoAnchor.transform.position + Vector3.up * dangoSpacing * (dangoList.Count - 1);
    }

    /// <summary>
    /// 選択時のイベント    </summary>
    public void OnSelect()
    {
        transform.position += Vector3.up;
    }
    /// <summary>
    /// 選択解除時のイベント    </summary>
    public void OnDeselect()
    {
        transform.position -= Vector3.up;
    }
}
