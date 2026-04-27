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

    [Tooltip("現在の団子数")]
    public int CurrentDangoCount => dangoList.Count;
    [Tooltip("追加可能な残り団子数")]
    public int RemainingDangoAddCount => maxDango - dangoList.Count;

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
            if (dangoList[i].DangoColor != dangoList[i + 1].DangoColor) return false;
        }
        // ここまで到達したら、全ての団子の色が一致しているのでTRUEを返す
        return true;
    }
    /// <summary>
    /// 団子の削除    </summary>
    private Dango RemoveDango(int index)
    {
        Dango removeDango = dangoList[index - 1];
        dangoList.Remove(removeDango);

        /* 団子が無くなれば、Empty状態へ遷移
        * それ以外は、Stack状態へ遷移 */
        if (dangoList.Count == 0) ChangeState(SkewerState.Empty);
        else ChangeState(SkewerState.Stack);

        return removeDango;
    }

    /// <summary>
    /// 団子を所持しているか判定    </summary>
    public bool HasDango()
    {
        return dangoList.Count > 0;
    }
    /// <summary>
    /// 選択可能な串かどうか判定    </summary>
    public bool CanSelect()
    {
        // 「団子が刺さっていて空きがある」かどうかで判定
        if (currentState == SkewerState.Stack) return true;
        return false;
    }
    /// <summary>
    /// 団子を移動可能か判定    </summary>
    /// <param name="from">
    /// 現在、追加予定の団子が刺さっている串    </param>
    public bool CanMoveDango(SkewerController from)
    {
        // 同じ串だったら、FALSEを返す
        if (from == this) return false;
        // 団子で埋まっている / 完成している状態なら、FALSEを返す
        else if (currentState == SkewerState.Full || currentState == SkewerState.Complete) return false;
        // 団子が刺さっていないなら、TRUEを返す
        else if(currentState == SkewerState.Empty) return true;
        // 団子の色が合致するかどうかで、真偽値を返す
        return from.GetTopDango().DangoColor == GetTopDango().DangoColor;
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
        dango.SetCurrentSkewer(this);
        // 団子の数が最大数に達したら、状態遷移
        if (dangoList.Count >= maxDango)
        {
            if (IsComplete()) ChangeState(SkewerState.Complete);
            else ChangeState(SkewerState.Full);
        }
        else ChangeState(SkewerState.Stack);
    }
    /// <summary>
    /// 指定位置に団子を追加    </summary>
    public void AddDangoAt(Dango dango, int index)
    {
        if(dango == null) return;

        Dango currentTarget = null;
        Dango previousTarget = null;
        if (dangoList.Count < index) AddDango(dango);
        else
        {
            for (int i = 0; i < dangoList.Count; i++)
            {
                currentTarget = dangoList[i];

                if (previousTarget != null) dangoList[i] = previousTarget;
                if (i == index - 1)
                {
                    dangoList[i] = dango;
                    previousTarget = currentTarget;
                }
            }
        }
        AddDango(previousTarget);

        dango.transform.SetParent(dangoAnchor.transform, false);
        dango.SetCurrentSkewer(this);
        RefreshDangoPosition();
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
    /// 一番上の団子を削除    </summary>
    /// <returns>
    /// 削除した団子    </returns>
    public Dango RemoveTopDango()
    {
        // 団子を所持しているか判定
        if (currentState == SkewerState.Empty) return null;

        Dango removedDango = RemoveDango(dangoList.Count);
        return removedDango;
    }
    /// <summary>
    /// 指定した団子の削除    </summary>
    /// <param name="index">
    /// 団子の指定番号    </param>
    /// <returns>
    /// 削除した団子  </returns>
    public Dango RemoveDangoAt(int index, bool isRefresh = true)
    {
        // 指定番号が所持数に収まっているか判定
        if (index > dangoList.Count) return null;

        Dango removedDango = RemoveDango(index);
        if(isRefresh) RefreshDangoPosition();
        return removedDango;
    }
    /// <summary>
    /// 指定した団子の全削除    </summary>
    public List<Dango> RemoveDangoAtIndices(List<int> indices, bool isRefresh = true)
    {
        // 指定番号数が所持数に収まっているか判定
        if (indices.Count > dangoList.Count) return null;

        List<Dango> removedDangos = new List<Dango>();
        foreach (int index in indices)
        {
            Dango removedDango = RemoveDangoAt(index, isRefresh);
            removedDangos.Add(removedDango);
        }
        return removedDangos;
    }
    /// <summary>
    /// 指定した団子と同色団子のインデックス（下からの順番）を取得    </summary>
    public List<int> GetMatchingDangoIndices(Dango dango)
    {
        List<int> matchingDangoIndices = new List<int>();
        DangoColor targetColor = dango.DangoColor;

        // 現在の団子の番号取得
        int targetIndex = 0;
        for (int i = 0; i < dangoList.Count; i++)
        {
            if (dangoList[i] == dango)
            {
                targetIndex = i;
                break;
            }
        }
        // 同色団子の中で、一番上の団子の番号取得
        int currentIndex = targetIndex;
        while (currentIndex < dangoList.Count)
        {
            if (dangoList[currentIndex].DangoColor == targetColor) currentIndex++;
            else break;
        }
        // 同色団子のインデックスを上から順に追加
        while (currentIndex > 0)
        {
            if (dangoList[currentIndex - 1].DangoColor == targetColor)
            {
                matchingDangoIndices.Add(currentIndex);
                currentIndex--;
            }
            else break;
        }

        return matchingDangoIndices;
    }

    /// <summary>
    /// 団子を再配置    </summary>
    public void RefreshDangoPosition()
    {
        for (int index = 0; index < dangoList.Count; index++)
        {
            dangoList[index].transform.position = dangoAnchor.transform.position + Vector3.up * dangoSpacing * index;
        }
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
