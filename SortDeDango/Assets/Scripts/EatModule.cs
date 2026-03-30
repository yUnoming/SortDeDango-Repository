using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatModule : MonoBehaviour
{
    [SerializeField, Tooltip("食べられる最大回数")]
    private int maxEatActionCount;
    public int MaxEatActionCount => maxEatActionCount;
    [SerializeField]
    private AudioData eatSE;
    [SerializeField]
    private AudioData eatFailSE;

    [Tooltip("食べられる残り回数")]
    private int remainingEatActionCount;
    public int RemainingEatActionCount => remainingEatActionCount;
    [Tooltip("食べる対象の団子")]
    private Dango targetDango;
    [Tooltip("キャンセルされたかどうか")]
    private bool isCanceled;

    private void Start()
    {
        maxEatActionCount = StageManager.Instance.CurrentStageData.maxEatActionCount;
        remainingEatActionCount = maxEatActionCount;
    }

    /// <summary>
    /// 団子を食べられるか判定    </summary>
    /// <param name="selectingSkewer">
    /// 判定対象の串    </param>
    public bool CanEat()
    {
        // 使用回数が残っていない
        if(remainingEatActionCount == 0)
        {
            AudioManager.Instance.PlaySE(eatFailSE);
            return false;
        }
        // 使用回数が残っている
        return true;
    }
    /// <summary>
    /// 指定した串をから団子食べる処理を行い、成功判定を返す    </summary>
    /// <param name="targetSkewer">
    /// 食べる対象の串    </param>
    /// <param name="targetDango">
    /// 食べる団子   </param>
    private bool TryEat(SkewerController targetSkewer, Dango targetDango)
    {
        // 団子を食べることに成功
        List<Dango> eatenDangoList = targetSkewer.RemoveMatchedDangos(targetDango);
        if (eatenDangoList != null)
        {
            foreach(Dango eatenDango  in eatenDangoList)
                Destroy(eatenDango.gameObject);    // オブジェクト削除
            GameplayManager.Instance.OnDangoEaten(eatenDangoList.Count);
            AudioManager.Instance.PlaySE(eatSE);
            remainingEatActionCount--;
            return true;
        }
        // 団子を食べることに失敗
        return false;
    }

    /// <summary>
    /// 団子を食べる処理の制御    </summary>
    /// <param name="targetSkewer">
    /// 食べる対象の串    </param>
    public IEnumerator EatSequence()
    {
        // 食べる対象の発見またはキャンセルされるまで待機
        while (targetDango == null && !isCanceled)
        {
            yield return null;
        }
        if(!isCanceled) TryEat(targetDango.CurrentSkewer, targetDango);
        isCanceled = false;
    }
    /// <summary>
    /// 食べる対象の団子を設定    </summary>
    public void SetTargetDango(Dango dango) { targetDango = dango; }
    /// <summary>
    /// 食べるアクションの中止    </summary>
    public void CancelEat()
    {
        isCanceled = true;
    }

}
