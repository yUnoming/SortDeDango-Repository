using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class DangoMoveAnimation : AnimationBase
{
    [SerializeField, Tooltip("持ち上げる高さ")]
    private float liftHeight = 1f;
    [SerializeField, Tooltip("持ち上げに掛かる時間")]
    private float liftDuration = 0.2f;
    [SerializeField, Tooltip("持ち上げに掛かる時間の重み")]
    private float liftDurationWeight = 0.25f;
    [SerializeField, Tooltip("移動に掛かる時間")]
    private float moveDuration = 0.2f;
    [SerializeField, Tooltip("落とすのにかかる時間")]
    private float dropDuration = 0.2f;
    [SerializeField, Tooltip("落とすのにかかる時間の重み")]
    private float dropDurationWeight = 0.05f;

    /// <summary>
    /// アニメーション挙動    </summary>
    /// <param name="from">
    /// 移動元の串   </param>
    /// <param name="to">
    /// 移動先の串    </param>
    /// <param name="index">
    /// 団子番号   </param>
    /// <param name="endPos">
    /// アニメーションの終了座標    </param>
    private IEnumerator AnimationSequence(SkewerController from, SkewerController to, int index, Vector3 endPos)
    {
        Transform target = this.transform;
        Vector3 startPos = target.position;
        Vector3 liftedStartPos = from.transform.position + Vector3.up * liftHeight;
        Vector3 liftedEndPos = to.transform.position + Vector3.up * liftHeight;

        // 持ち上げ
        yield return MoveOverTime(target, startPos, liftedStartPos, liftDuration * (index * liftDurationWeight + 1));
        // 移動
        yield return MoveOverTime(target, liftedStartPos, liftedEndPos, moveDuration);
        // 落とす
        yield return MoveOverTime(target, liftedEndPos, endPos, dropDuration - index * dropDurationWeight);

        isAnimation = false;
    }
    /// <summary>
    /// 一定時間を掛けて移動    </summary>
    /// <param name="target">
    /// 移動対象    </param>
    /// <param name="from">
    /// 開始座標    </param>
    /// <param name="to">
    /// 終了座標    </param>
    /// <param name="duration">
    /// 移動に掛かる時間    </param>
    private IEnumerator MoveOverTime(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            if (GameplayManager.Instance.isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            target.position = Vector3.Lerp(from, to, t);

            yield return null;
        }

        target.position = to;   // 終了座標へ
    }

    /// <summary>
    /// アニメーション再生    </summary>
    /// <param name="from">
    /// 移動元の串   </param>
    /// <param name="to">
    /// 移動先の串    </param>
    /// <param name="index">
    /// 団子番号   </param>
    /// <param name="endPos">
    /// アニメーションの終了座標    </param>
    public void Play(SkewerController from, SkewerController to, int index, Vector3 endPos)
    {
        if (!isAnimation)
        {
            isAnimation = true;
            StartCoroutine(AnimationSequence(from, to, index, endPos));
        }
    }
    /// <summary>
    /// アニメーション再生    </summary>
    /// <param name="from">
    /// 移動元の串   </param>
    /// <param name="to">
    /// 移動先の串    </param>
    /// <param name="index">
    /// 団子番号   </param>
    /// <param name="endPos">
    /// アニメーションの終了座標    </param>
    public IEnumerator PlayCoroutine(SkewerController from, SkewerController to, int index, Vector3 endPos)
    {
        if (!isAnimation)
        {
            isAnimation = true;
            yield return AnimationSequence(from, to, index, endPos);
        }
    }
}
