using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatModule : MonoBehaviour
{
    [SerializeField, Tooltip("食べられる最大回数")]
    private int maxEatCount;
    [Tooltip("食べられる残り回数")]
    private int remainingEatCount;

    private void Awake()
    {
        remainingEatCount = maxEatCount;
    }

    /// <summary>
    /// 団子を食べられるか判定    </summary>
    /// <param name="selectingSkewer">
    /// 判定対象の串    </param>
    public bool CanEat(SkewerController targetSkewer)
    {
        if (targetSkewer == null || remainingEatCount == 0) return false;
        return targetSkewer.HasDango();
    }
    /// <summary>
    /// 指定した串をから団子食べる処理を行い、成功判定を返す    </summary>
    /// <param name="targetSkewer">
    /// 食べる対象の串    </param>
    /// <param name="eatDangoIndex">
    /// 食べる団子のインデックス番号    </param>
    public bool TryEat(SkewerController targetSkewer, int eatDangoIndex)
    {
        // 団子を食べることに成功
        List<Dango> eatenDangoList = targetSkewer.RemoveMatchDangoAll(eatDangoIndex);
        if (eatenDangoList != null)
        {
            foreach(Dango eatenDango  in eatenDangoList)
                Destroy(eatenDango.gameObject);    // オブジェクト削除

            remainingEatCount--;
            return true;
        }
        // 団子を食べることに失敗
        return false;
    }
    /// <summary>
    /// 団子を食べる処理の制御    </summary>
    /// <param name="targetSkewer">
    /// 食べる対象の串    </param>
    public IEnumerator EatSequence(SkewerController targetSkewer)
    {
        // 入力されたキー入力を元に、食べる団子を指定
        int eatDangoIndex = 0;
        while (true)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                eatDangoIndex = 1;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                eatDangoIndex = 2;
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                eatDangoIndex = 3;
                break;
            }
            yield return null;
        }
        // 食べるのを試みる
        TryEat(targetSkewer, eatDangoIndex);
    }
}
